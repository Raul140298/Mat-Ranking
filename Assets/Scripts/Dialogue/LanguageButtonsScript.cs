using UnityEngine;
using PixelCrushers.DialogueSystem;
using UnityEngine.UI;

public class LanguageButtonsScript : MonoBehaviour
{
    [SerializeField] private Text spanish, english, quechua;
    [SerializeField] private OptionsSO options;

    public void Start()
    {
        setActiveLanguage();
    }

    public void setSpanish()
    {
        DialogueManager.SetLanguage("es");
        setActiveLanguage();
    }

    public void setEnglish()
    {
        DialogueManager.SetLanguage("en");
        setActiveLanguage();
    }

    public void setQuechua()
    {
        DialogueManager.SetLanguage("qu");
        setActiveLanguage();
    }

    public void setActiveLanguage()
    {
        switch (Localization.language)
        {
            case "es":
                spanish.color = Color.white;
                english.color = Color.gray;
                quechua.color = Color.gray;
                break;
            case "en":
                spanish.color = Color.gray;
                english.color = Color.white;
                quechua.color = Color.gray;
                break;
            case "qu":
                spanish.color = Color.gray;
                english.color = Color.gray;
                quechua.color = Color.white;
                break;
            default:
                // code block
                break;
        }
    }
}
