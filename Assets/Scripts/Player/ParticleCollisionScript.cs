using System.Collections.Generic;
using UnityEngine;

public class ParticleCollisionScript : MonoBehaviour
{
    public List<ParticleCollisionEvent> eventCol = new List<ParticleCollisionEvent>();

    private void OnParticleCollision(GameObject other)
    {
        int events = this.GetComponent<ParticleSystem>().GetCollisionEvents(other, eventCol);

        for (int i = 0; i < events; i++)
        {
            //this.transform.parent.GetComponent<enemyScript>().saveSystem.changeKnowledgePoints(1);
            SoundsScript.PlaySound("WIN POINTS");
        }
    }
}
