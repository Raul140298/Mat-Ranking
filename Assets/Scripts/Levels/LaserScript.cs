using System.Collections;
using UnityEngine;

public class LaserScript : MonoBehaviour
{
    public LineRenderer laser;
    public Material material;

    public void Init(Vector3 end, Color color)
    {
        laser.positionCount = 2;

        if(this.transform.parent.GetComponent<SpriteRenderer>().flipX == true)
        {
			laser.SetPositions(new Vector3[] { this.transform.position + new Vector3(-0.5f, 0f), end + new Vector3(0f, 0.25f) });
		}
        else
        {
			laser.SetPositions(new Vector3[] { this.transform.position + new Vector3(0.5f, 0f), end + new Vector3(0f, 0.25f) });
		}

		material.SetColor("_ColorChangeNewCol", color);

        StartCoroutine(ShootLaser());
    }

   IEnumerator ShootLaser()
    {
        laser.enabled = true;

        yield return new WaitForSeconds(0.5f);

        laser.enabled = false;
    }
}
