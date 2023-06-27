using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Google Play", menuName = "Google Play")]
public class GooglePlaySO : SerializedScriptableObject
{
    public string ranking;
    public Dictionary<eAchievements, string> achievements;
}
