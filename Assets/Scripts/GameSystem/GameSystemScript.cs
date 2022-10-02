using UnityEngine;
using UnityEngine.UI;
using PixelCrushers.DialogueSystem;
using UnityEngine.SceneManagement;

public class GameSystemScript : MonoBehaviour
{
	public LevelInteractionsScript player;
	public PlayerSO playerSO;
	public OptionsSO optionsSO;
	public RemoteSO remoteSO;
	public CurrentLevelSO currentLevelSO;
	public Text knowledgePoints;
	public PlayFabScript playFab;
	public GameObject dm;
	public SaveSystemScript saveSystem;
	[System.Serializable]
	public class EnemysInZone
	{
		public EnemySO[] enemys;
	}
	public EnemysInZone[] enemysInZone;
	public LevelGeneratorScript levelGenerator;
	public CinemachineShakeScript virtualCamera1, virtualCamera2;

	private void Start()
	{
		Application.targetFrameRate = 60;

		dm = GameObject.FindGameObjectWithTag("DialogueManager");
		DialogueSystemController aux = dm.GetComponent<DialogueSystemController>();
		if (SceneManager.GetActiveScene().buildIndex == 2)
		{
			aux.displaySettings.subtitleSettings.continueButton = DisplaySettings.SubtitleSettings.ContinueButtonMode.Never;
		}
		else
		{
			aux.displaySettings.subtitleSettings.continueButton = DisplaySettings.SubtitleSettings.ContinueButtonMode.Always;
		}
	}

	public void fitEnemyColors(int[] aux)
	{
		Color[] auxColors = new Color[4];
		EnemyScript currentEnemy = player.currentEnemy.transform.parent.GetComponent<EnemyScript>();
		
		auxColors[0] = currentEnemy.colors[0];
		auxColors[1] = currentEnemy.colors[1];
		auxColors[2] = currentEnemy.colors[2];
		auxColors[3] = currentEnemy.colors[3];

		for (int j = 0; j < 4; j++)
		{
			for (int k = 0; k < 4; k++)
			{
				if (aux[k] == j)
				{
					currentEnemy.colors[j] = auxColors[k];
				}
			}
		}
	}

	public void changeKnowledgePoints(int n)
	{
		if (playerSO.knowledgePoints + n >= 0)
		{
			playerSO.knowledgePoints += n;

			//Connection to bd on PlayFab
			saveSystem.sendRanking();
			setKnowledgePoints();
			saveSystem.saveLocal();
		}
	}

	public void setKnowledgePoints()
	{
		if (knowledgePoints) knowledgePoints.text = playerSO.knowledgePoints.ToString();
	}

	public void resetPlayerCurrentLevel()
	{
		currentLevelSO.playerLives = 3;
		currentLevelSO.currentLevel = 1;
		currentLevelSO.totalQuestions = 0;
		currentLevelSO.correctAnswers = 0;
		currentLevelSO.timePerQuestion = 0;
	}

	public void nextPlayerCurrentLevel()
	{
		currentLevelSO.currentLevel += 1;
	}

	public void prevPlayerCurrentLevel()
	{
		currentLevelSO.currentLevel -= 1;
	}

	public void enableSelectedEnemys()
	{
		for(int i=0; i < 4; i++)
		{
			for(int j=0; j < enemysInZone[i].enemys.Length; j++)
			{
				if (enemysInZone[i].enemys[j].configurations.selected == true)
				{
					levelGenerator.enemysInZone[i].enemys.Add(enemysInZone[i].enemys[j]);
				}
			}
		}
	}
}