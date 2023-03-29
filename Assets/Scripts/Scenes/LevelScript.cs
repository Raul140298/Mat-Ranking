using PixelCrushers.DialogueSystem;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Tilemaps;

public class LevelScript : SceneScript
{
    private static LevelScript instance;

    [Header("LEVEL DATA")]
    [SerializeField] private Text zone, level;
    [SerializeField] private GameObject roomEdges;
    [SerializeField] private TilemapCollider2D roomEdgesCollider;
    [SerializeField] private BattleSoundtrackScript battleSoundtrack;
    [SerializeField] private EnemysInZone[] enemysInZone;
    [SerializeField] private LevelGeneratorScript levelGenerator;

    [Header("UI")]
    [SerializeField] private GameObject topBar, bottomBar;
    [SerializeField] private Text tq1, ca2, tpq3;
    [SerializeField] private GameObject[] hearth, key;
    [SerializeField] private GameObject joystick;

    [Header("PLAYER")]
    [SerializeField] private LevelInteractionsScript player;
    [SerializeField] private CinemachineShakeScript virtualCamera1;
    [SerializeField] private CinemachineShakeScript virtualCamera2;
    [SerializeField] private DialogueCameraScript dialogueCamera;
    [SerializeField] private BulletGeneratorScript bullets;
    [SerializeField] private LaserScript laser;
    [SerializeField] private CapsuleCollider2D playerDialogueArea;

    [System.Serializable]
    public class EnemysInZone
    {
        [SerializeField] public EnemySO[] enemys;
    }

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        if (IsLevelDataEmpty() == true)
        {
            StartCoroutine(CRTNoChallenge());
        }
        else
        {
            GameSystemScript.DialogueSystem.displaySettings.subtitleSettings.continueButton = DisplaySettings.SubtitleSettings.ContinueButtonMode.Never;
            GameSystemScript.CurrentLevelSO.heart = false;
            GameSystemScript.CurrentLevelSO.playerKeyParts = 0;
            GameSystemScript.StartSounds(SoundsSlider);
            GameSystemScript.StartSoundtracks(SoundtracksSlider);
            if (GameSystemScript.FromLevelSO.fromLevel == false)
            {
                GameSystemScript.CurrentLevelSO.playerLives = 3;
                GameSystemScript.FromLevelSO.fromLevel = true;
            }

            SetLives();
            SetKeys();
            GameSystemScript.SetKnowledgePoints(KnowledgePoints);

            EnableSelectedEnemys();
            levelGenerator.GenerateLevel();

            StartCoroutine(CRTPlayerDialogueStart());
        }
    }

    public void SetLives()
    {
        if (GameSystemScript.CurrentLevelSO.playerLives == 0)
        {
            LevelScript.Instance.LoadAdventure(-1);
        }

        if (GameSystemScript.CurrentLevelSO.playerLives >= 0 && GameSystemScript.CurrentLevelSO.playerLives <= 3)
        {
            for (int i = GameSystemScript.CurrentLevelSO.playerLives; i < 3; i++)
            {
                hearth[i].SetActive(false);
            }

            for (int i = 0; i < GameSystemScript.CurrentLevelSO.playerLives; i++)
            {
                hearth[i].SetActive(true);
            }
        }
    }

    public void SetKeys()
    {
        if (GameSystemScript.CurrentLevelSO.playerKeyParts >= 0 && GameSystemScript.CurrentLevelSO.playerKeyParts <= 3)
        {
            for (int i = GameSystemScript.CurrentLevelSO.playerKeyParts; i < 3; i++)
            {
                key[i].SetActive(false);
            }

            for (int i = 0; i < GameSystemScript.CurrentLevelSO.playerKeyParts; i++)
            {
                key[i].SetActive(true);
            }
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

    public void EnableSelectedEnemys()
    {
        for (int i = 0; i < 4; i++)
        {
            //Clear previous Data
            levelGenerator.EnemiesInZone[i].enemys.Clear();

            for (int j = 0; j < enemysInZone[i].enemys.Length; j++)
            {
                if (enemysInZone[i].enemys[j].configurations.selected == true)
                {
                    levelGenerator.EnemiesInZone[i].enemys.Add(enemysInZone[i].enemys[j]);
                }
            }
        }
    }

    public void FitEnemyColors(int[] aux)
    {
        Color[] auxColors = new Color[4];
        EnemyScript currentEnemy = player.CurrentEnemy.transform.parent.GetComponent<EnemyScript>();

        auxColors[0] = currentEnemy.Colors[0];
        auxColors[1] = currentEnemy.Colors[1];
        auxColors[2] = currentEnemy.Colors[2];
        auxColors[3] = currentEnemy.Colors[3];

        for (int j = 0; j < 4; j++)
        {
            for (int k = 0; k < 4; k++)
            {
                if (aux[k] == j)
                {
                    currentEnemy.Colors[j] = auxColors[k];
                }
            }
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

    IEnumerator CRTPlayerDialogueStart()
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

    public override void LoadAdventure(float transitionTime)
    {
        StartCoroutine(CRTLoadAdventure(transitionTime));
    }

    public void LoadNextLevel()
    {
        GameSystemScript.NextPlayerCurrentLevel();
        GameSystemScript.SaveSystem.SaveLocal();

        player.GetComponent<OutlineScript>().OutlineLocked();

        topBar.SetActive(false);
        bottomBar.SetActive(false);

        if (GameSystemScript.CurrentLevelSO.currentLevel >= 4) //Max floors == 4 -> editable
        {
            LoadAdventure(5); //time for end level UI menu

            AverageTimePerQuestions();
        }
        else
        {
            StartCoroutine(CRTLoadNextLevel());
        }
    }

    public void LoadPrevLevel()
    {
        GameSystemScript.SaveSystem.SaveLocal();
        topBar.SetActive(false);
        bottomBar.SetActive(false);

        if (GameSystemScript.CurrentLevelSO.currentLevel <= 0)
        {
            LoadAdventure(1);
        }
        else
        {
            StartCoroutine(CRTLoadPrevLevel());
        }
    }

    IEnumerator CRTLoadAdventure(float transitionTime)
    {
        SoundtracksScript.ReduceVolume();
        AsignSummary();

        if (transitionTime == -1)
        {
            Debug.Log("Moriste");
            yield return new WaitForSeconds(2f);
            GameSystemScript.DialoguePanel.SetTrigger("Hide");
            TransitionAnimator.SetBool("lastFloor", false);
            TransitionAnimator.SetTrigger("end");
            yield return new WaitForSeconds(1f);
        }
        else if (transitionTime == 1)
        {
            Debug.Log("Perdiste la mazmorra");
            yield return new WaitForSeconds(1f);
            GameSystemScript.DialoguePanel.SetTrigger("Hide");
            TransitionAnimator.SetBool("lastFloor", false);
            TransitionAnimator.SetTrigger("end");
            yield return new WaitForSeconds(1f);
        }
        else if (transitionTime == 5)
        {
            Debug.Log("Ganaste, ver el resumen");
            yield return new WaitForSeconds(1f);
            GameSystemScript.DialoguePanel.SetTrigger("Hide");
            TransitionAnimator.SetBool("lastFloor", true);
            TransitionAnimator.SetTrigger("end");
            yield return new WaitForSeconds(3.5f);
            TransitionAnimator.SetTrigger("end");
            yield return new WaitForSeconds(1f);
        }
        else
        {
            Debug.Log("Acabaste el nivel");
            yield return new WaitForSeconds(1f);
            GameSystemScript.DialoguePanel.SetTrigger("Hide");
            yield return new WaitForSeconds(transitionTime);
            TransitionAnimator.SetBool("lastFloor", false);
            TransitionAnimator.SetTrigger("end");
            yield return new WaitForSeconds(1f);
        }

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

        TransitionAnimator.SetTrigger("end");
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(2); // 0: mainMenu, 1:adventure, 2:level
    }

    IEnumerator CRTLoadPrevLevel()
    {
        Debug.Log("Bajaste de piso");
        SoundtracksScript.ReduceVolume();
        yield return new WaitForSeconds(0.7f);
        GameSystemScript.DialoguePanel.SetTrigger("Hide");

        TransitionAnimator.SetTrigger("end");
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(2); // 0: mainMenu, 1:adventure, 2:level
    }

    public CinemachineShakeScript VirtualCamera2 => virtualCamera2;
    public LevelInteractionsScript Player => player;
    public BulletGeneratorScript Bullets => bullets;
    public GameObject Joystick => joystick;
    public DialogueCameraScript DialogueCamera => dialogueCamera;
    public TilemapCollider2D RoomEdgesCollider => roomEdgesCollider;
    public LaserScript Laser => laser;
    public BattleSoundtrackScript BattleSoundtrack => battleSoundtrack;

    void OnDestroy()
    {
        instance = null;
    }

    public static LevelScript Instance
    {
        get { return instance; }
    }
}
