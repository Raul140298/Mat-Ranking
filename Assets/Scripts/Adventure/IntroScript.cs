using UnityEngine;

public class IntroScript : MonoBehaviour
{
    public PlayerSO player;

    void Start()
    {
        if (player.tutorial == true) this.gameObject.SetActive(false);
    }
}
