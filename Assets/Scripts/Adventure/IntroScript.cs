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

    void Start()
    {
        if (player.tutorial == true) this.gameObject.SetActive(false);
    }

    public void startTutorial()
    {
        Debug.Log("Start Tutorial");
        proximitySelector.UseCurrentSelection();
    }

    public void finishTutorial()
    {
        player.tutorial = true;
        gameSystem.SaveSystem.saveLocal();
    }

    public void showHowToMove()
    {
        hand.SetActive(true);
    }

    public void hideHowToMove()
    {
        hand.SetActive(false);
    }
}
