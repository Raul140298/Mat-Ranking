using PixelCrushers.DialogueSystem;
using System.Collections;
using UnityEngine;

public class AdventureInteractionsScript : MonoBehaviour
{
	public GameObject currentNPC;
	public PlayerRendererScript playerRenderer;
	public DialogueCameraScript dialogueCamera;

	private void Start()
	{
		StartCoroutine(init());
	}

	IEnumerator init()
	{
		yield return new WaitForSeconds(0.5f);
		if(currentNPC)currentNPC.GetComponent<NpcDialogueAreaScript>().btn.interactable = true;
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.tag == "NPCDialogue")
		{
			if(collision.gameObject.name == "Tower")
			{
				currentNPC = collision.gameObject;
				dialogueCamera.target = null;
			}
			else
			{
				lookTarget(collision.gameObject);
				currentNPC = collision.gameObject;
				dialogueCamera.target = currentNPC;
			}
		}
	}

	//Functions
	public void lookTarget(GameObject target)
	{

		if (this.gameObject.transform.position.x > target.gameObject.transform.position.x && !playerRenderer.PlayerIsLookingLeft())
		{
			playerRenderer.spriteRenderer.flipX = true;
		}
		else if (this.gameObject.transform.position.x < target.gameObject.transform.position.x && playerRenderer.PlayerIsLookingLeft())
		{
			playerRenderer.spriteRenderer.flipX = false;
		}
	}

	public void lookPlayer()
	{

		if (this.gameObject.transform.position.x > currentNPC.gameObject.transform.position.x)
		{
			currentNPC.transform.parent.GetComponent<SpriteRenderer>().flipX = false;
		}
		else if (this.gameObject.transform.position.x < currentNPC.gameObject.transform.position.x)
		{
			currentNPC.transform.parent.GetComponent<SpriteRenderer>().flipX = true;
		}
	}

	public void UsableOn()
	{
		currentNPC.transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = true;
		currentNPC.transform.parent.GetComponent<OutlineScript>().OutlineOn();
	}
}
