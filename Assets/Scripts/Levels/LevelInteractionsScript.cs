using UnityEngine;
using PixelCrushers.DialogueSystem;
using UnityEngine.UI;
using System.Collections;

public class LevelInteractionsScript: MonoBehaviour
{
    public ProximitySelector proximitySelector;
	public SaveSystemScript saveSystem;
	public LevelScript level;
	public GameObject currentEnemy;
	public PlayerRendererScript playerRenderer;
	public CapsuleCollider2D playerDialogueArea;
	public DialogueCameraScript dialogueCamera;
	public Text tq1, ca2, tpq3;
	public CurrentLevelSO currentLevelSO;
	public float timerSummary;
	public GameObject timer;

	public void Start()
	{
		timerSummary = 0;

		asignSummary();

		StartCoroutine(setTimer());
	}

	IEnumerator setTimer()
	{
		yield return new WaitForSeconds(0.2f);
		timer = GameObject.FindGameObjectWithTag("DialogueManager").transform.GetChild(0).transform.GetChild(3).transform.GetChild(1).transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).gameObject;
	}

	void asignSummary()
	{
		tq1.text = currentLevelSO.totalQuestions.ToString();
		ca2.text = currentLevelSO.correctAnswers.ToString();
		tpq3.text = currentLevelSO.timePerQuestion.ToString();
	}

	public void averageTimePerQuestions()
	{
		currentLevelSO.timePerQuestion /= currentLevelSO.totalQuestions;
		tpq3.text = currentLevelSO.timePerQuestion.ToString();
	}

	//Interactions with colliders and triggers
	private void OnTriggerEnter2D(Collider2D collision)
	{
		if(collision.tag == "Enemy")
		{
			lookTarget(collision.gameObject);
			currentEnemy = collision.gameObject;
			dialogueCamera.target = currentEnemy;
			//Verify if the enemy data has been filled
			if (currentEnemy.transform.parent.GetComponent<EnemyScript>().enemyData != null)
			{
				currentEnemy.transform.parent.GetComponent<Animator>().SetTrigger("start");
				if (playerDialogueArea.enabled == true && currentEnemy.transform.parent.GetComponent<EnemyScript>().startQuestion == true)
				{
					SoundsScript.PlaySound("EXCLAMATION 2");

					currentLevelSO.totalQuestions += 1;
					asignSummary();

					timerSummary = Time.time;

					StartCoroutine(startTimer());
				}
				useCurrentSelection();
			}
		}		
	}

	IEnumerator startTimer()
	{
		yield return new WaitForSeconds(0.2f);
		timer.SetActive(false);
		timer.GetComponent<TimerScript>().startingTime = 10f;
		timer.GetComponent<TimerScript>().aux = 10f;
		yield return new WaitForSeconds(1.8f);
		timer.SetActive(true);
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		dialogueCamera.target = null;
		if (collision.gameObject.tag == "NextLevel")
		{
			lookTarget(collision.gameObject);
			timer.SetActive(false);
			proximitySelector.UseCurrentSelection();
		}
	}


	//Functions
	public void lookTarget(GameObject target)
	{

		if(this.gameObject.transform.position.x > target.gameObject.transform.position.x && !playerRenderer.PlayerIsLookingLeft())
		{
			playerRenderer.spriteRenderer.flipX = true;
		}
		else if (this.gameObject.transform.position.x < target.gameObject.transform.position.x && playerRenderer.PlayerIsLookingLeft())
		{
			playerRenderer.spriteRenderer.flipX = false;
		}
	}

	public void nextLevel()
	{
		saveSystem.nextPlayerCurrentLevel();
		level.LoadNextLevel();
	}

	public void useCurrentSelection()
	{
		//Edit vairables before the conversation start
		currentEnemy.transform.parent.GetComponent<EnemyScript>().setVariables();
		//then start the conversation
		proximitySelector.UseCurrentSelection();
	}

	public void defeatedEnemy()
	{
		if (currentEnemy)
		{
			if (timer) timer.SetActive(false);

			currentEnemy.gameObject.SetActive(false);
			currentEnemy.gameObject.transform.parent.GetComponent<EnemyScript>().defeated();

			currentLevelSO.correctAnswers += 1;
			asignSummary();

			currentLevelSO.timePerQuestion += Mathf.RoundToInt((Time.time - timerSummary) % 60);
		}
	}

	public void playerDefeated()
	{
		if (timer) timer.SetActive(false);

		if (currentEnemy)
		{
			currentEnemy.gameObject.SetActive(false);
			currentEnemy.gameObject.transform.parent.GetComponent<EnemyScript>().winner();
		}
	}
}
