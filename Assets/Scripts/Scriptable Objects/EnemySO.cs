using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy", menuName = "Enemy")]
public class EnemySO : ScriptableObject
{
    [Header("MODEL")]
    public int mobId = 0;
    public int knowledgePoints;
    public int hp;
    public float offset;
    public RuntimeAnimatorController animator;

    [Header("QUESTIONS")]
    public QuestionSO[] questions;
    public IloSO configurations;
}

