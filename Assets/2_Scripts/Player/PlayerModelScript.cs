using System.Collections;
using UnityEngine;
using PixelCrushers.DialogueSystem;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class PlayerModelScript : MonoBehaviour
{
    [Header("INFO")]
    [SerializeField] private RenderingScript compRendering;
    [SerializeField] private float speed;
    [SerializeField] private Rigidbody2D rb;
    private Vector2 movementInput;
    private eDirection lastDirection;
    private eDirection direction;
    private bool isThrowingProjectile = false;
    private bool isAttacking = false;
    private bool isDashing = false;
    
    [Header("DIALOGUE")]
    [SerializeField] private ProximitySelector proximitySelector;
    [SerializeField] private Collider2D playerDialogueArea;
    [SerializeField] private DialogueCameraScript dialogueCamera;

    [Header("INTERACTABLES")]
    [SerializeField] private ClickableScript clickable;
    [SerializeField] private NpcScript currentNPC;
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
    
    public void SetDirection(eDirection dir)
    {
        lastDirection = direction;
        direction = dir;
    }
    		
    private eDirection GetDirectionFromVector(Vector2 vector)
    {
    	float angle = Mathf.Atan2(vector.y, vector.x) * Mathf.Rad2Deg;

    	if (angle > -45 && angle < 45)
    	{
    		return eDirection.Right;
    	}
    	else if (angle >= 45 && angle <= 135)
    	{
    		return eDirection.Up;
    	}
    	else if (angle > 135 || angle < -135)
    	{
    		return eDirection.Left;
    	}
    	else
    	{
    		return eDirection.Down;
    	}
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        CheckStairsCollision(collision);
    }

    private void CheckStairsCollision(Collision2D collision)
    {
        if (collision.gameObject.tag == "NextLevel")
        {
            if (PlayerLevelInfo.playerKeyParts == 3)
            {
                LookTarget(collision.gameObject);
                DialoguePanelManager.Timer.gameObject.SetActive(false);
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
                    Feedback.Do(eFeedbackType.Exclamation);

                    LookTarget(currentEnemy);
                    LevelController.Instance.DialogueCamera.StartDialogue(currentEnemy);
                    LevelController.Instance.RoomEdgesCollider.enabled = true;
                    LevelController.Instance.RoomEdgesCollider.GetComponent<TilemapRenderer>().enabled = true;
                    
                    //In case the Behavior Tree was in timer
                    currentEnemyScript.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                    currentEnemyScript.GetComponent<Animator>().SetTrigger("start");

                    StartCoroutine(CRTStartTimer());

                    PlayerLevelInfo.totalQuestions += 1;
                    DialogueManager.displaySettings.inputSettings.responseTimeout = currentEnemyScript.EnemyData.configurations.ilo_parameters[0].default_value;
                    DialogueManager.displaySettings.inputSettings.responseTimeoutAction = ResponseTimeoutAction.Custom;

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
        DialoguePanelManager.Timer.StartingTime = currentEnemyScript.EnemyData.configurations.ilo_parameters[0].default_value;
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

        currentEnemyScript.Defeated();

        PlayerLevelInfo.correctAnswers += 1;
        PlayerLevelInfo.timePerQuestion += Mathf.RoundToInt((Time.time - timerSummary) % 60);
    }

    public void AnswerIncorrectly()
    {
        DialoguePanelManager.Timer.gameObject.SetActive(false);

        compRendering.OutlineOff();
        compRendering.OutlineLocked();

        currentEnemyScript.Winner();

        PlayerLevelInfo.timePerQuestion += Mathf.RoundToInt((Time.time - timerSummary) % 60);
    }

    private void LookTarget(GameObject target)
    {
        if (this.gameObject.transform.position.x > target.gameObject.transform.position.x)
        {
            if (target.tag == "Enemy") target.transform.parent.GetComponent<SpriteRenderer>().flipX = false;
        }
        else if (this.gameObject.transform.position.x < target.gameObject.transform.position.x)
        {
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
    
    private void ThrowingProjectileEnd()
    {
        isThrowingProjectile = false;
    }
	
    private void AttackEnd()
    {
        isAttacking = false;
    }
	
    private void DashEnd()
    {
        isDashing = false;
    }

    public bool IsThrowingProjectile => isThrowingProjectile;
    public bool IsDashing => isDashing;
    public bool IsAttacking => isAttacking;
    public RenderingScript CompRendering => compRendering;
    public DialogueCameraScript DialogueCamera => dialogueCamera;
    public Collider2D PlayerDialogueArea => playerDialogueArea;
    public eDirection LastDirection => lastDirection;
    public eDirection Direction => direction;

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
