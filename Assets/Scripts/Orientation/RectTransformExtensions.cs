using UnityEngine;
public static class RectTransformExtensions
{
    public static void SetLeft(RectTransform rt, float left)
    {
        rt.offsetMin = new Vector2(left, rt.offsetMin.y);
    }

    public static void SetRight(RectTransform rt, float right)
    {
        rt.offsetMax = new Vector2(-right, rt.offsetMax.y);
    }

    public static void SetTop(RectTransform rt, float top)
    {
        rt.offsetMax = new Vector2(rt.offsetMax.x, -top);
    }

    public static void SetBottom(RectTransform rt, float bottom)
    {
        rt.offsetMin = new Vector2(rt.offsetMin.x, bottom);
    }

    public static void SetPosY(RectTransform rt, float posY)
    {
        rt.localPosition = new Vector3(rt.localPosition.x, posY, rt.localPosition.z);
    }

    public static void SetPosX(RectTransform rt, float posX)
    {
        rt.localPosition = new Vector3(posX, rt.localPosition.y, rt.localPosition.z);
    }
}
