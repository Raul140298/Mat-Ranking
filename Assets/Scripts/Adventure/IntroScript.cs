using UnityEngine;

public class IntroScript : MonoBehaviour
{
    public PlayerSO player;
    public GameSystemScript gameSystem;

    void Start()
    {
        if (player.tutorial == true) this.gameObject.SetActive(false);
    }

    public void finishTutorial()
    {
        player.tutorial = true;
        gameSystem.saveSystem.saveLocal();
    }
}
