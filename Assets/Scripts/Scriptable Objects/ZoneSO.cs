using UnityEngine;

[CreateAssetMenu(fileName = "New Zone", menuName = "Zone")]
public class ZoneSO : ScriptableObject
{
    public string title;
    public string summary;
    public bool available;
    public int difficulty;
    public LevelSO[] LevelSO;
}
