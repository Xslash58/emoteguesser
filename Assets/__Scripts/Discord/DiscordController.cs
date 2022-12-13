using System.Text.RegularExpressions;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine;
using Discord;
using System;

public class DiscordController : MonoBehaviour
{
    public static DiscordController instance = null;
    public Discord.Discord discord;
    public bool discordEnabled;
    [SerializeField] bool disableRP;
    public bool isDisposed = false;

    public Discord.Activity activity;
    public ActivityManager activityManager;

    public Int64 appId = 1052273084922593420;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
        if (Application.isBatchMode)
            return;

        discord = new Discord.Discord(appId, (System.UInt64)Discord.CreateFlags.NoRequireDiscord);
        discordEnabled = true;

        if (disableRP)
            return;

        long timeStamp = new System.DateTimeOffset(System.DateTime.UtcNow).ToUnixTimeSeconds();
        activityManager = discord.GetActivityManager();
        
        activity = new Discord.Activity
        {
            State = "Loading...",
            Details = "",
            Timestamps =
            {
                Start = timeStamp,
            },
            Assets =
            {
                LargeImage = "logo",
                LargeText = "EmoteGuesser",
            }
        };

        activityManager.UpdateActivity(activity, (callback) =>
        {
            if (callback == Discord.Result.Ok)
            {
                Debug.Log("[DISCORD] UPDATED ACTIVITY");
            }
        });

        InvokeRepeating("UpdatePresence", 5, 5);
    }

    public void UpdatePresence()
    {
        switch (SceneManager.GetActiveScene().name)
        {
            default:
                activity.Details = "";
                activity.State = "In " + SceneManager.GetActiveScene().name;
                activity.Instance = false;
                break;
            case "Game":
                string ext = GameManager.instance.Emote.data.animated ? "gif" : "webp";
                activity.Assets.LargeImage = $"https:{GameManager.instance.Emote.data.host.url}/4x.{ext}";
                activity.State = "Guessing this emote";
                activity.Details = "";
                break;

        }

        activityManager.UpdateActivity(activity, (callback) =>
        {
            Debug.Log("[DISCORD] UPDATED ACTIVITY");
        });
    }

    private void Update()
    {
        discord.RunCallbacks();
    }

    private void OnApplicationQuit()
    {
        discord.Dispose();
    }
}
