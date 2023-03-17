
/*
 *  by @rblrbt
 *  March 8th, 2023
 */

using UnityEngine;
using System.Collections;

public static class ExtensionMaterial
{
    // COLOR CHANGE =====================================================================
    //
    public static void ChangePixelColor(this Material mat, Color colorToBeChanged, Color replacingColor)
    {
        string target = "_ColorChangeTarget";
        string newCol = "_ColorChangeNewCol";

        if (mat.HasColor(target) == false || mat.HasColor(newCol) == false)
        {
            Debug.LogError("[Shader] Missing AllIn1Shader component");
            return;
        }

        mat.SetColor(target, colorToBeChanged);
        mat.SetColor(newCol, replacingColor);
    }

    public static void AllowPixelColorChange(this Material mat)
    {
        string target = "_ColorChangeTolerance";

        if (mat.HasFloat(target) == false)
        {
            Debug.LogError("[Shader] Missing AllIn1Shader component");
            return;
        }

        mat.SetFloat(target, 0.5f);
    }

    public static void DisallowPixelColorChange(this Material mat)
    {
        string target = "_ColorChangeTolerance";

        if (mat.HasFloat(target) == false)
        {
            Debug.LogError("[Shader] Missing AllIn1Shader component");
            return;
        }

        mat.SetFloat(target, 1f);
    }

    // OUTLINE ==========================================================================
    //
    public static void SetOutlineSize(this Material mat, int size)
    {
        mat.SetFloat("_OutlinePixelWidth", size * 3);
    }

    public static void ChangeOutlineColor(this Material mat, Color outlineColor)
    {
        if (mat.HasColor("_OutlineColor") == false)
        {
            Debug.LogError("[Shader] Missing AllIn1Shader component");
            return;
        }

        mat.SetColor("_OutlineColor", outlineColor);
    }

    public static void ShowOutline(this Material mat)
    {
        if (mat.HasFloat("_OutlineAlpha") == false)
        {
            Debug.LogError("[Shader] Missing AllIn1Shader component");
            return;
        }

        mat.SetFloat("_OutlineAlpha", 1);
    }

    public static void HideOutline(this Material mat)
    {
        if (mat.HasFloat("_OutlineAlpha") == false)
        {
            Debug.LogError("[Shader] Missing AllIn1Shader component");
            return;
        }

        mat.SetFloat("_OutlineAlpha", 0);
    }

    // FLASHING EFFECT ==================================================================
    //
    public static void AllowHitEffect(this Material mat)
    {
        if (mat.HasFloat("_HitEffectBlend") == false)
        {
            Debug.LogError("[Shader] Missing AllIn1Shader component");
            return;
        }

        mat.SetFloat("_HitEffectBlend", 1);
    }

    public static void DisallowHitEffect(this Material mat)
    {
        if (mat.HasFloat("_HitEffectBlend") == false)
        {
            Debug.LogError("[Shader] Missing AllIn1Shader component");
            return;
        }

        mat.SetFloat("_HitEffectBlend", 0);
    }

    public static void ChangeHitEffectColor(this Material mat, Color color)
    {
        if (mat.HasColor("_HitEffectColor") == false)
        {
            Debug.LogError("[Shader] Missing AllIn1Shader component");
            return;
        }

        mat.SetColor("_HitEffectColor", color);
    }
}