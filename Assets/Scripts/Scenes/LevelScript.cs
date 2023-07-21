using PixelCrushers.DialogueSystem;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Tilemaps;

public class LevelScript : SceneScript
{
    private static LevelScript instance;

    [Header("UI")]
    [SerializeField] private Text knowledgePoints;
    [SerializeField] private GameObject topBar, bottomBar;
    [SerializeField] private Text tq1, ca2, tpq3;
    [SerializeField] private GameObject[] hearth, key;
    [SerializeField] private GameObject joystick;

    [Header("LEVEL DATA")]
    [SerializeField] private Text zone, level;
    [SerializeField] private GameObject roomEdges;
    [SerializeField] private TilemapCollider2D roomEdgesCollider;
    [SerializeField] private BattleSoundtrackScript battleSoundtrack;
    [SerializeField] private EnemiesInZone[] enemiesInZone;
    [SerializeField] private LevelGeneratorScript levelGenerator;

    [Header("PLAYER")]
    [SerializeField] private PlayerModelScript player;
    [SerializeField] private CinemachineShakeScript virtualCamera1;
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
            GameSystemScript.DialogueSystem.displaySettings.subtitleSettings.continueButton = DisplaySettings.SubtitleSettings.ContinueButtonMode.Never;
            GameSystemScript.StartSounds(base.soundsSlider);
            GameSystemScript.StartSoundtracks(base.soundtracksSlider);

            GameSystemScript.CurrentLevelSO.heart = false;
            GameSystemScript.CurrentLevelSO.playerKeyParts = 0;
            if (GameSystemScript.FromLevelSO.fromLevel == false)
            {
                GameSystemScript.CurrentLevelSO.playerLives = 3;
                GameSystemScript.FromLevelSO.fromLevel = true;
            }

            SetLives();
            SetKeys();
            GameSystemScript.SetKnowledgePoints(knowledgePoints);

            EnableSelectedEnemies();

            levelGenerator.GenerateLevel();

            DialogueLua.SetVariable("StartQuestion", 0);

            StartCoroutine(CRTStartChallenge());
        }
    }

    public void AsignSummary()
    {
        tq1.text = GameSystemScript.CurrentLevelSO.totalQuestions.ToString();
        ca2.text = GameSystemScript.CurrentLevelSO.correctAnswers.ToString();
        tpq3.text = GameSystemScript.CurrentLevelSO.timePerQuestion.ToString();
    }

    public void AverageTimePerQuestions()
    {
        GameSystemScript.CurrentLevelSO.timePerQuestion /= GameSystemScript.CurrentLevelSO.totalQuestions;
        tpq3.text = GameSystemScript.CurrentLevelSO.timePerQuestion.ToString();
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
        if ((GameSystemScript.CurrentLevelSO.currentZone == 0 &&
            GameSystemScript.RemoteSO.dgbl_features.ilos[0].ilos[0].selected == false && //L1
            (GameSystemScript.RemoteSO.dgbl_features.ilos[0].ilos[1].selected == false || //L2 or
            (GameSystemScript.RemoteSO.dgbl_features.ilos[0].ilos[1].ilos[0].selected == false && //L2.1
            GameSystemScript.RemoteSO.dgbl_features.ilos[0].ilos[1].ilos[1].selected == false))) ||//L2.2

            (GameSystemScript.CurrentLevelSO.currentZone == 1 &&
            GameSystemScript.RemoteSO.dgbl_features.ilos[1].ilos[0].selected == false && //L8
            GameSystemScript.RemoteSO.dgbl_features.ilos[1].ilos[1].selected == false) || //L9

            (GameSystemScript.CurrentLevelSO.currentZone == 2 &&
            (GameSystemScript.RemoteSO.dgbl_features.ilos[2].ilos[0].selected == false || //L13
            (GameSystemScript.RemoteSO.dgbl_features.ilos[2].ilos[0].ilos[0].selected == false && //L13.1
            GameSystemScript.RemoteSO.dgbl_features.ilos[2].ilos[0].ilos[1].selected == false && //L13.2
            GameSystemScript.RemoteSO.dgbl_features.ilos[2].ilos[0].ilos[2].selected == false))) || //L13.3

            (GameSystemScript.CurrentLevelSO.currentZone == 3 &&
            GameSystemScript.RemoteSO.dgbl_features.ilos[3].ilos[1].selected == false && //L19
            GameSystemScript.RemoteSO.dgbl_features.ilos[3].ilos[3].selected == false || //L21
            (GameSystemScript.RemoteSO.dgbl_features.ilos[3].ilos[3].ilos[0].selected == false && //L21.1
            GameSystemScript.RemoteSO.dgbl_features.ilos[3].ilos[3].ilos[1].selected == false))) //L21.2
        {
            switch (Localization.language)
            {
                case "es":
                    zone.text = "Desafío Desactivado";
                    level.text = "No hay ningun enemigo";
                    break;
                case "en":
                    zone.text = "Challenge off";
                    level.text = "There is no enemy";
                    break;
                case "qu":
                    zone.text = "Atipanakuy nisqa cancelasqa";
                    level.text = "Mana awqa kanchu";
                    break;
                default:
                    // code block
                    break;
            }

            return true;
        }
        else
        {
            switch (Localization.language)
            {
                case "es":
                    zone.text = "Desafío";
                    level.text = "Piso";
                    break;
                case "en":
                    zone.text = "Challenge";
                    level.text = "Floor";
                    break;
                case "qu":
                    zone.text = "Atipanakuy";
                    level.text = "Panpa";
                    break;
                default:
                    // code block
                    break;
            }
            zone.text += " " + (GameSystemScript.CurrentLevelSO.currentZone + 1).ToString();
            level.text += " " + GameSystemScript.CurrentLevelSO.currentLevel.ToString();

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
        SoundsScript.PlaySound("LEVEL START");
        yield return new WaitForSeconds(2f);
        SoundtracksScript.PlaySoundtrack("LEVEL" + GameSystemScript.CurrentLevelSO.currentZone.ToString());
        yield return new WaitForSeconds(0.9f);

        GameSystemScript.DialoguePanel.ResetTrigger("Hide");
        GameSystemScript.DialoguePanel.ResetTrigger("Show");

        playerDialogueArea.enabled = true;
    }

    public void LoadAdventureFromLevel(bool lastFloor = false)
    {
        StartCoroutine(CRTLoadAdventure(lastFloor));
    }

    public void LoadNextLevel()
    {
        GameSystemScript.NextPlayerCurrentLevel();
        GameSystemScript.SaveSystem.SaveLocal();

        player.CompRendering.OutlineLocked();

        topBar.SetActive(false);
        bottomBar.SetActive(false);

        if (GameSystemScript.CurrentLevelSO.currentLevel >= 4) //Max floors == 4 -> editable
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
        SoundtracksScript.ReduceVolume();
        AsignSummary();

        if (!lastFloor) yield return new WaitForSeconds(1f);

        yield return new WaitForSeconds(1f);
        GameSystemScript.DialoguePanel.SetTrigger("Hide");

        base.transitionAnimator.SetBool("lastFloor", lastFloor);

        base.transitionAnimator.SetTrigger("end");
        yield return new WaitForSeconds(1f);

        GameSystemScript.DialoguePanel.ResetTrigger("Hide");
        yield return new WaitForSeconds(0.5f);

        SceneManager.LoadScene(1);
    }

    IEnumerator CRTLoadNextLevel()
    {
        Debug.Log("Subiste de piso");
        SoundtracksScript.ReduceVolume();
        yield return new WaitForSeconds(1f);
        GameSystemScript.DialoguePanel.SetTrigger("Hide");

        base.transitionAnimator.SetTrigger("end");
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(2); // 0: mainMenu, 1:adventure, 2:level
    }

    public void SetLives()
    {
        int playerLives = GameSystemScript.CurrentLevelSO.playerLives;

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
        int playerKeyParts = GameSystemScript.CurrentLevelSO.playerKeyParts;

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
    public BattleSoundtrackScript BattleSoundtrack => battleSoundtrack;
    public Text KnowledgePoints => knowledgePoints;

    void OnDestroy()
    {
        instance = null;
    }

    public static LevelScript Instance
    {
        get { return instance; }
    }
}
