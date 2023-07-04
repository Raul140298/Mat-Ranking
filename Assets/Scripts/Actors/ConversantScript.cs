using PixelCrushers.DialogueSystem;
using UnityEngine;

public class ConversantScript : MonoBehaviour
{
    [Header("INFO")]
    [SerializeField] protected RenderingScript compRendering;

    [Header("CONVERSATION")]
    [SerializeField] protected DialogueSystemTrigger dialogue;
    [SerializeField] protected SpriteRenderer reticle;

    public void UseCurrentSelection()
    {
        AdventureScript.Instance.Player.UseCurrentSelection();
    }

    public RenderingScript RenderingScript => compRendering;
}
