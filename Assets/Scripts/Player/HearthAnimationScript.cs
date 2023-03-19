using UnityEngine;

public class HearthAnimationScript : MonoBehaviour
{
    [SerializeField] private float origin, amplitude, position, velocity, angle, anglePosition;
    private RectTransform rt;
    private float timer;

    void Start()
    {
        rt = GetComponent<RectTransform>();
    }

    private void Update()
    {
        timer += Time.deltaTime;

        rt.SetPosY(origin + amplitude * Mathf.Sin(position * Mathf.PI + timer * velocity));

        rt.SetRotationZ(angle * Mathf.Sin(anglePosition * Mathf.PI + timer * velocity));
    }
}
