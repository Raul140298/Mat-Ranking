using UnityEngine;

public class KnobResetScript : MonoBehaviour
{
    [SerializeField] private RectTransform knob;

    public void resetKnob()
    {
        knob.localPosition = Vector3.zero;
    }
}
