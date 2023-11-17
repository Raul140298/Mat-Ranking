using UnityEngine;

// PROBANDO PROPIEDADES EN VEZ DE USAR SETTERS/GETTERS
public static class PlayerLevelInfo
{
	public static  int currentLevel{ get; set; }
	public static  int currentZone{ get; set; }
	public static  int playerLives{ get; set; }
	public static  int playerKeyParts{ get; set; }
	public static  int totalQuestions { get; set; }
	public static  int correctAnswers { get; set; }
	public static  int timePerQuestion { get; set; }
	public static  Color[] colors{ get; set; }
	public static  int colorsCount{ get; set; }
	public static  bool heart{ get; set; }
	
	public static bool fromLevel { get; set; }

	public static void ResetLevelInfo()
	{
		currentLevel = 1;
		currentZone = -1;
		playerLives = 3;
		playerKeyParts = 0;
		totalQuestions = 0;
		correctAnswers = 0;
		timePerQuestion = 0;
		colors = new Color[]
			{ new Color(171, 219, 117), new Color(69, 199, 252), new Color(255, 224, 115), new Color(232, 92, 79) };
		colorsCount = 4;
		heart = false;
	}

	public static void SetFromLevel(bool fl)
	{
		fromLevel = fl;
	}

	public static void NextLevel()
	{
		currentLevel++;
	}
}