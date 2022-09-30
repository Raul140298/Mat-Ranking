using Cinemachine;
using UnityEngine;
using UnityEngine.UI;

public class OrientationScript : MonoBehaviour
{
    public Camera cam;
    public CinemachineVirtualCamera cmCam, cmCam2;
    public CanvasScaler canvas, dialogueCanvas;
    public RectTransform soundtracks, sounds, credits, mainTitle, ranking, header, summary;
    public RectTransform rectTransform;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        adjustComponents();
    }
    void OnRectTransformDimensionsChange()
    {
        adjustComponents();
    }

    //Method for adjust different components like UI, Camera Size, Fonts and buttons to fits the orientation of the mobile.
    private void adjustComponents()
    {
        if (rectTransform == null) return;

        if (mainTitle)
        {
            float aux = (float)rectTransform.rect.height / 2f - ((float)rectTransform.rect.height - 80f) / 4f;
            RectTransformExtensionsScript.SetPosY(mainTitle, aux);
        }

        if (rectTransform.rect.width < rectTransform.rect.height)
        {
            if (credits) RectTransformExtensionsScript.SetPosY(credits, -178.4087f);

            if (cmCam) cmCam.m_Lens.OrthographicSize = 6f;
            if (cmCam2) cmCam2.m_Lens.OrthographicSize = 6f;
            if (cam) cam.orthographicSize = 6f;

            if (canvas) canvas.matchWidthOrHeight = 0.5f;
            if (dialogueCanvas) dialogueCanvas.matchWidthOrHeight = 0.5f;

            RectTransformExtensionsScript.SetLeft(soundtracks, 40);
            RectTransformExtensionsScript.SetRight(soundtracks, 40);
            RectTransformExtensionsScript.SetLeft(sounds, 40);
            RectTransformExtensionsScript.SetRight(sounds, 40);
            if(ranking && header)
			{
                RectTransformExtensionsScript.SetLeft(header, 40);
                RectTransformExtensionsScript.SetRight(header, 40);

                RectTransformExtensionsScript.SetLeft(ranking, 40);
                RectTransformExtensionsScript.SetRight(ranking, 40);
            }

            if(summary)
			{
                RectTransformExtensionsScript.SetLeft(summary, 40);
                RectTransformExtensionsScript.SetRight(summary, 40);
            }
        }
        else
        {
            if (credits) RectTransformExtensionsScript.SetPosY(credits, -651.3242f);

            if (cmCam) cmCam.m_Lens.OrthographicSize = 3f;
            if (cmCam2) cmCam2.m_Lens.OrthographicSize = 3f;
            if (cam) cam.orthographicSize = 3f;

            if (canvas) canvas.matchWidthOrHeight = 1f;
            if (dialogueCanvas) dialogueCanvas.matchWidthOrHeight = 1f;

            RectTransformExtensionsScript.SetLeft(soundtracks, 500);
            RectTransformExtensionsScript.SetRight(soundtracks, 500);
            RectTransformExtensionsScript.SetLeft(sounds, 500);
            RectTransformExtensionsScript.SetRight(sounds, 500);

            if(ranking && header)
			{
                RectTransformExtensionsScript.SetLeft(header, 500);
                RectTransformExtensionsScript.SetRight(header, 500);

                RectTransformExtensionsScript.SetLeft(ranking, 500);
                RectTransformExtensionsScript.SetRight(ranking, 500);
            }

            if (summary)
            {
                RectTransformExtensionsScript.SetLeft(summary, 500);
                RectTransformExtensionsScript.SetRight(summary, 500);
            }
        }
    }
}

