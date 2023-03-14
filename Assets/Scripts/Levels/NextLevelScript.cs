using UnityEngine;

public class NextLevelScript : MonoBehaviour
{
    [SerializeField] private CurrentLevelSO currentLevel;
    [SerializeField] private SpriteRenderer usableSprite;

    public void activeUsableUI()
    {
        if (currentLevel.playerKeyParts < 3)
        {
            SoundsScript.PlaySound("LOCK");
            usableSprite.enabled = true;
        }
    }
}
