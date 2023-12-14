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
            Feedback.Do(eFeedbackType.Hit);

            this.gameObject.SetActive(false);

            enemy.HitPlayer(this.gameObject);
        }

        if (collision.tag == "LevelCollisions")
        {
            //this.gameObject.SetActive(false);
        }

        if (collision.tag == "NextLevel")
        {
            Feedback.Do(eFeedbackType.Hit);

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
