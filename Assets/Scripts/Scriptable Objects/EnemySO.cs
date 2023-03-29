using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy", menuName = "Enemy")]
public class EnemySO : ScriptableObject
{
    public int mobId = 0;
    public string[] conversationTitle;
    public int knowledgePoints;
    public int hp;
    public float offset;
    public RuntimeAnimatorController animator;
    public bool canPushYou;
    public IloSO configurations;
}

