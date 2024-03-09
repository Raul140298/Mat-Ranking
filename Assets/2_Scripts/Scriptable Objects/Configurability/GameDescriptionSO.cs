using UnityEngine;

[CreateAssetMenu(fileName = "New Game Description", menuName = "Game Description")]
public class GameDescriptionSO : ScriptableObject
{
    public string label;
    public string short_description;
    public string long_description;
    public UrlSO[] images;
    public UrlSO[] urls;
}