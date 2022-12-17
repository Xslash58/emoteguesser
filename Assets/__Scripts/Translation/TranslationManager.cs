using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class TranslationManager : MonoBehaviour
{
    public static TranslationManager instance = null;
    public static bool ready = false;
    public static bool allowId = false;
    [SerializeField] string lang = "en-US";
    public static List<Language> languages = new List<Language>();
    [SerializeField] List<Language> localLanguages = new List<Language>();
    Dictionary<string, string> translations = new Dictionary<string, string>();
    Dictionary<string, string> defaulttranslations = new Dictionary<string, string>();
    public delegate void UpdateContentHandler();
    public event UpdateContentHandler UpdateContent;

    [System.Serializable] public struct Language
    {
        public string name;
        public string locale;
    }

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        languages = localLanguages;

        if (!Directory.Exists(Application.streamingAssetsPath + "/translations"))
        {
            Directory.CreateDirectory(Application.streamingAssetsPath + "/translations");
        }

        SetDefaultTranslations(lang);

        if (PlayerPrefs.HasKey("translation_locale"))
            SetTranslation(PlayerPrefs.GetString("translation_locale"));
        else
            SetTranslation(lang);
    }

    public string GetTranslation(string name)
    {
        if (translations.ContainsKey(name))
            return translations[name];
        else if (defaulttranslations.ContainsKey(name) && !allowId)
            return defaulttranslations[name];
        else
            return name;
    }

    public string GetCurrentLanguage()
    {
        return lang;
    }

    public void SetTranslation(string locale)
    {
        lang = locale;

        string translationfile = Application.streamingAssetsPath + "/translations/" + locale + ".ini";
        FileToDictionary(translations, translationfile);
    }
    private void SetDefaultTranslations(string locale)
    {
        string translationfile = Application.streamingAssetsPath + "/translations/" + locale + ".ini";
        FileToDictionary(defaulttranslations, translationfile);
    }

    private void FileToDictionary(Dictionary<string, string> translations, string translationfile)
    {
        ready = false;
        translations.Clear();
        if (File.Exists(translationfile))
            using (StreamReader sr = new StreamReader(translationfile))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    string[] translation = line.Split(new[] { '=' }, 2);
                    if (translation.Length < 2)
                        continue;

                    string word = "";
                    for (int i = 1; i < translation.Length; i++)
                        word += translation[i];

                    //Replace \n with <br> if any exists because \n is buggy
                    word = word.Replace(@"\n", "<br>");

                    //Remove "" that crowdin adds
                    if (word.StartsWith("\""))
                        word = word.Replace("\"", "");
                    if (word.EndsWith("\""))
                        word = word.Replace("\"", "");

                    //Remove Comments if any
                    if (word.Contains(" /#/"))
                        word = word.Substring(0, word.LastIndexOf(" /#/"));

                    translations.Add(translation[0], word);
                }
            }
        ready = true;
    }

    public static string GetLocaleByName(string name)
    {
        Language lang = languages.Find(x => x.name == name);
        if (lang.locale != null)
            return lang.locale;
        else
            return "en-US";
    }

    public static string GetNameByLocale(string locale)
    {
        Language lang = languages.Find(x => x.locale == locale);
        if (lang.name != null)
            return lang.name;
        else
            return "English";
    }

    public static string[] GetAllLocales()
    {
        string translationspath = Application.streamingAssetsPath + "/translations/";
        List<string> translations = new List<string>();
        foreach (string translation in Directory.GetFiles(translationspath))
        {
            if (new FileInfo(translation).Length <= 2)
                continue;

            string locale = FileToLocale(translation.Replace(translationspath, ""));

            if(locale != null)
                translations.Add(locale);
        }
        return translations.ToArray();
    }
    public static string FileToLocale(string file)
    {
        if (Path.GetExtension(file) != ".ini")
            return null;

        return file.Replace(".ini", "");
    }

    public void SendUpdateEvent()
    {
        UpdateContent?.Invoke();
    }

}
