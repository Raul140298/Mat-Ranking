using UnityEngine;
using UnityEngine.UI;

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
    }

    private void Update()
    {
        background.uvRect = new Rect(background.uvRect.position + new Vector2(velocity, velocity) * Time.deltaTime, background.uvRect.size);
    }
}