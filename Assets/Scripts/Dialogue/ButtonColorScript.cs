using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonColorScript : MonoBehaviour
{
    public Image button;
    public GameSystemScript gameSystem;
	public bool active;

    void Awake()
    {
        button = this.GetComponent<Image>();
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
					button.color = Color.white;
				}
			}
			else
			{
				button.color = Color.white;
			}
		}
	}
}
