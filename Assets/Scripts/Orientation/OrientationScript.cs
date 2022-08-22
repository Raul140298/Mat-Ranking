using Cinemachine;
using UnityEngine;
using UnityEngine.UI;

public class OrientationScript : MonoBehaviour
{
    // This event will only be called when an orientation changed (i.e. won't be call at lanch)
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
            RectTransformExtensions.SetPosY(mainTitle, aux);
        }

        if (rectTransform.rect.width < rectTransform.rect.height)
        {
            if (credits) RectTransformExtensions.SetPosY(credits, 0f);

            if (cmCam) cmCam.m_Lens.OrthographicSize = 6f;
            if (cmCam2) cmCam2.m_Lens.OrthographicSize = 6f;
            if (cam) cam.orthographicSize = 6f;

            if (canvas) canvas.matchWidthOrHeight = 0.5f;
            if (dialogueCanvas) dialogueCanvas.matchWidthOrHeight = 0.5f;

            RectTransformExtensions.SetLeft(soundtracks, 40);
            RectTransformExtensions.SetRight(soundtracks, 40);
            RectTransformExtensions.SetLeft(sounds, 40);
            RectTransformExtensions.SetRight(sounds, 40);
            if(ranking && header)
			{
                RectTransformExtensions.SetLeft(header, 40);
                RectTransformExtensions.SetRight(header, 40);

                RectTransformExtensions.SetLeft(ranking, 40);
                RectTransformExtensions.SetRight(ranking, 40);
            }

            if(summary)
			{
                RectTransformExtensions.SetLeft(summary, 40);
                RectTransformExtensions.SetRight(summary, 40);
            }
        }
        else
        {
            if (credits) RectTransformExtensions.SetPosY(credits, -325.3171f);

            if (cmCam) cmCam.m_Lens.OrthographicSize = 3f;
            if (cmCam2) cmCam2.m_Lens.OrthographicSize = 3f;
            if (cam) cam.orthographicSize = 3f;

            if (canvas) canvas.matchWidthOrHeight = 1f;
            if (dialogueCanvas) dialogueCanvas.matchWidthOrHeight = 1f;

            RectTransformExtensions.SetLeft(soundtracks, 500);
            RectTransformExtensions.SetRight(soundtracks, 500);
            RectTransformExtensions.SetLeft(sounds, 500);
            RectTransformExtensions.SetRight(sounds, 500);

            if(ranking && header)
			{
                RectTransformExtensions.SetLeft(header, 500);
                RectTransformExtensions.SetRight(header, 500);

                RectTransformExtensions.SetLeft(ranking, 500);
                RectTransformExtensions.SetRight(ranking, 500);
            }

            if (summary)
            {
                RectTransformExtensions.SetLeft(summary, 500);
                RectTransformExtensions.SetRight(summary, 500);
            }
        }
    }
}

