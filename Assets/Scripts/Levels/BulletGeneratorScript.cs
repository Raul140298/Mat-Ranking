using System.Collections;
using UnityEngine;

public class BulletGeneratorScript : MonoBehaviour
{
    EnemyScript enemy;
    public BulletScript[] bullets;
	public bool start;
	public GameObject player;
	public int currentBulletId = 0;
	public Color[] bulletColors;

	public void Start()
	{
		bulletColors = new Color[9] {
			new Color(1.00f, 1.00f, 1.00f),
			new Color(1.00f, 0.48f, 0.20f),
			new Color(0.51f, 0.17f, 0.06f),
			new Color(0.25f, 1.00f, 0.00f),
			new Color(0.44f, 0.58f, 0.21f),
			new Color(0.62f, 1.00f, 0.94f),
			new Color(0.25f, 1.00f, 0.00f),
			new Color(0.62f, 1.00f, 0.94f),
			new Color(0.00f, 0.76f, 1.00f) };
	}

	public void Init(GameObject enemyGO, int nBullets)
	{
		if (start == true)
		{
			start = false;

			Debug.Log("Se preparan las balas");

			//Asign position and enemy who shoot
			this.transform.position = enemyGO.transform.position;
			enemy = enemyGO.GetComponent<EnemyScript>();

			//Offset for the random position
			int offset = Random.Range(0, 360);

			//Use some of the #n of bullets in the array
			//(Object pooling)
			int currenTop = currentBulletId + nBullets;
			for (int i = currentBulletId; i < currenTop; i++)
			{
				int aux = (i >= bullets.Length) ? (i - bullets.Length) : i;

				int a = offset + ((aux - currentBulletId) * 360 / nBullets);

				//Asign color
				bullets[aux].sprite.color = bulletColors[enemy.enemyData.mobId];
				
				//Asign position
				bullets[aux].transform.position = RandomCircle(this.transform.position, 1.0f, a);
				bullets[aux].enemy = enemy;
				bullets[aux].animator.Rebind();

				bullets[aux].gameObject.SetActive(true);

				StartCoroutine(shootBullet(aux, (aux - currentBulletId)/10f, nBullets));
			}

			currentBulletId += nBullets;
			if (currentBulletId >= bullets.Length) currentBulletId -= bullets.Length;
		}
    }

	IEnumerator shootBullet(int aux, float time, int nBullets)
	{
		yield return new WaitForSeconds(time + 0.2f);

		bullets[aux].rb.velocity = (3f + (4f - nBullets)/5 ) * (player.transform.position - bullets[aux].transform.position + new Vector3(0f,0.25f,0f)).normalized;

		yield return new WaitForSeconds(5f);
		bullets[aux].gameObject.SetActive(false);
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
