using System.Collections;
using UnityEngine;

public class BulletGeneratorScript : MonoBehaviour
{
    EnemyScript enemy;
    public BulletScript[] bullets;
	public bool start;

	public void Init(GameObject enemyGO)
	{
		if (start == true)
		{
			start = false;

			this.transform.position = enemyGO.transform.position;

			enemy = enemyGO.GetComponent<EnemyScript>();

			int offset = Random.Range(0, 360);

			for (int i = 0; i < bullets.Length; i++)
			{
				int a = offset + (i * 360 / bullets.Length);

				bullets[i].sprite.color = enemy.colors[i];
				bullets[i].transform.position = RandomCircle(this.transform.position, 1.0f, a);
				if (i == 0) bullets[i].canva.gameObject.SetActive(true);
				bullets[i].enemy = enemy;
				bullets[i].animator.Rebind();
				//bullets[i].animator.Update(0);
				bullets[i].gameObject.SetActive(true);
			}
		}

		this.gameObject.SetActive(true);

		StartCoroutine(shootBullets());
    }

	IEnumerator shootBullets()
	{
		yield return new WaitForSeconds(0.5f);
		ShootBullets();
	}

	public void ShootBullets()
	{
		//for (int i = 0; i < bullets.Length; i++)
		//{
		//	bullets[i].rb.velocity = -3 * (this.transform.position - bullets[i].transform.position).normalized;
		//}
	}

	Vector3 RandomCircle(Vector3 center, float radius, int a)
	{
		float ang = a;
		Vector3 pos;
		pos.x = center.x + radius * Mathf.Sin(ang * Mathf.Deg2Rad);
		pos.y = center.y + radius * Mathf.Cos(ang * Mathf.Deg2Rad);
		pos.z = center.z;
		return pos;
	}
}
