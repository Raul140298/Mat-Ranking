using UnityEngine;
using PixelCrushers.DialogueSystem;
using UnityEngine.SceneManagement;

public class ShuffleResponsesScript : MonoBehaviour
{
    void OnConversationResponseMenu(Response[] responses)
    {
        var currentEntry = DialogueManager.currentConversationState.subtitle.dialogueEntry;

        if (SceneManager.GetActiveScene().buildIndex == 2 && currentEntry.conversationID != 10) // If is level scene
        {
            int n = responses.Length; // Standard Fisher-Yates shuffle algorithm.
            for (int i = 0; i < n; i++)
            {
                int r = i + Random.Range(0, n - i);
                Response temp = responses[r];
                responses[r] = responses[i];
                responses[i] = temp;
            }
        }
    }
}