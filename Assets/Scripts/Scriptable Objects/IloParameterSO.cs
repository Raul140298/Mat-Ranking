using UnityEngine;

[CreateAssetMenu(fileName = "New ILO Parameter", menuName = "ILO Parameter")]
public class IloParameterSO : ScriptableObject
{
    public string label;
    public string description;
    public int default_value;
    public string parameter_type;
    public int min_value;
    public int max_value;
}
