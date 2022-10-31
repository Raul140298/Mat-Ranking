using UnityEngine;

public class HearthAnimationScript : MonoBehaviour
{
	//public float duration;

	////An array of sprites for the animation with a duration between sprites
	//[SerializeField] private Sprite[] sprites;

	//private Image image;
	//private int index = 0;
	//private float timer = 0;

	//void Start()
	//{
	//	image = GetComponent<Image>();
	//}
	//private void Update()
	//{
	//	if ((timer += Time.deltaTime) >= (duration / sprites.Length))
	//	{
	//		timer = 0;
	//		image.sprite = sprites[index];
	//		index = (index + 1) % sprites.Length;
	//	}
	//}

	private RectTransform rt;
	public float origin, amplitude, position, velocity, angle, anglePosition;
	private float timer;

	void Start()
	{
		rt = GetComponent<RectTransform>();
	}

	private void Update()
	{
		timer += Time.deltaTime;

		RectTransformExtensionsScript.SetPosY(rt, origin + amplitude * Mathf.Sin(position * Mathf.PI + timer * velocity));

		RectTransformExtensionsScript.SetRotationZ(rt, angle * Mathf.Sin(anglePosition * Mathf.PI + timer * velocity));
	}
}
