using UnityEngine;
using PixelCrushers.DialogueSystem;
using UnityEngine.SceneManagement;
using System.Collections;

public class ShuffleResponsesScript : MonoBehaviour
{
    private int idWrong;

    void OnConversationResponseMenu(Response[] responses)
    {
        var currentEntry = DialogueManager.currentConversationState.subtitle.dialogueEntry;

        if (SceneManager.GetActiveScene().buildIndex == 2 && currentEntry.conversationID != 10) // If is level scene
        {
            int n = responses.Length; // Standard Fisher-Yates shuffle algorithm.

            int[] aux = new int[4] { 0, 1, 2, 3 };

            for (int i = 0; i < n; i++)
            {
                int r = i + Random.Range(0, n - i);
                Response temp = responses[r];
                responses[r] = responses[i];
                responses[i] = temp;

                int temp2 = aux[r];
                aux[r] = aux[i]; ;
                aux[i] = temp2;
            }

            StartCoroutine(CRTFitColors(aux));

            for (int j = 0; j < n; j++)
            {
                if (aux[j] != 0)
                {
                    idWrong = j;
                    break;
                }
            }

            DialogueManager.customResponseTimeoutHandler = ChooseWrongResponse;
        }
    }

    IEnumerator CRTFitColors(int[] aux)
    {
        yield return new WaitForSeconds(0.2f);

        LevelScript.Instance.FitEnemyColors(aux);
    }

    void ChooseWrongResponse()
    {
        DialogueManager.standardDialogueUI.OnClick(DialogueManager.currentConversationState.pcResponses[idWrong]);
    }
}