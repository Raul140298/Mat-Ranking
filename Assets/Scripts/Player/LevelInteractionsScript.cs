using UnityEngine;
using PixelCrushers.DialogueSystem;
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
    [SerializeField] private float timerSummary;

    private void Awake()
    {
        timerSummary = 0;
    }

    //Interactions with colliders and triggers
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Enemy")
        {
            currentEnemy = collision.gameObject;
            currentEnemyScript = currentEnemy.transform.parent.GetComponent<EnemyScript>();

            if (currentEnemyScript.EnemyData != null &&
                currentEnemyScript.RoomEdgesPosition.x < (this.transform.position.x) &&
                currentEnemyScript.RoomEdgesEnd.x > (this.transform.position.x) &&
                currentEnemyScript.RoomEdgesPosition.y < this.transform.position.y &&
                currentEnemyScript.RoomEdgesEnd.y > this.transform.position.y)
            {
                if (playerDialogueArea.enabled == true &&
                    currentEnemyScript.IsAttacking == false &&
                    currentEnemyScript.StartQuestion == false)
                {
                    SoundsScript.PlaySound("EXCLAMATION");

                    LookTarget(currentEnemy);
                    LevelScript.Instance.DialogueCamera.StartDialogue(currentEnemy);
                    LevelScript.Instance.RoomEdgesCollider.enabled = true;
                    LevelScript.Instance.RoomEdgesCollider.GetComponent<TilemapRenderer>().enabled = true;
                    LevelScript.Instance.BattleSoundtrack.StartBattleSoundtrack();

                    //In case the Behavior Tree was in timer
                    currentEnemyScript.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                    currentEnemyScript.GetComponent<Animator>().SetTrigger("start");

                    StartCoroutine(CRTStartTimer());

                    GameSystemScript.CurrentLevelSO.totalQuestions += 1;
                    GameSystemScript.DialogueSystem.displaySettings.inputSettings.responseTimeout = currentEnemyScript.EnemyData.configurations.ilo_parameters[0].default_value;
                    GameSystemScript.DialogueSystem.displaySettings.inputSettings.responseTimeoutAction = ResponseTimeoutAction.Custom;

                    EnemySetQuestionParameters();

                    StartQuestion();
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
            LevelScript.Instance.SetLives();
        }
    }

    IEnumerator CRTStartTimer()
    {
        yield return new WaitForSeconds(0.2f);

        GameSystemScript.Timer.gameObject.SetActive(false);
        //Set question time limit based on LX
        GameSystemScript.Timer.StartingTime = currentEnemyScript.EnemyData.configurations.ilo_parameters[0].default_value;
        GameSystemScript.Timer.Aux = GameSystemScript.Timer.StartingTime;
        GameSystemScript.Timer.Slider.value = 1;
        GameSystemScript.Timer.Finish = false;
        yield return new WaitForSeconds(1.8f);

        //2 seconds ahead
        GameSystemScript.Timer.gameObject.SetActive(true);
        timerSummary = Time.time;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //dialogueCamera.Target = null;
        if (collision.gameObject.tag == "NextLevel" && GameSystemScript.CurrentLevelSO.playerKeyParts == 3)
        {
            LookTarget(collision.gameObject);
            GameSystemScript.Timer.gameObject.SetActive(false);
            //this.GetComponent<OutlineScript>().OutlineOff();
            proximitySelector.UseCurrentSelection();
        }
    }

    //Functions
    public void LookTarget(GameObject target)
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

    public void StartQuestion()
    {
        //then start the conversation
        proximitySelector.UseCurrentSelection();
    }

    public void EnemySetQuestionParameters()
    {
        //Edit vairables before the conversation start
        currentEnemyScript.SetQuestionParameters();
    }

    public void AnswerCorrectly()
    {
        GameSystemScript.Timer.gameObject.SetActive(false);

        currentEnemyScript.Defeated();

        GameSystemScript.CurrentLevelSO.correctAnswers += 1;
        GameSystemScript.CurrentLevelSO.timePerQuestion += Mathf.RoundToInt((Time.time - timerSummary) % 60);
    }

    public void AnswerIncorrectly()
    {
        GameSystemScript.Timer.gameObject.SetActive(false);

        this.GetComponent<OutlineScript>().OutlineOff();
        this.GetComponent<OutlineScript>().OutlineLocked();

        currentEnemyScript.Winner();

        GameSystemScript.CurrentLevelSO.timePerQuestion += Mathf.RoundToInt((Time.time - timerSummary) % 60);
    }

    public DialogueCameraScript DialogueCamera => dialogueCamera;

    public CapsuleCollider2D PlayerDialogueArea => playerDialogueArea;

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
