using Febucci.UI;
using UnityEngine;
using PixelCrushers.DialogueSystem;

public class DialoguePanelManager : MonoBehaviour
{
    private static TimerScript timer;
    private static Animator dialoguePanel;
    private static TypewriterByCharacter typeWriter;
    private static TextAnimator_TMP textAnimator;
    private static int idWrong;

    private void Start()
    {
        timer = this.transform.GetChild(0).transform.GetChild(0).transform.GetChild(1).transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).gameObject.GetComponent<TimerScript>();
        dialoguePanel = GameObject.FindGameObjectWithTag("DialoguePanel").transform.GetChild(1).GetComponent<Animator>();
        typeWriter = this.transform.GetChild(0).transform.GetChild(0).transform.GetChild(1).transform.GetChild(0).transform.GetChild(0).gameObject.GetComponent<TypewriterByCharacter>();
        textAnimator = this.transform.GetChild(0).transform.GetChild(0).transform.GetChild(1).transform.GetChild(0).transform.GetChild(0).gameObject.GetComponent<TextAnimator_TMP>();
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
    
    public static void ChooseWrongResponse()
    {
        DialogueManager.standardDialogueUI.OnClick(DialogueManager.currentConversationState.pcResponses[idWrong]);
    }

    // GETTERS ---------------------------------------------------------------------------------

    public static int IdWrong
    {
        set { idWrong = value; }
        get { return idWrong; }
    }
    public static bool IsDialogueFinished => textAnimator.allLettersShown;
    public static TypewriterByCharacter TypeWritter => typeWriter;
    public static TimerScript Timer => timer;
    public static Animator DialoguePanel => dialoguePanel;
}