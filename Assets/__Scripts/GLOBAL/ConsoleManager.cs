using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using UnityEngine;
using System;
using TMPro;

public class ConsoleManager : MonoBehaviour
{
    public static ConsoleManager instance = null;
    public List<Command> commands = new List<Command>();
    [SerializeField] Canvas ConsoleCanvas;
    [SerializeField] TextMeshProUGUI logs;
    [SerializeField] TMP_InputField input;

    [System.Serializable] public class Command
    {
        public string name;
        public string description;
        public UnityEvent<string[]> action;
    }

    private void Start()
    {

        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        logs.text = "";
        Write("Console", "You opened developer console! If you don't know what you're doing - you can leave it by clicking '. Have a nice day!", 1);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.BackQuote))
        {
            if (ConsoleCanvas.isActiveAndEnabled == true)
                Close();
            else
                Open();
        }
    }

    public void Open()
    {
        ConsoleCanvas.gameObject.SetActive(true);
    }
    public void Close()
    {
        ConsoleCanvas.gameObject.SetActive(false);
    }
    public void ConsoleInput(string content)
    {
        if (content != null)
        {
            string[] args = content.Split(' ');
            string cmd = args[0];
            SendCommand(cmd, args);
        }
        input.text = "";
    }

    public void SendCommand(string command, string[] args)
    {
        try
        {
            var fcmd = commands.Find(cmd => cmd.name.ToLower() == command.ToLower());
            if (fcmd != null)
                fcmd.action.Invoke(args);
            else
                Write("Console", "Invalid command!", 1);
        } catch(Exception e)
        {
            Write("Console", e.ToString(), 2);
        }
    }

    public void Write(string Prefix, string content, int debugLevel)
    {
        switch (debugLevel)
        {
            case 0:
                logs.text += "<color=#FFFFFF>" + "[" + System.DateTime.Now + "] " + "[" + Prefix + "] " + content + "\n";
                break;
            case 1:
                logs.text += "<color=#FFFC00>" + "[" + System.DateTime.Now + "] " + "[" + Prefix + "] " + content + "\n";
                break;
            case 2:
                logs.text += "<color=#FF0000>" + "[" + System.DateTime.Now + "] " + "[" + Prefix + "] " + content + "\n";
                break;
        }
        if (Application.isBatchMode)
            Debug.Log("[" + debugLevel + "] " + "[" + System.DateTime.Now + "] " + "[" + Prefix + "] " + content);
    }

    bool IsCommandAvailable(string[] scenes)
    {
        string activeScene = SceneManager.GetActiveScene().name;
        int success = 0;
        foreach (string scene in scenes)
            if (activeScene == scene)
                success++;
        if (success > 0)
            return true;
        else
            return false;
    }

    public void cmd_cls(string[] args)
    {
        logs.text = "";
    }
    public void cmd_scene(string[] args)
    {
        if (args.Length < 2)
        {
            Write("Console", "Invalid format! scene {name}", 1);
            return;
        }
        UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(args[1]);
    }
    public void cmd_game(string[] args)
    {
        Write("Console", "Build GUID: " + Application.buildGUID, 0);
        Write("Console", "Company Name: " + Application.companyName, 0);
        Write("Console", "Data Path: " + Application.dataPath, 0);
        Write("Console", "Cloud ID: " + Application.cloudProjectId, 0);
        Write("Console", "Genuine Check: " + Application.genuineCheckAvailable, 0);
        Write("Console", "Genuine: " + Application.genuine, 0);
        Write("Console", "Installer: " + Application.installerName, 0);
        Write("Console", "Internet: " + Application.internetReachability, 0);
        Write("Console", "Focus: " + Application.isFocused, 0);
        Write("Console", "Platform: " + Application.platform, 0);
        Write("Console", "Target FPS: " + Application.targetFrameRate, 0);
        Write("Console", "Version: " + Application.version, 0);
        Write("Console", "Unity Version: " + Application.unityVersion, 0);
    }
    public void cmd_remoteconfig(string[] args)
    {
        RemoteConfig rc = GameObject.FindGameObjectWithTag("remoteconfig").GetComponent<RemoteConfig>();
        if (args.Length < 2)
        {
            Write("Console", "Invalid format! remoteconfig {reload/display}", 1);
            return;
        }

        if (args[1] == "display")
            Write("RemoteConfig", "Displaying JSON: \n" + Unity.Services.RemoteConfig.RemoteConfigService.Instance.appConfig.config.ToString(), 0);
        else if (args[1] == "reload")
        {
            rc.GetConfig();
            Write("RemoteConfig", "reloaded!", 0);
        }
        else
            Write("Console", "Invalid format! remoteconfig {reload/display}", 1);
    }
    public void cmd_playerprefs(string[] args)
    {
        if (args.Length >= 4)
        {
            if (args[1] == "string")
                PlayerPrefs.SetString(args[2], args[3]);
            else if (args[1] == "int")
                PlayerPrefs.SetInt(args[2], int.Parse(args[3]));
            else
            {
                Write("Console", "Unknown type", 1);
                return;
            }
            Write("Console", "Set " + args[2] + " to " + args[3], 0);
        }
        else
            Write("Console", "Invalid format! playerprefs {type} {key} {value}", 1);
    }
    public void cmd_help(string[] args)
    {
        Write("Console", "Available commands (" + commands.Count + "):", 0);
        foreach (Command cmd in commands)
            Write("Console", "<color=#FF0000>"+cmd.name + "</color> - " + cmd.description, 0);
    }
    public void cmd_showtids(string[] args)
    {
        if (TranslationManager.instance.GetCurrentLanguage() == "tIDs")
        {
            if (PlayerPrefs.HasKey("translation_locale"))
                TranslationManager.instance.SetTranslation(PlayerPrefs.GetString("translation_locale"));
            else
                TranslationManager.instance.SetTranslation("en-US");
            TranslationManager.allowId = false;
            Write("TranslationManager", "Text IDs hidden", 0);
        }
        else
        {
            TranslationManager.instance.SetTranslation("tIDs");
            TranslationManager.allowId = true;
            Write("TranslationManager", "Text IDs shown", 0);
        }
        TranslationManager.instance.SendUpdateEvent();

    }
    public void cmd_res(string[] args)
    {
        Screen.SetResolution(int.Parse(args[1]), int.Parse(args[2]), false);
    }

    public void onContentChange(Vector2 size)
    {
    }
}
