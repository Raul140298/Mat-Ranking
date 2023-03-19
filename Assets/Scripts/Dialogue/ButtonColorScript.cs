using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonColorScript : MonoBehaviour
{
    [SerializeField] private Image button;
    [SerializeField] private bool active;
    private Color gray;
    private Color[] zoneColors;
    private Color[] colors;

    void Awake()
    {
        button = this.GetComponent<Image>();
        gray = new Color(0.35f, 0.35f, 0.35f);

        zoneColors = new Color[4] {
            new Color(0.56f, 0.42f, 0.19f),
            new Color(0.30f, 0.62f, 0.45f),
            new Color(0.29f, 0.40f, 0.60f),
            new Color(0.41f, 0.26f, 0.52f) };

        colors = new Color[4] {
            new Color(0.91f, 0.36f, 0.31f),
            new Color(0.67f, 0.86f, 0.46f),
            new Color(0.27f, 0.78f, 0.99f),
            new Color(1.00f, 0.88f, 0.45f) };
    }

    void OnEnable()
    {
        if (!this.gameObject.name.StartsWith("Response Button Template"))
        {
            if (SceneManager.GetActiveScene().buildIndex == 2)
            {
                if (GameSystemScript.CurrentLevelSO.colorsCount >= 0 && GameSystemScript.CurrentLevelSO.colorsCount < 4)
                {
                    GameSystemScript.CurrentLevelSO.colorsCount += 1;

                    if (GameSystemScript.CurrentLevelSO.colorsCount == 4)
                    {
                        GameObject[] btns = GameObject.FindGameObjectsWithTag("DialogueButton");
                        for (int k = 0; k < 4; k++)
                        {
                            btns[k].GetComponent<Image>().color = GameSystemScript.CurrentLevelSO.colors[k];
                        }
                    }
                    else
                    {
                        button.color = gray;
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

                if (btns.Length == 5 && (string.Equals(aux, "1") || string.Equals(aux, "2") || string.Equals(aux, "3") || string.Equals(aux, "4")))
                {
                    button.color = zoneColors[int.Parse(aux) - 1];
                }
                else if (btns.Length == 4)
                {
                    for (int k = 0; k < 4; k++)
                    {
                        btns[k].GetComponent<Image>().color = colors[k];
                    }
                }
                else
                {
                    button.color = gray;
                }
            }
        }
    }
}
