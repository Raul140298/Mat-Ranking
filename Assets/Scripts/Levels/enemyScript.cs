using PixelCrushers.DialogueSystem;
using System.Collections;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
	public GameSystemScript gameSystem;
	public EnemySO enemyData = null;
	public int knowledgePoints;
	public string question;
	public LevelScript level;
	public DialogueSystemTrigger dialogueSystemTrigger;
	public bool startQuestion = false;
	public ParticleSystem particles;

	private void Start()
	{
		gameSystem = GameObject.FindGameObjectWithTag("GameSystem").GetComponent<GameSystemScript>();
		level = GameObject.FindGameObjectWithTag("LevelScript").GetComponent<LevelScript>();
	}

	public void defeated()
	{
		gameSystem.changeKnowledgePoints(knowledgePoints);
		//After some time and animation
		StartCoroutine(dissappear(true));
	}

	public void winner()
	{
		gameSystem.changeKnowledgePoints(-knowledgePoints);
		//After some time and animation
		if (enemyData.canPushYou)
		{
			gameSystem.prevPlayerCurrentLevel();
			level.LoadPrevLevel();
		}
		else
		{
			//Have to be changed to only disappear the points, but the body stay it.
			StartCoroutine(dissappear(false));
		}
	}

	IEnumerator dissappear(bool particlesShow)
	{
		//animation.setTrigger("defeat");
		yield return new WaitForSeconds(1f);
		if(particlesShow) particles.Play();
		//Deactivate dialogue
		this.transform.GetChild(0).gameObject.SetActive(false);
		this.GetComponent<CircleCollider2D>().enabled = false;
		this.GetComponent<SpriteRenderer>().enabled = false;
		//this.gameObject.SetActive(false);
	}

	public void initEnemyData()
	{
		int aux = enemyData.knowledgePoints;
		knowledgePoints = Random.Range(aux -4, aux + 4);
		var main = particles.main;
		main.maxParticles = knowledgePoints;
		//main.startSize = 0.05f;

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
		//Get a random data for the variables
		int xn, xd, yn, yd, zn, zd;
		int[] validChoices = new int[2];
		double xnF, ynF, znF;
		string wa0, wa1, wa2, wa3;
		xn = Random.Range(1, 11);
		xd = Random.Range(1, 11);
		yn = Random.Range(1, 11);
		yd = Random.Range(1, 11);
		xnF = 1f;
		ynF = 1f;
		zn = 1;
		zd = 1;
		wa0 = "";
		wa1 = "";
		wa2 = "";
		wa3 = "";
		
		//Compendium of all the possible conversations that an enemy can have.
		switch (dialogueSystemTrigger.conversation)
		{
			case "Fracciones Suma":
				//Create correct answer
				zd = leastCommonMultiple(xd, yd);
				zn = xn * (zd / xd) + yn * (zd / yd);

				wa0 = simplifyFractions(zn, zd);
				wa1 = simplifyFractions(zn + Random.Range(1, zn + 1), zd);
				wa2 = simplifyFractions(zn - Random.Range(1, zn), zd);
				wa3 = simplifyFractions(zd + Random.Range(1, zd), zn);
				break;

			case "Fracciones Resta":
				//Create correct answer
				zd = leastCommonMultiple(xd, yd);
				zn = xn * (zd / xd) - yn * (zd / yd);

				wa0 = simplifyFractions(zn, zd);
				if(zn < 0)
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
				//Create correct answer
				zd = xd * yd;
				zn = xn * yn;

				wa0 = simplifyFractions(zn, zd);
				wa1 = simplifyFractions(zn + Random.Range(1, zn + 1), zd);
				wa2 = simplifyFractions(zn - Random.Range(1, zn), zd);
				wa3 = simplifyFractions(zd + Random.Range(1, zd), zn);
				break;

			case "Decimales Suma":
				//Create correct answer
				xn = Random.Range(10, 50);
				yn = Random.Range(10, 50);
				zn = xn + yn;

				xnF = System.Math.Round((xn / 100f), 3);
				ynF = System.Math.Round((yn / 100f), 3);
				znF = System.Math.Round((zn / 100f), 3);

				wa0 = (zn / 100f).ToString().Replace(",",".");
				wa1 = ((zn + Random.Range(1, zn / 2 + 1)) / 100f).ToString().Replace(",", ".");
				wa2 = ((zn + Random.Range(zn / 2, zn + 1)) / 100f).ToString().Replace(",", ".");
				wa3 = ((zn - Random.Range(1, zn)) / 100f).ToString().Replace(",", ".");
				break;

			case "Decimales Resta":
				//Create correct answer
				xn = Random.Range(10, 50);
				validChoices = new int[] { Random.Range(10, xn), Random.Range(xn + 1, 50) };
				yn = validChoices[Random.Range(0, 1)];
				zn = xn - yn;

				xnF = System.Math.Round( (xn / 100f) , 3);
				ynF = System.Math.Round((yn / 100f), 3);
				znF = System.Math.Round((zn / 100f), 3);

				wa0 = (zn / 100f).ToString().Replace(",", ".");
				if (zn < 0)
				{
					wa1 = "-" + ((-zn + Random.Range(1, -zn / 2 + 1)) / 100f).ToString().Replace(",", ".");
					wa2 = "-" + ((-zn + Random.Range(-zn / 2, -zn + 1)) / 100f).ToString().Replace(",", ".");
					wa3 = "-" + ((-zn - Random.Range(1, -zn)) / 100f).ToString().Replace(",", ".");
				}
				else
				{
					wa1 = ((zn + Random.Range(1, zn / 2 + 1)) / 100f).ToString().Replace(",", ".");
					wa2 = ((zn + Random.Range(zn / 2, zn + 1)) / 100f).ToString().Replace(",", ".");
					wa3 = ((zn - Random.Range(1, zn)) / 100f).ToString().Replace(",", ".");
				}
				break;

			case "Naturales Suma":
				//Create correct answer
				xn = Random.Range(10, 50);
				yn = Random.Range(10, 50);

				zn = xn + yn;

				wa0 = zn.ToString();
				wa1 = (zn + Random.Range(1, zn/2 + 1)).ToString();
				wa2 = (zn + Random.Range(zn/2, zn + 1)).ToString();
				wa3 = (zn - Random.Range(1, zn)).ToString();
				break;

			case "Naturales Resta":
				//Create correct answer
				xn = Random.Range(10, 50);

				//This way, yn will never be xn
				validChoices = new int[] { Random.Range(10, xn), Random.Range(xn + 1, 50) };
				yn = validChoices[Random.Range(0,1)];

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
				//Create correct answer
				xn = Random.Range(2, 25);
				yn = Random.Range(2, 25);

				zn = xn * yn;

				wa0 = zn.ToString();
				wa1 = (zn * Random.Range(2, 5)).ToString();
				wa2 = (zn * Random.Range(5, 8)).ToString();
				wa3 = (zn * Random.Range(8, 11)).ToString();
				break;

			case "Naturales Division":
				//Create correct answer
				znF = (double)xn / (double)yn;

				wa0 = System.Math.Round(znF, 3).ToString().Replace(",", ".");
				wa1 = System.Math.Round((znF / Random.Range(2, 5)), 3).ToString().Replace(",", ".");
				wa2 = System.Math.Round((znF / Random.Range(5, 8)), 3).ToString().Replace(",", ".");
				wa3 = System.Math.Round((znF / Random.Range(8, 11)), 3).ToString().Replace(",", ".");
				break;

			case "Ecuaciones Simples 1":
				//Create correct answer
				// xn * x + yn = 0 / xn * x + yn = xd * x + yd
				// xd != xn
				validChoices = new int[] { Random.Range(-11, xn), Random.Range(xn + 1, 11) };
				xd = validChoices[Random.Range(0, 1)];

				yd = Random.Range(yn + 1, 50);

				zn = (yd - yn);
				zd = (xn - xd);

				wa0 = simplifyFractions(zn, zd);
				wa1 = simplifyFractions(zn + Random.Range(1, zn + 1), zd);
				wa2 = simplifyFractions(zn - Random.Range(1, zn), zd);
				wa3 = simplifyFractions(zd + Random.Range(1, zd), zn);
				break;

			case "Ecuaciones Simples 2":
				//Create correct answer
				// xn * x + yn = 0 / xn * x + yn = xd * x + yd
				// xd != xn
				validChoices = new int[] { Random.Range(-11, xn), Random.Range(xn + 1, 11) };
				xd = validChoices[Random.Range(0, 1)];

				yd = Random.Range(yn + 1, 50);

				zn = (-yd - yn);
				zd = (xn - xd);

				wa0 = simplifyFractions(zn, zd);
				wa1 = simplifyFractions(zn + Random.Range(1, zn + 1), zd);
				wa2 = simplifyFractions(zn - Random.Range(1, zn), zd);
				wa3 = simplifyFractions(zd + Random.Range(1, zd), zn);
				break;

			case "Sucesiones":
				//Create correct answer
				xn = Random.Range(2, 20);
				int aux = Random.Range(10, 20);
				if(aux <= 15)
				{
					yn = 2 * xn + aux;
					xd = 3 * xn + aux;
					yd = 4 * xn + aux;
					xn = xn + aux;
				}
				else
				{
					yn = xn * aux;
					xd = yn* xn * aux;
					yd = xd * yn * xn * aux;
				}

				wa0 = zn.ToString();
				wa1 = (zn + Random.Range(1, zn / 2 + 1)).ToString();
				wa2 = (zn + Random.Range(zn / 2, zn + 1)).ToString();
				wa3 = (zn - Random.Range(1, zn)).ToString();
				break;

			default:
				Debug.Log("No se pudo asignar variables a la conversación " + dialogueSystemTrigger.conversation);
				break;
		}

		//Set variables
		if(dialogueSystemTrigger.conversation.Equals("Decimales Suma") || dialogueSystemTrigger.conversation.Equals("Decimales Resta"))
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

		if(auxd == 1)
		{
			return auxn.ToString();
		}
		else
		{
			return auxn.ToString() + " / " + auxd.ToString();
		}
	}
}
