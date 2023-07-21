using Cinemachine;
using UnityEngine;
using UnityEngine.UI;

public class OrientationScript : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private CinemachineVirtualCamera cmCam;
    [SerializeField] private CinemachineVirtualCamera cmCam2;
    [SerializeField] private CanvasScaler canvas;
    [SerializeField] private CanvasScaler dialogueCanvas;
    [SerializeField] private RectTransform soundtracks;
    [SerializeField] private RectTransform sounds;

    [SerializeField] private RectTransform mainTitle;

    [SerializeField] private RectTransform summary;
    [SerializeField] private RectTransform rectTransform;

    void Awake()
    {
        AdjustComponents();
    }

    void OnRectTransformDimensionsChange()
    {
        AdjustComponents();
    }

    //Method for adjust different components like UI, Camera Size, Fonts and buttons to fits the orientation of the mobile.
    private void AdjustComponents()
    {
        if (rectTransform == null) return;

        if (mainTitle)
        {
            float aux = (float)rectTransform.rect.height / 2f - ((float)rectTransform.rect.height - 80f) / 4f;
            mainTitle.SetPosY(aux);
        }

        if (rectTransform.rect.width < rectTransform.rect.height)
        {
            if (cmCam) cmCam.m_Lens.OrthographicSize = 6f;
            if (cmCam2) cmCam2.m_Lens.OrthographicSize = 6f;
            if (cam) cam.orthographicSize = 6f;

            if (canvas) canvas.matchWidthOrHeight = 0.5f;
            if (dialogueCanvas) dialogueCanvas.matchWidthOrHeight = 0.5f;

            soundtracks.SetLeft(40);
            soundtracks.SetRight(40);
            sounds.SetLeft(40);
            sounds.SetRight(40);

            if (summary)
            {
                summary.SetLeft(40);
                summary.SetRight(40);
            }
        }
        else
        {
            if (cmCam) cmCam.m_Lens.OrthographicSize = 3f;
            if (cmCam2) cmCam2.m_Lens.OrthographicSize = 3f;
            if (cam) cam.orthographicSize = 3f;

            if (canvas) canvas.matchWidthOrHeight = 1f;
            if (dialogueCanvas) dialogueCanvas.matchWidthOrHeight = 1f;

            soundtracks.SetLeft(500);
            soundtracks.SetRight(500);
            sounds.SetLeft(500);
            sounds.SetRight(500);

            if (summary)
            {
                summary.SetLeft(500);
                summary.SetRight(500);
            }
        }
    }
}

