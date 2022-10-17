using PixelCrushers.DialogueSystem;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonColorScript : MonoBehaviour
{
    public Image button;
    public GameSystemScript gameSystem;
	public bool active;
	private Color gray;
	private Color[] zoneColors;

    void Awake()
    {
        button = this.GetComponent<Image>();
		gray = new Color(0.35f, 0.35f, 0.35f);
		zoneColors = new Color[4] {
			new Color(0.56f, 0.42f, 0.19f),
			new Color(0.30f, 0.62f, 0.45f),
			new Color(0.29f, 0.40f, 0.60f),
			new Color(0.41f, 0.26f, 0.52f) };
	}

	void OnEnable()
	{
		if (!this.gameObject.name.StartsWith("Response Button Template"))
		{
			if (SceneManager.GetActiveScene().buildIndex == 2)
			{
				gameSystem = GameObject.FindGameObjectWithTag("GameSystem").GetComponent<GameSystemScript>();

				if (gameSystem.currentLevelSO.colorsCount >= 0 && gameSystem.currentLevelSO.colorsCount < 4)
				{
					gameSystem.currentLevelSO.colorsCount += 1;

					if (gameSystem.currentLevelSO.colorsCount == 4)
					{
						GameObject[] btns = GameObject.FindGameObjectsWithTag("DialogueButton");
						for (int k = 0; k < 4; k++)
						{
							btns[k].GetComponent<Image>().color = gameSystem.currentLevelSO.colors[k];
						}
					}
				}
				else //It's a next level
				{
					button.color = gray;
				}
			}
			else
			{
				GameObject[] btns = GameObject.FindGameObjectsWithTag("DialogueButton");

				string aux = transform.GetChild(0).GetComponent<Text>().text;

				if (btns.Length == 5 && ( string.Equals(aux, "1") || string.Equals(aux, "2") || string.Equals(aux, "3") || string.Equals(aux, "4")))
				{
					button.color = zoneColors[int.Parse(aux) - 1];
				}
				else
				{
					button.color = gray;
				}
			}
		}
	}
}
