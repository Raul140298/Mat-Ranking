using UnityEngine;

[CreateAssetMenu(fileName = "New Npc", menuName = "NPC")]
public class NpcSO : ScriptableObject
{
    public int state;
    public string npcName;
    public RuntimeAnimatorController animator;
}