/*
 *  by @gontzalve
 *  July 9th, 2016
 *  Updated: November 18th, 2017
 */

using UnityEngine;
using System.Collections;

public static class ExtensionsTransform
{
    // SETTING GLOBAL POSITIONS ======================================================

    public static void SetPositionXY(this Transform t, float newX, float newY)
    {
        t.position = new Vector3(newX, newY, t.position.z);
    }

    public static void SetPositionXY(this Transform t, Vector2 newPos)
    {
        t.position = new Vector3(newPos.x, newPos.y, t.position.z);
    }

    public static void SetPositionX(this Transform t, float newX)
    {
        t.position = new Vector3(newX, t.position.y, t.position.z);
    }

    public static void SetPositionY(this Transform t, float newY)
    {
        t.position = new Vector3(t.position.x, newY, t.position.z);
    }

    public static void SetPositionZ(this Transform t, float newZ)
    {
        t.position = new Vector3(t.position.x, t.position.y, newZ);
    }

    // SETTING LOCAL POSITIONS ======================================================

    public static void SetLocalPositionXY(this Transform t, float newX, float newY)
    {
        t.localPosition = new Vector3(newX, newY, t.localPosition.z);
    }

    public static void SetLocalPositionXY(this Transform t, Vector2 newPos)
    {
        t.localPosition = new Vector3(newPos.x, newPos.y, t.localPosition.z);
    }

    public static void SetLocalPositionX(this Transform t, float newX)
    {
        t.localPosition = new Vector3(newX, t.localPosition.y, t.localPosition.z);
    }

    public static void SetLocalPositionY(this Transform t, float newY)
    {
        t.localPosition = new Vector3(t.localPosition.x, newY, t.localPosition.z);
    }

    public static void SetLocalPositionZ(this Transform t, float newZ)
    {
        t.localPosition = new Vector3(t.localPosition.x, t.localPosition.y, newZ);
    }

    public static void SetLocalPositionXYZ(this Transform t, float newX, float newY, float newZ)
    {
        t.localPosition = new Vector3(newX, newY, newZ);
    }

    // GETTING GLOBAL POSITIONS ======================================================

    public static Vector2 GetPositionXY(this Transform t)
    {
        return new Vector2(t.position.x, t.position.y);
    }

    public static float GetPositionX(this Transform t)
    {
        return t.position.x;
    }

    public static float GetPositionY(this Transform t)
    {
        return t.position.y;
    }

    public static float GetPositionZ(this Transform t)
    {
        return t.position.z;
    }

    // GETTING LOCAL POSITIONS ======================================================

    public static Vector2 GetLocalPositionXY(this Transform t)
    {
        return new Vector2(t.localPosition.x, t.localPosition.y);
    }

    public static float GetLocalPositionX(this Transform t)
    {
        return t.localPosition.x;
    }

    public static float GetLocalPositionY(this Transform t)
    {
        return t.localPosition.y;
    }

    public static float GetLocalPositionZ(this Transform t)
    {
        return t.localPosition.z;
    }

    // ADDING OFFSETS TO POSITION ======================================================

    public static void AddPositionXY(this Transform t, float offsetX, float offsetY)
    {
        t.position = new Vector3(t.position.x + offsetX, t.position.y + offsetY, t.position.z);
    }

    public static void AddPositionXY(this Transform t, Vector2 offset)
    {
        t.position = new Vector3(t.position.x + offset.x, t.position.y + offset.y, t.position.z);
    }

    public static void AddPositionX(this Transform t, float offsetX)
    {
        t.position = new Vector3(t.position.x + offsetX, t.position.y, t.position.z);
    }

    public static void AddPositionY(this Transform t, float offsetY)
    {
        t.position = new Vector3(t.position.x, t.position.y + offsetY, t.position.z);
    }

    public static void AddLocalPositionXY(this Transform t, float offsetX, float offsetY)
    {
        t.localPosition = new Vector3(t.localPosition.x + offsetX, t.localPosition.y + offsetY, t.localPosition.z);
    }

    public static void AddLocalPositionX(this Transform t, float offsetX)
    {
        t.localPosition = new Vector3(t.localPosition.x + offsetX, t.localPosition.y, t.localPosition.z);
    }

    public static void AddLocalPositionY(this Transform t, float offsetY)
    {
        t.localPosition = new Vector3(t.localPosition.x, t.localPosition.y + offsetY, t.localPosition.z);
    }

    // MOVING POSITION TO TARGET ======================================================

    public static void SetPositionToMousePosition(this Transform t)
    {
        Vector3 screenPosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y);
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);
        t.SetPositionXY(worldPosition.x, worldPosition.y);
    }

    public static void SetPositionToGameObject(this Transform t, GameObject target)
    {
        t.SetPositionXY(target.transform.GetPositionX(), target.transform.GetPositionY());
    }

    // SETTING SCALE ======================================================

    public static void SetScaleXY(this Transform t, float scale)
    {
        t.localScale = new Vector3(scale, scale, 1);
    }

    public static void SetScaleXY(this Transform t, float scaleX, float scaleY)
    {
        t.localScale = new Vector3(scaleX, scaleY, 1);
    }

    public static void SetScaleX(this Transform t, float scaleX)
    {
        t.localScale = new Vector3(scaleX, t.localScale.y, t.localScale.z);
    }

    public static void SetScaleY(this Transform t, float scaleY)
    {
        t.localScale = new Vector3(t.localScale.x, scaleY, t.localScale.z);
    }

    public static void FlipScaleXToPositive(this Transform t)
    {
        Vector3 currentScale = t.localScale;
        currentScale.x = Mathf.Abs(currentScale.x);
        t.localScale = currentScale;
    }

    public static void FlipScaleXToNegative(this Transform t)
    {
        Vector3 currentScale = t.localScale;
        currentScale.x = -1 * Mathf.Abs(currentScale.x);
        t.localScale = currentScale;
    }

    public static void FlipScaleXHorizontally(this Transform t)
    {
        Vector3 currentScale = t.localScale;
        currentScale.x *= -1;
        t.localScale = currentScale;
    }

    // GETTING SCALE ======================================================

    public static Vector2 GetScaleXY(this Transform t)
    {
        return new Vector2(t.localScale.x, t.localScale.y);
    }

    public static float GetScaleX(this Transform t)
    {
        return t.localScale.x;
    }

    public static float GetScaleY(this Transform t)
    {
        return t.localScale.y;
    }

    // SETTING & GETTING ROTATIONS ======================================================

    public static void SetRotation(this Transform t, Vector3 newEulerAngles)
    {
        Quaternion newRotation = t.transform.rotation;
        newRotation.eulerAngles = newEulerAngles;
        t.transform.rotation = newRotation;
    }

    public static void SetRotationZ(this Transform t, float newAngle)
    {
        Quaternion newRotation = t.transform.rotation;
        newRotation.eulerAngles = new Vector3(newRotation.eulerAngles.x, newRotation.eulerAngles.y, newAngle);
        t.transform.rotation = newRotation;
    }

    /// <summary>
    /// Rotates a GameObject clockwise.
    /// </summary>
    /// <param name="angles">
    /// Amount of angles subtracted to the current angle.
    /// Remember: Negative rotation is clockwise. Positive is counterclockwise.
    /// </param>

    public static void RotateClockwise(this Transform t, float angles)
    {
        Quaternion newRotation = t.transform.rotation;
        newRotation.eulerAngles = new Vector3(newRotation.eulerAngles.x, newRotation.eulerAngles.y, newRotation.eulerAngles.z - angles);
        t.transform.rotation = newRotation;
    }

    /// <summary>
    /// Rotates a GameObject counterclockwise.
    /// </summary>
    /// <param name="angles">
    /// Amount of angles added to the current angle.
    /// Remember: Negative rotation is clockwise. Positive is counterclockwise.
    /// </param>

    public static void RotateCounterClockwise(this Transform t, float angles)
    {
        Quaternion newRotation = t.transform.rotation;
        newRotation.eulerAngles = new Vector3(newRotation.eulerAngles.x, newRotation.eulerAngles.y, newRotation.eulerAngles.z + angles);
        t.transform.rotation = newRotation;
    }

    public static float GetRotationZ(this Transform t)
    {
        return t.rotation.eulerAngles.z;
    }

    // GETTING CHILDREN GAMEOBJECTS ======================================================

    public static GameObject GetChildByName(this Transform t, string name)
    {
        for (int i = 0; i < t.childCount; i++)
        {
            if (t.GetChild(i).gameObject.name == name)
            {
                return t.GetChild(i).gameObject;
            }
        }

        return null;
    }

    public static GameObject GetChildByTag(this Transform t, string tag)
    {
        for (int i = 0; i < t.childCount; i++)
        {
            if (t.GetChild(i).gameObject.tag == tag)
            {
                return t.GetChild(i).gameObject;
            }
        }

        return null;
    }
}