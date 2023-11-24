using UnityEngine;
using UnityEngine.UI;

public class MainMenuController : SceneController
{
    [Header("MAIN MENU")]
    [SerializeField] private Text version;
    
    [Header("BACKGROUND")]
    [SerializeField] private RawImage background;
    [SerializeField] private float velocity;

    private void Start()
    {
        GooglePlayManager.Authenticate();
        
        GameManager.StartSounds(base.soundsSlider);
        GameManager.StartSoundtracks(base.soundtracksSlider);

        SoundtracksManager.PlaySoundtrack("GARDEN OF MATH");
        
        version.text = Application.version;
        
        PlayerLevelInfo.SetFromLevel(false);
        
        /*switch (Application.systemLanguage)
        {
            case SystemLanguage.Spanish:
                DialogueManager.SetLanguage("es");
                break;
            case SystemLanguage.English:
                DialogueManager.SetLanguage("en");
                break;
            default:
                DialogueManager.SetLanguage("en");
                break;
        }*/
    }

    private void Update()
    {
        Vector2 position = background.uvRect.position + new Vector2(velocity, velocity) * Time.deltaTime;
        Vector2 size = background.uvRect.size;

        background.uvRect = new Rect(position, size);
    }
}