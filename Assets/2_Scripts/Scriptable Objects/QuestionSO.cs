using UnityEngine;

[System.Serializable]
public class QuestionsPerDifficult
{
    public string[] questions;
}

[CreateAssetMenu(fileName = "New Question", menuName = "Question")]
public class QuestionSO : ScriptableObject
{
    public QuestionsPerDifficult[] questionES;
    public QuestionsPerDifficult[] questionEN;
}
