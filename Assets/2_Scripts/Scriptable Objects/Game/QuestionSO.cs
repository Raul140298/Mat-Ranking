using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[System.Serializable]
public class QuestionsPerDifficult
{
    public string[] questions;
}

[CreateAssetMenu(fileName = "New Question", menuName = "Question")]
public class QuestionSO : SerializedScriptableObject
{
    public Dictionary<eLanguage, QuestionsPerDifficult[]> questionsPerDifficult;
}
