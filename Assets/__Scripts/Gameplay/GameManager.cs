using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using SevenTV.Types;
using UnityEngine;
using TMPro;
using System.Threading.Tasks;
using UnityEngine.Networking;

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
        int id = Random.Range(-1, emoteset.emotes.Length);

        Emote = emoteset.emotes[id];

        isGif = Emote.data.animated;

        string hosturl = Emote.data.host.url;
        string ext = isGif ? "gif" : "png";
        string finalurl = "https:" + hosturl + $"/4x.{ext}";

        StartCoroutine(DownloadImage(finalurl, EmotePreview));
    }


    IEnumerator DownloadImage(string Url, RawImage image)
    {
        frames.Clear();

        UnityWebRequest request;

        request = UnityWebRequestTexture.GetTexture(Url);

        if (isGif)
        {
            DownloadInfo.SetActive(true);
            DownloadHandlerBuffer dH = new DownloadHandlerBuffer();
            request.downloadHandler = dH;
        }

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            Debug.Log(request.error);
        else
        {
            if (isGif)
                StartCoroutine(UniGif.GetTextureListCoroutine(request.downloadHandler.data, (gifTexList, loopCount, width, height) =>
                {
                    foreach (var texture in gifTexList)
                        frames.Add(texture.m_texture2d);
                    framesPerSecond = (int)frames.Count /2;
                    DownloadInfo.SetActive(false);
                }));
            else
                image.texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
        }

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
