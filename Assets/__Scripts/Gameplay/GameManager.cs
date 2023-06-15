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
using EmoteGuesser.Types;
using static EmoteGuesser.Utilities.Image.WebP;
using System.Linq;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;

    [SerializeField] RawImage EmotePreview;
    [SerializeField] TMP_InputField IF_emote;
    [SerializeField] Button B_giveup;
    [SerializeField] GameObject DownloadInfo;
    [SerializeField] TextMeshProUGUI T_counter;

    public EmoteGuesser.Types.Emote Emote;
    public bool isGif = false;
    public List<frame> frames = new List<frame>();
    public int framesPerSecond = 10;

    public int guessed, unguessed;

    public delegate void RerollHandle();
    public static event RerollHandle OnReroll;


    IEnumerator animCoroutine;

    SevenTV.SevenTV sevenTv;
    public List<EmoteGuesser.Types.Emote> emotes = new List<EmoteGuesser.Types.Emote>();


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
        sevenTv = new SevenTV.SevenTV();
        bool o_stv, o_ffz, o_bttv;

        if (!PlayerPrefs.HasKey("settings_provider_seventv"))
            o_stv = true;
        else
            o_stv = bool.Parse(PlayerPrefs.GetString("settings_provider_seventv"));

        if (!PlayerPrefs.HasKey("settings_provider_betterttv"))
            o_bttv = false;
        else
            o_bttv = bool.Parse(PlayerPrefs.GetString("settings_provider_betterttv"));

        if (!PlayerPrefs.HasKey("settings_provider_frankerfacez"))
            o_ffz = false;
        else
            o_ffz = bool.Parse(PlayerPrefs.GetString("settings_provider_frankerfacez"));

        if (o_stv && PlayerPrefs.HasKey("7tv_emoteset"))
        {
            EmoteSet emoteset = await sevenTv.GetEmoteSet(PlayerPrefs.GetString("7tv_emoteset"));
            emotes.AddRange(emoteset.emotes.Select(stvEmote => new EmoteGuesser.Types.Emote
            {
                name = stvEmote.name,
                emoteUrl = $"https:{stvEmote.data.host.url}/4x.webp",
                animated = stvEmote.data.animated,
                provider = Provider.SEVENTV
            }));
        }

        if (PlayerPrefs.HasKey("channelName"))
        {
            string channelName = PlayerPrefs.GetString("channelName");

            //Try to fetch BTTV Emotes
            if (o_bttv)
                try
                {
                    BTTV.BTTV bttv = new BTTV.BTTV();
                    string id = (await sevenTv.GetTwitchUser(channelName))[0].id;
                    BTTV.Types.User usr = await bttv.GetUser(BTTV.Types.ConnectionType.TWITCH, id);
                    emotes.AddRange(usr.sharedEmotes.Select(bttvEmote => new EmoteGuesser.Types.Emote
                    {
                        name = bttvEmote.code,
                        emoteUrl = $"https://cdn.betterttv.net/emote/{bttvEmote.id}/3x.webp",
                        animated = bttvEmote.animated,
                        provider = Provider.BETTERTTV
                    }));
                }
                catch (Exception e) { ConsoleManager.instance.Write("GameManager", "Failed to load BTTV Emotes: " + e, 2); }

            //Try to fetch FFZ Emotes
            if (o_ffz)
                try
                {
                    FFZ.FFZ ffz = new FFZ.FFZ();
                    FFZ.Types.Room room = await ffz.GetRoom(channelName);
                    emotes.AddRange(room.sets.First().Value.emoticons.Select(ffzEmote => new EmoteGuesser.Types.Emote
                    {
                        name = ffzEmote.name,
                        emoteUrl = ffzEmote.urls["4"],
                        animated = false,
                        provider = Provider.FRANKERFACEZ
                    }));
                }
                catch (Exception e) { ConsoleManager.instance.Write("GameManager", "Failed to load FFZ Emotes: " + e, 2); }
        }

        if(emotes.Count < 1)
        {
            Debug.LogWarning("No emotes found. Returning to menu.");
            INFOBOX.instance.Request("EmoteGuesser", TranslationManager.instance.GetTranslation("gui_infobox_emoteguesser_noemotes"));
            await SceneManager.LoadSceneAsync("Menu");
            return;
        }

        RerollEmote();
    }

    public void RerollEmote()
    {
        if (T_counter)
            T_counter.text = $"{guessed}<size=75%>/{unguessed}";

        int id = UnityEngine.Random.Range(0, emotes.Count);

        Emote = emotes[id];

        isGif = Emote.animated;

        //string hosturl = Emote.data.host.url;
        //string finalurl = $"https:{hosturl}/4x.webp";
        if (Emote.provider != EmoteGuesser.Types.Provider.FRANKERFACEZ)
            DownloadImage(Emote.emoteUrl, EmotePreview, isGif);
        else
            StartCoroutine(DownloadImagePNG(Emote.emoteUrl, EmotePreview));

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
    IEnumerator DownloadImagePNG(string Url, RawImage image)
    {
        //Start
        DownloadInfo.SetActive(true);
        B_giveup.interactable = false;

        //reset currently displayed image
        frames.Clear();
        StopCoroutine(animCoroutine);

        Texture2D texture = null;
        //Request image from given Url
        using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(Url))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(www.error);
            }
            else
            {
                texture = DownloadHandlerTexture.GetContent(www);
                image.texture = texture;
            }
        }

        if(texture)
            EmotePreview.transform.localScale = new Vector3(texture.width / 128f, texture.height / 128f, 1);
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
