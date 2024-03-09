using PixelCrushers.DialogueSystem;
using UnityEngine;

public class NpcScript : MonoBehaviour
{
    [Header("INFO")]
    [SerializeField] private NpcSO npcData;
    [SerializeField] private RenderingScript compRendering;

    [Header("CONVERSATION")]
    [SerializeField] private DialogueSystemTrigger dialogue;
    [SerializeField] private SpriteRenderer reticle;
    [SerializeField] private NpcDialogueAreaScript npcDialogueArea;

    private bool wantToTalk;

    private void Awake()
    {
        dialogue.conversation = "Conversation " + npcData.npcName;

        //compRendering.SetAnimations(npcData.animations);
    }

    private void Start()
    {
        compRendering.OutlineOff();

        compRendering.PlayAnimation(eAnimation.Idle);
        CheckIfWantToTalk();
    }

    public void CheckIfWantToTalk()
    {
        wantToTalk = DialogueLua.GetVariable(npcData.npcName + "ConversationState").asInt >= 0;
        reticle.enabled = wantToTalk;
    }

    public void UseCurrentSelection()
    {
        AdventureController.Instance.Player.UseCurrentSelection();
    }

    public NpcDialogueAreaScript NpcDialogueArea
    {
        get { return npcDialogueArea; }
        set { npcDialogueArea = value; }
    }

    public RenderingScript RenderingScript => compRendering;
}
