using System.Collections;
using UnityEngine;
using PixelCrushers.DialogueSystem;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class PlayerModelScript : ActorModelScript
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float speed;
    private Vector2 movementInput;
    
    [Header("DIALOGUE")]
    [SerializeField] private ProximitySelector proximitySelector;
    [SerializeField] private Collider2D playerDialogueArea;
    [SerializeField] private DialogueCameraScript dialogueCamera;

    [Header("INTERACTABLES")]
    [SerializeField] private ClickableScript clickable;
    [SerializeField] private NpcScript currentNPC;
    [SerializeField] private GameObject currentEnemy;
    [SerializeField] private EnemyModelScript currentEnemyModelScript;

    private float timerSummary;

    private void Awake()
    {
        timerSummary = 0;
    }

    private void Start()
    {
        if (clickable) clickable.MakeClickable();
    }
    
    void FixedUpdate()
    {
        rb.velocity = movementInput.normalized * speed;
    }

    public void OnMovement(InputAction.CallbackContext value)
    {
        movementInput = value.ReadValue<Vector2>();

        if (movementInput != Vector2.zero)
        {
            SetDirection(GetDirectionFromVector(movementInput));
            compRendering.PlayAnimation(eAnimation.Walk);
        }
        else
        {
            compRendering.PlayAnimation(eAnimation.Idle);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //CheckStairsCollision(collision);
    }
    
    private void OnTriggerEnter2D(Collider2D collider)
    {
        //CheckDialoguerEnter(collider);
        //CheckEnemyEnter(collider);
        //CheckHeartEnter(collider);
    }
    
    private void OnTriggerExit2D(Collider2D collision)
    {
        //CheckDialoguerExit(collision);
    }
    
    //###### METHODS ##########################################################################################

/*
    private void CheckStairsCollision(Collision2D collision)
    {
        if (collision.gameObject.tag == "NextLevel")
        {
            if (PlayerLevelInfo.playerKeyParts == 3)
            {
                LookTarget(collision.gameObject);
                DialoguePanelManager.Timer.gameObject.SetActive(false);
                proximitySelector.UseCurrentSelection();
            }
        }
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
    
    private void CheckHeartEnter(Collider2D collision)
    {
        if (collision.tag == "Heart")
        {
            if (PlayerLevelInfo.playerLives < 3 &&
                PlayerLevelInfo.heart == false)
            {
                PlayerLevelInfo.heart = true;

                Debug.Log("Se gano un corazon");

                collision.gameObject.SetActive(false);
                
                Feedback.Do(eFeedbackType.WinHeart);

                PlayerLevelInfo.playerLives += 1;
                LevelController.Instance.SetLives();
            }
        }
    }

    private void CheckEnemyEnter(Collider2D collider)
    {
        if (collider.tag == "Enemy")
        {
            currentEnemy = collider.gameObject;
            currentEnemyModelScript = currentEnemy.transform.parent.GetComponent<EnemyModelScript>();

            if (currentEnemyModelScript.EnemyData != null &&
                currentEnemyModelScript.RoomEdgesPosition.x < (this.transform.position.x) &&
                currentEnemyModelScript.RoomEdgesEnd.x > (this.transform.position.x) &&
                currentEnemyModelScript.RoomEdgesPosition.y < this.transform.position.y &&
                currentEnemyModelScript.RoomEdgesEnd.y > this.transform.position.y)
            {
                if (playerDialogueArea.enabled == true &&
                    currentEnemyModelScript.IsAttacking == false &&
                    currentEnemyModelScript.StartQuestion == false)
                {
                    Feedback.Do(eFeedbackType.Exclamation);

                    LookTarget(currentEnemy);
                    LevelController.Instance.DialogueCamera.StartDialogue(currentEnemy);
                    LevelController.Instance.RoomEdgesCollider.enabled = true;
                    LevelController.Instance.RoomEdgesCollider.GetComponent<TilemapRenderer>().enabled = true;
                    
                    //In case the Behavior Tree was in timer
                    currentEnemyModelScript.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                    currentEnemyModelScript.GetComponent<Animator>().SetTrigger("start");

                    StartCoroutine(CRTStartTimer());

                    PlayerLevelInfo.totalQuestions += 1;
                    DialogueManager.displaySettings.inputSettings.responseTimeout = currentEnemyModelScript.EnemyData.configurations.ilo_parameters[0].default_value;
                    DialogueManager.displaySettings.inputSettings.responseTimeoutAction = ResponseTimeoutAction.Custom;

                    currentEnemyModelScript.SetQuestionParameters();

                    UseCurrentSelection();
                }
            }
        }
    }
*/

    public void UseCurrentSelection()
    {
        if (currentNPC) LookPlayer();
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

        DialoguePanelManager.Timer.gameObject.SetActive(false);
        //Set question time limit based on LX
        DialoguePanelManager.Timer.StartingTime = currentEnemyModelScript.EnemyData.configurations.ilo_parameters[0].default_value;
        DialoguePanelManager.Timer.Aux = DialoguePanelManager.Timer.StartingTime;
        DialoguePanelManager.Timer.Slider.value = 1;
        DialoguePanelManager.Timer.Finish = false;
        yield return new WaitForSeconds(1.8f);

        //2 seconds ahead
        DialoguePanelManager.Timer.gameObject.SetActive(true);
        timerSummary = Time.time;
    }
    
    public void AnswerCorrectly()
    {
        DialoguePanelManager.Timer.gameObject.SetActive(false);

        currentEnemyModelScript.Defeated();

        PlayerLevelInfo.correctAnswers += 1;
        PlayerLevelInfo.timePerQuestion += Mathf.RoundToInt((Time.time - timerSummary) % 60);
    }

    public void AnswerIncorrectly()
    {
        DialoguePanelManager.Timer.gameObject.SetActive(false);

        compRendering.OutlineOff();
        compRendering.OutlineLocked();

        currentEnemyModelScript.Winner();

        PlayerLevelInfo.timePerQuestion += Mathf.RoundToInt((Time.time - timerSummary) % 60);
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
    
    public DialogueCameraScript DialogueCamera => dialogueCamera;
    public Collider2D PlayerDialogueArea => playerDialogueArea;
    
    public EnemyModelScript CurrentEnemyModelScript
    {
        get { return currentEnemyModelScript; }
        set { currentEnemyModelScript = value; }
    }

    public GameObject CurrentEnemy
    {
        get { return currentEnemy; }
        set { currentEnemy = value; }
    }
}
