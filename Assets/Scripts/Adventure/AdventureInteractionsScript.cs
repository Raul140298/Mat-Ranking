using UnityEngine;

public class AdventureInteractionsScript : MonoBehaviour
{
	public GameObject currentNPC;
	public PlayerRenderer playerRenderer;
	public dialogueCameraScript dialogueCamera;

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.tag == "NPCDialogue")
		{
			if(collision.gameObject.name == "Tower")
			{
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
}
