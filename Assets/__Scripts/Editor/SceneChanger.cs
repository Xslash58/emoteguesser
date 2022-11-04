using UnityEditor.SceneManagement;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : EditorWindow
{
    string levelID;
    string coopID;
    [MenuItem("Xslash/Scene Changer")]
    private static void ShowWindow() {
        GetWindow<SceneChanger>("Change Scene");
    }

    private void OnGUI()
    {

        if (EditorApplication.isPlaying)
        {
            if (GUILayout.Button("Editor in Play Mode!")) 
            {
                EditorApplication.isPlaying = false;
            }

            if (GUILayout.Button("Setup"))
            {
                SceneManager.LoadSceneAsync("Setup");
            }
            if (GUILayout.Button("Load"))
            {
                SceneManager.LoadSceneAsync("Load");
            }
            if (GUILayout.Button("Menu"))
            {
                SceneManager.LoadSceneAsync("Menu");
            }
            if (GUILayout.Button("Game"))
            {
                SceneManager.LoadSceneAsync("Game");
            }
            if (GUILayout.Button(""))
            {
            }
            if (GUILayout.Button("")) 
            {
            }
            if (GUILayout.Button("")){}
            if (GUILayout.Button("Clear PlayerPrefs"))
            {
                PlayerPrefs.DeleteAll();
                Debug.LogWarning("PlayerPrefs cleared!");
            }
            return;
        }

        if (GUILayout.Button("Setup"))
        {
            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
            EditorSceneManager.OpenScene("Assets/__Scenes/Setup.unity");
        }
        if (GUILayout.Button("Load"))
        {
            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
            EditorSceneManager.OpenScene("Assets/__Scenes/Load.unity");
        }
        if (GUILayout.Button("Menu")) {
            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
            EditorSceneManager.OpenScene("Assets/__Scenes/Menu.unity");
        }
        if (GUILayout.Button("Game"))
        {
            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
            EditorSceneManager.OpenScene("Assets/__Scenes/Game.unity");
        }
        if (GUILayout.Button("")) { }
        if (GUILayout.Button("")) { }
        if (GUILayout.Button("")) { }
        if (GUILayout.Button("Clear PlayerPrefs"))
        {
            PlayerPrefs.DeleteAll();
            Debug.LogWarning("PlayerPrefs cleared!");
        }

    }
}
