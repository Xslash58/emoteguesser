using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SevenTV.Types;
using System;
using UnityEngine.SceneManagement;
using TMPro;
using System.Threading.Tasks;

public class Menu : MonoBehaviour
{
    public static Menu instance = null;
    [SerializeField] Canvas MenuCanvas, SettingsCanvas, CreditsCanvas;
    [SerializeField] TMP_InputField IF_channel;
    [SerializeField] TextMeshProUGUI T_Info;

    SevenTV.SevenTV sevenTv;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
    }

    public void Start()
    {
        sevenTv = new SevenTV.SevenTV();

        //Check game version
        if (Application.version != RemoteConfig.instance.NewestGameVersion)
        {
            string ibcontent = TranslationManager.instance.GetTranslation("gui_infobox_outdatedgame_content")
                .Replace("{gameversion}", Application.version)
                .Replace("{newestgameversion}", RemoteConfig.instance.NewestGameVersion)
                .Replace("{downloadURL}", RemoteConfig.instance.DownloadURL);
            INFOBOX.instance.Request(TranslationManager.instance.GetTranslation("gui_infobox_outdatedgame_title"), ibcontent);
        }
    }

    public async void EnterChannel()
    {
        try
        {
            T_Info.gameObject.SetActive(true);
            EmoteSet eset = await SearchEmoteSet(IF_channel.text);

            if (eset == null)
            {
                T_Info.gameObject.SetActive(false);
                return;
            }

            //Get emotesets again to gain data about emotes
            List<EmoteSet> sets = new List<EmoteSet>();
            User user = await sevenTv.GetUser(eset.owner.id);

            foreach (EmoteSet set in user.emote_sets)
            {
                EmoteSet tempset = await sevenTv.GetEmoteSet(set.id);
                if(tempset.emotes == null || tempset.emotes.Length <= 0)
                    continue;

                //since tags are never used by EmoteGuesser, we use them to mark active emote set
                if (set.name == eset.name)
                    tempset.tags = new string[] { "eg_active" };

                sets.Add(tempset);
            }

            //Open EmoteSetSelector
            EmoteSetSelector.instance.Show(sets);

            T_Info.gameObject.SetActive(false);
        }
        catch (Exception e)
        {
            ConsoleManager.instance.Write("Menu", e.ToString(), 2);
        }
    }
    public async Task<EmoteSet> SearchEmoteSet(string channelName)
    {
        TwitchUser[] ttvUser = await sevenTv.GetTwitchUser(channelName);

        if (ttvUser == null || ttvUser.Length == 0)
        {
            INFOBOX.instance.Request("Twitch", TranslationManager.instance.GetTranslation("gui_infobox_twitch_usernotfound_content"));
            return null;
        }

        Connection conn = await sevenTv.GetConnection(ConnectionType.TWITCH, ttvUser[0].id);
        if (conn == null)
        {
            INFOBOX.instance.Request("7TV", TranslationManager.instance.GetTranslation("gui_infobox_7tv_twitchnotfound_content"));
            return null;
        }

        if (conn.emote_set == null)
        {
            INFOBOX.instance.Request("7TV", TranslationManager.instance.GetTranslation("gui_infobox_7tv_emotesetnotfound_content"));
            return null;
        }

        return conn.emote_set;
    }
    public void Play(string emoteSetId)
    {
        PlayerPrefs.SetString("7tv_emoteset", emoteSetId);
        SceneManager.LoadSceneAsync("Game");
    }

    
    public void Settings()
    {
        if (!SettingsCanvas.isActiveAndEnabled)
        {
            SettingsCanvas.gameObject.SetActive(true);
            MenuCanvas.gameObject.SetActive(false);
        } else
        {
            SettingsCanvas.gameObject.SetActive(false);
            MenuCanvas.gameObject.SetActive(true);
        }
    }
    public void Credits()
    {
        if (!CreditsCanvas.isActiveAndEnabled)
        {
            CreditsCanvas.gameObject.SetActive(true);
            MenuCanvas.gameObject.SetActive(false);
        }
        else
        {
            CreditsCanvas.gameObject.SetActive(false);
            MenuCanvas.gameObject.SetActive(true);
        }
    }
    public void Quit()
    {
        Application.Quit();
    }

    public void Github() =>
        Application.OpenURL(RemoteConfig.instance.GithubURL);
    public void Discord() =>
        Application.OpenURL(RemoteConfig.instance.DiscordURL);
}
