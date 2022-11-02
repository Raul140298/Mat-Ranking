using PixelCrushers.DialogueSystem;
using UnityEngine;
using UnityEngine.UI;

public class TimerScript : MonoBehaviour
{
    public float startingTime, aux;
	public Slider slider;
    private LevelInteractionsScript player;
	public bool finish;
    public StandardUIContinueButtonFastForward continueButton;

	void Start()
    {
        if(!slider) slider = GetComponent<Slider>();
        if(!player) player = GameObject.FindGameObjectWithTag("Player").GetComponent<LevelInteractionsScript>();

		slider.value = 1;
        finish = false;
        Debug.Log("Temporizador Iniciado");
	}

    // Update is called once per frame
    void Update()
    {
        if (aux > 0)
        {
            aux -= Time.deltaTime;
            slider.value = aux / startingTime;
			if(slider.value <= 0.5)
            {
				player.gameSystem.virtualCamera2.ShakeCamera((0.5f - slider.value) * 0.7f, aux);
			}
		}
        //if (aux < 0 && finish == false)
        //{
        //    finish = true;
        //    SoundsScript.PlaySound("POP NEGATIVE");//It could be POP NEUTRAL
        //                                           //DialogueManager.StopConversation();

        //    //player.gameSystem.virtualCamera2.ShakeCamera(0, 0.1f);
        //    player.playerDefeated();
        //}
    }
}
