using System.IO;
using UnityEngine;

public class GLOBAL : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
    private void Start()
    {
        //Application.targetFrameRate = 25565;
        if (Application.version.Contains("DEV") || Debug.isDebugBuild)
        {
            INFOBOX.instance.Request(new INFOBOX.INFOBOXMessage()
            {
                title = "<color=#FF0000>WARNING",
                description = "You're using developer version of <b>Emote Guesser</b>. Original game is <b>ONLY</b> downloadable from <color=#FF0000><b>xslash.itch.io/emoteguesser</b></color>. Make sure that you got this version from game developer. <b>DON'T DISTRIBUTE/SHARE/RECORD/STREAM THIS COPY OF THE GAME!</b>"
            });
        }
        if (Application.genuineCheckAvailable)
        {
            if (!Application.genuine)
            {
                INFOBOX.instance.Request(new INFOBOX.INFOBOXMessage()
                {
                    title = "<color=#FF0000>WARNING",
                    description = "You're using modified version of <b>Emote Guesser</b>. Original game is <b>ONLY</b> downloadable from <color=#FF0000><b>xslash.itch.io/emoteguesser</b></color>. If you was told that this version is original and not modified, you were lied. Uninstall game and do anti-virus scan right away!"
                });
            }
        }
    }
}
