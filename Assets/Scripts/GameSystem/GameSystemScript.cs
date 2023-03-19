using UnityEngine;
using UnityEngine.UI;
using PixelCrushers.DialogueSystem;
using UnityEngine.SceneManagement;

public class GameSystemScript : MonoBehaviour
{
    [Header("Systems")]
    [SerializeField] private SaveSystemScript saveSystem;
    [SerializeField] private static SaveSystemScript saveSystemStatic;

    [SerializeField] private GooglePlaySystemScript googlePlaySystem;
    [SerializeField] private static GooglePlaySystemScript googlePlaySystemStatic;

    private GameObject dialogueManager;
    private static DialogueSystemController dialogueSystem;
    private static GameObject timer;
    private static Animator dialoguePanel;

    [Header("Scriptable Objects")]
    [SerializeField] private PlayerSO playerSO;
    private static PlayerSO playerSOStatic;

    [SerializeField] private OptionsSO optionsSO;
    private static OptionsSO optionsSOStatic;

    [SerializeField] private RemoteSO remoteSO;
    private static RemoteSO remoteSOStatic;

    [SerializeField] private CurrentLevelSO currentLevelSO;
    private static CurrentLevelSO currentLevelSOStatic;

    [SerializeField] private FromLevelSO fromLevelSO;
    private static FromLevelSO fromLevelSOStatic;

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

    private void Awake()
    {
        GameObject[] objs = GameObject.FindGameObjectsWithTag("GameSystem");
        if (objs.Length > 1)
        {
            Destroy(this);
        }
        else
        {
            DontDestroyOnLoad(this);
        }

        saveSystemStatic = saveSystem;
        remoteSOStatic = remoteSO;
        currentLevelSOStatic = currentLevelSO;
        fromLevelSOStatic = fromLevelSO;
        optionsSOStatic = optionsSO;
        playerSOStatic = playerSO;

        myPhraseList = JsonUtility.FromJson<PhraseList>(textJSON.text);

        if (saveSystem) saveSystem.AwakeSystem();
    }

    private void Start()
    {
        Application.targetFrameRate = 60;

        //AudioConfiguration config = AudioSettings.GetConfiguration();
        //config.dspBufferSize = 64;
        //AudioSettings.Reset(config);

        int nScene = SceneManager.GetActiveScene().buildIndex;

        dialogueManager = GameObject.FindGameObjectWithTag("DialogueManager");
        dialogueSystem = dialogueManager.GetComponent<DialogueSystemController>();
        timer = dialogueManager.transform.GetChild(0).transform.GetChild(3).transform.GetChild(1).transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).gameObject;
        dialoguePanel = GameObject.FindGameObjectWithTag("DialoguePanel").transform.GetChild(1).GetComponent<Animator>();

        // SYSTEMS ----------------------------------------------------------------

        if (saveSystem) saveSystem.StartSystem(dialogueManager);
        if (googlePlaySystem) googlePlaySystem.StartSystem();
    }

    // SOUNDS ------------------------------------------------------------------------

    public static void StartSounds(Slider slider)
    {
        SoundsScript.Slider = slider;
        slider.value = optionsSOStatic.soundsVolume;
        SoundsScript.ChangeVolume(optionsSOStatic.soundsVolume);
        slider.onValueChanged.AddListener(val => SoundsScript.ChangeVolume(val));
    }

    public static void StartSoundtracks(Slider slider)
    {
        SoundtracksScript.Slider = slider;
        slider.value = optionsSOStatic.soundtracksVolume;
        SoundtracksScript.ChangeVolume(optionsSOStatic.soundtracksVolume);
        slider.onValueChanged.AddListener(val => SoundtracksScript.ChangeVolume(val));
    }

    // GOOGLE PLAY --------------------------------------------------------------------------

    public static void ShowRanking()
    {
        googlePlaySystemStatic.ShowRanking();
    }

    public static void SendRanking()
    {
        googlePlaySystemStatic.SendRanking(playerSOStatic.knowledgePoints);
    }

    // UI --------------------------------------------------------------------------

    public static void SetKnowledgePoints(Text knowledgePoints)
    {
        knowledgePoints.text = playerSOStatic.knowledgePoints.ToString("D3");
    }

    public static void ChangeKnowledgePoints(int n, Text knowledgePoints)
    {
        if (playerSOStatic.knowledgePoints + n >= 0)
        {
            playerSOStatic.knowledgePoints += n;

            //Connection to bd on PlayFab
            SendRanking();
            SetKnowledgePoints(knowledgePoints);
            SaveSystem.SaveLocal();
        }
    }

    public static void ResetPlayerCurrentLevel()
    {
        currentLevelSOStatic.playerLives = 3;
        currentLevelSOStatic.currentLevel = 1;
        currentLevelSOStatic.totalQuestions = 0;
        currentLevelSOStatic.correctAnswers = 0;
        currentLevelSOStatic.timePerQuestion = 0;
    }

    public static void NextPlayerCurrentLevel()
    {
        currentLevelSOStatic.currentLevel += 1;
    }

    public static void PrevPlayerCurrentLevel()
    {
        currentLevelSOStatic.currentLevel -= 1;
    }

    // GETTERS ---------------------------------------------------------------------------------

    public static SaveSystemScript SaveSystem => saveSystemStatic;
    public static RemoteSO RemoteSO => remoteSOStatic;
    public static CurrentLevelSO CurrentLevelSO => currentLevelSOStatic;
    public static FromLevelSO FromLevelSO => fromLevelSOStatic;
    public static OptionsSO OptionsSO => optionsSOStatic;
    public static PlayerSO PlayerSO => playerSOStatic;

    public static DialogueSystemController DialogueSystem => dialogueSystem;
    public static GameObject Timer => timer;
    public static Animator DialoguePanel => dialoguePanel;
    public static PhraseList MyPhraseList => myPhraseList;
}