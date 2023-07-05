using PixelCrushers.DialogueSystem;
using UnityEngine;

public class ContinuousButtonSelectorScript : MonoBehaviour
{
    DialogueSystemController dialogueSystemController;
    // Start is called before the first frame update
    void Start()
    {
        dialogueSystemController = GameSystemScript.DialogueSystem;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "ContinuousButtonSelector")
        {
            dialogueSystemController.displaySettings.subtitleSettings.continueButton = DisplaySettings.SubtitleSettings.ContinueButtonMode.Never;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.name == "ContinuousButtonSelector")
        {
            dialogueSystemController.displaySettings.subtitleSettings.continueButton = DisplaySettings.SubtitleSettings.ContinueButtonMode.Always;
        }
    }
}
