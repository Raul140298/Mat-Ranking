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
                        LookTarget(currentEnemy);

                        dialogueCamera.Target = currentEnemy;
                        LevelScript.Instance.DialogueCamera.StartDialogue();

                        //this.GetComponent<OutlineScript>().OutlineOff();

                        SoundsScript.PlaySound("EXCLAMATION");

                        GameSystemScript.CurrentLevelSO.totalQuestions += 1;

                        StartCoroutine(CRTStartTimer());

                        timerSummary = Time.time;

                        LevelScript.Instance.BattleSoundtrack.StartBattleSoundtrack();

                        //In case the Behavior Tree was in timer
                        currentEnemyScript.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                        currentEnemyScript.GetComponent<Animator>().SetTrigger("start");


                        LevelScript.Instance.RoomEdgesCollider.enabled = true;
                        LevelScript.Instance.RoomEdgesCollider.GetComponent<TilemapRenderer>().enabled = true;
                    }

                    GameSystemScript.DialogueSystem.displaySettings.inputSettings.responseTimeout = currentEnemyScript.EnemyData.configurations.ilo_parameters[0].default_value;
                    GameSystemScript.DialogueSystem.displaySettings.inputSettings.responseTimeoutAction = ResponseTimeoutAction.Custom;

                    UseCurrentSelection();
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
        if (GameSystemScript.Timer.Slider) GameSystemScript.Timer.Slider.value = 1;
        GameSystemScript.Timer.Finish = false;
        yield return new WaitForSeconds(1.8f);

        //2 seconds ahead
        GameSystemScript.Timer.gameObject.SetActive(true);
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

    public void UseCurrentSelection()
    {
        //Edit vairables before the conversation start
        currentEnemyScript.SetVariables();
        //then start the conversation
        proximitySelector.UseCurrentSelection();
    }

    public void DefeatedEnemy()
    {
        if (GameSystemScript.Timer) GameSystemScript.Timer.gameObject.SetActive(false);

        if (currentEnemyScript)
        {
            currentEnemyScript.Defeated();

            GameSystemScript.CurrentLevelSO.correctAnswers += 1;
            GameSystemScript.CurrentLevelSO.timePerQuestion += Mathf.RoundToInt((Time.time - timerSummary) % 60);
        }
    }

    public void PlayerDefeated()
    {
        if (GameSystemScript.Timer) GameSystemScript.Timer.gameObject.SetActive(false);

        this.GetComponent<OutlineScript>().OutlineOff();
        this.GetComponent<OutlineScript>().OutlineLocked();

        if (currentEnemyScript)
        {
            currentEnemyScript.Winner();

            GameSystemScript.CurrentLevelSO.timePerQuestion += Mathf.RoundToInt((Time.time - timerSummary) % 60);
        }
    }

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
