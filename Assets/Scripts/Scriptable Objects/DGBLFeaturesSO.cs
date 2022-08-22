using UnityEngine;

[CreateAssetMenu(fileName = "New DGBL Features", menuName = "DGBL Features")]
public class DGBLFeaturesSO : ScriptableObject
{
    public AreaSO[] learning_AreaSOs;
    public IloSO[] ilos;
}
