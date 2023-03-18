using UnityEngine;
using UnityEngine.UI;
using PixelCrushers.DialogueSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class GameSystemScript : MonoBehaviour
{
    private static GameSystemScript instance;

    [SerializeField] private LevelScript levelScene;

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

    [SerializeField] private CinemachineShakeScript virtualCamera1;
    [SerializeField] private CinemachineShakeScript virtualCamera2;
    [SerializeField] private DialogueCameraScript dialogueCamera;
    [SerializeField] private GameObject joystick;
    [SerializeField] private Text knowledgePoints;

    [Header("Level")]
    [SerializeField] private LevelInteractionsScript player;
    [SerializeField] private EnemysInZone[] enemysInZone;
    [SerializeField] private LevelGeneratorScript levelGenerator;
    [SerializeField] private BulletGeneratorScript bullets;
    [SerializeField] private GameObject roomEdges;
    [SerializeField] private TilemapCollider2D roomEdgesCollider;
    [SerializeField] private LaserScript laser;

    [System.Serializable]
    public class EnemysInZone
    {
        [SerializeField] private EnemySO[] enemys;

        public EnemySO[] Enemys => enemys;
    }

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
        Debug.Log("HOLI");

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

    public void ChangeKnowledgePoints(int n)
    {
        if (playerSO.knowledgePoints + n >= 0)
        {
            playerSO.knowledgePoints += n;

            //Connection to bd on PlayFab
            SendRanking();
            SetKnowledgePoints();
            saveSystem.SaveLocal();
        }
    }

    public void SetKnowledgePoints()
    {
        if (knowledgePoints) knowledgePoints.text = playerSO.knowledgePoints.ToString("D3");
    }

    // LEVEL-----------------------------------------------------------------------

    public void FitEnemyColors(int[] aux)
    {
        Color[] auxColors = new Color[4];
        EnemyScript currentEnemy = player.CurrentEnemy.transform.parent.GetComponent<EnemyScript>();

        auxColors[0] = currentEnemy.colors[0];
        auxColors[1] = currentEnemy.colors[1];
        auxColors[2] = currentEnemy.colors[2];
        auxColors[3] = currentEnemy.colors[3];

        for (int j = 0; j < 4; j++)
        {
            for (int k = 0; k < 4; k++)
            {
                if (aux[k] == j)
                {
                    currentEnemy.colors[j] = auxColors[k];
                }
            }
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

    public void EnableSelectedEnemys()
    {
        for (int i = 0; i < 4; i++)
        {
            //Clear previous Data
            levelGenerator.EnemiesInZone[i].enemys.Clear();

            for (int j = 0; j < enemysInZone[i].Enemys.Length; j++)
            {
                if (enemysInZone[i].Enemys[j].configurations.selected == true)
                {
                    levelGenerator.EnemiesInZone[i].enemys.Add(enemysInZone[i].Enemys[j]);
                }
            }
        }
    }

    // GETTERS ---------------------------------------------------------------------------------

    public SaveSystemScript SaveSystem => saveSystem;
    public RemoteSO RemoteSO => remoteSO;
    public CurrentLevelSO CurrentLevelSO => currentLevelSO;
    public FromLevelSO FromLevelSO => fromLevelSO;
    public OptionsSO OptionsSO => optionsSO;
    public PlayerSO PlayerSO => playerSO;
    public LevelInteractionsScript Player => player;
    public TilemapCollider2D RoomEdgesCollider => roomEdgesCollider;
    public CinemachineShakeScript VirtualCamera1 => virtualCamera1;
    public CinemachineShakeScript VirtualCamera2 => virtualCamera2;
    public BulletGeneratorScript Bullets => bullets;
    public LaserScript Laser => laser;
    public GameObject Joystick => joystick;
    public DialogueCameraScript DialogueCamera => dialogueCamera;
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