using PixelCrushers.DialogueSystem;
using UnityEngine;
using UnityEngine.UI;

public class TimerScript : MonoBehaviour
{
    public float startingTime, aux;
    private Slider slider;
    private LevelInteractionsScript player;
    private bool finish;

    void Start()
    {
        if(!slider) slider = GetComponent<Slider>();
        if(!player) player = GameObject.FindGameObjectWithTag("Player").GetComponent<LevelInteractionsScript>();
        slider.value = 1;
        finish = false;
        Debug.Log("Start timer");
    }

    // Update is called once per frame
    void Update()
    {
        if (aux > 0 && finish == false)
        {
            aux -= Time.deltaTime;
            slider.value = aux / startingTime;
        }
        if (aux < 0 && finish == false)
		{
            finish = true;
            SoundsScript.PlaySound("POP NEUTRAL");//It could be POP NEUTRAL
            DialogueManager.StopConversation();
            player.playerDefeated();
            //this.gameObject.SetActive(false);
            //Dialogue Manager stop question
		}
    }
}
