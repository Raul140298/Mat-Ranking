using UnityEngine;
using UnityEngine.UI;

public class MainMenuScript : SceneScript
{
    [Header("MAIN MENU")]
    [SerializeField] private Text version;
    [SerializeField] private RawImage background;
    [SerializeField] private float velocity;

    private void Start()
    {
        GameSystemScript.StartSounds(base.soundsSlider);
        GameSystemScript.StartSoundtracks(base.soundtracksSlider);

        GameSystemScript.SaveSystem.LoadOptions(); //Sounds and soundtracks need to be started before this

        SoundtracksScript.PlaySoundtrack("GARDEN OF MATH");

        GameSystemScript.FromLevelSO.fromLevel = false;
        version.text = Application.version;
    }

    private void Update()
    {
        Vector2 position = background.uvRect.position + new Vector2(velocity, velocity) * Time.deltaTime;
        Vector2 size = background.uvRect.size;

        background.uvRect = new Rect(position, size);
    }
}