using UnityEngine;

[CreateAssetMenu(fileName = "New ILO", menuName = "ILO")]
public class IloSO : ScriptableObject
{
    public string label;
    public string description;
    public bool selectable;
    public bool selected;
    public IloSO[] ilos;
    public IloParameterSO[] ilo_parameters;
}
