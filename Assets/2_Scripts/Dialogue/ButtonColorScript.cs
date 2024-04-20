using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonColorScript : MonoBehaviour
{
    [SerializeField] private Image button;
    private Color gray;
    private Color[] zoneColors;
    private Color[] colors;

    void Awake()
    {
        button = this.GetComponent<Image>();
        gray = new Color(0.35f, 0.35f, 0.35f);
        zoneColors = WorldValues.DEFAULT_ZONE_COLORS;
        colors = WorldValues.DEFAULT_BTN_COLORS;
    }

    void OnEnable()
    {
        if (!this.gameObject.name.StartsWith("Response Button Template") && 
            SceneManager.GetActiveScene().buildIndex == 3)
        {
            if (PlayerLevelInfo.colorsCount >= 0 && PlayerLevelInfo.colorsCount < 4)
            {
                PlayerLevelInfo.colorsCount += 1;

                if (PlayerLevelInfo.colorsCount == 4)
                {
                    GameObject[] btns = GameObject.FindGameObjectsWithTag("DialogueButton");
                    for (int k = 0; k < 4; k++)
                    {
                        btns[k].GetComponent<Image>().color = PlayerLevelInfo.colors[k];
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
