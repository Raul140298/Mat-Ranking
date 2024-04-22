using UnityEngine;
using UnityEngine.UI;

public class KnobScript : MonoBehaviour
{
    [SerializeField] private RectTransform knob;
    [SerializeField] private Button knobBtn;
    [SerializeField] private Image[] knobImages;

    public void ResetKnob()
    {
        knob.localPosition = Vector3.zero;
        MakeKnobNormal();
    }
    
    public void MakeKnobTransparent()
    {
        foreach (Image i in knobImages)
        {
            i.color = new Color(1, 1, 1, 0.3f);
        }
    }

    public void MakeKnobNormal()
    {
        foreach (Image i in knobImages)
        {
            i.color = new Color(1, 1, 1, 1);
        }
    }
}
