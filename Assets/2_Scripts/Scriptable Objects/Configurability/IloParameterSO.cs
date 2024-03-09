using UnityEngine;

[CreateAssetMenu(fileName = "New ILO Parameter", menuName = "ILO Parameter")]
public class IloParameterSO : ScriptableObject
{
    public string label;
    public string description;
    public string parameter_type;
	public int default_value;
	public int min_value;
    public int max_value;
    public bool is_active;
    public string text;
}
