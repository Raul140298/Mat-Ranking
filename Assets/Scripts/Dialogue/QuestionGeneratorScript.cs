using PixelCrushers.DialogueSystem;
using UnityEngine;

public class QuestionGeneratorScript : MonoBehaviour
{
    // Start is called before the first frame update
    public DialogueDatabase dataBase;

    void Start()
    {
        
    }

    public void initQuestion()
	{
        //The 1 QUESTION:
        //dataBase.conversations[0].dialogueEntries[1].DialogueText = "Cuanto es 2 * 2 + 2 / 2";

        ////The 4 ANSWERS: ELEMENT 2 IS THE CORRECT ANSWER
        //dataBase.conversations[0].dialogueEntries[2].DialogueText = "3";
        //dataBase.conversations[0].dialogueEntries[3].DialogueText = "4";
        //dataBase.conversations[0].dialogueEntries[4].DialogueText = "5";
        //dataBase.conversations[0].dialogueEntries[5].DialogueText = "6";
        //dataBase.conversations[0].dialogueEntries[6].DialogueText = "Esa era la respuesta";

        //dataBase.conversations[0].dialogueEntries[2].outgoingLinks.Clear();
        //dataBase.conversations[0].dialogueEntries[3].outgoingLinks.Clear();
        //dataBase.conversations[0].dialogueEntries[4].outgoingLinks.Clear();
        //dataBase.conversations[0].dialogueEntries[5].outgoingLinks.Clear();

        //int x = Random.Range(2, 6);
        //Link link = new Link(dataBase.conversations[0].id, dataBase.conversations[0].dialogueEntries[x].id, dataBase.conversations[0].id, dataBase.conversations[0].dialogueEntries[6].id);
        //link.isConnector = true;

        //dataBase.conversations[0].dialogueEntries[x].outgoingLinks.Add(link);

        //Shuffle answers
    }
}
