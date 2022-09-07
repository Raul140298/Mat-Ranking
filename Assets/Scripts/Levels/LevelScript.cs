using PixelCrushers.DialogueSystem;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelScript : MonoBehaviour
{
	public Animator transitionAnimator;
	public FromLevelSO fromLevelSO;
	public CurrentLevelSO currentLevelSO;
	public SaveSystemScript saveSystem;
	public GameSystemScript gameSystem;
	public Text zone, level;
	public CapsuleCollider2D playerDialogueArea;
	public Animator dialoguePanel;
	public GameObject topBar, bottomBar;
	public SoundtracksScript soundtracks;
	public LevelInteractionsScript playerLevelInteractions;

	private void Start()
	{
		fromLevelSO.fromLevel = true;
		gameSystem.setKnowledgePoints();
		switch (Localization.language)
		{
			case "es":
				zone.text = "Prueba";
				level.text = "Piso";
				break;
			case "en":
				zone.text = "Test";
				level.text = "Floor";
				break;
			case "qu":
				break;
			default:
				// code block
				break;
		}
		zone.text += " " + (currentLevelSO.currentZone + 1).ToString();
		level.text += " " + currentLevelSO.currentLevel.ToString();

		StartCoroutine(playerDialogueStart());
	}

	IEnumerator playerDialogueStart()
	{
		yield return new WaitForSeconds(0.1f);
		SoundsScript.PlaySound("LEVEL START");
		yield return new WaitForSeconds(2f);
		SoundtracksScript.PlaySoundtrack("LEVEL" + currentLevelSO.currentZone.ToString());
		yield return new WaitForSeconds(0.9f);

		dialoguePanel = GameObject.FindGameObjectWithTag("DialoguePanel").transform.GetChild(1).GetComponent<Animator>();
		dialoguePanel.ResetTrigger("Hide");
		dialoguePanel.ResetTrigger("Show");

		playerDialogueArea.enabled = true;
	}

	public void LoadAdventure(float transitionTime)
	{
		StartCoroutine(loadAdventure(transitionTime));
	}


	public void LoadNextLevel()
	{
		saveSystem.saveLocal();
		topBar.SetActive(false);
		bottomBar.SetActive(false);

		if (currentLevelSO.currentLevel >= 4) //Max floors == 4 -> editable
		{
			LoadAdventure(5); //time for end level UI menu

			playerLevelInteractions.averageTimePerQuestions();
		}
		else
		{
			StartCoroutine(loadNextLevel());
		}
	}

	public void LoadPrevLevel()
	{
		saveSystem.saveLocal();
		topBar.SetActive(false);
		bottomBar.SetActive(false);

		soundtracks.reduceVolume();

		if (currentLevelSO.currentLevel <= 0)
		{		
			LoadAdventure(1);
		}
		else
		{
			StartCoroutine(loadPrevLevel());
		}
	}

	IEnumerator loadAdventure(float transitionTime)
	{
		if(transitionTime == 1)
		{
			Debug.Log("Perdiste la mazmorra");
			yield return new WaitForSeconds(1f);
			dialoguePanel.SetTrigger("Hide");
			transitionAnimator.SetBool("lastFloor", false);
			transitionAnimator.SetTrigger("end");
			yield return new WaitForSeconds(1f);
		}
		else if (transitionTime == 5)
		{
			Debug.Log("Ganaste, ver el resumen");
			yield return new WaitForSeconds(1f);
			dialoguePanel.SetTrigger("Hide");
			transitionAnimator.SetBool("lastFloor", true);
			transitionAnimator.SetTrigger("end");
			yield return new WaitForSeconds(3.5f);
			transitionAnimator.SetTrigger("end");
			yield return new WaitForSeconds(1f);
		}
		else
		{
			Debug.Log("Acabaste el nivel");
			yield return new WaitForSeconds(1f);
			dialoguePanel.SetTrigger("Hide");
			yield return new WaitForSeconds(transitionTime);
			transitionAnimator.SetBool("lastFloor", false);
			transitionAnimator.SetTrigger("end");
			yield return new WaitForSeconds(1f);
		}

		dialoguePanel.ResetTrigger("Hide");
		yield return new WaitForSeconds(0.5f);

		SceneManager.LoadScene(1);
	}

	IEnumerator loadNextLevel()
	{
		Debug.Log("Subiste de piso");
		yield return new WaitForSeconds(1f);
		dialoguePanel.SetTrigger("Hide");
		
		transitionAnimator.SetTrigger("end");
		yield return new WaitForSeconds(1f);
		SceneManager.LoadScene(2); // 0: mainMenu, 1:adventure, 2:level
	}

	IEnumerator loadPrevLevel()
	{
		Debug.Log("Bajaste de piso");
		yield return new WaitForSeconds(0.7f);
		dialoguePanel.SetTrigger("Hide");

		transitionAnimator.SetTrigger("end");
		yield return new WaitForSeconds(1f);
		SceneManager.LoadScene(2); // 0: mainMenu, 1:adventure, 2:level
	}

	public void Exit()
	{
		Application.Quit();
	}
}
