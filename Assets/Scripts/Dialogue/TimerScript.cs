using PixelCrushers.DialogueSystem;
using UnityEngine;
using UnityEngine.UI;

public class TimerScript : MonoBehaviour
{
    [SerializeField] private float startingTime, aux;
    [SerializeField] private Slider slider;
    [SerializeField] private bool finish;
    [SerializeField] private StandardUIContinueButtonFastForward continueButton;
    private LevelInteractionsScript player;

    void Start()
    {
        if (!slider) slider = GetComponent<Slider>();
        if (!player) player = GameObject.FindGameObjectWithTag("Player").GetComponent<LevelInteractionsScript>();

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
            if (slider.value <= 0.5)
            {
                player.GameSystem.VirtualCamera2.ShakeCamera((0.5f - slider.value) * 0.7f, aux);
            }
        }
    }

    public Slider Slider
    {
        get { return slider; }
        set { slider = value; }
    }

    public float StartingTime
    {
        get { return startingTime; }
        set { startingTime = value; }
    }

    public float Aux
    {
        get { return aux; }
        set { aux = value; }
    }

    public bool Finish
    {
        get { return finish; }
        set { finish = value; }
    }
}
