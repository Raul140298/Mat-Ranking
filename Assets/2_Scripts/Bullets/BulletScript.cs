using UnityEngine;
using UnityEngine.Serialization;

public class BulletScript : MonoBehaviour
{
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private Animator animator;
    [SerializeField] private Rigidbody2D rb;
    [FormerlySerializedAs("enemy")] [SerializeField] private EnemyModelScript enemyModel;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "BulletCollisions")
        {
            Feedback.Do(eFeedbackType.Hit);

            this.gameObject.SetActive(false);

            enemyModel.HitPlayer(this.gameObject);
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

    public EnemyModelScript EnemyModel
    {
        get { return enemyModel; }

        set { enemyModel = value; }
    }
}
