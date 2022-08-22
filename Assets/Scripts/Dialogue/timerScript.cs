using PixelCrushers.DialogueSystem;
using UnityEngine;
using UnityEngine.UI;

public class timerScript : MonoBehaviour
{
    public float startingTime, aux;
    public Slider slider;
    public LevelInteractionsScript player;

    void Start()
    {
        if(!slider) slider = GetComponent<Slider>();
        if(!player) player = GameObject.FindGameObjectWithTag("Player").GetComponent<LevelInteractionsScript>();
        slider.value = 1;
        Debug.Log("Start timer");
    }

    // Update is called once per frame
    void Update()
    {
        if (aux > 0)
        {
            aux -= Time.deltaTime;
            slider.value = aux / startingTime;
        }
        if (aux < 0)
		{
            SoundsScript.PlaySound("POP NEUTRAL");//It could be POP NEUTRAL
            DialogueManager.StopConversation();
            player.playerDefeated();
            //this.gameObject.SetActive(false);
            //Dialogue Manager stop question
		}
    }
}
