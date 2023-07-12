using System.Diagnostics;
using System.Linq;
using UnityEngine;

public class StreamerMode : MonoBehaviour
{
    public static StreamerMode instance = null;

    public string[] streamProcesses = new string[] { "obs64", "obs", "PRISMLiveStudio", "XSplit.Core", "TwitchStudio", "vMix64" };

    public bool active = false;

    public void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
    }

    public void Start()
    {
        Process[] processes = Process.GetProcesses();

        foreach (Process process in processes)
            if (streamProcesses.Contains(process.ProcessName))
                EnableStreamerMode(process);
    }

    public void EnableStreamerMode(Process detectedProcess)
    {
        PlayerPrefs.SetString("settings_unlistedemotes", "false");
        INFOBOX.instance.Request("EmoteGuesser", TranslationManager.instance.GetTranslation("gui_infobox_streamermode_streamsoftware").Replace("{processName}", detectedProcess.ProcessName));
    }
}
