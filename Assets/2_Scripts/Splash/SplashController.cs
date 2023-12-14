using System.Collections;
using UnityEngine;

public class SplashController : MonoBehaviour
{
    public GameObject goLogo;
	
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(ExecuteLogoSequence());
    }

    private IEnumerator ExecuteLogoSequence()
    { 
		goLogo.HideSprite();
        
        SceneLoader.Instance.ChangeScreen(eScreen.MainMenu, false);

		yield return new WaitForSeconds(1.5f);
        
        goLogo.ShowSprite();
        
        yield return new WaitForSeconds(0.5f);


        goLogo.HideSprite();

		yield return new WaitForSeconds(0.5f);

        SceneLoader.Instance.AllowScreenChange();
    }
}
