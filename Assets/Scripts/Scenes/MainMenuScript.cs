using UnityEngine;
using UnityEngine.UI;

public class MainMenuScript : SceneScript
{
    [SerializeField] private Text version;

    private void Start()
    {
        GameSystemScript.FromLevelSO.fromLevel = false;
        GameSystemScript.StartSounds(SoundsSlider);
        GameSystemScript.StartSoundtracks(SoundtracksSlider);
        GameSystemScript.SaveSystem.LoadOptions();

        version.text = Application.version;
        SoundtracksScript.PlaySoundtrack("GARDEN OF MATH");
    }
}