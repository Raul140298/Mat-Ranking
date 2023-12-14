using UnityEngine;
using PixelCrushers.DialogueSystem;

public class DialoguePanelManager : MonoBehaviour
{
    private static TimerScript timer;
    private static Animator dialoguePanel;

    private void Start()
    {
        timer = this.transform.GetChild(0).transform.GetChild(0).transform.GetChild(1).transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).gameObject.GetComponent<TimerScript>();
        dialoguePanel = GameObject.FindGameObjectWithTag("DialoguePanel").transform.GetChild(1).GetComponent<Animator>();
    }

    // UI --------------------------------------------------------------------------

    public static void SetContinueButtonAlways()
    {
        DialogueManager.displaySettings.subtitleSettings.continueButton = DisplaySettings.SubtitleSettings.ContinueButtonMode.Always;
    }

    public static void SetContinueButtonNever()
    {
        DialogueManager.displaySettings.subtitleSettings.continueButton = DisplaySettings.SubtitleSettings.ContinueButtonMode.Never;
    }

    // GETTERS ---------------------------------------------------------------------------------
    
    public static TimerScript Timer => timer;
    public static Animator DialoguePanel => dialoguePanel;
}