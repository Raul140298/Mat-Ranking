using UnityEngine;
using UnityEngine.UI;
using PixelCrushers.DialogueSystem;

public class GameManager : MonoBehaviour
{
    [Header("Systems")]
    [SerializeField] private SaveManager saveSystem;
    private static SaveManager saveSystemStatic;
    
    [Header("Remote")]
    [SerializeField] private RemoteSO remoteSO;
    private static RemoteSO remoteSOStatic;

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

    private void Awake()
    {
        bool exist = GameObject.FindGameObjectsWithTag("GameSystem").Length > 1;

        if (exist)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            DontDestroyOnLoad(this);
        }

        saveSystemStatic = saveSystem;
        remoteSOStatic = remoteSO;
        myPhraseList = JsonUtility.FromJson<PhraseList>(textJSON.text);

        saveSystem.AwakeSystem(remoteSO);
    }

    private void Start()
    {
        Application.targetFrameRate = 60;

        dialogueManager = GameObject.FindGameObjectWithTag("DialogueManager");
        dialogueSystem = dialogueManager.GetComponent<DialogueSystemController>();
        timer = dialogueManager.transform.GetChild(0).transform.GetChild(0).transform.GetChild(1).transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).gameObject.GetComponent<TimerScript>();
        dialoguePanel = GameObject.FindGameObjectWithTag("DialoguePanel").transform.GetChild(1).GetComponent<Animator>();
    }

    // SOUNDS ------------------------------------------------------------------------

    public static void StartSounds(Slider slider)
    {
        SoundsManager.Slider = slider;
        slider.value = PlayerSessionInfo.soundsVolume;
        SoundsManager.ChangeVolume(PlayerSessionInfo.soundsVolume);
        slider.onValueChanged.AddListener(val => SoundsManager.ChangeVolume(val));
    }

    public static void StartSoundtracks(Slider slider)
    {
        SoundtracksManager.Slider = slider;
        slider.value = PlayerSessionInfo.soundtracksVolume;
        SoundtracksManager.ChangeVolume(PlayerSessionInfo.soundtracksVolume);
        slider.onValueChanged.AddListener(val => SoundtracksManager.ChangeVolume(val));
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
            SaveSystem.SaveLocal();
            
            GooglePlayManager.SendRanking(PlayerSessionInfo.knowledgePoints);
            SetKnowledgePoints(knowledgePoints);
        }
    }
    
    public static void SetKnowledgePoints(Text knowledgePoints)
    {
        knowledgePoints.text = PlayerSessionInfo.knowledgePoints.ToString("D3");
    }

    // GETTERS ---------------------------------------------------------------------------------

    public static SaveManager SaveSystem => saveSystemStatic;
    public static RemoteSO RemoteSO => remoteSOStatic;
    public static DialogueSystemController DialogueSystem => dialogueSystem;
    public static TimerScript Timer => timer;
    public static Animator DialoguePanel => dialoguePanel;
    public static PhraseList MyPhraseList => myPhraseList;
}