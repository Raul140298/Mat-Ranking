using UnityEngine;
using UnityEngine.UI;
/* using PixelCrushers.DialogueSystem; */

public class MainMenuScript : SceneScript
{
    [Header("MAIN MENU")]
    [SerializeField] private Text version;
    [SerializeField] private RawImage background;
    [SerializeField] private float velocity;

    private void Start()
    {
        GameSystemScript.StartSounds(SoundsSlider);
        GameSystemScript.StartSoundtracks(SoundtracksSlider);

        GameSystemScript.SaveSystem.LoadOptions(); //Sounds and soundtracks need to be started before this

        SoundtracksScript.PlaySoundtrack("GARDEN OF MATH");

        GameSystemScript.FromLevelSO.fromLevel = false;
        version.text = Application.version;

        /* DialogueLua.SetVariable("IntroConversationState", 0);
        DialogueLua.SetVariable("SofiaConversationState", 0);
        DialogueLua.SetVariable("BradConversationState", 0);
        DialogueLua.SetVariable("DanielConversationState", 0);
        DialogueLua.SetVariable("ValentinConversationState", 0);
        DialogueLua.SetVariable("MarianaConversationState", 0);
        DialogueLua.SetVariable("AlexConversationState", 0);
        DialogueLua.SetVariable("IreneConversationState", 0); */
    }

    private void Update()
    {
        background.uvRect = new Rect(background.uvRect.position + new Vector2(velocity, velocity) * Time.deltaTime, background.uvRect.size);
    }
}