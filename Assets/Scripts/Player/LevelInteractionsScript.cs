using UnityEngine;
using PixelCrushers.DialogueSystem;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Tilemaps;

public class LevelInteractionsScript : MonoBehaviour
{
    [SerializeField] private ProximitySelector proximitySelector;
    [SerializeField] private GameSystemScript gameSystem;
    [SerializeField] private LevelScript level;
    [SerializeField] private GameObject currentEnemy;
    [SerializeField] private EnemyScript currentEnemyScript;
    [SerializeField] private PlayerRendererScript playerRenderer;
    [SerializeField] private CapsuleCollider2D playerDialogueArea;
    [SerializeField] private DialogueCameraScript dialogueCamera;
    [SerializeField] private Text tq1, ca2, tpq3;
    [SerializeField] private CurrentLevelSO currentLevelSO;
    [SerializeField] private GameObject[] hearth, key;
    [SerializeField] private float timerSummary;
    [SerializeField] private GameObject timer;
    [SerializeField] private GameObject dialogueManager;
    [SerializeField] private DialogueSystemController dialogueSystemController;
    [SerializeField] private BattleSoundtrackScript battleSoundtrack;

    private void Start()
    {
        timerSummary = 0;

        asignSummary();

        StartCoroutine(CRTSetTimer());
    }

    IEnumerator CRTSetTimer()
    {
        yield return new WaitForSeconds(0.2f);
        dialogueManager = GameObject.FindGameObjectWithTag("DialogueManager");
        dialogueSystemController = GameObject.FindGameObjectWithTag("DialogueManager").GetComponent<DialogueSystemController>();
        timer = dialogueManager.transform.GetChild(0).transform.GetChild(3).transform.GetChild(1).transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).gameObject;
    }

    void asignSummary()
    {
        tq1.text = currentLevelSO.totalQuestions.ToString();
        ca2.text = currentLevelSO.correctAnswers.ToString();
        tpq3.text = currentLevelSO.timePerQuestion.ToString();
    }

    public void averageTimePerQuestions()
    {
        currentLevelSO.timePerQuestion /= currentLevelSO.totalQuestions;
        tpq3.text = currentLevelSO.timePerQuestion.ToString();
    }

    //Interactions with colliders and triggers
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Enemy")
        {
            currentEnemy = collision.gameObject;
            currentEnemyScript = currentEnemy.transform.parent.GetComponent<EnemyScript>();

            if (currentEnemyScript.IsAttacking == false)
            {
                //Verify if the enemy data has been filled
                if (currentEnemyScript.EnemyData != null &&
                    currentEnemyScript.RoomEdgesPosition.x < (this.transform.position.x) &&
                    currentEnemyScript.RoomEdgesEnd.x > (this.transform.position.x) &&
                    currentEnemyScript.RoomEdgesPosition.y < this.transform.position.y &&
                    currentEnemyScript.RoomEdgesEnd.y > this.transform.position.y)
                {
                    if (playerDialogueArea.enabled == true &&
                        currentEnemyScript.StartQuestion == true)
                    {
                        lookTarget(currentEnemy);

                        dialogueCamera.Target = currentEnemy;

                        LevelScript.Instance.DialogueCamera.StartDialogue();

                        //this.GetComponent<OutlineScript>().OutlineOff();

                        SoundsScript.PlaySound("EXCLAMATION");

                        currentLevelSO.totalQuestions += 1;
                        asignSummary();

                        StartCoroutine(CRTStartTimer());

                        timerSummary = Time.time;

                        battleSoundtrack.StartBattleSoundtrack();

                        //In case the Behavior Tree was in timer
                        currentEnemyScript.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                        currentEnemyScript.GetComponent<Animator>().SetTrigger("start");


                        LevelScript.Instance.RoomEdgesCollider.enabled = true;
                        LevelScript.Instance.RoomEdgesCollider.GetComponent<TilemapRenderer>().enabled = true;
                        //gameSystem.roomEdges.transform.position = currentEnemyScript.roomEdgesPosition;
                        //gameSystem.roomEdges.GetComponent<SpriteRenderer>().size = currentEnemyScript.roomEdgesSize;
                        //gameSystem.roomEdges.SetActive(true);
                    }

                    dialogueSystemController.displaySettings.inputSettings.responseTimeout = currentEnemyScript.EnemyData.configurations.ilo_parameters[0].default_value;
                    dialogueSystemController.displaySettings.inputSettings.responseTimeoutAction = ResponseTimeoutAction.Custom;

                    useCurrentSelection();
                }
            }
        }
        else if (collision.tag == "Heart" && currentLevelSO.playerLives < 3 && currentLevelSO.heart == false)
        {
            currentLevelSO.heart = true;

            Debug.Log("Se gano un corazon");

            collision.gameObject.SetActive(false);

            SoundsScript.PlaySound("WIN HEART");

            currentLevelSO.playerLives += 1;
            setLives();
        }
    }

    IEnumerator CRTStartTimer()
    {
        yield return new WaitForSeconds(0.2f);

        timer.SetActive(false);
        //Set question time limit based on LX
        timer.GetComponent<TimerScript>().StartingTime = currentEnemyScript.EnemyData.configurations.ilo_parameters[0].default_value;
        timer.GetComponent<TimerScript>().Aux = timer.GetComponent<TimerScript>().StartingTime;
        if (timer.GetComponent<TimerScript>().Slider) timer.GetComponent<TimerScript>().Slider.value = 1;
        timer.GetComponent<TimerScript>().Finish = false;
        yield return new WaitForSeconds(1.8f);

        //2 seconds ahead
        timer.SetActive(true);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        dialogueCamera.Target = null;
        if (collision.gameObject.tag == "NextLevel" && currentLevelSO.playerKeyParts == 3)
        {
            lookTarget(collision.gameObject);
            timer.SetActive(false);
            //this.GetComponent<OutlineScript>().OutlineOff();
            proximitySelector.UseCurrentSelection();
        }
    }

    //Functions
    public void lookTarget(GameObject target)
    {
        if (this.gameObject.transform.position.x > target.gameObject.transform.position.x)
        {
            playerRenderer.SpriteRenderer.flipX = true;
            if (target.tag == "Enemy") target.transform.parent.GetComponent<SpriteRenderer>().flipX = false;
        }
        else if (this.gameObject.transform.position.x < target.gameObject.transform.position.x)
        {
            playerRenderer.SpriteRenderer.flipX = false;
            if (target.tag == "Enemy") target.transform.parent.GetComponent<SpriteRenderer>().flipX = true;
        }
    }

    public void nextLevel()
    {
        GameSystemScript.NextPlayerCurrentLevel();
        level.LoadNextLevel();
    }

    public void useCurrentSelection()
    {
        //Edit vairables before the conversation start
        currentEnemyScript.setVariables();
        //then start the conversation
        proximitySelector.UseCurrentSelection();
    }

    public void defeatedEnemy()
    {
        if (timer) timer.SetActive(false);

        if (currentEnemyScript)
        {
            currentEnemyScript.GetComponent<EnemyScript>().defeated();

            currentLevelSO.correctAnswers += 1;
            currentLevelSO.timePerQuestion += Mathf.RoundToInt((Time.time - timerSummary) % 60);

            asignSummary();
        }
    }

    public void playerDefeated()
    {
        if (timer) timer.SetActive(false);

        if (currentEnemyScript)
        {
            currentEnemyScript.GetComponent<EnemyScript>().winner();

            currentLevelSO.timePerQuestion += Mathf.RoundToInt((Time.time - timerSummary) % 60);

            asignSummary();
        }
    }

    public void setLives()
    {
        if (currentLevelSO.playerLives == 0)
        {
            level.LoadAdventure(-1);
        }

        if (currentLevelSO.playerLives >= 0 && currentLevelSO.playerLives <= 3)
        {
            for (int i = currentLevelSO.playerLives; i < 3; i++)
            {
                hearth[i].SetActive(false);
            }

            for (int i = 0; i < currentLevelSO.playerLives; i++)
            {
                hearth[i].SetActive(true);
            }
        }
    }

    public void setKeys()
    {
        if (currentLevelSO.playerKeyParts >= 0 && currentLevelSO.playerKeyParts <= 3)
        {
            for (int i = currentLevelSO.playerKeyParts; i < 3; i++)
            {
                key[i].SetActive(false);
            }

            for (int i = 0; i < currentLevelSO.playerKeyParts; i++)
            {
                key[i].SetActive(true);
            }
        }
    }

    public GameSystemScript GameSystem => gameSystem;
    public BattleSoundtrackScript BattleSoundtrack => battleSoundtrack;
    public EnemyScript CurrentEnemyScript
    {
        get { return currentEnemyScript; }
        set { currentEnemyScript = value; }
    }
    public GameObject CurrentEnemy
    {
        get { return currentEnemy; }
        set { currentEnemy = value; }
    }
}
