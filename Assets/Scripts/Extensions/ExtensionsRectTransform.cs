/*
 *  by @Raul140298
 *  March 16th, 2023
 */

using UnityEngine;

public static class ExtensionsRectTransform
{
    public static void SetLeft(this RectTransform rt, float left)
    {
        rt.offsetMin = new Vector2(left, rt.offsetMin.y);
    }

    public static void SetRight(this RectTransform rt, float right)
    {
        rt.offsetMax = new Vector2(-right, rt.offsetMax.y);
    }

    public static void SetTop(this RectTransform rt, float top)
    {
        rt.offsetMax = new Vector2(rt.offsetMax.x, -top);
    }

    public static void SetBottom(this RectTransform rt, float bottom)
    {
        rt.offsetMin = new Vector2(rt.offsetMin.x, bottom);
    }

    public static void SetPosY(this RectTransform rt, float posY)
    {
        rt.localPosition = new Vector3(rt.localPosition.x, posY, rt.localPosition.z);
    }

    public static void SetPosX(this RectTransform rt, float posX)
    {
        rt.localPosition = new Vector3(posX, rt.localPosition.y, rt.localPosition.z);
    }

    public static void SetScaleX(this RectTransform rt, float x)
    {
        rt.localScale = new Vector3(x, rt.localScale.y);
    }

    public static void SetScaleY(this RectTransform rt, float y)
    {
        rt.localScale = new Vector3(rt.localScale.x, y);
    }

    public static void SetRotationZ(this RectTransform rt, float z)
    {
        rt.localRotation = Quaternion.Euler(0f, 0f, z);
    }
}
