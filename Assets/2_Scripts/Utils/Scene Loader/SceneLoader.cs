using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    private static SceneLoader instance;

    [Header("SCREEN INFO")]
    [SerializeField]
    private eScreen currentScreen;
    [SerializeField]
    private eScreen previousScreen;
    [SerializeField]
    private eScreen targetScreen; // FOR LOADING

    private AsyncOperation loadingScreenOP;

    void Awake()
    {
        InitializeSingleton();
    }
	
    private void InitializeSingleton()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    public void Initialize()
    {
        GetCurrentScreenFromName();
        previousScreen = eScreen.Splash;
    }

    private void GetCurrentScreenFromName()
    {
        string screenName = SceneManager.GetActiveScene().name;
        currentScreen = (eScreen)Enum.Parse(typeof(eScreen), screenName);
    }

    public void ChangeScreen(eScreen nextScreen, bool asapLoading = true)
    {
        previousScreen = currentScreen;
        currentScreen = nextScreen;
        string nextScreenName = nextScreen.ToString();
        loadingScreenOP = SceneManager.LoadSceneAsync(nextScreenName);
        loadingScreenOP.allowSceneActivation = asapLoading;
    }

    public void AllowScreenChange()
    {
        loadingScreenOP.allowSceneActivation = true;
    }

    public float GetLoadingProgress()
    {
        if (loadingScreenOP == null)
        {
            return 0;
        }

        return loadingScreenOP.progress;
    }

    public bool IsScreenLoadingCompleted()
    {
        if (loadingScreenOP == null)
        {
            return false;
        }

        return loadingScreenOP.isDone;
    }

    public eScreen CurrentScreen => currentScreen;
    public eScreen PreviousScreen => previousScreen;

    public void SetTargetScreen(eScreen targetScreen)
    {
        this.targetScreen = targetScreen;
    }

    public eScreen GetTargetScreen()
    {
        return targetScreen;
    }
}
