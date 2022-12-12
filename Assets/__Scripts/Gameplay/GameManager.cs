using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using SevenTV.Types;
using UnityEngine;
using TMPro;
using System.Threading.Tasks;
using UnityEngine.Networking;
using WebP;
using System;
using unity.libwebp;
using UnityEngine.Assertions;
using unity.libwebp.Interop;

public class GameManager : MonoBehaviour
{
    [SerializeField] RawImage EmotePreview;
    [SerializeField] TMP_InputField IF_emote;
    [SerializeField] GameObject DownloadInfo;

    public Emote Emote;
    public bool isGif = false;
    public List<Texture2D> frames = new List<Texture2D>();
    public int framesPerSecond = 10;


    SevenTV.SevenTV sevenTv;
    EmoteSet emoteset;
    string EmoteSetID = "60be4f7dec711f52802d4235";

    private async void Start()
    {
        sevenTv = new SevenTV.SevenTV();
        emoteset = await sevenTv.GetEmoteSet(EmoteSetID);
    }

    public void RerollEmote()
    {
        int id = UnityEngine.Random.Range(-1, emoteset.emotes.Length);

        Emote = emoteset.emotes[id];

        isGif = Emote.data.animated;

        string hosturl = Emote.data.host.url;
        //bool anim = isGif ? "gif" : "png";
        string ext = "webp";
        string finalurl = "https:" + hosturl + $"/4x.{ext}";

        StartCoroutine(DownloadImage(finalurl, EmotePreview, isGif));
    }


    IEnumerator DownloadImage(string Url, RawImage image, bool animated)
    {
        frames.Clear();

        UnityWebRequest request;

        request = UnityWebRequestTexture.GetTexture(Url);
        DownloadInfo.SetActive(true);
        DownloadHandlerBuffer dH = new DownloadHandlerBuffer();
        request.downloadHandler = dH;

        yield return request.SendWebRequest();

        var bytes = request.downloadHandler.data;

        List<(Texture2D, int)> list = LoadAnimation(bytes);
        foreach ((Texture2D, int) item in list)
        {
            frames.Add(item.Item1);
        }
        DownloadInfo.SetActive(false);
    }

    private unsafe List<(Texture2D, int)> LoadAnimation(byte[] bytes)
    {
        List<(Texture2D, int)> ret = new List<(Texture2D, int)>();

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
            //dec->config_.options.flip = 1;

            WebPAnimInfo anim_info = new WebPAnimInfo();

            NativeLibwebpdemux.WebPAnimDecoderGetInfo(dec, &anim_info);

            Debug.LogWarning($"{anim_info.frame_count} {anim_info.canvas_width}/{anim_info.canvas_height}");

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

                {// Flip updown.
                 // ref: https://github.com/netpyoung/unity.webp/issues/25
                 // ref: https://github.com/netpyoung/unity.webp/issues/21
                 // ref: https://github.com/webmproject/libwebp/blob/master/src/demux/anim_decode.c#L309
                    Color[] pixels = texture.GetPixels();
                    Color[] pixelsFlipped = new Color[pixels.Length];
                    for (int y = 0; y < anim_info.canvas_height; y++)
                    {
                        Array.Copy(pixels, y * anim_info.canvas_width, pixelsFlipped, (anim_info.canvas_height - y - 1) * anim_info.canvas_width, anim_info.canvas_width);
                    }
                    texture.SetPixels(pixelsFlipped);
                }

                texture.Apply();
                ret.Add((texture, timestamp));
            }
            NativeLibwebpdemux.WebPAnimDecoderReset(dec);
            NativeLibwebpdemux.WebPAnimDecoderDelete(dec);
        }
        return ret;
    }

    private void Update()
    {
        if (frames.Count > 0)
        {
            int index = (int)(Time.time * framesPerSecond) % frames.Count;
            EmotePreview.texture = frames[index];
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            Debug.LogWarning(Emote.name);
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            RerollEmote();
        }
    }
}
