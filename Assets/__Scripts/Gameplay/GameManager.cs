using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using SevenTV.Types;
using UnityEngine;
using TMPro;
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
    public List<frame> frames = new List<frame>();
    public int framesPerSecond = 10;

    [System.Serializable] public struct frame
    {
        public Texture2D texture;
        public int timestamp;
    }

    IEnumerator animCoroutine;

    SevenTV.SevenTV sevenTv;
    EmoteSet emoteset;
    string EmoteSetID = "60be4f7dec711f52802d4235";

    private async void Start()
    {
        sevenTv = new SevenTV.SevenTV();
        emoteset = await sevenTv.GetEmoteSet(EmoteSetID);

        animCoroutine = PlayAnimation();
    }

    public void RerollEmote()
    {
        int id = UnityEngine.Random.Range(-1, emoteset.emotes.Length);

        Emote = emoteset.emotes[id];

        isGif = Emote.data.animated;

        string hosturl = Emote.data.host.url;
        string finalurl = $"https:{hosturl}/4x.webp";

        StartCoroutine(DownloadImage(finalurl, EmotePreview, isGif));
    }


    IEnumerator DownloadImage(string Url, RawImage image, bool animated)
    {
        frames.Clear();
        StopCoroutine(animCoroutine);

        UnityWebRequest request;

        request = UnityWebRequestTexture.GetTexture(Url);
        DownloadInfo.SetActive(true);
        DownloadHandlerBuffer dH = new DownloadHandlerBuffer();
        request.downloadHandler = dH;

        yield return request.SendWebRequest();

        var bytes = request.downloadHandler.data;

        (List<frame> list, WebPAnimInfo anim_info) = LoadAnimation(bytes);
        frames = list;

        animCoroutine = PlayAnimation();
        StartCoroutine(animCoroutine);

        EmotePreview.transform.localScale = new Vector3(anim_info.canvas_width / 128f, anim_info.canvas_height / 128f, 1);
        EmotePreview.rectTransform.localPosition = new Vector3(0, 50, 0);

        DownloadInfo.SetActive(false);
    }
    IEnumerator PlayAnimation()
    {
        int prevTimestamp = 0;
        for (int i = 0; i < frames.Count; ++i)
        {
            frame frame = frames[i];
            if (EmotePreview == null)
            {
                yield break;
            }
            EmotePreview.texture = frame.texture;
            int delay = frame.timestamp - prevTimestamp;
            prevTimestamp = frame.timestamp;

            if (delay < 0)
            {
                delay = 0;
            }

            yield return new WaitForSeconds(delay / 1000.0f);
            if (i == frames.Count - 1)
            {
                i = -1;
            }
        }
    }

    private unsafe (List<frame>, WebPAnimInfo) LoadAnimation(byte[] bytes)
    {
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
            //dec->config_.options.flip = 1;

            NativeLibwebpdemux.WebPAnimDecoderGetInfo(dec, &anim_info);

           // Debug.LogWarning($"{anim_info.frame_count} {anim_info.canvas_width}/{anim_info.canvas_height}");

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

    public void Guess(string name)
    {
        if (name == Emote.name)
        {
            Debug.Log("You guessed it!");
            RerollEmote();
        } else
        {
            Debug.Log("nope");
        }
    }

    public void GiveUp()
    {
        if(Emote != null)
            Debug.Log("Emote name was: " + Emote.name);
        RerollEmote();
    }

}
