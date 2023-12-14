using PixelCrushers.DialogueSystem;
using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EnemyScript : MonoBehaviour
{
    [Header("DATA")]
    [SerializeField] private int knowledgePoints;
    [SerializeField] private int hp;
    [SerializeField] private float velocity;

    [Header("STATE")]
    [SerializeField] private bool startQuestion = false;
    [SerializeField] private bool isMoving = false;
    [SerializeField] private bool isAttacking = false;

    [Header("UI")]
    [SerializeField] private DialogueSystemTrigger dialogueSystemTrigger;
    [SerializeField] private ParticleSystem pointsParticles;
    [SerializeField] private ParticleSystem[] keysParticles;
    [SerializeField] private Color[] colors;

    [Header("BODY")]
    [SerializeField] private Rigidbody2D rbody;
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private Animator animator;
    [SerializeField] private CircleCollider2D coll2D;
    [SerializeField] private CircleCollider2D blockColl2D;
    [SerializeField] private Vector2 roomEdgesPosition;
    [SerializeField] private Vector2 roomEdgesSize;
    [SerializeField] private Vector2 roomEdgesEnd;
    [SerializeField] private GameObject characterCollisionBlocker;

    [Header("BULLETS")]
    [SerializeField] private BulletGeneratorScript bulletGenerator;

    private EnemySO enemyData;
    private QuestionSO questionData;

    private void Update()
    {
        ManageMovement();
    }

    public void ManageMovement()
    {
        Vector2 movementInput = rbody.velocity;

        if (movementInput.magnitude > 0f)
        {
            animator.SetBool("isWalking", true);
        }
        else
        {
            animator.SetBool("isWalking", false);
        }

        if (movementInput.x > 0.01f)
        {
            sprite.flipX = false;
        }
        else if (movementInput.x < -0.01f)
        {
            sprite.flipX = true;
        }
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "LevelCollisions")
        {
            isMoving = false;
        }
    }

    public void ShootBullets()
    {
        bulletGenerator.StartBullets = true;
        bulletGenerator.Init(this, 3);
    }

    public void Defeated()
    {
        //After some time and animation
        StartCoroutine(CRTHitEnemy());
    }

    IEnumerator CRTHitEnemy()
    {
        LevelController.Instance.VirtualCamera2.ShakeCamera(0f, 0f);
        //Time for player animation
        yield return new WaitForSeconds(0.5f);

        LevelController.Instance.Player.GetComponent<Animator>().SetTrigger("attacked");

        yield return new WaitForSeconds(1 / 3f);

        yield return new WaitForSeconds(1 / 12f);

        LevelController.Instance.VirtualCamera2.ShakeCamera(1.5f, 0.2f);

        if (enemyData.mobId != 0)
        {
            animator.SetTrigger("wasHit");
        }

        hp -= 1;

        if (hp == 0)
        {
            StartCoroutine(CRTDissappear());
        }

        yield return new WaitForSeconds(1.2f);

        isAttacking = true;
    }

    IEnumerator CRTDissappear()
    {
        LevelController.Instance.Player.DialogueCamera.Target = null;
        DialogueLua.SetVariable("StartQuestion", 0);

        LevelController.Instance.VirtualCamera2.ShakeCamera(0f, 0f);
        LevelController.Instance.DialogueCamera.EndDialogue();

        yield return new WaitForSeconds(0.5f);

        //Deactivate dialogue
        characterCollisionBlocker.SetActive(false);
        this.transform.GetChild(0).gameObject.SetActive(false);
        LevelController.Instance.RoomEdgesCollider.GetComponent<TilemapRenderer>().enabled = false;
        //GameSystemScript.roomEdges.SetActive(false);
        LevelController.Instance.RoomEdgesCollider.enabled = false;
        coll2D.enabled = false;
        LevelController.Instance.Player.CurrentEnemy = null;
        LevelController.Instance.Player.CurrentEnemyScript = null;

        pointsParticles.Play();

        yield return new WaitForSeconds(0.01f);
        sprite.enabled = false;

        if (PlayerLevelInfo.playerKeyParts < 3)
        {
            keysParticles[PlayerLevelInfo.playerKeyParts].Play();
            yield return new WaitForSeconds(0.5f);

            PlayerLevelInfo.playerKeyParts += 1;

            Feedback.Do(eFeedbackType.KeyUnlocking);

            LevelController.Instance.SetKeys();
        }

        GameManager.ChangeKnowledgePoints(knowledgePoints, LevelController.Instance.KnowledgePoints);
    }

    public void Winner()
    {
        //Have to be changed to only disappear the points, but the body stay it.
        StartCoroutine(CRTRestart());
    }

    IEnumerator CRTRestart()
    {
        LevelController.Instance.VirtualCamera2.ShakeCamera(1.5f, 0.2f);

        //Shoot 1 bullet
        bulletGenerator.StartBullets = true;
        bulletGenerator.Init(this, 1);

        yield return new WaitForSeconds(1f);

        isAttacking = true;
    }

    public void HitPlayer(GameObject bullet)
    {
        StartCoroutine(CRTHitPlayer(bullet));
    }

    IEnumerator CRTHitPlayer(GameObject bullet)
    {
        if (PlayerLevelInfo.playerLives > 0)
        {
            LevelController.Instance.VirtualCamera2.ShakeCamera(2f, 0.2f);

            //Hit player
            LevelController.Instance.Player.GetComponent<Animator>().SetTrigger("wasHit");

            LevelController.Instance.Player.GetComponent<Rigidbody2D>().AddForce(500f * (LevelController.Instance.Player.transform.position - bullet.transform.position).normalized);

            GameManager.ChangeKnowledgePoints(-knowledgePoints, LevelController.Instance.KnowledgePoints);

            PlayerLevelInfo.playerLives -= 1;

            LevelController.Instance.SetLives();
        }

        yield return new WaitForSeconds(0f);

        if (PlayerLevelInfo.playerLives == 0)
        {
            LevelController.Instance.Joystick.SetActive(false);

            this.transform.GetChild(0).gameObject.SetActive(false);

            //this.GetComponent<CircleCollider2D>().enabled = false;
            rbody.constraints = RigidbodyConstraints2D.FreezeAll;

            yield return new WaitForSeconds(0.5f);

            LevelController.Instance.Player.GetComponent<Animator>().SetTrigger("isDead");
        }
    }

    public void InitEnemyData()
    {
        //Shuffle Button's colors
        colors = new Color[4] {
            new Color(0.91f, 0.36f, 0.31f),
            new Color(0.67f, 0.86f, 0.46f),
            new Color(0.27f, 0.78f, 0.99f),
            new Color(1.00f, 0.88f, 0.45f) };

        for (int i = 0; i < 4; i++)
        {
            int r = i + Random.Range(0, 4 - i);
            Color temp = colors[r];
            colors[r] = colors[i];
            colors[i] = temp;
        }
        //Color 0 will be Correct Answer

        if (enemyData.mobId != 0)
        {
            Physics2D.IgnoreCollision(this.GetComponent<Collider2D>(),
                LevelController.Instance.Player.transform.GetChild(0).GetComponent<Collider2D>());
        }
        else
        {
            coll2D.offset = Vector2.zero;
            coll2D.radius = 0.24f;
            coll2D.enabled = false;
            blockColl2D.offset = Vector2.zero;
            blockColl2D.radius = 0.24f;
            rbody.constraints = RigidbodyConstraints2D.FreezeAll;
            characterCollisionBlocker.SetActive(false);
        }

        knowledgePoints = enemyData.knowledgePoints;
        hp = enemyData.hp;
        velocity = enemyData.velocity;

        var main = pointsParticles.main;
        main.maxParticles = knowledgePoints;

        //Get a random question of the enemyData questions database
        int auxQuestion = Random.Range(0, enemyData.questions.Length);
        questionData = enemyData.questions[auxQuestion];
    }

    private void ChooseLearningAchievement()
    {
        string question;

        int questionLevel;

        if (PlayerLevelInfo.currentLevel < questionData.questionES.Length ||
            PlayerLevelInfo.currentLevel < questionData.questionEN.Length)
        {
            questionLevel = PlayerLevelInfo.currentLevel;
        }
        else
        {
            questionLevel = 0;
        }

        int rndQuestion;

        switch (Localization.language)
        {
            case "es":
                rndQuestion = Random.Range(0, questionData.questionES[questionLevel].questions.Length);
                question = questionData.questionES[questionLevel].questions[rndQuestion];
                break;

            case "en":
                rndQuestion = Random.Range(0, questionData.questionEN[questionLevel].questions.Length);
                question = questionData.questionEN[questionLevel].questions[rndQuestion];
                break;

            case "qu":
            default:
                question = "";
                break;
        }

        DialogueManager.masterDatabase.GetConversation("Math Question").GetDialogueEntry(1).DialogueText =
            "<waitfor=0.5>" + question;
    }

    private void SetDefaultButtonsColors()
    {
        //Set Colors
        PlayerLevelInfo.colors[0] = colors[0];
        PlayerLevelInfo.colors[1] = colors[1];
        PlayerLevelInfo.colors[2] = colors[2];
        PlayerLevelInfo.colors[3] = colors[3];
        PlayerLevelInfo.colorsCount = 0;
    }

    public void SetQuestionParameters()
    {
        startQuestion = true;

        ChooseLearningAchievement();

        SetDefaultButtonsColors();

        MathHelper.CreateQuestion(questionData.name);
    }

    public DialogueSystemTrigger DialogueSystemTrigger => dialogueSystemTrigger;

    public Color[] Colors => colors;

    public float Velocity => velocity;

    public Vector2 RoomEdgesPosition
    {
        get { return roomEdgesPosition; }
        set { roomEdgesPosition = value; }
    }

    public Vector2 RoomEdgesEnd
    {
        get { return roomEdgesEnd; }
        set { roomEdgesEnd = value; }
    }

    public Vector2 RoomEdgesSize
    {
        get { return roomEdgesSize; }
        set { roomEdgesSize = value; }
    }

    public EnemySO EnemyData
    {
        get { return enemyData; }
        set { enemyData = value; }
    }

    public bool IsMoving
    {
        get { return isMoving; }
        set { isMoving = value; }
    }

    public bool IsAttacking
    {
        get { return isAttacking; }
        set { isAttacking = value; }
    }

    public bool StartQuestion
    {
        get { return startQuestion; }
        set { startQuestion = value; }
    }
}
