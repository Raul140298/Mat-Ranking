using UnityEngine;
using PixelCrushers.DialogueSystem;

public class IntroScript : MonoBehaviour
{
    [SerializeField] private PlayerSO player;
    [SerializeField] private GameSystemScript gameSystem;
    [SerializeField] private GameObject hand;
    [SerializeField] private ProximitySelector proximitySelector;
    [SerializeField] private PlayFabScript playFab;
    [SerializeField] private GameObject ranking, nameCreation, rank, ui;

    public void StartSystem()
    {
        if (player.tutorial == true) this.gameObject.SetActive(false);
    }

    public void StartTutorial()
    {
        Debug.Log("Start Tutorial");
        proximitySelector.UseCurrentSelection();
    }

    public void FinishTutorial()
    {
        player.tutorial = true;
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
