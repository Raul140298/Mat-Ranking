using UnityEngine;
using UnityEngine.UI;
using PixelCrushers.DialogueSystem;
using UnityEngine.SceneManagement;

public class GameSystemScript : MonoBehaviour
{
    private static GameSystemScript instance;

    [Header("Systems")]
    [SerializeField] private PlayFabScript playFab;
    [SerializeField] private SaveSystemScript saveSystem;
    [SerializeField] private GooglePlaySystemScript googlePlaySystem;
    private GameObject dialogueManager;
    private DialogueSystemController dialogueSystem;
    private GameObject timer;
    private Animator dialoguePanel;

    [Header("Scriptable Objects")]
    [SerializeField] private PlayerSO playerSO;
    [SerializeField] private OptionsSO optionsSO;
    [SerializeField] private RemoteSO remoteSO;
    [SerializeField] private CurrentLevelSO currentLevelSO;
    [SerializeField] private FromLevelSO fromLevelSO;

    [Header("UI")]
    [SerializeField] private TextAsset textJSON;
    private PhraseList myPhraseList;

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
        if (instance == null)
        {
            DontDestroyOnLoad(this);
            instance = this;
        }
        else
        {
            Destroy(this);
        }

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
        if (playFab) playFab.StartSystem();
    }

    // SOUNDS ------------------------------------------------------------------------

    public void StartSounds(Slider slider)
    {
        SoundsScript.Slider = slider;
        slider.value = optionsSO.soundsVolume;
        SoundsScript.ChangeVolume(optionsSO.soundsVolume);
        slider.onValueChanged.AddListener(val => SoundsScript.ChangeVolume(val));
    }

    public void StartSoundtracks(Slider slider)
    {
        SoundtracksScript.Slider = slider;
        slider.value = optionsSO.soundtracksVolume;
        SoundtracksScript.ChangeVolume(optionsSO.soundtracksVolume);
        slider.onValueChanged.AddListener(val => SoundtracksScript.ChangeVolume(val));
    }

    // UI --------------------------------------------------------------------------

    public void ShowRanking()
    {
        googlePlaySystem.ShowRanking();
    }

    public void SendRanking()
    {
        googlePlaySystem.SendRanking(playerSO.knowledgePoints);
        playFab.SendRanking(playerSO.knowledgePoints);
    }

    public void SetKnowledgePoints(Text knowledgePoints)
    {
        knowledgePoints.text = playerSO.knowledgePoints.ToString("D3");
    }

    public void ChangeKnowledgePoints(int n, Text knowledgePoints)
    {
        if (playerSO.knowledgePoints + n >= 0)
        {
            playerSO.knowledgePoints += n;

            //Connection to bd on PlayFab
            SendRanking();
            SetKnowledgePoints(knowledgePoints);
            SaveSystem.SaveLocal();
        }
    }

    public void ResetPlayerCurrentLevel()
    {
        currentLevelSO.playerLives = 3;
        currentLevelSO.currentLevel = 1;
        currentLevelSO.totalQuestions = 0;
        currentLevelSO.correctAnswers = 0;
        currentLevelSO.timePerQuestion = 0;
    }

    public void NextPlayerCurrentLevel()
    {
        currentLevelSO.currentLevel += 1;
    }

    public void PrevPlayerCurrentLevel()
    {
        currentLevelSO.currentLevel -= 1;
    }

    // GETTERS ---------------------------------------------------------------------------------

    public SaveSystemScript SaveSystem => saveSystem;
    public RemoteSO RemoteSO => remoteSO;
    public CurrentLevelSO CurrentLevelSO => currentLevelSO;
    public FromLevelSO FromLevelSO => fromLevelSO;
    public OptionsSO OptionsSO => optionsSO;
    public PlayerSO PlayerSO => playerSO;
    public DialogueSystemController DialogueSystem => dialogueSystem;
    public GameObject Timer => timer;
    public Animator DialoguePanel => dialoguePanel;
    public PhraseList MyPhraseList => myPhraseList;

    void OnDestroy()
    {
        instance = null;
    }

    public static GameSystemScript Instance
    {
        get { return instance; }
    }
}