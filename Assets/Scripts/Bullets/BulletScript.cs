using UnityEngine;

public class BulletScript : MonoBehaviour
{
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private Animator animator;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private EnemyScript enemy;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "BulletCollisions")
        {
            SoundsScript.PlaySound("HIT");

            this.gameObject.SetActive(false);

            enemy.hitPlayer(this.gameObject);
        }

        if (collision.tag == "LevelCollisions")
        {
            //this.gameObject.SetActive(false);
        }

        if (collision.tag == "NextLevel")
        {
            SoundsScript.PlaySound("HIT");

            this.gameObject.SetActive(false);
        }
    }

    public SpriteRenderer Sprite
    {
        get { return sprite; }

        set { sprite = value; }
    }

    public Animator Animator
    {
        get { return animator; }

        set { animator = value; }
    }

    public Rigidbody2D Rb
    {
        get { return rb; }

        set { rb = value; }
    }

    public EnemyScript Enemy
    {
        get { return enemy; }

        set { enemy = value; }
    }
}