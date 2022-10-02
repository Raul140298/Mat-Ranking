using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Current Level", menuName = "Current Level")]
public class CurrentLevelSO : ScriptableObject
{
	public int currentLevel;
	public int currentZone;
	public int playerLives;
	public int playerKeyParts;
	public int totalQuestions, correctAnswers, timePerQuestion;
	public Color[] colors;
	public int colorsCount;
}
