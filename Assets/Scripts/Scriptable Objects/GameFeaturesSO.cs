using System;
using UnityEngine;

[CreateAssetMenu(fileName = "New Game Features", menuName = "Game Features")]
[Serializable]
public class GameFeaturesSO : ScriptableObject
{
    public string level;
}
