using UnityEngine;
using PixelCrushers.DialogueSystem;

public class IntroScript : MonoBehaviour
{
    public PlayerSO player;
    public GameSystemScript gameSystem;
    public GameObject hand;
	public ProximitySelector proximitySelector;
    public PlayFabScript playFab;
    public GameObject ranking, nameCreation, rank, ui;

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
        gameSystem.saveSystem.saveLocal();
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
