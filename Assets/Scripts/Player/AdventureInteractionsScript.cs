using System.Collections;
using UnityEngine;

public class AdventureInteractionsScript : MonoBehaviour
{
    [SerializeField] private GameObject currentNPC;
    [SerializeField] private PlayerRendererScript playerRenderer;
    [SerializeField] private DialogueCameraScript dialogueCamera;

    private void Start()
    {
        StartCoroutine(CRTInit());
    }

    IEnumerator CRTInit()
    {
        yield return new WaitForSeconds(0.5f);
        if (currentNPC) currentNPC.GetComponent<NpcDialogueAreaScript>().Btn.interactable = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "NPCDialogue")
        {
            if (collision.gameObject.name == "Tower")
            {
                currentNPC = collision.gameObject;
                dialogueCamera.Target = null;
            }
            else
            {
                LookTarget(collision.gameObject);
                currentNPC = collision.gameObject;
                dialogueCamera.Target = currentNPC;
            }
        }
    }

    //Functions
    private void LookTarget(GameObject target)
    {

        if (this.gameObject.transform.position.x > target.gameObject.transform.position.x && !playerRenderer.PlayerIsLookingLeft())
        {
            playerRenderer.SpriteRenderer.flipX = true;
        }
        else if (this.gameObject.transform.position.x < target.gameObject.transform.position.x && playerRenderer.PlayerIsLookingLeft())
        {
            playerRenderer.SpriteRenderer.flipX = false;
        }
    }

    public void LookPlayer()
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

    private void UsableOn()
    {
        currentNPC.transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = true;
        currentNPC.transform.parent.GetComponent<OutlineScript>().OutlineOn();
    }
}
