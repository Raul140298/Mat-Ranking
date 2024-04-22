using Cinemachine;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private GameObject target1, target2;
    private Vector3 offset;
    [SerializeField] private CinemachineVirtualCamera vcam2;

    // Update is called once per frame
    void LateUpdate()
    {
        if (target1 == null && target2 == null) return;

        Vector3 centerPoint = GetCenterPoint();
        this.transform.position = centerPoint;
    }

    Vector3 GetCenterPoint()
    {
        if (target2 == null)
        {
            return target1.transform.position;
        }
        
        var bounds = new Bounds(target1.transform.position, Vector3.zero);
        bounds.Encapsulate(target2.transform.position);
        return bounds.center;
    }
    
    public void StartDialogue(GameObject target1, GameObject target2 = null)
    {
        this.target1 = target1;
        if (target2 != null) this.target2 = target2;
        vcam2.Priority = 20;
    }

    public void EndDialogue()
    {
        target1 = null;
        target2 = null;
        vcam2.Priority = 0;
    }
}
