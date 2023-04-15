using System.Collections;
using UnityEngine;

public class BulletGeneratorScript : MonoBehaviour
{
    [SerializeField] private bool start;
    private Color[] bulletColors;

    private EnemyScript enemy;

    public void Awake()
    {
        bulletColors = new Color[9] {
            new Color(1.00f, 1.00f, 1.00f),
            new Color(1.00f, 0.48f, 0.24f),
            new Color(0.44f, 0.18f, 0.09f),
            new Color(0.60f, 1.00f, 0.21f),
            new Color(0.48f, 0.57f, 0.25f),
            new Color(0.73f, 0.83f, 0.96f),
            new Color(1.00f, 0.88f, 0.12f),
            new Color(0.75f, 1.00f, 0.94f),
            new Color(0.24f, 0.76f, 1.00f) };
    }

    public void Init(EnemyScript enemy, int nBullets)
    {
        if (start == true)
        {
            start = false;

            Debug.Log("Se preparan las balas");

            //Offset for the random position
            int offset = Random.Range(0, 360);

            //Use some of the #n of bullets in the array
            //(Object pooling)
            for (int i = 0; i < nBullets; i++)
            {
                int a = offset + (i * 360 / nBullets);

                BulletScript bullet = LevelScript.Instance.BulletPooler.GetObject().GetComponent<BulletScript>();

                bullet.GetComponent<Poolable>().Activate();

                //Asign color
                bullet.Sprite.color = bulletColors[enemy.EnemyData.mobId];

                //Asign position
                bullet.transform.position = MathHelper.RandomCircle(this.transform.position, 1.0f, a);
                bullet.Enemy = enemy;
                bullet.Animator.Rebind();

                StartCoroutine(CRTShootBullet(bullet, 0.8f));
            }
        }
    }

    IEnumerator CRTShootBullet(BulletScript bullet, float time)
    {
        yield return new WaitForSeconds(time);

        bullet.Rb.velocity = 6f * (LevelScript.Instance.Player.transform.position - bullet.transform.position + new Vector3(0f, 0.25f, 0f)).normalized;

        yield return new WaitForSeconds(5f);

        bullet.GetComponent<Poolable>().Deactivate();
    }

    public bool StartBullets
    {
        get { return start; }
        set { start = value; }
    }
}
