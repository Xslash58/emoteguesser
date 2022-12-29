using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.RemoteConfig;

public class RemoteConfig : MonoBehaviour
{
    public static RemoteConfig instance;
    public struct userAttributes { }
    public struct appAttributes { }
    public bool Ready;
    public string NewestGameVersion;
    public string DownloadURL;
    public string TranslationsURL;
    public string GithubURL;
    public string DiscordURL;

    public delegate void ReadyHandler();
    public event ReadyHandler ReadyEvent;


    void Start()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        Services.instance.Ready += SetupConfig;
        ReadyEvent += ConfigApplied;
    }

    void SetupConfig()
    {
        GetConfig();
        RemoteConfigService.Instance.FetchCompleted += ApplySettings;
    }
    public void GetConfig()
    {
        RemoteConfigService.Instance.FetchConfigs(new userAttributes(), new appAttributes());
    }
    public void ApplySettings(ConfigResponse configResponse)
    {
        switch (configResponse.requestOrigin)
        {
            case ConfigOrigin.Default:
                Debug.Log("No settings loaded this session; using default values.");
                ApplyConfig(RemoteConfigService.Instance.appConfig);
                break;
            case ConfigOrigin.Cached:
                Debug.Log("No settings loaded this session; using cached values from a previous session.");
                ApplyConfig(RemoteConfigService.Instance.appConfig);
                break;
            case ConfigOrigin.Remote:
                Debug.Log("New settings loaded this session; update values accordingly.");
                ApplyConfig(RemoteConfigService.Instance.appConfig);
                break;
        }
    }

    void ApplyConfig(RuntimeConfig cfget)
    {
        NewestGameVersion = cfget.GetString("Game_version");
        DownloadURL = cfget.GetString("downloadurl");
        TranslationsURL = cfget.GetString("translationurl");
        GithubURL = cfget.GetString("githuburl");
        DiscordURL = cfget.GetString("discordurl");
        ReadyEvent?.Invoke();
    }

    void ConfigApplied()
    {
        Ready = true;
    }
}