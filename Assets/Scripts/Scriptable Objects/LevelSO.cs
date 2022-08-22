using UnityEngine;

[CreateAssetMenu(fileName = "New Level", menuName = "Level")]
public class LevelSO : ScriptableObject
{
    public int scoreLastAttempt;
    public int numberAttempts;
    public int timeLastAttempt;
}
