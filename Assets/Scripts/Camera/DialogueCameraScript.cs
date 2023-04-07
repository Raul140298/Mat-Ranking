using Cinemachine;
using UnityEngine;

public class DialogueCameraScript : MonoBehaviour
{
    [SerializeField] private GameObject player, target;
    [SerializeField] private Vector3 offset;
    [SerializeField] private CinemachineVirtualCamera vcam2;

    // Update is called once per frame
    void LateUpdate()
    {
        if (target == null) return;

        Vector3 centerPoint = GetCenterPoint();

        this.transform.position = centerPoint;
    }

    Vector3 GetCenterPoint()
    {
        var bounds = new Bounds(player.transform.position, Vector3.zero);
        bounds.Encapsulate(target.transform.position);
        return bounds.center;
    }

    public void StartDialogue(GameObject target = null)
    {
        this.target = target;
        vcam2.Priority = 20;
    }

    public void EndDialogue()
    {
        vcam2.Priority = 0;
    }

    public GameObject Target
    {
        get { return target; }
        set { target = value; }
    }
}
