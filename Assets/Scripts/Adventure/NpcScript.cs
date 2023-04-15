using PixelCrushers.DialogueSystem;
using UnityEngine;

public class NpcScript : MonoBehaviour
{
    [SerializeField] NpcSO npcData;
    private bool wantToTalk;
    [SerializeField] private DialogueSystemTrigger dialogue;
    [SerializeField] private Animator animator;
    [SerializeField] private OutlineScript outline;
    [SerializeField] private SpriteRenderer reticle;

    private void Awake()
    {
        dialogue.conversation = "Conversation " + npcData.npcName;
        animator.runtimeAnimatorController = npcData.animator;
    }

    private void Start()
    {
        animator.Rebind();
        outline.OutlineOff();

        CheckIfWantToTalk();
    }

    public void CheckIfWantToTalk()
    {
        wantToTalk = DialogueLua.GetVariable(npcData.npcName + "ConversationState").asInt >= 0;
        reticle.enabled = wantToTalk;
    }

    public void UseCurrentSelection()
    {
        AdventureScript.Instance.Player.UseCurrentSelection();
    }
}
