using UnityEngine;

[CreateAssetMenu(fileName = "New Player", menuName ="Player")]
public class PlayerSO : ScriptableObject
{
	public new string name = "";
	public int knowledgePoints;
	public int timePlayed;
	public Vector3 playerPosition = Vector3.zero;
	public int adventureWorldLevel;
	public bool tutorial;
}
