using UnityEngine;

public class AudioManager : MonoBehaviour
{
    void Awake()
    {
        bool exist = GameObject.FindGameObjectsWithTag("Audio").Length > 1;

        if (exist)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            DontDestroyOnLoad(this);
        }
    }
}
