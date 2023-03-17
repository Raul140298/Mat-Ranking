
/*
 *  by @gontzalve
 *  July 22nd, 2021 
 */

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public static class ExtensionsImageUI 
{
    public static void SetAlpha(this Image img, float alpha)
    {
        Color c = img.color;
        c.a = alpha;
        img.color = c;
    }
}