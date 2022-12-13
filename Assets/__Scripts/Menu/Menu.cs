using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SevenTV.Types;
using System;
using UnityEngine.SceneManagement;
using TMPro;

public class Menu : MonoBehaviour
{
    [SerializeField] TMP_InputField IF_channel;

    SevenTV.SevenTV sevenTv;

    public void Start()
    {
        sevenTv = new SevenTV.SevenTV();
    }
    public async void EnterChannelName()
    {
        string channelName = IF_channel.text;
        string finalurl = $"https://api.ivr.fi/v2/twitch/user?login={channelName}";
        Uri uri = new Uri(finalurl);
        Debug.Log(uri);
        TwitchUser[] ttvUser = await sevenTv.GetTwitchUser(channelName);

        if (ttvUser == null)
            return;

        Connection conn = await sevenTv.GetConnection(ConnectionType.TWITCH, ttvUser[0].id);
        if (conn.emote_set == null)
            return;

        PlayerPrefs.SetString("7tv_emoteset", conn.emote_set.id);

        SceneManager.LoadSceneAsync("Game");
    }
}
