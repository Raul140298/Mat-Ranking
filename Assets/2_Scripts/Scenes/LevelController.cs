using PixelCrushers.DialogueSystem;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Tilemaps;

public class LevelController : SceneController
{
    private static LevelController instance;

    [Header("UI")]
    [SerializeField] private Text knowledgePoints;
    [SerializeField] private GameObject topBar, bottomBar;
    [SerializeField] private GameObject[] hearth, key;
    [SerializeField] private GameObject joystick;

    [Header("LEVEL DATA")]
    [SerializeField] private TilemapCollider2D roomEdgesCollider;
    [SerializeField] private EnemiesInZone[] enemiesInZone;
    [SerializeField] private LevelGeneratorScript levelGenerator;

    [Header("PLAYER")]
    [SerializeField] private PlayerModelScript player;
    [SerializeField] private CinemachineShakeScript virtualCamera2;

    [Header("POOLERS")]
    [SerializeField] private Pooler bulletPooler;

    private bool questionIsRunning;
    private bool levelIsInitialized;
    private Coroutine challenge;
    
    [System.Serializable]
    public class EnemiesInZone
    {
        [SerializeField] public EnemyData[] enemies;
    }

    private void Awake()
    {
        instance = this;
        levelIsInitialized = false;
    }

    private void Start()
    {
        if (RemoteManager.Instance.IsLevelDataEmpty())
        {
            StartCoroutine(CRTNoLevel());
        }
        else
        {
#if UNITY_EDITOR
            PlayerSessionInfo.sfxVolume = 1;
            PlayerSessionInfo.bgmVolume = 1;
#endif
            
            AudioManager.StartAudio(sfxSlider, bgmSlider);
            
            DialoguePanelManager.SetContinueButtonNever();
            
            PlayerLevelInfo.heart = false;
            PlayerLevelInfo.playerKeyParts = 0;
            if (!PlayerLevelInfo.fromLevel)
            {
                PlayerLevelInfo.SetFromLevel(true);
                PlayerLevelInfo.ResetLevelInfo();
            }

            SetLives();
            SetKeys();
            SetKnowledgePoints(knowledgePoints);
            
            DialogueLua.SetVariable("StartQuestion", false);
            questionIsRunning = false;

            EnableSelectedEnemies();

            levelGenerator.GenerateLevel();

            PlaySoundtrack();

            levelIsInitialized = true;
        }
    }

    private void PlaySoundtrack()
    {
        switch (PlayerLevelInfo.currentZone)
        {
            case 0:
                Feedback.Do(eFeedbackType.Level0);
                break;
            case 1:
                Feedback.Do(eFeedbackType.Level1);
                break;
            case 2:
                Feedback.Do(eFeedbackType.Level2);
                break;
            default:
                Feedback.Do(eFeedbackType.Level3);
                break;
        }
    }

    public void StartChallenge(RoomScript room)
    {
        if (challenge == null)
        {
            challenge = StartCoroutine(CRTStartChallenge(room));
        }
    }

    private IEnumerator CRTStartChallenge(RoomScript room)
    {
        yield return new WaitUntil(() => levelIsInitialized);
        
        roomEdgesCollider.enabled = true;
        roomEdgesCollider.GetComponent<TilemapRenderer>().enabled = true;

        foreach (EnemyModelScript enemy in room.EnemiesInRoom)
        {
            questionIsRunning = true;
            
            enemy.SetQuestionParameters();

            string question = enemy.ChooseQuestionFromIlo();

            if (question != "")
            {
                MathHelper.CreateQuestion(question);
                DialogueManager.instance.StartConversation("Math Question");
            }
            else
            {
                questionIsRunning = false;
            }
            
            yield return new WaitWhile(() => questionIsRunning);
            
            //SHOULD START BATTLE PHASE
            
            yield return new WaitForSeconds(2f);
        }

        challenge = null;
    }
    
    public void AnswerCorrectly()
    {
        Feedback.Do(eFeedbackType.PopPositive);
        
        DialoguePanelManager.Timer.gameObject.SetActive(false);

        //currentEnemyModelScript.Defeated();

        PlayerLevelInfo.correctAnswers += 1;
        //PlayerLevelInfo.timePerQuestion += Mathf.RoundToInt((Time.time - timerSummary) % 60);

        questionIsRunning = false;
    }

    public void AnswerIncorrectly()
    {
        Feedback.Do(eFeedbackType.PopNegative);
        
        DialoguePanelManager.Timer.gameObject.SetActive(false);

        //compRendering.OutlineOff();
        //compRendering.OutlineLocked();

        //currentEnemyModelScript.Winner();

        //PlayerLevelInfo.timePerQuestion += Mathf.RoundToInt((Time.time - timerSummary) % 60);

        questionIsRunning = false;
    }

    public void EnableSelectedEnemies()
    {
        for (int i = 0; i < 4; i++)
        {
            levelGenerator.EnemiesUsedInZone[i].enemies.Clear();//Clear previous Data

            for (int j = 0; j < enemiesInZone[i].enemies.Length; j++)
            {
                if (enemiesInZone[i].enemies[j].configurations.selected)
                {
                    levelGenerator.EnemiesUsedInZone[i].enemies.Add(enemiesInZone[i].enemies[j]);
                }
            }
        }
    }

    public void FitEnemyColors(int[] aux)
    {
        EnemyModelScript currentEnemyModel = player.CurrentEnemyModelScript;

        for (int j = 0; j < 4; j++)
        {
            currentEnemyModel.Colors[j] = currentEnemyModel.Colors[aux[j]];
        }
    }

    IEnumerator CRTNoLevel()
    {
        yield return new WaitForSeconds(2.3f);
        Debug.Log("No habia enemigos en la mazmorra");
        SceneManager.LoadScene(1);
    }
    
    public void LoadNextLevel()
    {
        PlayerLevelInfo.NextLevel();
        GooglePlayManager.OpenSavedGameForSave("MatRanking");

        player.CompRendering.OutlineLocked();

        topBar.SetActive(false);
        bottomBar.SetActive(false);

        if (PlayerLevelInfo.currentLevel >= 4) //Max floors == 4 -> editable
        {
            LoadAdventureFromLevel(true); //time for end level UI menu
        }
        else
        {
            StartCoroutine(CRTLoadNextLevel());
        }
    }

    private void LoadAdventureFromLevel(bool lastFloor = false)
    {
        StartCoroutine(CRTLoadAdventure(lastFloor));
    }

    IEnumerator CRTLoadAdventure(bool lastFloor)
    {
        AudioManager.FadeOutBgm();

        if (!lastFloor) yield return new WaitForSeconds(1f);

        yield return new WaitForSeconds(1f);
        DialoguePanelManager.DialoguePanel.SetTrigger("Hide");

        base.transitionAnimator.SetBool("lastFloor", lastFloor);

        base.transitionAnimator.SetTrigger("end");
        yield return new WaitForSeconds(1f);

        DialoguePanelManager.DialoguePanel.ResetTrigger("Hide");
        yield return new WaitForSeconds(0.5f);

        SceneManager.LoadScene(1);
    }

    IEnumerator CRTLoadNextLevel()
    {
        Debug.Log("Subiste de piso");
        AudioManager.FadeOutBgm();
        yield return new WaitForSeconds(1f);
        DialoguePanelManager.DialoguePanel.SetTrigger("Hide");

        base.transitionAnimator.SetTrigger("end");
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(2); // 0: mainMenu, 1:adventure, 2:level
    }

    public void SetLives()
    {
        int playerLives = PlayerLevelInfo.playerLives;

        if (playerLives >= 0 && playerLives <= 3)
        {
            for (int i = playerLives; i < 3; i++)
            {
                hearth[i].SetActive(false);
            }

            for (int i = 0; i < playerLives; i++)
            {
                hearth[i].SetActive(true);
            }
        }

        if (playerLives == 0)
        {
            LoadAdventureFromLevel();
        }
    }

    public void SetKeys()
    {
        int playerKeyParts = PlayerLevelInfo.playerKeyParts;

        if (playerKeyParts >= 0 && playerKeyParts <= 3)
        {
            for (int i = playerKeyParts; i < 3; i++)
            {
                key[i].SetActive(false);
            }

            for (int i = 0; i < playerKeyParts; i++)
            {
                key[i].SetActive(true);
            }
        }
    }

    public Pooler BulletPooler => bulletPooler;
    public CinemachineShakeScript VirtualCamera2 => virtualCamera2;
    public PlayerModelScript Player => player;
    public GameObject Joystick => joystick;
    public DialogueCameraScript DialogueCamera => player.DialogueCamera;
    public TilemapCollider2D RoomEdgesCollider => roomEdgesCollider;
    public Text KnowledgePoints => knowledgePoints;

    void OnDestroy()
    {
        instance = null;
    }

    public static LevelController Instance
    {
        get { return instance; }
    }
}
