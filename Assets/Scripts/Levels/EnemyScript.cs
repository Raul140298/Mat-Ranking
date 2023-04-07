using PixelCrushers.DialogueSystem;
using System.Collections;
using System.Collections.Generic;
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

    private EnemySO enemyData;
    private QuestionSO questionData;

    private void Update()
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
        LevelScript.Instance.Bullets.StartBullets = true;
        LevelScript.Instance.Bullets.Init(this.gameObject, this.hp);
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

    public void Winner()
    {
        //Have to be changed to only disappear the points, but the body stay it.
        StartCoroutine(CRTRestart());
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

    IEnumerator CRTRestart()
    {
        LevelScript.Instance.VirtualCamera2.ShakeCamera(1.5f, 0.2f);

        //Shoot 1 bullet
        LevelScript.Instance.Bullets.StartBullets = true;
        LevelScript.Instance.Bullets.Init(this.gameObject, 1);

        yield return new WaitForSeconds(1f);

        isAttacking = true;
    }

    IEnumerator CRTHitEnemy()
    {
        LevelScript.Instance.VirtualCamera2.ShakeCamera(0f, 0f);
        //Time for player animation
        yield return new WaitForSeconds(0.5f);

        LevelScript.Instance.Player.GetComponent<Animator>().SetTrigger("attacked");

        yield return new WaitForSeconds(1 / 3f);

        LevelScript.Instance.Laser.Init(this.transform.position, colors[0]);

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

    private void SetQuestion()
    {
        startQuestion = true;

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
            question;
    }

    private void SetColors()
    {
        //Set Colors
        GameSystemScript.CurrentLevelSO.colors[0] = colors[0];
        GameSystemScript.CurrentLevelSO.colors[1] = colors[1];
        GameSystemScript.CurrentLevelSO.colors[2] = colors[2];
        GameSystemScript.CurrentLevelSO.colors[3] = colors[3];
        GameSystemScript.CurrentLevelSO.colorsCount = 0;
    }

    public void SetVariables()
    {
        SetQuestion();

        SetColors();

        //Initialize variables
        int xn, xd, yn, yd, zn, zd, aux, u, uE, min, max, numDec;
        int[] validChoices;
        double xnF, ynF, znF;
        string ca, wa1, wa2, wa3, q0, u0, u1;
        xn = xd = yn = yd = zn = zd = 1;
        xnF = ynF = 1;
        ca = wa1 = wa2 = wa3 = q0 = u0 = u1 = "";
        u = uE = 0;
        numDec = 2;

        //Configurations
        //COMPETENCE 1 =======================================================================
        switch (GameSystemScript.CurrentLevelSO.currentZone)
        {
            //COMPETENCE 1 =======================================================================
            case 0:
                //L2.2
                numDec = GameSystemScript.RemoteSO.dgbl_features.ilos[0].ilos[1].ilos[1].ilo_parameters[3].default_value;

                //L5
                if (GameSystemScript.RemoteSO.dgbl_features.ilos[0].ilos[4].selected == true)
                {
                    u = Random.Range(0, 2);
                    uE = Random.Range(0, 2);
                    if (u == 0)
                    {
                        u0 = "kg";
                        u1 = "g";
                    }
                    else
                    {
                        u0 = "m";
                        u1 = "cm";
                    }
                }
                else
                {
                    u0 = "";
                    u1 = "";
                }

                break;

            //COMPETENCE 2 =======================================================================
            case 1:
                break;

            //COMPETENCE 3 =======================================================================
            case 2:
                break;

            //COMPETENCE 4 =======================================================================
            case 3:
                break;

            default:
                break;
        }

        //Compendium of all the possible conversations that an enemy can have.
        switch (questionData.name)
        {
            //COMPETENCE 1 =======================================================================
            //L1----------------------------------------------------------------------------------
            case "Naturales Suma":
                //Configurations
                min = GameSystemScript.RemoteSO.dgbl_features.ilos[0].ilos[0].ilo_parameters[1].default_value;
                max = GameSystemScript.RemoteSO.dgbl_features.ilos[0].ilos[0].ilo_parameters[2].default_value;

                if (uE == 0) u1 = u0; //Same units

                xn = Random.Range(min, max);//kg or m
                if (uE == 0) yn = Random.Range(min, max);
                else
                {
                    xnF = xn / 100f;
                    ynF = yn;
                }

                zn = xn + yn;

                ca = zn.ToString() + u1;
                break;

            case "Naturales Resta":
                min = GameSystemScript.RemoteSO.dgbl_features.ilos[0].ilos[0].ilo_parameters[1].default_value;
                max = GameSystemScript.RemoteSO.dgbl_features.ilos[0].ilos[0].ilo_parameters[2].default_value;

                xn = Random.Range(min, max);
                //This way, yn will never be xn
                validChoices = new int[] { Random.Range(min, xn), Random.Range(xn + 1, max) };
                yn = validChoices[Random.Range(0, 1)];

                zn = xn - yn;

                ca = zn.ToString();
                break;

            case "Naturales Multiplicacion":
                min = GameSystemScript.RemoteSO.dgbl_features.ilos[0].ilos[0].ilo_parameters[1].default_value;
                max = GameSystemScript.RemoteSO.dgbl_features.ilos[0].ilos[0].ilo_parameters[2].default_value;

                xn = Random.Range(min, max);
                yn = Random.Range(1, min);

                zn = xn * yn;

                ca = zn.ToString();
                break;

            case "Naturales Division":
                min = GameSystemScript.RemoteSO.dgbl_features.ilos[0].ilos[0].ilo_parameters[1].default_value;
                max = GameSystemScript.RemoteSO.dgbl_features.ilos[0].ilos[0].ilo_parameters[2].default_value;

                xn = Random.Range(min, max);
                yn = Random.Range(1, min);

                znF = (double)xn / (double)yn;

                ca = System.Math.Round(znF, 3).ToString().Replace(",", ".");
                break;

            case "Naturales Potencia":
                min = GameSystemScript.RemoteSO.dgbl_features.ilos[0].ilos[0].ilo_parameters[3].default_value;
                max = GameSystemScript.RemoteSO.dgbl_features.ilos[0].ilos[0].ilo_parameters[4].default_value;

                xn = Random.Range(min, max);
                yn = Random.Range(2, 4);// ^2 or ^3

                zn = (int)Mathf.Pow(xn, yn);

                ca = zn.ToString();
                break;

            //L2----------------------------------------------------------------------------------
            case "Fracciones Suma":
                min = GameSystemScript.RemoteSO.dgbl_features.ilos[0].ilos[1].ilos[0].ilo_parameters[1].default_value;
                max = GameSystemScript.RemoteSO.dgbl_features.ilos[0].ilos[1].ilos[0].ilo_parameters[2].default_value;

                xn = Random.Range(min, max);
                xd = Random.Range(min, max);
                yn = Random.Range(min, max);
                if (GameSystemScript.RemoteSO.dgbl_features.ilos[0].ilos[1].ilos[0].ilo_parameters[3].is_active)
                {
                    yd = xd;
                }
                else
                {
                    yd = Random.Range(min, max);
                }

                zd = MathHelper.LeastCommonMultiple(xd, yd);
                zn = xn * (zd / xd) + yn * (zd / yd);

                ca = MathHelper.SimplifyFractions(zn, zd);
                break;

            case "Fracciones Resta":
                min = GameSystemScript.RemoteSO.dgbl_features.ilos[0].ilos[1].ilos[0].ilo_parameters[1].default_value;
                max = GameSystemScript.RemoteSO.dgbl_features.ilos[0].ilos[1].ilos[0].ilo_parameters[2].default_value;

                xn = Random.Range(min, max);
                xd = Random.Range(min, max);
                yn = Random.Range(min, max);
                if (GameSystemScript.RemoteSO.dgbl_features.ilos[0].ilos[1].ilos[0].ilo_parameters[3].is_active)
                {
                    yd = xd;
                }
                else
                {
                    yd = Random.Range(min, max);
                }

                zd = MathHelper.LeastCommonMultiple(xd, yd);
                zn = xn * (zd / xd) - yn * (zd / yd);

                ca = MathHelper.SimplifyFractions(zn, zd);
                break;

            case "Fracciones Multiplicacion":
                min = GameSystemScript.RemoteSO.dgbl_features.ilos[0].ilos[1].ilos[0].ilo_parameters[1].default_value;
                max = GameSystemScript.RemoteSO.dgbl_features.ilos[0].ilos[1].ilos[0].ilo_parameters[2].default_value;

                xn = Random.Range(min, max);
                xd = Random.Range(min, max);
                yn = Random.Range(min, max);
                if (GameSystemScript.RemoteSO.dgbl_features.ilos[0].ilos[1].ilos[0].ilo_parameters[3].is_active)
                {
                    yd = xd;
                }
                else
                {
                    yd = Random.Range(min, max);
                }

                zd = xd * yd;
                zn = xn * yn;

                ca = MathHelper.SimplifyFractions(zn, zd);
                break;

            case "Fracciones Division":
                min = GameSystemScript.RemoteSO.dgbl_features.ilos[0].ilos[1].ilos[0].ilo_parameters[1].default_value;
                max = GameSystemScript.RemoteSO.dgbl_features.ilos[0].ilos[1].ilos[0].ilo_parameters[2].default_value;

                xn = Random.Range(min, max);
                xd = Random.Range(min, max);
                yn = Random.Range(min, max);
                if (GameSystemScript.RemoteSO.dgbl_features.ilos[0].ilos[1].ilos[0].ilo_parameters[3].is_active)
                {
                    yd = xd;
                }
                else
                {
                    yd = Random.Range(min, max);
                }

                zd = xd / yd;
                zn = xn / yn;

                ca = MathHelper.SimplifyFractions(zn, zd);
                break;

            case "Decimales Suma":
                min = GameSystemScript.RemoteSO.dgbl_features.ilos[0].ilos[1].ilos[1].ilo_parameters[1].default_value;
                max = GameSystemScript.RemoteSO.dgbl_features.ilos[0].ilos[1].ilos[1].ilo_parameters[2].default_value;
                if (uE == 0) u0 = u1; //Same units
                else u1 = u0;

                xn = Random.Range(min, max);//kg or m
                yn = Random.Range(min, max);
                xnF = xn + Random.Range(0, (10 ^ numDec)) / (float)(10 ^ numDec);
                ynF = yn + Random.Range(0, (10 ^ numDec)) / (float)(10 ^ numDec);

                znF = xnF + ynF;
                xnF = System.Math.Round((xnF), numDec);
                ynF = System.Math.Round((ynF), numDec);

                ca = System.Math.Round(znF, numDec).ToString().Replace(",", ".") + u0;
                break;

            case "Decimales Resta":
                min = GameSystemScript.RemoteSO.dgbl_features.ilos[0].ilos[1].ilos[1].ilo_parameters[1].default_value;
                max = GameSystemScript.RemoteSO.dgbl_features.ilos[0].ilos[1].ilos[1].ilo_parameters[2].default_value;

                xn = Random.Range(min, max);//kg or m
                yn = Random.Range(min, max);
                xnF = xn + Random.Range(0, (10 ^ numDec)) / (float)(10 ^ numDec);
                ynF = yn + Random.Range(0, (10 ^ numDec)) / (float)(10 ^ numDec);

                if ((xnF - ynF) < 0)
                {
                    double auxF = xnF;
                    xnF = ynF;
                    ynF = auxF;
                }

                znF = xnF - ynF;
                xnF = System.Math.Round((xnF), numDec);
                ynF = System.Math.Round((ynF), numDec);

                ca = System.Math.Round(znF, numDec).ToString().Replace(",", ".") + u0;
                break;

            case "Decimales Multiplicacion":
                min = GameSystemScript.RemoteSO.dgbl_features.ilos[0].ilos[1].ilos[1].ilo_parameters[1].default_value;
                max = GameSystemScript.RemoteSO.dgbl_features.ilos[0].ilos[1].ilos[1].ilo_parameters[2].default_value;

                xn = Random.Range(min, max);//kg or m
                yn = Random.Range(2, 11);
                xnF = xn + Random.Range(0, (10 ^ numDec)) / (float)(10 ^ numDec);
                ynF = yn;

                znF = xnF * ynF;
                xnF = System.Math.Round((xnF), numDec);
                ynF = System.Math.Round((ynF), numDec);

                ca = System.Math.Round(znF, numDec).ToString().Replace(",", ".") + u0;
                break;

            case "Decimales Division":
                min = GameSystemScript.RemoteSO.dgbl_features.ilos[0].ilos[1].ilos[1].ilo_parameters[1].default_value;
                max = GameSystemScript.RemoteSO.dgbl_features.ilos[0].ilos[1].ilos[1].ilo_parameters[2].default_value;

                xn = Random.Range(min, max);//kg or m
                yn = Random.Range(2, 11);
                xnF = xn + Random.Range(0, (10 ^ numDec)) / (float)(10 ^ numDec);
                ynF = yn;

                znF = xnF / ynF;
                xnF = System.Math.Round((xnF), numDec);
                ynF = System.Math.Round((ynF), numDec);

                ca = System.Math.Round(znF, numDec).ToString().Replace(",", ".") + u0;
                break;

            //COMPETENCE 2 =======================================================================
            //L8----------------------------------------------------------------------------------
            case "Ecuaciones Simples 1":
                min = GameSystemScript.RemoteSO.dgbl_features.ilos[1].ilos[0].ilo_parameters[1].default_value;
                max = GameSystemScript.RemoteSO.dgbl_features.ilos[1].ilos[0].ilo_parameters[2].default_value;

                // xn * x + yn = 0 / xn * x + yn = xd * x + yd
                // xd != xn
                xn = Random.Range(min, max);
                validChoices = new int[] { Random.Range(-min, 0), Random.Range(1, xn), Random.Range(xn, min) };
                xd = validChoices[Random.Range(0, 2)];

                yd = Random.Range(yn + 1, max);

                zn = (yd - yn);
                zd = (xn - xd);

                ca = MathHelper.SimplifyFractions(zn, zd);
                break;

            case "Ecuaciones Simples 2":
                min = GameSystemScript.RemoteSO.dgbl_features.ilos[1].ilos[0].ilo_parameters[1].default_value;
                max = GameSystemScript.RemoteSO.dgbl_features.ilos[1].ilos[0].ilo_parameters[2].default_value;

                // xn * x + yn = 0 / xn * x + yn = xd * x + yd
                // xd != xn
                xn = Random.Range(min, max);
                validChoices = new int[] { Random.Range(-min, 0), Random.Range(1, xn), Random.Range(xn, min) };
                xd = validChoices[Random.Range(0, 2)];

                yd = Random.Range(yn + 1, max);

                zn = (-yd - yn);
                zd = (xn - xd);

                ca = MathHelper.SimplifyFractions(zn, zd);
                break;

            //L9----------------------------------------------------------------------------------
            case "Sucesiones":
                min = GameSystemScript.RemoteSO.dgbl_features.ilos[1].ilos[1].ilo_parameters[1].default_value;
                max = GameSystemScript.RemoteSO.dgbl_features.ilos[1].ilos[1].ilo_parameters[2].default_value;

                xn = Random.Range(1, min);
                aux = Random.Range(min, max);

                if (aux % 2 == 0)
                {
                    yn = 2 * xn + aux;
                    xd = 3 * xn + aux;
                    yd = 4 * xn + aux;
                    zn = 5 * xn + aux;
                    xn = 1 * xn + aux;
                }
                else
                {
                    yn = xn + aux;
                    xd = yn + xn;
                    yd = xd + yn;
                    zn = yd + xd;
                }

                ca = zn.ToString();
                break;

            //COMPETENCE 3 =======================================================================
            //L13---------------------------------------------------------------------------------
            case "Area Triangulo":
                min = GameSystemScript.RemoteSO.dgbl_features.ilos[2].ilos[0].ilos[1].ilo_parameters[1].default_value;
                max = GameSystemScript.RemoteSO.dgbl_features.ilos[2].ilos[0].ilos[1].ilo_parameters[2].default_value;

                xn = Random.Range(min, max);
                yn = Random.Range(min, max);

                znF = (double)xn * (double)yn / 2f;

                ca = System.Math.Round(znF, 3).ToString().Replace(",", ".");
                break;

            case "Perimetro Triangulo":
                min = GameSystemScript.RemoteSO.dgbl_features.ilos[2].ilos[0].ilos[0].ilo_parameters[1].default_value;
                max = GameSystemScript.RemoteSO.dgbl_features.ilos[2].ilos[0].ilos[0].ilo_parameters[2].default_value;

                xn = Random.Range(min, max);
                yn = Random.Range(min, max);
                xd = Random.Range(min, max);

                zn = xn + yn + xd;

                ca = zn.ToString();
                break;

            case "Area Rectangulo":
                min = GameSystemScript.RemoteSO.dgbl_features.ilos[2].ilos[0].ilos[1].ilo_parameters[1].default_value;
                max = GameSystemScript.RemoteSO.dgbl_features.ilos[2].ilos[0].ilos[1].ilo_parameters[2].default_value;

                xn = Random.Range(min, max);
                yn = Random.Range(min, max);

                zn = xn * yn;

                ca = zn.ToString();
                break;

            case "Perimetro Rectangulo":
                min = GameSystemScript.RemoteSO.dgbl_features.ilos[2].ilos[0].ilos[0].ilo_parameters[1].default_value;
                max = GameSystemScript.RemoteSO.dgbl_features.ilos[2].ilos[0].ilos[0].ilo_parameters[2].default_value;

                xn = Random.Range(min, max);
                yn = Random.Range(min, max);

                zn = 2 * (xn + yn);

                ca = zn.ToString();
                break;

            case "Volumen Paralelepipedo":
                min = GameSystemScript.RemoteSO.dgbl_features.ilos[2].ilos[0].ilos[2].ilo_parameters[1].default_value;
                max = GameSystemScript.RemoteSO.dgbl_features.ilos[2].ilos[0].ilos[2].ilo_parameters[2].default_value;

                xn = Random.Range(min, max);
                yn = Random.Range(min, max);
                xd = Random.Range(min, max);

                zn = xn * yn * xd;

                ca = zn.ToString();
                break;

            case "Planos":
                min = GameSystemScript.RemoteSO.dgbl_features.ilos[0].ilos[0].ilo_parameters[1].default_value;
                max = GameSystemScript.RemoteSO.dgbl_features.ilos[0].ilos[0].ilo_parameters[2].default_value;

                xn = Random.Range(min, max);
                yn = Random.Range(1, min);

                znF = (double)xn / (double)yn;

                ca = System.Math.Round(znF, 3).ToString().Replace(",", ".");
                break;

            //COMPETENCE 4 =======================================================================
            //L21---------------------------------------------------------------------------------
            case "Media Aritmetica":
                min = GameSystemScript.RemoteSO.dgbl_features.ilos[3].ilos[3].ilos[0].ilo_parameters[1].default_value;
                max = GameSystemScript.RemoteSO.dgbl_features.ilos[3].ilos[3].ilos[0].ilo_parameters[2].default_value;

                xn = Random.Range(min, max);
                yn = Random.Range(min, max);
                xd = Random.Range(min, max);
                yd = Random.Range(min, max);

                znF = (double)(xn + yn + xd + yd) / 4;
                zn = (znF < 0 ? 2 : (int)znF);

                ca = System.Math.Round(znF, 3).ToString().Replace(",", ".");
                break;

            case "Moda":
                xn = Random.Range(1, 10);
                yn = Random.Range(10, 20);
                xd = Random.Range(20, 30);
                yd = Random.Range(30, 40);

                int[] vals = { xn, yn, xd, yd };
                int temp;

                //Shuffle frecuency
                for (int i = 0; i < vals.Length; i++)
                {
                    int rnd = Random.Range(0, vals.Length);
                    temp = vals[rnd];
                    vals[rnd] = vals[i];
                    vals[i] = temp;
                }

                xn = Random.Range(6, 10);
                yn = Random.Range(3, 5);
                xd = Random.Range(5, 6);
                yd = Random.Range(1, 3);

                int[] frecuency = { xn, yn, xd, yd };

                List<int> pob = new List<int> { };

                for (int i = 0; i < 4; i++)
                {
                    for (int j = 0; j < frecuency[i]; j++)
                    {
                        pob.Add(vals[i]);
                    }
                }

                ShuffleListScript.Shuffle(pob);
                for (int i = 0; i < pob.Count; i++)
                {
                    if (i == 0) q0 += pob[i].ToString();
                    else q0 += ", " + pob[i].ToString();
                }

                ca = vals[0].ToString();
                wa1 = vals[1].ToString();
                wa2 = vals[2].ToString();
                wa3 = vals[3].ToString();
                break;

            case "Probabilidad":
                min = GameSystemScript.RemoteSO.dgbl_features.ilos[3].ilos[1].ilo_parameters[1].default_value;
                max = GameSystemScript.RemoteSO.dgbl_features.ilos[3].ilos[1].ilo_parameters[2].default_value;

                xn = Random.Range(min, max);
                yn = Random.Range(min, max);

                znF = (double)xn / (double)yn;

                ca = System.Math.Round(znF, 3).ToString().Replace(",", ".");
                break;

            default:
                Debug.Log("No se pudo asignar variables a la conversación " + questionData.name);
                break;
        }

        //Set variables
        if (questionData.name.StartsWith("Decimales") ||
            (questionData.name.Equals("Naturales Suma") && uE == 1))
        {
            DialogueLua.SetVariable("Xn", xnF); //Set numerator
            DialogueLua.SetVariable("Yn", ynF); //Set numerator
        }
        else
        {
            DialogueLua.SetVariable("Xn", xn); //Set numerator
            DialogueLua.SetVariable("Yn", yn); //Set numerator
        }

        if (!questionData.name.Equals("Moda"))
        {
            //MathHelpers.GenerateWrongAnswers(ca, out wa1, out wa2, out wa3);
        }

        DialogueLua.SetVariable("Xd", xd); //Set denominator
        DialogueLua.SetVariable("Yd", yd); //Set denominator

        //Set question string auxiliar
        DialogueLua.SetVariable("Q0", q0);

        //Set some units in questions
        DialogueLua.SetVariable("U0", u0);
        DialogueLua.SetVariable("U1", u1);

        //Set the correct answer
        DialogueLua.SetVariable("Ca", ca);

        //Set the wrong answers from Zn and Zd values
        DialogueLua.SetVariable("Wa1", wa1);
        DialogueLua.SetVariable("Wa2", wa2);
        DialogueLua.SetVariable("Wa3", wa3);

        //Finally, each conversation will determine whether to display numerators or denominators.
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
