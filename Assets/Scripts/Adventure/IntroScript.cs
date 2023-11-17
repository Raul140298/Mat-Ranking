using UnityEngine;
using PixelCrushers.DialogueSystem;

public class IntroScript : MonoBehaviour
{
    [SerializeField] private GameObject hand;
    [SerializeField] private ProximitySelector proximitySelector;
    [SerializeField] private RenderingScript compRendering;

    public void StartTutorial()
    {
        Debug.Log("Start Tutorial");
        proximitySelector.UseCurrentSelection();
    }

    public void FinishTutorial()
    {
        PlayerSessionInfo.tutorial = true;
        GameManager.SaveSystem.SaveLocal();
    }

    public void OutlineOff()
    {
        compRendering.OutlineOff();
    }

    public void ShowHowToMove()
    {
        hand.SetActive(true);
    }

    public void HideHowToMove()
    {
        hand.SetActive(false);
    }
}
