using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnobResetScript : MonoBehaviour
{
    public void resetKnob()
	{
		this.gameObject.GetComponent<RectTransform>().localPosition = Vector3.zero;
	}
}
