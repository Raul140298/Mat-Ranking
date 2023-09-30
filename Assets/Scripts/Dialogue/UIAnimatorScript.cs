using UnityEngine;
using UnityEngine.UI;

public class UIAnimatorScript : MonoBehaviour
{
    //An array of sprites for the animation with a duration between sprites
    [SerializeField] private Sprite[] sprites;
	[SerializeField] private Image image;
	[SerializeField]  float duration;
    
	private int index = 0;
    private float timer = 0;
    
    private void Update()
    {
    	if ((timer += Time.deltaTime) >= (duration / sprites.Length))
    	{
    		timer = 0;
    		image.sprite = sprites[index];
    		index = (index + 1) % sprites.Length;
    	}
    }
}
