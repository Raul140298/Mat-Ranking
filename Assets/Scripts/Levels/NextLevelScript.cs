using UnityEngine;

public class NextLevelScript : MonoBehaviour
{
    [SerializeField] private SpriteRenderer usableSprite;

    public void activeUsableUI()
    {
        if (PlayerLevelInfo.playerKeyParts < 3)
        {
            SoundsManager.PlaySound("LOCK");
            usableSprite.enabled = true;
        }
    }
}
