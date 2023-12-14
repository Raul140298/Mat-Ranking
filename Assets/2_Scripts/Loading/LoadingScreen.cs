using UnityEngine;

public class LoadingScreen : MonoBehaviour
{
    [SerializeField] private TextAsset textJSON;
    
    void Awake()
    {
        Time.timeScale = 1;
    }

    void Start()
    {
        eScreen targetScreen = SceneLoader.Instance.GetTargetScreen();
        SceneLoader.Instance.ChangeScreen(targetScreen, false);
        Invoke("AllowScreenChange", 1);
    }

    void AllowScreenChange()
    {
        SceneLoader.Instance.AllowScreenChange();
    }
}
