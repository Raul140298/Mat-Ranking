using PixelCrushers.DialogueSystem;
using System.Collections;
using System.Collections.Generic;
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
		//gameSystem.virtualCamera1.ShakeCamera(10f, 1f);
		gameSystem.virtualCamera2.ShakeCamera(2f, 0.2f);
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
		yield return new WaitForSeconds(1f);
		if(particlesShow) particles.Play();
		//Deactivate dialogue
		this.transform.GetChild(0).gameObject.SetActive(false);
		this.GetComponent<CircleCollider2D>().enabled = false;
		this.GetComponent<SpriteRenderer>().enabled = false;
	}

	public void initEnemyData()
	{
		knowledgePoints = enemyData.knowledgePoints;

		var main = particles.main;
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
		//Get a random data for the variables
		int xn, xd, yn, yd, zn, zd, aux;
		int[] validChoices = new int[2];
		double xnF, ynF, znF;
		string wa0, wa1, wa2, wa3, q0;
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
		q0 = "";
		
		//Compendium of all the possible conversations that an enemy can have.
		switch (dialogueSystemTrigger.conversation)
		{
			//COMPETENCE 1 =======================================================================
			//L1----------------------------------------------------------------------------------
			case "Naturales Suma":
				xn = Random.Range(10, 50);
				yn = Random.Range(10, 50);

				zn = xn + yn;

				wa0 = zn.ToString();
				wa1 = (zn + Random.Range(1, zn / 2 + 1)).ToString();
				wa2 = (zn + Random.Range(zn / 2, zn + 1)).ToString();
				wa3 = (zn - Random.Range(1, zn)).ToString();
				break;

			case "Naturales Resta":
				xn = Random.Range(10, 50);

				//This way, yn will never be xn
				validChoices = new int[] { Random.Range(10, xn), Random.Range(xn + 1, 50) };
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
				xn = Random.Range(2, 25);
				yn = Random.Range(2, 25);

				zn = xn * yn;

				wa0 = zn.ToString();
				wa1 = (zn * Random.Range(2, 5)).ToString();
				wa2 = (zn * Random.Range(5, 8)).ToString();
				wa3 = (zn * Random.Range(8, 11)).ToString();
				break;

			case "Naturales Division":
				znF = xn / yn;

				wa0 = System.Math.Round(znF, 3).ToString().Replace(",", ".");
				wa1 = System.Math.Round((znF / Random.Range(2, 5)), 3).ToString().Replace(",", ".");
				wa2 = System.Math.Round((znF / Random.Range(5, 8)), 3).ToString().Replace(",", ".");
				wa3 = System.Math.Round((znF / Random.Range(8, 11)), 3).ToString().Replace(",", ".");
				break;

			case "Naturales Potencia":
				xn = Random.Range(12, 25);
				yn = Random.Range(2, 4);

				zn = (int) Mathf.Pow(xn, yn);

				wa0 = zn.ToString();
				wa1 = (zn + Random.Range(1, zn / 2 + 1)).ToString();
				wa2 = (zn + Random.Range(zn / 2, zn + 1)).ToString();
				wa3 = (zn - Random.Range(1, zn)).ToString();
				break;

			//L2----------------------------------------------------------------------------------
			case "Fracciones Suma":
				zd = leastCommonMultiple(xd, yd);
				zn = xn * (zd / xd) + yn * (zd / yd);

				wa0 = simplifyFractions(zn, zd);
				wa1 = simplifyFractions(zn + Random.Range(1, zn + 1), zd);
				wa2 = simplifyFractions(zn - Random.Range(1, zn), zd);
				wa3 = simplifyFractions(zd + Random.Range(1, zd), zn);
				break;

			case "Fracciones Resta":
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
				zd = xd * yd;
				zn = xn * yn;

				wa0 = simplifyFractions(zn, zd);
				wa1 = simplifyFractions(zn + Random.Range(1, zn + 1), zd);
				wa2 = simplifyFractions(zn - Random.Range(1, zn), zd);
				wa3 = simplifyFractions(zd + Random.Range(1, zd), zn);
				break;

			case "Decimales Suma":
				xn = Random.Range(10, 50);
				yn = Random.Range(10, 50);
				zn = xn + yn;

				xnF = System.Math.Round((xn / 100f), 3);
				ynF = System.Math.Round((yn / 100f), 3);

				wa0 = System.Math.Round((zn / 100f), 3).ToString().Replace(",",".");
				wa1 = System.Math.Round(((zn + Random.Range(1, zn / 2 + 1)) / 100f), 3).ToString().Replace(",", ".");
				wa2 = System.Math.Round(((zn + Random.Range(zn / 2, zn + 1)) / 100f), 3).ToString().Replace(",", ".");
				wa3 = System.Math.Round(((zn - Random.Range(1, zn)) / 100f), 3).ToString().Replace(",", ".");
				break;

			case "Decimales Resta":
				xn = Random.Range(10, 50);
				validChoices = new int[] { Random.Range(10, xn), Random.Range(xn + 1, 50) };
				yn = validChoices[Random.Range(0, 1)];
				zn = xn - yn;

				xnF = System.Math.Round((xn / 100f), 3);
				ynF = System.Math.Round((yn / 100f), 3);

				wa0 = (zn / 100f).ToString().Replace(",", ".");
				if (zn < 0)
				{
					wa1 = "-" + System.Math.Round(((-zn + Random.Range(1, -zn / 2 + 1)) / 100f), 3).ToString().Replace(",", ".");
					wa2 = "-" + System.Math.Round(((-zn + Random.Range(-zn / 2, -zn + 1)) / 100f), 3).ToString().Replace(",", ".");
					wa3 = "-" + System.Math.Round(((-zn - Random.Range(1, -zn)) / 100f), 3).ToString().Replace(",", ".");
				}
				else
				{
					wa1 = System.Math.Round(((zn + Random.Range(1, zn / 2 + 1)) / 100f), 3).ToString().Replace(",", ".");
					wa2 = System.Math.Round(((zn + Random.Range(zn / 2, zn + 1)) / 100f), 3).ToString().Replace(",", ".");
					wa3 = System.Math.Round(((zn - Random.Range(1, zn)) / 100f), 3).ToString().Replace(",", ".");
				}
				break;

			case "Decimales Multiplicacion":
				xn = Random.Range(10, 50);
				yn = Random.Range(10, 50);
				zn = xn * yn;

				xnF = System.Math.Round((xn / 100f), 3);
				ynF = System.Math.Round((yn / 100f), 3);

				wa0 = System.Math.Round((zn / 100f), 3).ToString().Replace(",", ".");
				wa1 = System.Math.Round(((zn + Random.Range(1, zn / 2 + 1)) / 100f), 3).ToString().Replace(",", ".");
				wa2 = System.Math.Round(((zn + Random.Range(zn / 2, zn + 1)) / 100f), 3).ToString().Replace(",", ".");
				wa3 = System.Math.Round(((zn - Random.Range(1, zn)) / 100f), 3).ToString().Replace(",", ".");
				break;

			//COMPETENCE 2 =======================================================================
			//L8----------------------------------------------------------------------------------
			case "Ecuaciones Simples 1":
				// xn * x + yn = 0 / xn * x + yn = xd * x + yd
				// xd != xn
				validChoices = new int[] { Random.Range(-11, 0), Random.Range(1, xn), Random.Range(xn, 11) };
				xd = validChoices[Random.Range(0, 2)];

				yd = Random.Range(yn + 1, 50);

				zn = (yd - yn);
				zd = (xn - xd);

				wa0 = simplifyFractions(zn, zd);
				wa1 = simplifyFractions(zn + Random.Range(1, zn + 1), zd);
				wa2 = simplifyFractions(zn - Random.Range(1, zn), zd);
				wa3 = simplifyFractions(zd + Random.Range(1, zd), zn);
				break;

			case "Ecuaciones Simples 2":
				// xn * x + yn = 0 / xn * x + yn = xd * x + yd
				// xd != xn
				validChoices = new int[] { Random.Range(-11, 0), Random.Range(1, xn), Random.Range(xn, 11) };
				xd = validChoices[Random.Range(0, 2)];

				yd = Random.Range(yn + 1, 50);

				zn = (-yd - yn);
				zd = (xn - xd);

				wa0 = simplifyFractions(zn, zd);
				wa1 = simplifyFractions(zn + Random.Range(1, zn + 1), zd);
				wa2 = simplifyFractions(zn - Random.Range(1, zn), zd);
				wa3 = simplifyFractions(zd + Random.Range(1, zd), zn);
				break;

			//L9----------------------------------------------------------------------------------
			case "Sucesiones":
				xn = Random.Range(2, 10);
				validChoices = new int[] { Random.Range(2, xn), Random.Range(xn, 10) };
				aux = validChoices[Random.Range(0, 1)];

				if(aux <= 5)
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
				xn = Random.Range(10, 20);
				yn = Random.Range(10, 20);

				znF = xn * yn / 2;

				wa0 = System.Math.Round(znF, 3).ToString().Replace(",", ".");
				wa1 = System.Math.Round((znF / Random.Range(2, 5)), 3).ToString().Replace(",", ".");
				wa2 = System.Math.Round((znF / Random.Range(5, 8)), 3).ToString().Replace(",", ".");
				wa3 = System.Math.Round((znF / Random.Range(8, 11)), 3).ToString().Replace(",", ".");
				break;

			case "Perimetro Triangulo":
				xn = Random.Range(10, 50);
				yn = Random.Range(10, 50);
				xd = Random.Range(10, 50);

				zn = xn + yn + xd;

				wa0 = zn.ToString();
				wa1 = (zn + Random.Range(1, zn / 2 + 1)).ToString();
				wa2 = (zn + Random.Range(zn / 2, zn + 1)).ToString();
				wa3 = (zn - Random.Range(1, zn)).ToString();
				break;

			case "Area Rectangulo":
				xn = Random.Range(10, 20);
				yn = Random.Range(10, 20);

				zn = xn * yn;

				wa0 = zn.ToString();
				wa1 = (zn + Random.Range(1, zn / 2 + 1)).ToString();
				wa2 = (zn + Random.Range(zn / 2, zn + 1)).ToString();
				wa3 = (zn - Random.Range(1, zn)).ToString();
				break;

			case "Perimetro Rectangulo":
				xn = Random.Range(10, 50);
				yn = Random.Range(10, 50);

				zn = 2 * (xn + yn);

				wa0 = zn.ToString();
				wa1 = (zn + Random.Range(1, zn / 2 + 1)).ToString();
				wa2 = (zn + Random.Range(zn / 2, zn + 1)).ToString();
				wa3 = (zn - Random.Range(1, zn)).ToString();
				break;

			case "Volumen Paralelepipedo":
				xn = Random.Range(10, 20);
				yn = Random.Range(10, 20);
				xd = Random.Range(10, 20);

				zn = xn * yn * xd;

				wa0 = zn.ToString();
				wa1 = (zn + Random.Range(1, zn / 2 + 1)).ToString();
				wa2 = (zn + Random.Range(zn / 2, zn + 1)).ToString();
				wa3 = (zn - Random.Range(1, zn)).ToString();
				break;

			//COMPETENCE 4 =======================================================================
			//L21---------------------------------------------------------------------------------
			case "Media Aritmetica":
				xn = Random.Range(10, 50);
				yn = Random.Range(10, 50);
				xd = Random.Range(10, 50);
				yd = Random.Range(10, 50);

				zn = (xn + yn + xd + yd) / 4;
				znF = (xn + yn + xd + yd) / 4;

				wa0 = System.Math.Round(znF, 3).ToString().Replace(",", ".");
				wa1 = System.Math.Round((znF + Random.Range(1, zn / 2 + 1)), 3).ToString().Replace(",", ".");
				wa2 = System.Math.Round((znF + Random.Range(zn / 2, zn + 1)), 3).ToString().Replace(",", ".");
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

			default:
				Debug.Log("No se pudo asignar variables a la conversación " + dialogueSystemTrigger.conversation);
				break;
		}

		//Set variables
		if(dialogueSystemTrigger.conversation.Equals("Decimales Suma") || dialogueSystemTrigger.conversation.Equals("Decimales Resta") || dialogueSystemTrigger.conversation.Equals("Decimales Multiplicacion"))
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
