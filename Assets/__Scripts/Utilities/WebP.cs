using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Assertions;
using UnityEngine;
using unity.libwebp;
using unity.libwebp.Interop;
using WebP;
using UnityEngine.Networking;
using System.Threading.Tasks;

namespace EmoteGuesser.Utilities.Image
{
    public static class WebP
    {
        [System.Serializable]
        public struct frame
        {
            public Texture2D texture;
            public int timestamp;
        }

        public static async Task<byte[]> GetBytes(string Url)
        {
            UnityWebRequest request;

            request = UnityWebRequestTexture.GetTexture(Url);
            DownloadHandlerBuffer dH = new DownloadHandlerBuffer();
            request.downloadHandler = dH;

            await request.SendWebRequest();

            return request.downloadHandler.data;
        }

        //Returns all webp frames with an animation data from specified bytes (Most likely from GetBytes function)
        public static unsafe (List<frame>, WebPAnimInfo) LoadAnimation(byte[] bytes)
        {
            if (bytes == null)
            {
                Debug.LogWarning("Can't process null bytes.");
                return (null, new WebPAnimInfo());
            }

            List<frame> ret = new List<frame>();
            WebPAnimInfo anim_info = new WebPAnimInfo();

            WebPAnimDecoderOptions option = new WebPAnimDecoderOptions
            {
                use_threads = 1,
                color_mode = WEBP_CSP_MODE.MODE_RGBA,
            };
            option.padding[5] = 1;

            NativeLibwebpdemux.WebPAnimDecoderOptionsInit(&option);
            fixed (byte* p = bytes)
            {
                WebPData webpdata = new WebPData
                {
                    bytes = p,
                    size = new UIntPtr((uint)bytes.Length)
                };
                WebPAnimDecoder* dec = NativeLibwebpdemux.WebPAnimDecoderNew(&webpdata, &option);

                NativeLibwebpdemux.WebPAnimDecoderGetInfo(dec, &anim_info);

                uint size = anim_info.canvas_width * 4 * anim_info.canvas_height;

                int timestamp = 0;

                IntPtr pp = new IntPtr();
                byte** unmanagedPointer = (byte**)&pp;
                for (int i = 0; i < anim_info.frame_count; ++i)
                {
                    int result = NativeLibwebpdemux.WebPAnimDecoderGetNext(dec, unmanagedPointer, &timestamp);
                    Assert.AreEqual(1, result);

                    int lWidth = (int)anim_info.canvas_width;
                    int lHeight = (int)anim_info.canvas_height;
                    bool lMipmaps = false;
                    bool lLinear = false;

                    Texture2D texture = Texture2DExt.CreateWebpTexture2D(lWidth, lHeight, lMipmaps, lLinear);
                    texture.LoadRawTextureData(pp, (int)size);

                    // Flip updown.
                    Color[] pixels = texture.GetPixels();
                    Color[] pixelsFlipped = new Color[pixels.Length];
                    for (int y = 0; y < anim_info.canvas_height; y++)
                    {
                        Array.Copy(pixels, y * anim_info.canvas_width, pixelsFlipped, (anim_info.canvas_height - y - 1) * anim_info.canvas_width, anim_info.canvas_width);
                    }
                    texture.SetPixels(pixelsFlipped);


                    texture.Apply();
                    ret.Add(new frame
                    {
                        texture = texture,
                        timestamp = timestamp
                    });
                }
                NativeLibwebpdemux.WebPAnimDecoderReset(dec);
                NativeLibwebpdemux.WebPAnimDecoderDelete(dec);
            }
            return (ret, anim_info);
        }
    }
}
