using UnityEngine;
using PixelCrushers.DialogueSystem;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Tilemaps;

public class LevelInteractionsScript : MonoBehaviour
{
    [SerializeField] private ProximitySelector proximitySelector;
    [SerializeField] private GameObject currentEnemy;
    [SerializeField] private EnemyScript currentEnemyScript;
    [SerializeField] private PlayerRendererScript playerRenderer;
    [SerializeField] private CapsuleCollider2D playerDialogueArea;
    [SerializeField] private DialogueCameraScript dialogueCamera;
    [SerializeField] private Text tq1, ca2, tpq3;
    [SerializeField] private GameObject[] hearth, key;
    [SerializeField] private float timerSummary;
    [SerializeField] private BattleSoundtrackScript battleSoundtrack;

    private void Start()
    {
        timerSummary = 0;

        asignSummary();
    }

    void asignSummary()
    {
        tq1.text = GameSystemScript.CurrentLevelSO.totalQuestions.ToString();
        ca2.text = GameSystemScript.CurrentLevelSO.correctAnswers.ToString();
        tpq3.text = GameSystemScript.CurrentLevelSO.timePerQuestion.ToString();
    }

    public void averageTimePerQuestions()
    {
        GameSystemScript.CurrentLevelSO.timePerQuestion /= GameSystemScript.CurrentLevelSO.totalQuestions;
        tpq3.text = GameSystemScript.CurrentLevelSO.timePerQuestion.ToString();
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

                        GameSystemScript.CurrentLevelSO.totalQuestions += 1;
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

                    GameSystemScript.DialogueSystem.displaySettings.inputSettings.responseTimeout = currentEnemyScript.EnemyData.configurations.ilo_parameters[0].default_value;
                    GameSystemScript.DialogueSystem.displaySettings.inputSettings.responseTimeoutAction = ResponseTimeoutAction.Custom;

                    useCurrentSelection();
                }
            }
        }
        else if (collision.tag == "Heart" && GameSystemScript.CurrentLevelSO.playerLives < 3 && GameSystemScript.CurrentLevelSO.heart == false)
        {
            GameSystemScript.CurrentLevelSO.heart = true;

            Debug.Log("Se gano un corazon");

            collision.gameObject.SetActive(false);

            SoundsScript.PlaySound("WIN HEART");

            GameSystemScript.CurrentLevelSO.playerLives += 1;
            setLives();
        }
    }

    IEnumerator CRTStartTimer()
    {
        yield return new WaitForSeconds(0.2f);

        GameSystemScript.Timer.SetActive(false);
        //Set question time limit based on LX
        GameSystemScript.Timer.GetComponent<TimerScript>().StartingTime = currentEnemyScript.EnemyData.configurations.ilo_parameters[0].default_value;
        GameSystemScript.Timer.GetComponent<TimerScript>().Aux = GameSystemScript.Timer.GetComponent<TimerScript>().StartingTime;
        if (GameSystemScript.Timer.GetComponent<TimerScript>().Slider) GameSystemScript.Timer.GetComponent<TimerScript>().Slider.value = 1;
        GameSystemScript.Timer.GetComponent<TimerScript>().Finish = false;
        yield return new WaitForSeconds(1.8f);

        //2 seconds ahead
        GameSystemScript.Timer.SetActive(true);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        dialogueCamera.Target = null;
        if (collision.gameObject.tag == "NextLevel" && GameSystemScript.CurrentLevelSO.playerKeyParts == 3)
        {
            lookTarget(collision.gameObject);
            GameSystemScript.Timer.SetActive(false);
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
        LevelScript.Instance.LoadNextLevel();
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
        if (GameSystemScript.Timer) GameSystemScript.Timer.SetActive(false);

        if (currentEnemyScript)
        {
            currentEnemyScript.GetComponent<EnemyScript>().defeated();

            GameSystemScript.CurrentLevelSO.correctAnswers += 1;
            GameSystemScript.CurrentLevelSO.timePerQuestion += Mathf.RoundToInt((Time.time - timerSummary) % 60);

            asignSummary();
        }
    }

    public void playerDefeated()
    {
        if (GameSystemScript.Timer) GameSystemScript.Timer.SetActive(false);

        if (currentEnemyScript)
        {
            currentEnemyScript.GetComponent<EnemyScript>().winner();

            GameSystemScript.CurrentLevelSO.timePerQuestion += Mathf.RoundToInt((Time.time - timerSummary) % 60);

            asignSummary();
        }
    }

    public void setLives()
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

    public void setKeys()
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
