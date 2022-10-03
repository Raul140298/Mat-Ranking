using UnityEngine;

public class NextLevelScript : MonoBehaviour
{
    public CurrentLevelSO currentLevel;
    public SpriteRenderer usableSprite;

    public void activeUsableUI()
    {
        if (currentLevel.playerKeyParts < 3)
        {
            //SoundsScript.PlaySound("LOCK");
			usableSprite.enabled = true;
		}

	}
}
