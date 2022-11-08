using NUnit.Framework;
using PixelCrushers.DialogueSystem;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;

public class UnitTests
{
	// A Test behaves as an ordinary method
	[Test]
	public void MainMenu()
	{
		GameObject testObject = new GameObject();
		FromLevelSO fromLevelSO = ScriptableObject.CreateInstance<FromLevelSO>();
		MainMenuScript mainMenuScript = testObject.AddComponent<MainMenuScript>();
		mainMenuScript.fromLevelSO = fromLevelSO;

		mainMenuScript.Init();

		Assert.IsFalse(mainMenuScript.fromLevelSO.fromLevel);
	}

	[UnityTest]
	public IEnumerator Adventure()
	{
		GameObject testObject = new GameObject();
		GameObject testObject2 = new GameObject();
		DialogueSystemController dialogueSystemController = testObject2.AddComponent<DialogueSystemController>();
		AdventureScript adventureScript = testObject.AddComponent<AdventureScript>();
		adventureScript.dialogueSystemController = dialogueSystemController;

		//adventureScript calls Init on his special function Start()

		adventureScript.Init();

		while(dialogueSystemController.displaySettings.inputSettings.responseTimeout != 0)
		{
			yield return null;
		}

		Assert.IsTrue(dialogueSystemController.displaySettings.inputSettings.responseTimeout == 0);
	}

	[Test]
	public void Level()
	{
		GameObject testObject = new GameObject();
		FromLevelSO fromLevelSO = ScriptableObject.CreateInstance<FromLevelSO>();
		CurrentLevelSO currentLevelSO = ScriptableObject.CreateInstance<CurrentLevelSO>();
		LevelScript levelScript = testObject.AddComponent<LevelScript>();
		levelScript.fromLevelSO = fromLevelSO;
		levelScript.currentLevelSO = currentLevelSO;
		levelScript.Init();

		Assert.IsTrue(levelScript.fromLevelSO.fromLevel == true && levelScript.currentLevelSO.playerLives == 3);
	}

	[Test]
	public void GameSystem()
	{
		GameObject testObject = new GameObject();
		CurrentLevelSO currentLevelSO = ScriptableObject.CreateInstance<CurrentLevelSO>();
		GameSystemScript gameSystemScript = testObject.AddComponent<GameSystemScript>();
		gameSystemScript.currentLevelSO = currentLevelSO;

		gameSystemScript.resetPlayerCurrentLevel();

		Assert.IsTrue(gameSystemScript.currentLevelSO.playerLives == 3 &&
		gameSystemScript.currentLevelSO.currentLevel == 1 &&
		gameSystemScript.currentLevelSO.totalQuestions == 0 &&
		gameSystemScript.currentLevelSO.correctAnswers == 0 &&
		gameSystemScript.currentLevelSO.timePerQuestion == 0);
	}

	[Test]
	public void SaveSystem()
	{
		GameObject testObject = new GameObject();
		SaveSystemScript saveSystem = testObject.AddComponent<SaveSystemScript>();

		saveSystem.Init();

		Assert.IsTrue(saveSystem.PLAYER_PATH != null && saveSystem.OPTIONS_PATH != null && saveSystem.REMOTE_PATH != null);
	}

	[Test]
	public void PlayFabSystem()
	{
		GameObject testObject = new GameObject();
		PlayFabScript playFabSystem = testObject.AddComponent<PlayFabScript>();
		PlayerSO playerSO = ScriptableObject.CreateInstance<PlayerSO>();
		playFabSystem.player = playerSO;

		playFabSystem.Login();

		Assert.IsFalse(playFabSystem.player.name == null);
	}

	[Test]
	public void Player()
	{
		GameObject testObject = new GameObject();
		FromLevelSO fromLevelSO = ScriptableObject.CreateInstance<FromLevelSO>();
		PlayerControllerScript playerControllerScript = testObject.AddComponent<PlayerControllerScript>();
		Rigidbody2D rb = testObject.AddComponent<Rigidbody2D>();
		BoxCollider2D dialogueArea = testObject.AddComponent<BoxCollider2D>();
		playerControllerScript.fromLevel = fromLevelSO;
		playerControllerScript.rb = rb;
		playerControllerScript.dialogueArea = dialogueArea;
		playerControllerScript.speed = 3f;

		playerControllerScript.movementInput = Vector2.right;
		playerControllerScript.move();

		Assert.IsTrue(playerControllerScript.rb.velocity.x > 0f);
	}

	[Test]
	public void Enemy()
	{
		GameObject testObject = new GameObject();
		EnemyScript enemyScript = testObject.AddComponent<EnemyScript>();

		GameObject testObject2 = new GameObject();
		CurrentLevelSO currentLevelSO = ScriptableObject.CreateInstance<CurrentLevelSO>();
		GameSystemScript gameSystemScript = testObject2.AddComponent<GameSystemScript>();
		DialogueSystemTrigger dialogueSystemTrigger = testObject.AddComponent<DialogueSystemTrigger>();

		gameSystemScript.currentLevelSO = currentLevelSO;
		enemyScript.gameSystem = gameSystemScript;
		enemyScript.colors = new Color[4] {
			new Color(0.91f, 0.36f, 0.31f),
			new Color(0.67f, 0.86f, 0.46f),
			new Color(0.27f, 0.78f, 0.99f),
			new Color(1.00f, 0.88f, 0.45f) };

		gameSystemScript.currentLevelSO.colors = new Color[4];
		gameSystemScript.currentLevelSO.currentZone = 3;//Estadistica

		dialogueSystemTrigger.conversation = "Moda";
		enemyScript.dialogueSystemTrigger = dialogueSystemTrigger;

		enemyScript.setVariables();

		Assert.IsTrue(enemyScript.startQuestion == true);
	}

	[Test]
	public void LevelGenerator()
	{
		GameObject testObject = new GameObject();
		CurrentLevelSO currentLevelSO = ScriptableObject.CreateInstance<CurrentLevelSO>();
		LevelGeneratorScript levelGeneratorScript = testObject.AddComponent<LevelGeneratorScript>();
		levelGeneratorScript.currentLevel = currentLevelSO;
		levelGeneratorScript.Init();

		Assert.IsTrue(testObject != null);
	}
}
