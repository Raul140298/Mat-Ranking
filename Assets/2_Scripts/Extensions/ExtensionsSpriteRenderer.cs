/*
 *  by @gontzalve
 *  July 9th, 2016
 *  Updated: November 18th, 2017
 */

using UnityEngine;
using System.Collections;
using DG.Tweening;

public static class ExtensionsSpriteRenderer
{
    public static void ShowSprite(this SpriteRenderer rnd)
    {
        rnd.enabled = true;
    }

    public static void HideSprite(this SpriteRenderer rnd)
    {
        rnd.enabled = false;
    }

	public static void FadeIn(this SpriteRenderer rnd, float time = 0.2f)
    {
        rnd.DOFade(1, time);
    }

	public static void FadeOut(this SpriteRenderer rnd, float time = 0.2f)
    {
        rnd.DOFade(0, time);
    }

    public static void SetAlpha(this SpriteRenderer rnd, float alpha)
    {
        rnd.color = new Color(rnd.color.r, rnd.color.g, rnd.color.b, alpha);
    }

    public static void SetColor(this SpriteRenderer rnd, int r, int g, int b, int a = 255)
    { 
        rnd.color = new Color(r / 255f, g / 255f, b / 255f, a / 255f);
    }
}