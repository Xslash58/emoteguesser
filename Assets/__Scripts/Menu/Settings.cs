using System.Collections;
using System.Collections.Generic;
using Lexone.UnityTwitchChat;
using EmoteGuesser.Twitch;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using System;
using Unity.VisualScripting;

public class Settings : MonoBehaviour
{
    [SerializeField] TMP_Dropdown DP_Resolution, DP_Language, DP_Twitch_Gamemodes;
    [SerializeField] Toggle TG_Fullscreen, TG_TwitchEnabled, TG_PROVIDER_SevenTV, TG_PROVIDER_BetterTTV, TG_PROVIDER_FrankerFaceZ, TG_UnlistedEmotes;
    [SerializeField] TMP_InputField IF_TwitchName;

    [SerializeField] TextMeshProUGUI T_version;

    bool fullscreen;

    private void Start()
    {
        T_version.text = $"Unity {Application.unityVersion}\n{Application.version}";

        //Populate Dropdowns
        Reload();
        TranslationManager.instance.UpdateContent += Reload;

        //Promote 'TWITCHPLAYS' since its better and makes more sense
        if (!PlayerPrefs.HasKey("settings_twitch_gamemode"))
            ChangeTwitchGamemode(1);

        //Restore settings
        if (PlayerPrefs.HasKey("settings_resolution"))
        {
            int sel = PlayerPrefs.GetInt("settings_resolution");
            DP_Resolution.value = sel;
            ChangeResolution(sel);
        }
        if (PlayerPrefs.HasKey("settings_fullscreen"))
        {
            string sel = PlayerPrefs.GetString("settings_fullscreen");
            bool choice = bool.Parse(sel);
            TG_Fullscreen.isOn = choice;
            ChangeFullscreen(choice);
        }
        if (PlayerPrefs.HasKey("settings_language"))
        {
            int sel = PlayerPrefs.GetInt("settings_language");
            DP_Language.value = sel;
        }

        if (PlayerPrefs.HasKey("settings_twitch_enabled"))
        {
            string sel = PlayerPrefs.GetString("settings_twitch_enabled");
            bool choice = bool.Parse(sel);
            TG_TwitchEnabled.isOn = choice;
        }
        if (PlayerPrefs.HasKey("settings_twitch_channel"))
        {
            string sel = PlayerPrefs.GetString("settings_twitch_channel");
            IF_TwitchName.text = sel;
            ChangeTwitchChannel(sel);
        }
        if (PlayerPrefs.HasKey("settings_twitch_gamemode"))
        {
            int sel = PlayerPrefs.GetInt("settings_twitch_gamemode");
            DP_Twitch_Gamemodes.value = sel;
        }

        if (PlayerPrefs.HasKey("settings_provider_seventv"))
        {
            string sel = PlayerPrefs.GetString("settings_provider_seventv");
            bool choice = bool.Parse(sel);
            TG_PROVIDER_SevenTV.isOn = choice;
        }
        else
            TG_PROVIDER_SevenTV.isOn = true;
        if (PlayerPrefs.HasKey("settings_provider_betterttv"))
        {
            string sel = PlayerPrefs.GetString("settings_provider_betterttv");
            bool choice = bool.Parse(sel);
            TG_PROVIDER_BetterTTV.isOn = choice;
        }
        else
            TG_PROVIDER_BetterTTV.isOn = false;
        if (PlayerPrefs.HasKey("settings_provider_frankerfacez"))
        {
            string sel = PlayerPrefs.GetString("settings_provider_frankerfacez");
            bool choice = bool.Parse(sel);
            TG_PROVIDER_FrankerFaceZ.isOn = choice;
        }
        else
            TG_PROVIDER_FrankerFaceZ.isOn = false;

        if (PlayerPrefs.HasKey("settings_unlistedemotes"))
        {
            string sel = PlayerPrefs.GetString("settings_unlistedemotes");
            bool choice = bool.Parse(sel);
            TG_UnlistedEmotes.isOn = choice;
        }
        else
            TG_UnlistedEmotes.isOn = true;
    }

    public void ChangeResolution(int choice)
    {
        string restext = DP_Resolution.options[choice].text;
        string[] resolution = restext.Split('x');

        Screen.SetResolution(int.Parse(resolution[0]), int.Parse(resolution[1]), fullscreen);
        PlayerPrefs.SetInt("settings_resolution", choice);
    }
    public void ChangeLanguage(int choice)
    {
        if (choice == DP_Language.options.Count - 1)
        {
            Application.OpenURL(RemoteConfig.instance.TranslationsURL);
            return;
        }

        PlayerPrefs.SetInt("settings_language", choice);
        string locale = TranslationManager.GetLocaleByName(DP_Language.options[choice].text);
        TranslationManager.instance.SetTranslation(locale);

        PlayerPrefs.SetString("translation_locale", locale);
        TranslationManager.instance.SendUpdateEvent();
    }
    public void ChangeFullscreen(bool state)
    {
        fullscreen = state;
        Screen.fullScreen = state;
        PlayerPrefs.SetString("settings_fullscreen", state.ToString());
    }

    public void ChangeTwitchIntegration(bool state)
    {
        PlayerPrefs.SetString("settings_twitch_enabled", state.ToString());
    }
    public void ChangeTwitchChannel(string name)
    {
        IRC.Instance.channel = name;
        PlayerPrefs.SetString("settings_twitch_channel", name);
    }
    public void ChangeTwitchGamemode(int choice)
    {
        PlayerPrefs.SetInt("settings_twitch_gamemode", choice);
    }

    public void ChangeProviderSevenTV(bool state)
    {
        PlayerPrefs.SetString("settings_provider_seventv", state.ToString());
    }
    public void ChangeProviderBetterTTV(bool state)
    {
        PlayerPrefs.SetString("settings_provider_betterttv", state.ToString());
    }
    public void ChangeProviderFrankerFaceZ(bool state)
    {
        PlayerPrefs.SetString("settings_provider_frankerfacez", state.ToString());
    }

    public void ChangeUnlistedEmotes(bool state)
    {
        PlayerPrefs.SetString("settings_unlistedemotes", state.ToString());
    }

    void Reload()
    {
        if (DP_Language)
        {
            DP_Language.options.Clear();
            string[] locales = TranslationManager.GetAllLocales();
            foreach (string locale in locales)
            {
                string name = TranslationManager.GetNameByLocale(locale);
                DP_Language.options.Add(new TMP_Dropdown.OptionData { text = name });
            }
            DP_Language.options.Add(new TMP_Dropdown.OptionData { text = TranslationManager.instance.GetTranslation("settings_language_contribute") });

        }

        if (DP_Resolution)
        {
            DP_Resolution.options.Clear();

            Resolution[] resolutions = Screen.resolutions;
            List<string> stringres = new List<string>();

            foreach (Resolution res in resolutions)
                if (!stringres.Contains($"{res.width}x{res.height}"))
                    stringres.Add($"{res.width}x{res.height}");

            foreach (string res in stringres)
            {
                DP_Resolution.options.Add(new TMP_Dropdown.OptionData { text = res });
            }
        }

        if (DP_Twitch_Gamemodes)
        {
            DP_Twitch_Gamemodes.options.Clear();
            foreach (string name in Enum.GetNames(typeof(GameModes)))
                DP_Twitch_Gamemodes.options.Add(new TMP_Dropdown.OptionData { text = TranslationManager.instance.GetTranslation($"twitch_gamemode_{name}") });
        }
    }
}
