using UnityEngine;

public class BulletScript : MonoBehaviour
{
    public SpriteRenderer sprite;
    public Rigidbody2D rb;
    public Canvas canva;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void onClick()
    {
        this.gameObject.SetActive(false);
    }
}
