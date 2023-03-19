using UnityEngine;
using PixelCrushers.DialogueSystem;

public class IntroScript : MonoBehaviour
{
    [SerializeField] private GameObject hand;
    [SerializeField] private ProximitySelector proximitySelector;

    public void StartTutorial()
    {
        Debug.Log("Start Tutorial");
        proximitySelector.UseCurrentSelection();
    }

    public void FinishTutorial()
    {
        GameSystemScript.PlayerSO.tutorial = true;
        GameSystemScript.SaveSystem.SaveLocal();
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
