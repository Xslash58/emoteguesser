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
    [SerializeField] TMP_InputField IF_channel;
    [SerializeField] TextMeshProUGUI T_Info;

    SevenTV.SevenTV sevenTv;

    public void Start()
    {
        sevenTv = new SevenTV.SevenTV();
    }

    public async void EnterChannel()
    {
        T_Info.gameObject.SetActive(true);
        await SearchEmoteSet(IF_channel.text);
        T_Info.gameObject.SetActive(false);
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
}
