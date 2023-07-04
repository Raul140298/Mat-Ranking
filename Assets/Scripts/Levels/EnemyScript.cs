using PixelCrushers.DialogueSystem;
using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EnemyScript : MonoBehaviour
{
    [Header("DATA")]
    [SerializeField] private int knowledgePoints;
    [SerializeField] private int hp;

    [Header("STATE")]
    [SerializeField] private bool startQuestion = false;
    [SerializeField] private bool isMoving = false;
    [SerializeField] private bool isAttacking = false;

    [Header("UI")]
    [SerializeField] private DialogueSystemTrigger dialogueSystemTrigger;
    [SerializeField] private ParticleSystem pointsParticles;
    [SerializeField] private ParticleSystem[] keysParticles;
    [SerializeField] private Color[] colors;
    [SerializeField] private AudioSource enemyAudioSource;

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

    IEnumerator CRTMakeSounds()
    {
        float aux = (float)Random.Range(30, 100) / 10f;
        yield return new WaitForSeconds(aux);
        PlayNeutralSound();

        if (this.GetComponent<SpriteRenderer>().enabled) StartCoroutine(CRTMakeSounds());
    }

    public void PlayNeutralSound()
    {
        SoundsScript.PlayEnemySound("MOB" + enemyData.mobId.ToString(), enemyAudioSource);//1 have to be changed by distance from the player
    }

    public void ChangeVolume(float value)
    {
        enemyAudioSource.volume = value;
    }

    public void Defeated()
    {
        //After some time and animation
        StartCoroutine(CRTHitEnemy());
    }

    IEnumerator CRTHitEnemy()
    {
        LevelScript.Instance.VirtualCamera2.ShakeCamera(0f, 0f);
        //Time for player animation
        yield return new WaitForSeconds(0.5f);

        LevelScript.Instance.Player.GetComponent<Animator>().SetTrigger("attacked");

        yield return new WaitForSeconds(1 / 3f);

        yield return new WaitForSeconds(1 / 12f);

        LevelScript.Instance.VirtualCamera2.ShakeCamera(1.5f, 0.2f);

        if (enemyData.mobId != 0)
        {
            animator.SetTrigger("wasHit");
            enemyAudioSource.volume = 0;
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
        LevelScript.Instance.Player.DialogueCamera.Target = null;
        DialogueLua.SetVariable("StartQuestion", 0);

        LevelScript.Instance.VirtualCamera2.ShakeCamera(0f, 0f);
        LevelScript.Instance.BattleSoundtrack.EndBattleSoundtrack();
        LevelScript.Instance.DialogueCamera.EndDialogue();

        yield return new WaitForSeconds(0.5f);

        //Deactivate dialogue
        characterCollisionBlocker.SetActive(false);
        this.transform.GetChild(0).gameObject.SetActive(false);
        LevelScript.Instance.RoomEdgesCollider.GetComponent<TilemapRenderer>().enabled = false;
        //GameSystemScript.roomEdges.SetActive(false);
        LevelScript.Instance.RoomEdgesCollider.enabled = false;
        coll2D.enabled = false;
        LevelScript.Instance.Player.CurrentEnemy = null;
        LevelScript.Instance.Player.CurrentEnemyScript = null;

        pointsParticles.Play();

        yield return new WaitForSeconds(0.01f);
        sprite.enabled = false;

        if (GameSystemScript.CurrentLevelSO.playerKeyParts < 3)
        {
            keysParticles[GameSystemScript.CurrentLevelSO.playerKeyParts].Play();
            yield return new WaitForSeconds(0.5f);

            GameSystemScript.CurrentLevelSO.playerKeyParts += 1;

            SoundsScript.PlaySound("KEY UNLOCKING");

            LevelScript.Instance.SetKeys();
        }

        GameSystemScript.ChangeKnowledgePoints(knowledgePoints, LevelScript.Instance.KnowledgePoints);
    }

    public void Winner()
    {
        //Have to be changed to only disappear the points, but the body stay it.
        StartCoroutine(CRTRestart());
    }

    IEnumerator CRTRestart()
    {
        LevelScript.Instance.VirtualCamera2.ShakeCamera(1.5f, 0.2f);

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
        if (GameSystemScript.CurrentLevelSO.playerLives > 0)
        {
            LevelScript.Instance.VirtualCamera2.ShakeCamera(2f, 0.2f);

            //Hit player
            LevelScript.Instance.Player.GetComponent<Animator>().SetTrigger("wasHit");

            LevelScript.Instance.Player.GetComponent<Rigidbody2D>().AddForce(500f * (LevelScript.Instance.Player.transform.position - bullet.transform.position).normalized);

            GameSystemScript.ChangeKnowledgePoints(-knowledgePoints, LevelScript.Instance.KnowledgePoints);

            GameSystemScript.CurrentLevelSO.playerLives -= 1;

            LevelScript.Instance.SetLives();
        }

        yield return new WaitForSeconds(0f);

        if (GameSystemScript.CurrentLevelSO.playerLives == 0)
        {
            LevelScript.Instance.BattleSoundtrack.EndBattleSoundtrack();

            LevelScript.Instance.Joystick.SetActive(false);

            this.transform.GetChild(0).gameObject.SetActive(false);

            //this.GetComponent<CircleCollider2D>().enabled = false;
            rbody.constraints = RigidbodyConstraints2D.FreezeAll;

            yield return new WaitForSeconds(0.5f);

            LevelScript.Instance.Player.GetComponent<Animator>().SetTrigger("isDead");
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

        //Sounds
        enemyAudioSource.volume = GameSystemScript.OptionsSO.soundsVolume;
        SoundsScript.Slider.onValueChanged.AddListener(val => ChangeVolume(val));

        if (enemyData.mobId != 0)
        {
            StartCoroutine(CRTMakeSounds());
            Physics2D.IgnoreCollision(this.GetComponent<Collider2D>(),
                LevelScript.Instance.Player.transform.GetChild(0).GetComponent<Collider2D>());
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
        hp = 3;


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

        if (GameSystemScript.CurrentLevelSO.currentLevel < questionData.questionES.Length ||
            GameSystemScript.CurrentLevelSO.currentLevel < questionData.questionEN.Length)
        {
            questionLevel = GameSystemScript.CurrentLevelSO.currentLevel;
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
        GameSystemScript.CurrentLevelSO.colors[0] = colors[0];
        GameSystemScript.CurrentLevelSO.colors[1] = colors[1];
        GameSystemScript.CurrentLevelSO.colors[2] = colors[2];
        GameSystemScript.CurrentLevelSO.colors[3] = colors[3];
        GameSystemScript.CurrentLevelSO.colorsCount = 0;
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
