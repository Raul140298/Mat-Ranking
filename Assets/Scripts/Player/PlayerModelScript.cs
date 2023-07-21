using System.Collections;
using UnityEngine;
using PixelCrushers.DialogueSystem;

public class PlayerModelScript : MonoBehaviour
{
    [Header("INFO")]
    [SerializeField] private ProximitySelector proximitySelector;
    [SerializeField] private PlayerRendererScript playerRenderer;
    [SerializeField] private RenderingScript compRendering;

    [Header("CAMERA")]
    [SerializeField] private DialogueCameraScript dialogueCamera;

    [Header("INTERACTABLES")]
    [SerializeField] private NpcScript currentNPC;
    [SerializeField] private ClickableScript clickable;

    private void Start()
    {
        StartCoroutine(CRTInit());
    }

    IEnumerator CRTInit()
    {
        yield return new WaitForSeconds(0.5f);
        if (clickable) clickable.MakeClickable();
    }

    public void UseCurrentSelection()
    {
        LookPlayer();
        proximitySelector.UseCurrentSelection();
    }

    public void CheckIfNpcWantToTalk()
    {
        if (currentNPC && currentNPC.name != "Tower")
        {
            currentNPC.CheckIfWantToTalk();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        CheckDialoguerEnter(collision);
    }

    private void CheckDialoguerEnter(Collider2D collision)
    {
        if (collision.tag == "NPCDialogue")
        {
            if (collision.gameObject.name != "Tower")
            {
                LookTarget(collision.gameObject);
                currentNPC = collision.transform.parent.GetComponent<NpcScript>();
                dialogueCamera.Target = currentNPC.gameObject;
            }

            clickable = collision.transform.parent.GetComponent<ClickableScript>();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        CheckDialoguerExit(collision);
    }

    private void CheckDialoguerExit(Collider2D collision)
    {
        if (collision.tag == "NPCDialogue")
        {
            if (currentNPC == collision.transform.parent.GetComponent<NpcScript>())
            {
                dialogueCamera.Target = null;
                currentNPC = null;
            }
        }
    }

    //Functions
    private void LookTarget(GameObject target)
    {
        if (this.gameObject.transform.position.x > target.gameObject.transform.position.x &&
            !playerRenderer.PlayerIsLookingLeft())
        {
            playerRenderer.SpriteRenderer.flipX = true;
        }
        else if (this.gameObject.transform.position.x < target.gameObject.transform.position.x &&
            playerRenderer.PlayerIsLookingLeft())
        {
            playerRenderer.SpriteRenderer.flipX = false;
        }
    }

    public void LookPlayer()
    {
        bool isLookingPlayer = this.gameObject.transform.position.x < currentNPC.transform.position.x;
        currentNPC.RenderingScript.FlipX(isLookingPlayer);
    }

    public void MakeDialoguerClickable()
    {
        clickable.MakeClickable();
    }

    public void MakeDialoguerNonClickable()
    {
        clickable.MakeNonClickable();
    }

    public RenderingScript CompRendering => compRendering;
}
