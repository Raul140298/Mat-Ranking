using UnityEngine;

public class IntroScript : MonoBehaviour
{
    public PlayerSO player;
    public GameSystemScript gameSystem;
    public GameObject hand;

    void Start()
    {
        if (player.tutorial == true) this.gameObject.SetActive(false);
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
