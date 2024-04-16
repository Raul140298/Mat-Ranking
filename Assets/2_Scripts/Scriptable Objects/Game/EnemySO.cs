using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy", menuName = "Enemy")]
public class EnemySO : ScriptableObject
{
    [Header("MODEL")]
    public int mobId = 0;
    public int knowledgePoints;
    public int hp;
    public float velocity;
    public float offset;
    public AnimationData animationData;

    [Header("QUESTIONS")]
    public QuestionSO questions;
    public IloSO configurations;
}

