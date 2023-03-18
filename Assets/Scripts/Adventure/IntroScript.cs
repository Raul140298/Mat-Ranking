using UnityEngine;
using PixelCrushers.DialogueSystem;

public class IntroScript : MonoBehaviour
{
    [SerializeField] private GameObject hand;
    [SerializeField] private ProximitySelector proximitySelector;
    private GameSystemScript gameSystem;

    public void StartIntro()
    {
        gameSystem = GameSystemScript.Instance;

        if (gameSystem.PlayerSO.tutorial == true) this.gameObject.SetActive(false);
    }

    public void StartTutorial()
    {
        Debug.Log("Start Tutorial");
        proximitySelector.UseCurrentSelection();
    }

    public void FinishTutorial()
    {
        gameSystem.PlayerSO.tutorial = true;
        gameSystem.SaveSystem.SaveLocal();
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
