using System;
using UnityEngine;
using UnityEngine.Events;

public class ClickableScript : MonoBehaviour
{
    [Header("INFO")]
    [SerializeField] private RenderingScript compRendering;
    [SerializeField] private GameObject compButton;
    [SerializeField] private UnityEvent clickableAction;

    public void MakeClickable()
    {
        compRendering.OutlineOn();
        compButton.SetActive(true);
    }

    public void MakeNonClickable()
    {
        compRendering.OutlineOff();
        compButton.SetActive(false);
    }

    public void DoClickableAction()
    {
        clickableAction.Invoke();
    }
}
