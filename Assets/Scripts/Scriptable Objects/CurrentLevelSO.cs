using UnityEngine;

[CreateAssetMenu(fileName = "Current Level", menuName = "Current Level")]
public class CurrentLevelSO : ScriptableObject
{
	public int currentLevel;
	public int currentZone;
	public int totalQuestions, correctAnswers, timePerQuestion;
}
