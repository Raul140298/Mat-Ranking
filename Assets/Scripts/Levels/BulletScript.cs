using UnityEngine;

public class BulletScript : MonoBehaviour
{
    public SpriteRenderer sprite;
    public Animator animator;
    public Rigidbody2D rb;
    public Canvas canva;
    public EnemyScript enemy;

    public void onClick()
    {
        this.gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "BulletCollisions")
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
			this.gameObject.SetActive(false);
		}
	}
}
