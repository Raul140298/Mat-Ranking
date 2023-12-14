using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "New Npc", menuName = "NPC")]
public class NpcSO : SerializedScriptableObject
{
    public int state;
    public string npcName;
    public Dictionary<eAnimation, AnimationClip> animations;
}