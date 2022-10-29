using System;
using UnityEngine;

[CreateAssetMenu(fileName = "New Remote Data", menuName = "Remote Data")]
[Serializable]
public class RemoteSO : ScriptableObject
{
    public GameDescriptionSO game_description;
    public DGBLFeaturesSO dgbl_features;
}
