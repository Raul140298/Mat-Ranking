using UnityEngine;
using UnityEngine.UI;
using PixelCrushers.DialogueSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class GameSystemScript : MonoBehaviour
{
    [SerializeField] private LevelInteractionsScript player;
    [SerializeField] private PlayerSO playerSO;
    [SerializeField] private OptionsSO optionsSO;
    [SerializeField] private RemoteSO remoteSO;
    [SerializeField] private CurrentLevelSO currentLevelSO;
    [SerializeField] private Text knowledgePoints;
    [SerializeField] private PlayFabScript playFab;
    [SerializeField] private GameObject dm;
    [SerializeField] private SaveSystemScript saveSystem;
    [SerializeField] private EnemysInZone[] enemysInZone;
    [SerializeField] private LevelGeneratorScript levelGenerator;
    [SerializeField] private CinemachineShakeScript virtualCamera1;
    [SerializeField] private CinemachineShakeScript virtualCamera2;
    [SerializeField] private Slider soundsSlider;
    [SerializeField] private Slider soundtracksSlider;
    [SerializeField] private GameObject joystick;
    [SerializeField] private BulletGeneratorScript bullets;
    [SerializeField] private GameObject roomEdges;
    [SerializeField] private TilemapCollider2D roomEdgesCollider;
    [SerializeField] private DialogueCameraScript dialogueCamera;
    [SerializeField] private LaserScript laser;
    [System.Serializable]
    public class EnemysInZone
    {
        [SerializeField] private EnemySO[] enemys;

        public EnemySO[] Enemys => enemys;
    }

    private void Start()
    {
        Application.targetFrameRate = 60;

        dm = GameObject.FindGameObjectWithTag("DialogueManager");
        DialogueSystemController aux = dm.GetComponent<DialogueSystemController>();

        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            //AudioConfiguration config = AudioSettings.GetConfiguration();
            //config.dspBufferSize = 64;
            //AudioSettings.Reset(config);
        }

        if (SceneManager.GetActiveScene().buildIndex == 2)
        {
            aux.displaySettings.subtitleSettings.continueButton = DisplaySettings.SubtitleSettings.ContinueButtonMode.Never;
        }
        else
        {
            aux.displaySettings.subtitleSettings.continueButton = DisplaySettings.SubtitleSettings.ContinueButtonMode.Always;
        }
    }

    public void ShowRanking()
    {
        GooglePlaySystemScript.instance.ShowRanking();
    }

    public void fitEnemyColors(int[] aux)
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

    public void changeKnowledgePoints(int n)
    {
        if (playerSO.knowledgePoints + n >= 0)
        {
            playerSO.knowledgePoints += n;

            //Connection to bd on PlayFab
            saveSystem.sendRanking();
            setKnowledgePoints();
            saveSystem.saveLocal();
        }
    }

    public void setKnowledgePoints()
    {
        if (knowledgePoints) knowledgePoints.text = playerSO.knowledgePoints.ToString("D3");
    }

    public void resetPlayerCurrentLevel()
    {
        currentLevelSO.playerLives = 3;
        currentLevelSO.currentLevel = 1;
        currentLevelSO.totalQuestions = 0;
        currentLevelSO.correctAnswers = 0;
        currentLevelSO.timePerQuestion = 0;
    }

    public void nextPlayerCurrentLevel()
    {
        currentLevelSO.currentLevel += 1;
    }

    public void prevPlayerCurrentLevel()
    {
        currentLevelSO.currentLevel -= 1;
    }

    public void enableSelectedEnemys()
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

    public PlayerSO PlayerSO => playerSO;
    public SaveSystemScript SaveSystem => saveSystem;
    public RemoteSO RemoteSO => remoteSO;
    public CurrentLevelSO CurrentLevelSO => currentLevelSO;
    public OptionsSO OptionsSO => optionsSO;
    public LevelInteractionsScript Player => player;
    public TilemapCollider2D RoomEdgesCollider => roomEdgesCollider;
    public CinemachineShakeScript VirtualCamera1 => virtualCamera1;
    public CinemachineShakeScript VirtualCamera2 => virtualCamera2;
    public Slider SoundsSlider => soundsSlider;
    public Slider SoundtracksSlider => soundtracksSlider;
    public BulletGeneratorScript Bullets => bullets;
    public LaserScript Laser => laser;
    public GameObject Joystick => joystick;
    public DialogueCameraScript DialogueCamera => dialogueCamera;
}