using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy", menuName = "Enemy")]
public class EnemySO : ScriptableObject
{
    public string[] conversationTitle;
    public int knowledgePoints;
    public float offset;
    public RuntimeAnimatorController animator;
    public bool canPushYou;
}

