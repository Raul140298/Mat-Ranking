using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy", menuName = "Enemy")]
public class EnemySO : ScriptableObject
{
    public string[] conversationTitle;
    public int knowledgePoints;
    public float offsetX, offsetY;
    public GameObject animator;
    public bool canPushYou;
    public IloSO configurations;
}

