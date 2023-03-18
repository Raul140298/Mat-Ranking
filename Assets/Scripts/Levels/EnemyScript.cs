using PixelCrushers.DialogueSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EnemyScript : MonoBehaviour
{
    public GameSystemScript gameSystem;
    public EnemySO enemyData = null;
    public int knowledgePoints;
    public string question;
    public LevelScript level;
    public DialogueSystemTrigger dialogueSystemTrigger;
    public bool startQuestion = false;
    public ParticleSystem pointsParticles;
    public ParticleSystem[] keysParticles;
    public Color[] colors;
    public AudioSource enemyAudioSource;
    public OptionsSO optionsSO;
    public bool isMoving = false, isAttacking = false;
    public Rigidbody2D rbody;
    public SpriteRenderer sprite;
    public Animator animator;
    public CircleCollider2D coll2D;
    public CircleCollider2D blockColl2D;
    public int hp;
    public Vector2 roomEdgesPosition, roomEdgesSize;
    public Vector2 roomEdgesEnd;
    public GameObject characterCollisionBlocker;

    private void Start()
    {
        gameSystem = GameObject.FindGameObjectWithTag("GameSystem").GetComponent<GameSystemScript>();
        level = GameObject.FindGameObjectWithTag("LevelScript").GetComponent<LevelScript>();
        hp = 3;

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
        enemyAudioSource.volume = gameSystem.OptionsSO.soundsVolume;
        SoundsScript.Slider.onValueChanged.AddListener(val => changeVolume(val));

        if (enemyData.mobId != 0)
        {
            StartCoroutine(CRTMakeSounds());
            Physics2D.IgnoreCollision(this.GetComponent<CircleCollider2D>(),
                gameSystem.Player.transform.GetChild(0).GetComponent<CircleCollider2D>());
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
    }

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
        gameSystem.Bullets.StartBullets = true;
        gameSystem.Bullets.Init(this.gameObject, this.hp);
    }

    IEnumerator CRTMakeSounds()
    {
        float aux = (float)Random.Range(30, 100) / 10f;
        yield return new WaitForSeconds(aux);
        playNeutralSound();

        if (this.GetComponent<SpriteRenderer>().enabled) StartCoroutine(CRTMakeSounds());
    }

    public void playNeutralSound()
    {
        SoundsScript.PlayEnemySound("MOB" + enemyData.mobId.ToString(), enemyAudioSource);//1 have to be changed by distance from the player
    }

    public void changeVolume(float value)
    {
        enemyAudioSource.volume = value;
    }

    public void defeated()
    {
        //After some time and animation
        StartCoroutine(CRTHitEnemy());
    }

    public void winner()
    {
        //Have to be changed to only disappear the points, but the body stay it.
        StartCoroutine(CRTRestart());
    }

    public void hitPlayer(GameObject bullet)
    {
        StartCoroutine(CRTHitPlayer(bullet));
    }

    IEnumerator CRTHitPlayer(GameObject bullet)
    {
        if (gameSystem.CurrentLevelSO.playerLives > 0)
        {
            gameSystem.VirtualCamera2.ShakeCamera(2f, 0.2f);

            //Hit player
            gameSystem.Player.GetComponent<Animator>().SetTrigger("wasHit");

            gameSystem.Player.GetComponent<Rigidbody2D>().AddForce(500f * (gameSystem.Player.transform.position - bullet.transform.position).normalized);

            gameSystem.ChangeKnowledgePoints(-knowledgePoints);

            gameSystem.CurrentLevelSO.playerLives -= 1;

            gameSystem.Player.setLives();
        }

        yield return new WaitForSeconds(0f);

        if (gameSystem.CurrentLevelSO.playerLives == 0)
        {
            gameSystem.Player.BattleSoundtrack.EndBattleSoundtrack();

            gameSystem.Joystick.SetActive(false);

            this.transform.GetChild(0).gameObject.SetActive(false);

            //this.GetComponent<CircleCollider2D>().enabled = false;
            rbody.constraints = RigidbodyConstraints2D.FreezeAll;

            yield return new WaitForSeconds(0.5f);

            gameSystem.Player.GetComponent<Animator>().SetTrigger("isDead");
        }
    }

    IEnumerator CRTRestart()
    {
        gameSystem.VirtualCamera2.ShakeCamera(1.5f, 0.2f);

        //Shoot 1 bullet
        gameSystem.Bullets.StartBullets = true;
        gameSystem.Bullets.Init(this.gameObject, 1);

        yield return new WaitForSeconds(1f);

        isAttacking = true;
    }

    IEnumerator CRTHitEnemy()
    {
        gameSystem.VirtualCamera2.ShakeCamera(0f, 0f);
        //Time for player animation
        yield return new WaitForSeconds(0.5f);

        gameSystem.Player.GetComponent<Animator>().SetTrigger("attacked");

        yield return new WaitForSeconds(1 / 3f);

        gameSystem.Laser.Init(this.transform.position, colors[0]);

        yield return new WaitForSeconds(1 / 12f);

        gameSystem.VirtualCamera2.ShakeCamera(1.5f, 0.2f);

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
        gameSystem.VirtualCamera2.ShakeCamera(0f, 0f);
        gameSystem.Player.BattleSoundtrack.EndBattleSoundtrack();
        gameSystem.DialogueCamera.EndDialogue();

        yield return new WaitForSeconds(0.5f);

        //Deactivate dialogue
        characterCollisionBlocker.SetActive(false);
        this.transform.GetChild(0).gameObject.SetActive(false);
        gameSystem.RoomEdgesCollider.GetComponent<TilemapRenderer>().enabled = false;
        //gameSystem.roomEdges.SetActive(false);
        gameSystem.RoomEdgesCollider.enabled = false;
        coll2D.enabled = false;
        gameSystem.Player.CurrentEnemy = null;
        gameSystem.Player.CurrentEnemyScript = null;

        pointsParticles.Play();

        yield return new WaitForSeconds(0.01f);
        sprite.enabled = false;

        if (gameSystem.CurrentLevelSO.playerKeyParts < 3)
        {
            keysParticles[gameSystem.CurrentLevelSO.playerKeyParts].Play();
            yield return new WaitForSeconds(0.5f);

            gameSystem.CurrentLevelSO.playerKeyParts += 1;

            SoundsScript.PlaySound("KEY UNLOCKING");

            gameSystem.Player.setKeys();
        }

        gameSystem.ChangeKnowledgePoints(knowledgePoints);
    }

    public void initEnemyData()
    {
        knowledgePoints = enemyData.knowledgePoints;

        var main = pointsParticles.main;
        main.maxParticles = knowledgePoints;

        //Get a random question of the enemyData questions database
        int auxQUestion = Random.Range(0, enemyData.conversationTitle.Length);
        question = enemyData.conversationTitle[auxQUestion];

        //Asign the question to his dialogue
        dialogueSystemTrigger = this.transform.GetChild(0).GetComponent<DialogueSystemTrigger>();
        dialogueSystemTrigger.conversation = question;
    }

    public void setVariables()
    {
        startQuestion = true;

        //Set Colors
        gameSystem.CurrentLevelSO.colors[0] = colors[0];
        gameSystem.CurrentLevelSO.colors[1] = colors[1];
        gameSystem.CurrentLevelSO.colors[2] = colors[2];
        gameSystem.CurrentLevelSO.colors[3] = colors[3];
        gameSystem.CurrentLevelSO.colorsCount = 0;

        //Initialize variables
        int xn, xd, yn, yd, zn, zd, aux, u, uE, min, max, numDec;
        int[] validChoices;
        double xnF, ynF, znF;
        string wa0, wa1, wa2, wa3, q0, u0, u1;
        xn = 1;
        xd = 1;
        yn = 1;
        yd = 1;
        zn = 1;
        zd = 1;
        xnF = 1f;
        ynF = 1f;
        wa0 = "";
        wa1 = "";
        wa2 = "";
        wa3 = "";
        q0 = "";
        u0 = "";
        u1 = "";
        uE = 0;
        numDec = 3;

        //Configurations
        //COMPETENCE 1 =======================================================================
        switch (gameSystem.CurrentLevelSO.currentZone)
        {
            //COMPETENCE 1 =======================================================================
            case 0:
                //L2.2
                numDec = gameSystem.RemoteSO.dgbl_features.ilos[0].ilos[1].ilos[1].ilo_parameters[3].default_value;

                //L5
                if (gameSystem.RemoteSO.dgbl_features.ilos[0].ilos[4].selected == true)
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
        switch (dialogueSystemTrigger.conversation)
        {
            case "Pregunta Propuesta":
                //q0  = gameSystem.RemoteSO.dgbl_features.ilos[gameSystem.CurrentLevelSO.currentZone].ilos[0].ilo_parameters[3].question; ;
                //wa0 = gameSystem.RemoteSO.dgbl_features.ilos[gameSystem.CurrentLevelSO.currentZone].ilos[0].ilo_parameters[3].correctAnswer; ;
                //wa1 = gameSystem.RemoteSO.dgbl_features.ilos[gameSystem.CurrentLevelSO.currentZone].ilos[0].ilo_parameters[3].wrongAnswer1; ;
                //wa2 = gameSystem.RemoteSO.dgbl_features.ilos[gameSystem.CurrentLevelSO.currentZone].ilos[0].ilo_parameters[3].wrongAnswer2; ;
                //wa3 = gameSystem.RemoteSO.dgbl_features.ilos[gameSystem.CurrentLevelSO.currentZone].ilos[0].ilo_parameters[3].wrongAnswer3; ;
                break;

            //COMPETENCE 1 =======================================================================
            //L1----------------------------------------------------------------------------------
            case "Naturales Suma":
                //Configurations
                min = gameSystem.RemoteSO.dgbl_features.ilos[0].ilos[0].ilo_parameters[1].default_value;
                max = gameSystem.RemoteSO.dgbl_features.ilos[0].ilos[0].ilo_parameters[2].default_value;
                if (uE == 0) u1 = u0; //Same units

                xn = Random.Range(min, max);//kg or m
                if (uE == 0) yn = Random.Range(min, max);
                else
                {
                    xnF = xn / 100f;
                    ynF = yn;
                }

                zn = xn + yn;

                wa0 = zn.ToString() + u1;

                wa1 = (zn + Random.Range(1, zn / 2 + 1)).ToString() + u1;
                wa2 = (zn * (u1 != "" ? 100 : 1) + Random.Range(zn / 2, zn + 1)).ToString() + u0;
                wa3 = (zn * (u1 != "" ? 100 : 1) - Random.Range(1, zn)).ToString() + u0;
                break;

            case "Naturales Resta":
                min = gameSystem.RemoteSO.dgbl_features.ilos[0].ilos[0].ilo_parameters[1].default_value;
                max = gameSystem.RemoteSO.dgbl_features.ilos[0].ilos[0].ilo_parameters[2].default_value;

                xn = Random.Range(min, max);
                //This way, yn will never be xn
                validChoices = new int[] { Random.Range(min, xn), Random.Range(xn + 1, max) };
                yn = validChoices[Random.Range(0, 1)];

                zn = xn - yn;

                wa0 = zn.ToString();
                if (zn < 0)
                {
                    wa1 = "-" + (-zn + Random.Range(1, -zn / 2 + 1)).ToString();
                    wa2 = "-" + (-zn + Random.Range(-zn / 2, -zn + 1)).ToString();
                    wa3 = "-" + (-zn - Random.Range(1, -zn)).ToString();
                }
                else
                {
                    wa1 = (zn + Random.Range(1, zn / 2 + 1)).ToString();
                    wa2 = (zn + Random.Range(zn / 2, zn + 1)).ToString();
                    wa3 = (zn - Random.Range(1, zn)).ToString();
                }
                break;

            case "Naturales Multiplicacion":
                min = gameSystem.RemoteSO.dgbl_features.ilos[0].ilos[0].ilo_parameters[1].default_value;
                max = gameSystem.RemoteSO.dgbl_features.ilos[0].ilos[0].ilo_parameters[2].default_value;

                xn = Random.Range(min, max);
                yn = Random.Range(1, min);

                zn = xn * yn;

                wa0 = zn.ToString();
                wa1 = (zn * Random.Range(2, 5)).ToString();
                wa2 = (zn * Random.Range(5, 8)).ToString();
                wa3 = (zn * Random.Range(8, 11)).ToString();
                break;

            case "Naturales Division":
                min = gameSystem.RemoteSO.dgbl_features.ilos[0].ilos[0].ilo_parameters[1].default_value;
                max = gameSystem.RemoteSO.dgbl_features.ilos[0].ilos[0].ilo_parameters[2].default_value;

                xn = Random.Range(min, max);
                yn = Random.Range(1, min);

                znF = (double)xn / (double)yn;

                wa0 = System.Math.Round(znF, 3).ToString().Replace(",", ".");
                wa1 = System.Math.Round((znF / Random.Range(2, 5)), 3).ToString().Replace(",", ".");
                wa2 = System.Math.Round((znF / Random.Range(5, 8)), 3).ToString().Replace(",", ".");
                wa3 = System.Math.Round((znF / Random.Range(8, 11)), 3).ToString().Replace(",", ".");
                break;

            case "Naturales Potencia":
                min = gameSystem.RemoteSO.dgbl_features.ilos[0].ilos[0].ilo_parameters[3].default_value;
                max = gameSystem.RemoteSO.dgbl_features.ilos[0].ilos[0].ilo_parameters[4].default_value;

                xn = Random.Range(min, max);
                yn = Random.Range(2, 4);// ^2 or ^3

                zn = (int)Mathf.Pow(xn, yn);

                wa0 = zn.ToString();
                wa1 = (zn + Random.Range(1, zn / 2 + 1)).ToString();
                wa2 = (zn + Random.Range(zn / 2, zn + 1)).ToString();
                wa3 = (zn - Random.Range(1, zn)).ToString();
                break;

            //L2----------------------------------------------------------------------------------
            case "Fracciones Suma":
                min = gameSystem.RemoteSO.dgbl_features.ilos[0].ilos[1].ilos[0].ilo_parameters[1].default_value;
                max = gameSystem.RemoteSO.dgbl_features.ilos[0].ilos[1].ilos[0].ilo_parameters[2].default_value;

                xn = Random.Range(min, max);
                xd = Random.Range(min, max);
                yn = Random.Range(min, max);
                if (gameSystem.RemoteSO.dgbl_features.ilos[0].ilos[1].ilos[0].ilo_parameters[3].is_active)
                {
                    yd = xd;
                }
                else
                {
                    yd = Random.Range(min, max);
                }

                zd = leastCommonMultiple(xd, yd);
                zn = xn * (zd / xd) + yn * (zd / yd);

                wa0 = simplifyFractions(zn, zd);
                wa1 = simplifyFractions(zn + Random.Range(1, zn + 1), zd);
                wa2 = simplifyFractions(zn - Random.Range(1, zn), zd);
                wa3 = simplifyFractions(zd + Random.Range(1, zd), zn);
                break;

            case "Fracciones Resta":
                min = gameSystem.RemoteSO.dgbl_features.ilos[0].ilos[1].ilos[0].ilo_parameters[1].default_value;
                max = gameSystem.RemoteSO.dgbl_features.ilos[0].ilos[1].ilos[0].ilo_parameters[2].default_value;

                xn = Random.Range(min, max);
                xd = Random.Range(min, max);
                yn = Random.Range(min, max);
                if (gameSystem.RemoteSO.dgbl_features.ilos[0].ilos[1].ilos[0].ilo_parameters[3].is_active)
                {
                    yd = xd;
                }
                else
                {
                    yd = Random.Range(min, max);
                }

                zd = leastCommonMultiple(xd, yd);
                zn = xn * (zd / xd) - yn * (zd / yd);

                wa0 = simplifyFractions(zn, zd);
                if (zn < 0)
                {
                    wa1 = "-" + simplifyFractions(-zn + Random.Range(1, -zn + 1), zd);
                    wa2 = "-" + simplifyFractions(-zn - Random.Range(1, -zn), zd);
                    wa3 = "-" + simplifyFractions(zd + Random.Range(1, zd), -zn);
                }
                else if (zn == 0)
                {
                    wa1 = simplifyFractions(zn + Random.Range(1, xn), zd);
                    wa2 = simplifyFractions(zn - Random.Range(1, xn), yn);
                    wa3 = simplifyFractions(zd + Random.Range(1, xn), xn);
                }
                else
                {
                    wa1 = simplifyFractions(zn + Random.Range(1, zn), zd);
                    wa2 = simplifyFractions(zn - Random.Range(1, zn - 1), zd);
                    wa3 = simplifyFractions(zd + Random.Range(1, zd), zn);
                }
                break;

            case "Fracciones Multiplicacion":
                min = gameSystem.RemoteSO.dgbl_features.ilos[0].ilos[1].ilos[0].ilo_parameters[1].default_value;
                max = gameSystem.RemoteSO.dgbl_features.ilos[0].ilos[1].ilos[0].ilo_parameters[2].default_value;

                xn = Random.Range(min, max);
                xd = Random.Range(min, max);
                yn = Random.Range(min, max);
                if (gameSystem.RemoteSO.dgbl_features.ilos[0].ilos[1].ilos[0].ilo_parameters[3].is_active)
                {
                    yd = xd;
                }
                else
                {
                    yd = Random.Range(min, max);
                }

                zd = xd * yd;
                zn = xn * yn;

                wa0 = simplifyFractions(zn, zd);
                wa1 = simplifyFractions(zn + Random.Range(1, zn + 1), zd);
                wa2 = simplifyFractions(zn - Random.Range(1, zn), zd);
                wa3 = simplifyFractions(zd + Random.Range(1, zd), zn);
                break;

            case "Fracciones Division":
                min = gameSystem.RemoteSO.dgbl_features.ilos[0].ilos[1].ilos[0].ilo_parameters[1].default_value;
                max = gameSystem.RemoteSO.dgbl_features.ilos[0].ilos[1].ilos[0].ilo_parameters[2].default_value;

                xn = Random.Range(min, max);
                xd = Random.Range(min, max);
                yn = Random.Range(min, max);
                if (gameSystem.RemoteSO.dgbl_features.ilos[0].ilos[1].ilos[0].ilo_parameters[3].is_active)
                {
                    yd = xd;
                }
                else
                {
                    yd = Random.Range(min, max);
                }

                zd = xd / yd;
                zn = xn / yn;

                wa0 = simplifyFractions(zn, zd);
                wa1 = simplifyFractions(zn + Random.Range(1, zn + 1), zd);
                wa2 = simplifyFractions(zn - Random.Range(1, zn), zd);
                wa3 = simplifyFractions(zd + Random.Range(1, zd), zn);
                break;

            case "Decimales Suma":
                min = gameSystem.RemoteSO.dgbl_features.ilos[0].ilos[1].ilos[1].ilo_parameters[1].default_value;
                max = gameSystem.RemoteSO.dgbl_features.ilos[0].ilos[1].ilos[1].ilo_parameters[2].default_value;
                if (uE == 0) u0 = u1; //Same units
                else u1 = u0;

                xn = Random.Range(min, max);//kg or m
                yn = Random.Range(min, max);
                xnF = xn + Random.Range(0, (10 ^ numDec)) / (float)(10 ^ numDec);
                ynF = yn + Random.Range(0, (10 ^ numDec)) / (float)(10 ^ numDec);

                znF = xnF + ynF;
                xnF = System.Math.Round((xnF), numDec);
                ynF = System.Math.Round((ynF), numDec);

                wa0 = System.Math.Round(znF, numDec).ToString().Replace(",", ".") + u0;
                wa1 = System.Math.Round(znF + 1, numDec).ToString().Replace(",", ".") + u0;
                wa2 = System.Math.Round(znF - 1, numDec).ToString().Replace(",", ".") + u0;
                wa3 = System.Math.Round(znF + 2 + Random.Range(0, (10 ^ numDec)) / (float)(10 ^ numDec), numDec).ToString().Replace(",", ".") + u0;
                break;

            case "Decimales Resta":
                min = gameSystem.RemoteSO.dgbl_features.ilos[0].ilos[1].ilos[1].ilo_parameters[1].default_value;
                max = gameSystem.RemoteSO.dgbl_features.ilos[0].ilos[1].ilos[1].ilo_parameters[2].default_value;

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

                wa0 = System.Math.Round(znF, numDec).ToString().Replace(",", ".") + u0;
                wa1 = System.Math.Round(znF + 1, numDec).ToString().Replace(",", ".") + u0;
                wa2 = System.Math.Round(znF - 1, numDec).ToString().Replace(",", ".") + u0;
                wa3 = System.Math.Round(znF + 2 + Random.Range(0, (10 ^ numDec)) / (float)(10 ^ numDec), numDec).ToString().Replace(",", ".") + u0;
                break;

            case "Decimales Multiplicacion":
                min = gameSystem.RemoteSO.dgbl_features.ilos[0].ilos[1].ilos[1].ilo_parameters[1].default_value;
                max = gameSystem.RemoteSO.dgbl_features.ilos[0].ilos[1].ilos[1].ilo_parameters[2].default_value;

                xn = Random.Range(min, max);//kg or m
                yn = Random.Range(2, 11);
                xnF = xn + Random.Range(0, (10 ^ numDec)) / (float)(10 ^ numDec);
                ynF = yn;

                znF = xnF * ynF;
                xnF = System.Math.Round((xnF), numDec);
                ynF = System.Math.Round((ynF), numDec);

                wa0 = System.Math.Round(znF, numDec).ToString().Replace(",", ".") + u0;
                wa1 = System.Math.Round(znF + 1, numDec).ToString().Replace(",", ".") + u0;
                wa2 = System.Math.Round(znF - 1, numDec).ToString().Replace(",", ".") + u0;
                wa3 = System.Math.Round(znF + 2 + Random.Range(0, (10 ^ numDec)) / (float)(10 ^ numDec), numDec).ToString().Replace(",", ".") + u0;
                break;

            case "Decimales Division":
                min = gameSystem.RemoteSO.dgbl_features.ilos[0].ilos[1].ilos[1].ilo_parameters[1].default_value;
                max = gameSystem.RemoteSO.dgbl_features.ilos[0].ilos[1].ilos[1].ilo_parameters[2].default_value;

                xn = Random.Range(min, max);//kg or m
                yn = Random.Range(2, 11);
                xnF = xn + Random.Range(0, (10 ^ numDec)) / (float)(10 ^ numDec);
                ynF = yn;

                znF = xnF / ynF;
                xnF = System.Math.Round((xnF), numDec);
                ynF = System.Math.Round((ynF), numDec);

                wa0 = System.Math.Round(znF, numDec).ToString().Replace(",", ".") + u0;
                wa1 = System.Math.Round(znF + 1, numDec).ToString().Replace(",", ".") + u0;
                wa2 = System.Math.Round(znF - 1, numDec).ToString().Replace(",", ".") + u0;
                wa3 = System.Math.Round(znF + 2 + Random.Range(0, (10 ^ numDec)) / (float)(10 ^ numDec), numDec).ToString().Replace(",", ".") + u0;
                break;

            //COMPETENCE 2 =======================================================================
            //L8----------------------------------------------------------------------------------
            case "Ecuaciones Simples 1":
                min = gameSystem.RemoteSO.dgbl_features.ilos[1].ilos[0].ilo_parameters[1].default_value;
                max = gameSystem.RemoteSO.dgbl_features.ilos[1].ilos[0].ilo_parameters[2].default_value;

                // xn * x + yn = 0 / xn * x + yn = xd * x + yd
                // xd != xn
                xn = Random.Range(min, max);
                validChoices = new int[] { Random.Range(-min, 0), Random.Range(1, xn), Random.Range(xn, min) };
                xd = validChoices[Random.Range(0, 2)];

                yd = Random.Range(yn + 1, max);

                zn = (yd - yn);
                zd = (xn - xd);

                wa0 = simplifyFractions(zn, zd);
                wa1 = simplifyFractions(zn + Random.Range(1, zn + 1), zd);
                wa2 = simplifyFractions(zn - Random.Range(1, zn), zd);
                wa3 = simplifyFractions(zd + Random.Range(1, zd), zn);
                break;

            case "Ecuaciones Simples 2":
                min = gameSystem.RemoteSO.dgbl_features.ilos[1].ilos[0].ilo_parameters[1].default_value;
                max = gameSystem.RemoteSO.dgbl_features.ilos[1].ilos[0].ilo_parameters[2].default_value;

                // xn * x + yn = 0 / xn * x + yn = xd * x + yd
                // xd != xn
                xn = Random.Range(min, max);
                validChoices = new int[] { Random.Range(-min, 0), Random.Range(1, xn), Random.Range(xn, min) };
                xd = validChoices[Random.Range(0, 2)];

                yd = Random.Range(yn + 1, max);

                zn = (-yd - yn);
                zd = (xn - xd);

                wa0 = simplifyFractions(zn, zd);
                wa1 = simplifyFractions(zn + Random.Range(1, zn + 1), zd);
                wa2 = simplifyFractions(zn - Random.Range(1, zn), zd);
                wa3 = simplifyFractions(zd + Random.Range(1, zd), zn);
                break;

            //L9----------------------------------------------------------------------------------
            case "Sucesiones":
                min = gameSystem.RemoteSO.dgbl_features.ilos[1].ilos[1].ilo_parameters[1].default_value;
                max = gameSystem.RemoteSO.dgbl_features.ilos[1].ilos[1].ilo_parameters[2].default_value;

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

                wa0 = zn.ToString();
                wa1 = (zn + Random.Range(1, zn / 2 + 1)).ToString();
                wa2 = (zn + Random.Range(zn / 2, zn + 1)).ToString();
                wa3 = (zn - Random.Range(1, zn)).ToString();
                break;

            //COMPETENCE 3 =======================================================================
            //L13---------------------------------------------------------------------------------
            case "Area Triangulo":
                min = gameSystem.RemoteSO.dgbl_features.ilos[2].ilos[0].ilos[1].ilo_parameters[1].default_value;
                max = gameSystem.RemoteSO.dgbl_features.ilos[2].ilos[0].ilos[1].ilo_parameters[2].default_value;

                xn = Random.Range(min, max);
                yn = Random.Range(min, max);

                znF = (double)xn * (double)yn / 2f;

                wa0 = System.Math.Round(znF, 3).ToString().Replace(",", ".");
                wa1 = System.Math.Round((znF / Random.Range(2, 5)), 3).ToString().Replace(",", ".");
                wa2 = System.Math.Round((znF / Random.Range(5, 8)), 3).ToString().Replace(",", ".");
                wa3 = System.Math.Round((znF / Random.Range(8, 11)), 3).ToString().Replace(",", ".");
                break;

            case "Perimetro Triangulo":
                min = gameSystem.RemoteSO.dgbl_features.ilos[2].ilos[0].ilos[0].ilo_parameters[1].default_value;
                max = gameSystem.RemoteSO.dgbl_features.ilos[2].ilos[0].ilos[0].ilo_parameters[2].default_value;

                xn = Random.Range(min, max);
                yn = Random.Range(min, max);
                xd = Random.Range(min, max);

                zn = xn + yn + xd;

                wa0 = zn.ToString();
                wa1 = (zn + Random.Range(1, zn / 2 + 1)).ToString();
                wa2 = (zn + Random.Range(zn / 2, zn + 1)).ToString();
                wa3 = (zn - Random.Range(1, zn)).ToString();
                break;

            case "Area Rectangulo":
                min = gameSystem.RemoteSO.dgbl_features.ilos[2].ilos[0].ilos[1].ilo_parameters[1].default_value;
                max = gameSystem.RemoteSO.dgbl_features.ilos[2].ilos[0].ilos[1].ilo_parameters[2].default_value;

                xn = Random.Range(min, max);
                yn = Random.Range(min, max);

                zn = xn * yn;

                wa0 = zn.ToString();
                wa1 = (zn + Random.Range(1, zn / 2 + 1)).ToString();
                wa2 = (zn + Random.Range(zn / 2, zn + 1)).ToString();
                wa3 = (zn - Random.Range(1, zn)).ToString();
                break;

            case "Perimetro Rectangulo":
                min = gameSystem.RemoteSO.dgbl_features.ilos[2].ilos[0].ilos[0].ilo_parameters[1].default_value;
                max = gameSystem.RemoteSO.dgbl_features.ilos[2].ilos[0].ilos[0].ilo_parameters[2].default_value;

                xn = Random.Range(min, max);
                yn = Random.Range(min, max);

                zn = 2 * (xn + yn);

                wa0 = zn.ToString();
                wa1 = (zn + Random.Range(1, zn / 2 + 1)).ToString();
                wa2 = (zn + Random.Range(zn / 2, zn + 1)).ToString();
                wa3 = (zn - Random.Range(1, zn)).ToString();
                break;

            case "Volumen Paralelepipedo":
                min = gameSystem.RemoteSO.dgbl_features.ilos[2].ilos[0].ilos[2].ilo_parameters[1].default_value;
                max = gameSystem.RemoteSO.dgbl_features.ilos[2].ilos[0].ilos[2].ilo_parameters[2].default_value;

                xn = Random.Range(min, max);
                yn = Random.Range(min, max);
                xd = Random.Range(min, max);

                zn = xn * yn * xd;

                wa0 = zn.ToString();
                wa1 = (zn + Random.Range(1, zn / 2 + 1)).ToString();
                wa2 = (zn + Random.Range(zn / 2, zn + 1)).ToString();
                wa3 = (zn - Random.Range(1, zn)).ToString();
                break;

            case "Planos":
                min = gameSystem.RemoteSO.dgbl_features.ilos[0].ilos[0].ilo_parameters[1].default_value;
                max = gameSystem.RemoteSO.dgbl_features.ilos[0].ilos[0].ilo_parameters[2].default_value;

                xn = Random.Range(min, max);
                yn = Random.Range(1, min);

                znF = (double)xn / (double)yn;

                wa0 = System.Math.Round(znF, 3).ToString().Replace(",", ".");
                wa1 = System.Math.Round((znF / Random.Range(2, 5)), 3).ToString().Replace(",", ".");
                wa2 = System.Math.Round((znF / Random.Range(5, 8)), 3).ToString().Replace(",", ".");
                wa3 = System.Math.Round((znF / Random.Range(8, 11)), 3).ToString().Replace(",", ".");
                break;

            //COMPETENCE 4 =======================================================================
            //L21---------------------------------------------------------------------------------
            case "Media Aritmetica":
                min = gameSystem.RemoteSO.dgbl_features.ilos[3].ilos[3].ilos[0].ilo_parameters[1].default_value;
                max = gameSystem.RemoteSO.dgbl_features.ilos[3].ilos[3].ilos[0].ilo_parameters[2].default_value;

                xn = Random.Range(min, max);
                yn = Random.Range(min, max);
                xd = Random.Range(min, max);
                yd = Random.Range(min, max);

                znF = (double)(xn + yn + xd + yd) / 4;
                zn = (znF < 0 ? 2 : (int)znF);

                wa0 = System.Math.Round(znF, 3).ToString().Replace(",", ".");
                wa1 = System.Math.Round((znF + Random.Range(1, zn / 2 + 2)), 3).ToString().Replace(",", ".");
                wa2 = System.Math.Round((znF + Random.Range(zn / 2, zn + 2)), 3).ToString().Replace(",", ".");
                wa3 = System.Math.Round((znF - Random.Range(1, zn)), 3).ToString().Replace(",", ".");
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

                wa0 = vals[0].ToString();
                wa1 = vals[1].ToString();
                wa2 = vals[2].ToString();
                wa3 = vals[3].ToString();
                break;

            case "Probabilidad":
                min = gameSystem.RemoteSO.dgbl_features.ilos[3].ilos[1].ilo_parameters[1].default_value;
                max = gameSystem.RemoteSO.dgbl_features.ilos[3].ilos[1].ilo_parameters[2].default_value;

                xn = Random.Range(min, max);
                yn = Random.Range(min, max);

                znF = (double)xn / (double)yn;

                wa0 = System.Math.Round(znF, 3).ToString().Replace(",", ".");
                wa1 = System.Math.Round((znF / Random.Range(2, 5)), 3).ToString().Replace(",", ".");
                wa2 = System.Math.Round((znF / Random.Range(5, 8)), 3).ToString().Replace(",", ".");
                wa3 = System.Math.Round((znF / Random.Range(8, 11)), 3).ToString().Replace(",", ".");
                break;

            default:
                Debug.Log("No se pudo asignar variables a la conversación " + dialogueSystemTrigger.conversation);
                break;
        }

        //Set variables
        if (dialogueSystemTrigger.conversation.StartsWith("Decimales") ||
            (dialogueSystemTrigger.conversation.Equals("Naturales Suma") && uE == 1))
        {
            DialogueLua.SetVariable("Xn", xnF); //Set numerator
            DialogueLua.SetVariable("Yn", ynF); //Set numerator
        }
        else
        {
            DialogueLua.SetVariable("Xn", xn); //Set numerator
            DialogueLua.SetVariable("Yn", yn); //Set numerator
        }

        DialogueLua.SetVariable("Xd", xd); //Set denominator
        DialogueLua.SetVariable("Yd", yd); //Set denominator

        //Set question string auxiliar
        DialogueLua.SetVariable("Q0", q0);

        //Set some units in questions
        DialogueLua.SetVariable("U0", u0);
        DialogueLua.SetVariable("U1", u1);

        //Set the correct answer
        DialogueLua.SetVariable("Wa0", wa0);

        //Set the wrong answers from Zn and Zd values
        DialogueLua.SetVariable("Wa1", wa1);
        DialogueLua.SetVariable("Wa2", wa2);
        DialogueLua.SetVariable("Wa3", wa3);

        //Finally, each conversation will determine whether to display numerators or denominators.
    }

    //auxiliar methods
    private int greatestCommonFactor(int a, int b)
    {
        while (b != 0)
        {
            int temp = b;
            b = a % b;
            a = temp;
        }
        return a;
    }

    private int leastCommonMultiple(int a, int b)
    {
        return (a / greatestCommonFactor(a, b)) * b;
    }

    private string simplifyFractions(int n, int d)
    {
        if (n == 0) return "0";

        int auxn = n, auxd = d;
        int aux = greatestCommonFactor(n, d);
        if (aux != 1) //they have multiples
        {
            auxn /= aux;
            auxd /= aux;

            if (auxd < 0)
            {
                auxn *= -1;
                auxd *= -1;
            }
        }

        if (auxd == 1)
        {
            return auxn.ToString();
        }
        else
        {
            return auxn.ToString() + " / " + auxd.ToString();
        }
    }
}
