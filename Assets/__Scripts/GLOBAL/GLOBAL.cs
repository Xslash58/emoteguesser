using UnityEngine;

public class GLOBAL : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
    private void Start()
    {
        if (Application.version.Contains("DEV") || Debug.isDebugBuild)
        {
            INFOBOX.instance.Request(new INFOBOX.INFOBOXMessage()
            {
                title = "<color=#FF0000>WARNING",
                description = "You're using developer version of <b>Emote Guesser</b>. Official game is <b>ONLY</b> downloadable from <color=#FF0000><b>xslash.itch.io/emoteguesser</b></color>. Make sure that you got this version from game developer."
            });
        }
    }
}
