using UnityEngine;

[CreateAssetMenu(fileName = "Options", menuName = "Options")]
public class OptionsSO : ScriptableObject
{
    public float soundtracksVolume;
    public float soundsVolume;
    public string language;
}