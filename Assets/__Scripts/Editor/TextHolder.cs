using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TextHolder : EditorWindow
{
    [MenuItem("Xslash/Text Holder")]
    private static void ShowWindow()
    {
        GetWindow<SceneChanger>("Hold Text");
    }
    private void OnGUI()
    {
        if (GUILayout.Button("Setup"))
        {
        }
    }
}
