using Cinemachine;
using UnityEngine;

public class DialogueCameraScript : MonoBehaviour
{
    public GameObject player, target;
    public Vector3 offset;
    public CinemachineVirtualCamera vcam2;

    // Update is called once per frame
    void LateUpdate()
    {
        if (target == null) return;

        Vector3 centerPoint = GetCenterPoint();

        Vector3 newPosition = centerPoint + offset;

        this.transform.position = centerPoint;
    }

    Vector3 GetCenterPoint()
	{
        if (target)
		{
            var bounds = new Bounds(player.transform.position, Vector3.zero);
            bounds.Encapsulate(target.transform.position);
            return bounds.center;
		}
		else
		{
            return Vector3.zero;
		}
	}

    public void StartDialogue()
	{
        if(target != null) vcam2.Priority = 20;
    }

    public void EndDialogue()
	{
        if (target != null) vcam2.Priority = 0;
    }
}
