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
		
#if UNITY_EDITOR
        PlayerSessionInfo.soundsVolume = 1;
		PlayerSessionInfo.soundtracksVolume = 1;
        PlayerSessionInfo.playerPosition = new Vector3(300, -30, 0);
        
        PlayerLevelInfo.ResetLevelInfo();
#endif
        
        GameManager.StartSounds(base.soundsSlider);
        GameManager.StartSoundtracks(base.soundtracksSlider);
        
        PlayerLevelInfo.SetFromLevel(false);
        
        version.text = Application.version;
        
        SoundtracksManager.PlaySoundtrack("GARDEN OF MATH");
    }

    private void Update()
    {
        Vector2 position = background.uvRect.position + new Vector2(velocity, velocity) * Time.deltaTime;
        Vector2 size = background.uvRect.size;

        background.uvRect = new Rect(position, size);
    }
}