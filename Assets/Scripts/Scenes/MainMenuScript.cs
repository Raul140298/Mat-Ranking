using UnityEngine;
using UnityEngine.UI;
/* using PixelCrushers.DialogueSystem; */

public class MainMenuScript : SceneScript
{
    [SerializeField] private Text version;
    [SerializeField] private RawImage background;
    [SerializeField] private float velocity;

    private void Start()
    {
        GameSystemScript.FromLevelSO.fromLevel = false;
        GameSystemScript.StartSounds(SoundsSlider);
        GameSystemScript.StartSoundtracks(SoundtracksSlider);
        GameSystemScript.SaveSystem.LoadOptions();

        version.text = Application.version;
        SoundtracksScript.PlaySoundtrack("GARDEN OF MATH");

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