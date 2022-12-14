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
    [SerializeField] Canvas MenuCanvas, SettingsCanvas, CreditsCanvas;
    [SerializeField] TMP_InputField IF_channel;
    [SerializeField] TextMeshProUGUI T_Info;

    SevenTV.SevenTV sevenTv;

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
            await SearchEmoteSet(IF_channel.text);
            T_Info.gameObject.SetActive(false);
        }
        catch (Exception e)
        {
            ConsoleManager.instance.Write("Menu", e.ToString(), 2);
        }
    }
    public async Task<bool> SearchEmoteSet(string channelName)
    {
        TwitchUser[] ttvUser = await sevenTv.GetTwitchUser(channelName);

        if (ttvUser == null || ttvUser.Length == 0)
        {
            INFOBOX.instance.Request("Twitch", TranslationManager.instance.GetTranslation("gui_infobox_twitch_usernotfound_content"));
            return false;
        }

        Connection conn = await sevenTv.GetConnection(ConnectionType.TWITCH, ttvUser[0].id);
        if (conn == null)
        {
            INFOBOX.instance.Request("7TV", TranslationManager.instance.GetTranslation("gui_infobox_7tv_twitchnotfound_content"));
            return false;
        }

        if (conn.emote_set == null)
        {
            INFOBOX.instance.Request("7TV", TranslationManager.instance.GetTranslation("gui_infobox_7tv_emotesetnotfound_content"));
            return false;
        }

        if (conn.emote_set.emotes == null)
        {
            INFOBOX.instance.Request("7TV", TranslationManager.instance.GetTranslation("gui_infobox_7tv_emotesetnoemotes_content"));
            return false;
        }

        PlayerPrefs.SetString("7tv_emoteset", conn.emote_set.id);

        SceneManager.LoadSceneAsync("Game");

        return true;
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
