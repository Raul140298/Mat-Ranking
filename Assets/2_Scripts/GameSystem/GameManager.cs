using UnityEngine;
using UnityEngine.UI;
using PixelCrushers.DialogueSystem;
using UnityEngine.Serialization;

public class GameManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextAsset textJSON;
    private static PhraseList myPhraseList;

    [System.Serializable]
    public class Phrase
    {
        public string id;
        public string autor;
        public string frase;
        public string phrase;
    }

    [System.Serializable]
    public class PhraseList
    {
        public Phrase[] phrases;
    }

    private GameObject dialogueManager;
    private static DialogueSystemController dialogueSystem;
    private static TimerScript timer;
    private static Animator dialoguePanel;
    private static GameManager instance;
    
    void Awake()
    {
        InitializeSingleton();
        
        myPhraseList = JsonUtility.FromJson<PhraseList>(textJSON.text);
    }
	
    private void InitializeSingleton()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        Application.targetFrameRate = 60;

        dialogueManager = GameObject.FindGameObjectWithTag("DialogueManager");
        dialogueSystem = dialogueManager.GetComponent<DialogueSystemController>();
        timer = dialogueManager.transform.GetChild(0).transform.GetChild(0).transform.GetChild(1).transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).gameObject.GetComponent<TimerScript>();
        dialoguePanel = GameObject.FindGameObjectWithTag("DialoguePanel").transform.GetChild(1).GetComponent<Animator>();
    }

    // UI --------------------------------------------------------------------------

    public static void SetContinueButtonAlways()
    {
        dialogueSystem.displaySettings.subtitleSettings.continueButton = DisplaySettings.SubtitleSettings.ContinueButtonMode.Always;

    }

    public static void SetContinueButtonNever()
    {
        dialogueSystem.displaySettings.subtitleSettings.continueButton = DisplaySettings.SubtitleSettings.ContinueButtonMode.Never;
    }

    public static void ChangeKnowledgePoints(int n, Text knowledgePoints)
    {
        if (PlayerSessionInfo.knowledgePoints + n >= 0)
        {
            PlayerSessionInfo.knowledgePoints += n;
            GooglePlayManager.OpenSavedGameForSave("MatRanking");
            
            GooglePlayManager.SendRanking(PlayerSessionInfo.knowledgePoints);
            SetKnowledgePoints(knowledgePoints);
        }
    }
    
    public static void SetKnowledgePoints(Text knowledgePoints)
    {
        knowledgePoints.text = PlayerSessionInfo.knowledgePoints.ToString("D3");
    }

    // GETTERS ---------------------------------------------------------------------------------
    
    public static DialogueSystemController DialogueSystem => dialogueSystem;
    public static TimerScript Timer => timer;
    public static Animator DialoguePanel => dialoguePanel;
    public static PhraseList MyPhraseList => myPhraseList;
}