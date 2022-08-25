using UnityEngine;
using UnityEngine.UI;

public class NpcDialogueAreaScript : MonoBehaviour
{
	public Button btn;

	private void Start()
	{
		btn = this.transform.parent.GetChild(1).GetChild(0).GetComponent<Button>();
		Invoke("resetBtn", 0.1f);
	}

	private void resetBtn()
	{
		btn.interactable = false;
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		btn.interactable = true;
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		btn.interactable = false;
	}
}
