using UnityEngine;

public class KnobResetScript : MonoBehaviour
{
    [SerializeField] private RectTransform knob;

    public void ResetKnob()
    {
        knob.localPosition = Vector3.zero;
    }
}
