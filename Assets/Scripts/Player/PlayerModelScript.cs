using System.Collections;
using UnityEngine;
using PixelCrushers.DialogueSystem;
using UnityEngine.Tilemaps;

public class PlayerModelScript : MonoBehaviour
{
    [Header("INFO")]
    [SerializeField] private ProximitySelector proximitySelector;
    [SerializeField] private PlayerRendererScript playerRenderer;
    [SerializeField] private RenderingScript compRendering;
    [SerializeField] private Collider2D playerDialogueArea;

    [Header("CAMERA")]
    [SerializeField] private DialogueCameraScript dialogueCamera;

    [Header("INTERACTABLES")]
    [SerializeField] private NpcScript currentNPC;
    [SerializeField] private ClickableScript clickable;
    [SerializeField] private GameObject currentEnemy;
    [SerializeField] private EnemyScript currentEnemyScript;

    [SerializeField] private float timerSummary;

    private void Awake()
    {
        timerSummary = 0;
    }

    private void Start()
    {
        StartCoroutine(CRTInit());
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        CheckStairsCollision(collision);
    }

    private void CheckStairsCollision(Collision2D collision)
    {
        if (collision.gameObject.tag == "NextLevel")
        {
            if (GameSystemScript.CurrentLevelSO.playerKeyParts == 3)
            {
                LookTarget(collision.gameObject);
                GameSystemScript.Timer.gameObject.SetActive(false);
                //this.GetComponent<OutlineScript>().OutlineOff();
                proximitySelector.UseCurrentSelection();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        CheckDialoguerEnter(collider);
        CheckEnemyEnter(collider);
        CheckHeartEnter(collider);
    }

    private void CheckDialoguerEnter(Collider2D collider)
    {
        if (collider.tag == "NPCDialogue")
        {
            if (collider.gameObject.name != "Tower")
            {
                LookTarget(collider.gameObject);
                currentNPC = collider.transform.parent.GetComponent<NpcScript>();
                dialogueCamera.Target = currentNPC.gameObject;
            }

            clickable = collider.transform.parent.GetComponent<ClickableScript>();
        }
    }

    private void CheckEnemyEnter(Collider2D collider)
    {
        if (collider.tag == "Enemy")
        {
            currentEnemy = collider.gameObject;
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

                    currentEnemyScript.SetQuestionParameters();

                    UseCurrentSelection();
                }
            }
        }
    }

    private void CheckHeartEnter(Collider2D collision)
    {
        if (collision.tag == "Heart")
        {
            if (GameSystemScript.CurrentLevelSO.playerLives < 3 &&
                GameSystemScript.CurrentLevelSO.heart == false)
            {
                GameSystemScript.CurrentLevelSO.heart = true;

                Debug.Log("Se gano un corazon");

                collision.gameObject.SetActive(false);

                SoundsScript.PlaySound("WIN HEART");

                GameSystemScript.CurrentLevelSO.playerLives += 1;
                LevelScript.Instance.SetLives();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        CheckDialoguerExit(collision);
    }

    private void CheckDialoguerExit(Collider2D collision)
    {
        if (collision.tag == "NPCDialogue")
        {
            if (currentNPC == collision.transform.parent.GetComponent<NpcScript>())
            {
                dialogueCamera.Target = null;
                currentNPC = null;
            }
        }
    }

    //Functions
    IEnumerator CRTInit()
    {
        yield return new WaitForSeconds(0.5f);
        if (clickable) clickable.MakeClickable();
    }

    public void UseCurrentSelection()
    {
        LookPlayer();
        proximitySelector.UseCurrentSelection();
    }

    public void CheckIfNpcWantToTalk()
    {
        if (currentNPC && currentNPC.name != "Tower")
        {
            currentNPC.CheckIfWantToTalk();
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

        compRendering.OutlineOff();
        compRendering.OutlineLocked();

        currentEnemyScript.Winner();

        GameSystemScript.CurrentLevelSO.timePerQuestion += Mathf.RoundToInt((Time.time - timerSummary) % 60);
    }

    private void LookTarget(GameObject target)
    {
        if (this.gameObject.transform.position.x > target.gameObject.transform.position.x &&
            !playerRenderer.PlayerIsLookingLeft())
        {
            playerRenderer.SpriteRenderer.flipX = true;
            if (target.tag == "Enemy") target.transform.parent.GetComponent<SpriteRenderer>().flipX = false;
        }
        else if (this.gameObject.transform.position.x < target.gameObject.transform.position.x &&
            playerRenderer.PlayerIsLookingLeft())
        {
            playerRenderer.SpriteRenderer.flipX = false;
            if (target.tag == "Enemy") target.transform.parent.GetComponent<SpriteRenderer>().flipX = true;
        }
    }

    public void LookPlayer()
    {
        bool isLookingPlayer = this.gameObject.transform.position.x < currentNPC.transform.position.x;
        currentNPC.RenderingScript.FlipX(isLookingPlayer);
    }

    public void MakeDialoguerClickable()
    {
        clickable.MakeClickable();
    }

    public void MakeDialoguerNonClickable()
    {
        clickable.MakeNonClickable();
    }

    public RenderingScript CompRendering => compRendering;
    public DialogueCameraScript DialogueCamera => dialogueCamera;
    public Collider2D PlayerDialogueArea => playerDialogueArea;

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
