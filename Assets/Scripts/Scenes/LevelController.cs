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
    [SerializeField] private Text tq1, ca2, tpq3;
    [SerializeField] private GameObject[] hearth, key;
    [SerializeField] private GameObject joystick;

    [Header("LEVEL DATA")]
    [SerializeField] private Text zone, level;
    [SerializeField] private TilemapCollider2D roomEdgesCollider;
    [SerializeField] private EnemiesInZone[] enemiesInZone;
    [SerializeField] private LevelGeneratorScript levelGenerator;

    [Header("PLAYER")]
    [SerializeField] private PlayerModelScript player;
    [SerializeField] private CinemachineShakeScript virtualCamera2;
    [SerializeField] private DialogueCameraScript dialogueCamera;
    [SerializeField] private CapsuleCollider2D playerDialogueArea;

    [Header("POOLERS")]
    [SerializeField] private Pooler bulletPooler;

    [System.Serializable]
    public class EnemiesInZone
    {
        [SerializeField] public EnemySO[] enemies;
    }

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        if (IsLevelDataEmpty())
        {
            StartCoroutine(CRTNoChallenge());
        }
        else
        {
            AudioManager.StartAudio(sfxSlider, bgmSlider);
            
            GameManager.SetContinueButtonNever();
            
            PlayerLevelInfo.heart = false;
            PlayerLevelInfo.playerKeyParts = 0;
            if (!PlayerLevelInfo.fromLevel)
            {
                PlayerLevelInfo.playerLives = 3;
                PlayerLevelInfo.SetFromLevel(true);
            }

            SetLives();
            SetKeys();
            GameManager.SetKnowledgePoints(knowledgePoints);
            
            DialogueLua.SetVariable("StartQuestion", 0);

            EnableSelectedEnemies();

            levelGenerator.GenerateLevel();

            StartCoroutine(CRTStartChallenge());
        }
    }

    public void AsignSummary()
    {
        tq1.text = PlayerLevelInfo.totalQuestions.ToString();
        ca2.text = PlayerLevelInfo.correctAnswers.ToString();
        tpq3.text = PlayerLevelInfo.timePerQuestion.ToString();
    }

    public void AverageTimePerQuestions()
    {
        PlayerLevelInfo.timePerQuestion /= PlayerLevelInfo.totalQuestions;
        tpq3.text = PlayerLevelInfo.timePerQuestion.ToString();
    }

    public void EnableSelectedEnemies()
    {
        for (int i = 0; i < 4; i++)
        {
            levelGenerator.EnemiesUsedInZone[i].enemies.Clear();//Clear previous Data

            for (int j = 0; j < enemiesInZone[i].enemies.Length; j++)
            {
                if (enemiesInZone[i].enemies[j].configurations.selected == true)
                {
                    levelGenerator.EnemiesUsedInZone[i].enemies.Add(enemiesInZone[i].enemies[j]);
                }
            }
        }
    }

    public void FitEnemyColors(int[] aux)
    {
        EnemyScript currentEnemy = player.CurrentEnemyScript;

        for (int j = 0; j < 4; j++)
        {
            currentEnemy.Colors[j] = currentEnemy.Colors[aux[j]];
        }
    }

    private bool IsLevelDataEmpty()
    {
        //If there aren't enemys in the zone
        if ((PlayerLevelInfo.currentZone == 0 &&
            GameManager.RemoteSO.dgbl_features.ilos[0].ilos[0].selected == false && //L1
            (GameManager.RemoteSO.dgbl_features.ilos[0].ilos[1].selected == false || //L2 or
            (GameManager.RemoteSO.dgbl_features.ilos[0].ilos[1].ilos[0].selected == false && //L2.1
            GameManager.RemoteSO.dgbl_features.ilos[0].ilos[1].ilos[1].selected == false))) ||//L2.2

            (PlayerLevelInfo.currentZone == 1 &&
            GameManager.RemoteSO.dgbl_features.ilos[1].ilos[0].selected == false && //L8
            GameManager.RemoteSO.dgbl_features.ilos[1].ilos[1].selected == false) || //L9

            (PlayerLevelInfo.currentZone == 2 &&
            (GameManager.RemoteSO.dgbl_features.ilos[2].ilos[0].selected == false || //L13
            (GameManager.RemoteSO.dgbl_features.ilos[2].ilos[0].ilos[0].selected == false && //L13.1
            GameManager.RemoteSO.dgbl_features.ilos[2].ilos[0].ilos[1].selected == false && //L13.2
            GameManager.RemoteSO.dgbl_features.ilos[2].ilos[0].ilos[2].selected == false))) || //L13.3

            (PlayerLevelInfo.currentZone == 3 &&
            GameManager.RemoteSO.dgbl_features.ilos[3].ilos[1].selected == false && //L19
            GameManager.RemoteSO.dgbl_features.ilos[3].ilos[3].selected == false || //L21
            (GameManager.RemoteSO.dgbl_features.ilos[3].ilos[3].ilos[0].selected == false && //L21.1
            GameManager.RemoteSO.dgbl_features.ilos[3].ilos[3].ilos[1].selected == false))) //L21.2
        {
            zone.text = GameManager.DialogueSystem.GetLocalizedText("challengeOff");
            level.text = GameManager.DialogueSystem.GetLocalizedText("noEnemies");

            return true;
        }
        else
        {
            zone.text = GameManager.DialogueSystem.GetLocalizedText("challenge");
            level.text = GameManager.DialogueSystem.GetLocalizedText("floor");
            zone.text += " " + (PlayerLevelInfo.currentZone + 1).ToString();
            level.text += " " + PlayerLevelInfo.currentLevel.ToString();

            return false;
        }
    }

    IEnumerator CRTNoChallenge()
    {
        yield return new WaitForSeconds(2.3f);
        Debug.Log("No habia enemigos en la mazmorra");
        SceneManager.LoadScene(1);
    }

    IEnumerator CRTStartChallenge()
    {
        yield return new WaitForSeconds(0.1f);
        Feedback.Do(eFeedbackType.LevelStart);
        yield return new WaitForSeconds(2f);
        
        Feedback.Do(eFeedbackType.LevelStart);

        if (PlayerLevelInfo.currentZone == 0)
        {
            Feedback.Do(eFeedbackType.Level0);
        }
        else if (PlayerLevelInfo.currentZone == 1)
        {
            Feedback.Do(eFeedbackType.Level1);
        }
        else if (PlayerLevelInfo.currentZone == 2)
        {
            Feedback.Do(eFeedbackType.Level2);
        }
        else
        {
            Feedback.Do(eFeedbackType.Level3);
        }
        
        yield return new WaitForSeconds(0.9f);

        GameManager.DialoguePanel.ResetTrigger("Hide");
        GameManager.DialoguePanel.ResetTrigger("Show");

        playerDialogueArea.enabled = true;
    }

    public void LoadAdventureFromLevel(bool lastFloor = false)
    {
        StartCoroutine(CRTLoadAdventure(lastFloor));
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
            AverageTimePerQuestions();
            LoadAdventureFromLevel(true); //time for end level UI menu
        }
        else
        {
            StartCoroutine(CRTLoadNextLevel());
        }
    }

    IEnumerator CRTLoadAdventure(bool lastFloor)
    {
        AudioManager.FadeOutBgmVolume();
        AsignSummary();

        if (!lastFloor) yield return new WaitForSeconds(1f);

        yield return new WaitForSeconds(1f);
        GameManager.DialoguePanel.SetTrigger("Hide");

        base.transitionAnimator.SetBool("lastFloor", lastFloor);

        base.transitionAnimator.SetTrigger("end");
        yield return new WaitForSeconds(1f);

        GameManager.DialoguePanel.ResetTrigger("Hide");
        yield return new WaitForSeconds(0.5f);

        SceneManager.LoadScene(1);
    }

    IEnumerator CRTLoadNextLevel()
    {
        Debug.Log("Subiste de piso");
        AudioManager.FadeOutBgmVolume();
        yield return new WaitForSeconds(1f);
        GameManager.DialoguePanel.SetTrigger("Hide");

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
    public DialogueCameraScript DialogueCamera => dialogueCamera;
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
