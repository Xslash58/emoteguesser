using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using UnityEngine.Assertions;
using UnityEngine.UI;
using UnityEngine;

using unity.libwebp;
using unity.libwebp.Interop;

using SevenTV.Types;
using TMPro;
using static EmoteGuesser.Utilities.Image.WebP;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;

    [SerializeField] RawImage EmotePreview;
    [SerializeField] TMP_InputField IF_emote;
    [SerializeField] Button B_giveup;
    [SerializeField] GameObject DownloadInfo;
    [SerializeField] TextMeshProUGUI T_counter;

    public Emote Emote;
    public bool isGif = false;
    public List<frame> frames = new List<frame>();
    public int framesPerSecond = 10;

    public int guessed, unguessed;

    public delegate void RerollHandle();
    public static event RerollHandle OnReroll;


    IEnumerator animCoroutine;

    SevenTV.SevenTV sevenTv;
    EmoteSet emoteset;

    public void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
    }

    private async void Start()
    {
        animCoroutine = PlayAnimation();


        if (PlayerPrefs.HasKey("7tv_emoteset"))
        {
            sevenTv = new SevenTV.SevenTV();
            emoteset = await sevenTv.GetEmoteSet(PlayerPrefs.GetString("7tv_emoteset"));
            RerollEmote();
        } else
        {
            Debug.LogWarning("EmoteSet ID not found. Returning to menu.");
            await SceneManager.LoadSceneAsync("Menu");
        }
    }

    public void RerollEmote()
    {
        if (T_counter)
            T_counter.text = $"{guessed}<size=75%>/{unguessed}";

        int id = UnityEngine.Random.Range(0, emoteset.emotes.Length-1);

        Emote = emoteset.emotes[id];

        isGif = Emote.data.animated;

        string hosturl = Emote.data.host.url;
        string finalurl = $"https:{hosturl}/4x.webp";

        DownloadImage(finalurl, EmotePreview, isGif);

        OnReroll?.Invoke();
    }


    async void DownloadImage(string Url, RawImage image, bool animated)
    {
        //Start
        DownloadInfo.SetActive(true);
        B_giveup.interactable = false;

        //reset currently displayed image
        frames.Clear();
        StopCoroutine(animCoroutine);

        //Request image from given Url
        var bytes = await GetBytes(Url);

        //Preview image
        (List<frame> list, WebPAnimInfo anim_info) = LoadAnimation(bytes);
        frames = list;

        animCoroutine = PlayAnimation();
        StartCoroutine(animCoroutine);

        EmotePreview.transform.localScale = new Vector3(anim_info.canvas_width / 128f, anim_info.canvas_height / 128f, 1);
        EmotePreview.rectTransform.localPosition = new Vector3(0, 50, 0);

        //End
        B_giveup.interactable = true;
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


    public void Guess()
    {
        string name = IF_emote.text;
        if (name.ToLower() == Emote.name.ToLower())
        {
            MatchResult.instance.Request(0, Emote.name);
            IF_emote.text = "";
            guessed++;
            RerollEmote();
        } else
        {
            MatchResult.instance.Request(1, Emote.name);
        }
    }

    public void GiveUp()
    {
        if(Emote != null)
            MatchResult.instance.Request(2, Emote.name);
        unguessed++;
        RerollEmote();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            SceneManager.LoadScene("Menu");
    }

}
