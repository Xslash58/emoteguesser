using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SevenTV.Types;
using System;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public SevenTV.SevenTV sevenTv;
    public void Start()
    {
        sevenTv = new SevenTV.SevenTV();
    }
    public async void EnterChannelName(string channelName)
    {
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
