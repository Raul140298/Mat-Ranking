using System.Collections;
using UnityEngine;
using PixelCrushers.DialogueSystem;

public class PlayerModelScript : MonoBehaviour
{
    [SerializeField] private ProximitySelector proximitySelector;
    [SerializeField] private NpcScript currentNPC;
    [SerializeField] private PlayerRendererScript playerRenderer;
    [SerializeField] private DialogueCameraScript dialogueCamera;
    [SerializeField] private RenderingScript compRendering;

    private void Start()
    {
        StartCoroutine(CRTInit());
    }

    IEnumerator CRTInit()
    {
        yield return new WaitForSeconds(0.5f);
        if (currentNPC) currentNPC.NpcDialogueArea.Btn.interactable = true;
    }

    public void UseCurrentSelection()
    {
        LookPlayer();
        proximitySelector.UseCurrentSelection();
    }

    public void CheckIfSpeakerWantToTalk()
    {
        if (currentNPC && currentNPC.name != "Tower")
        {
            currentNPC.CheckIfWantToTalk();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "NPCDialogue")
        {
            if (collision.gameObject.name != "Tower")
            {
                LookTarget(collision.gameObject);
                currentNPC = collision.transform.parent.GetComponent<NpcScript>();
                dialogueCamera.Target = currentNPC.gameObject;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
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

    public void ShowSpeakerOutline()
    {
        if (currentNPC) currentNPC.RenderingScript.OutlineOn();
    }

    public RenderingScript CompRendering => compRendering;
}
