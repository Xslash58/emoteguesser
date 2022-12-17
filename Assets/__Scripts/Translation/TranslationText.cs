using UnityEngine;
using TMPro;

public class TranslationText : MonoBehaviour
{
    [SerializeField] string key = "";
    private void Awake()
    {
        if (!TranslationManager.instance)
            return;

        UpdateContent();
        TranslationManager.instance.UpdateContent += UpdateContent;
    }

    void UpdateContent()
    {
        string text = TranslationManager.instance.GetTranslation(key);
        if (text == key && !TranslationManager.allowId)
            return;

        if (GetComponent<TextMeshPro>())
            GetComponent<TextMeshPro>().text = TranslationManager.instance.GetTranslation(key);

        else if (GetComponent<TextMeshProUGUI>())
            GetComponent<TextMeshProUGUI>().text = TranslationManager.instance.GetTranslation(key);
    }

    private void OnDestroy()
    {
        TranslationManager.instance.UpdateContent -= UpdateContent;
    }
}
