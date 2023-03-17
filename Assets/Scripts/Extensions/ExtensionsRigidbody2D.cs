/*
 *  by @gontzalve
 *  July 9th, 2016
 *  Updated: November 18th, 2017
 */

using UnityEngine;
using System.Collections;

public static class ExtensionsRigidbody2D
{
    public static void SetVelocity(this Rigidbody2D rb, float velX, float velY)
    {
        rb.velocity = new Vector2(velX, velY);
    }

    public static void SetVelocityX(this Rigidbody2D rb, float velX)
    {
        rb.velocity = new Vector2(velX, rb.velocity.y);
    }

    public static void SetVelocityY(this Rigidbody2D rb, float velY)
    {
        rb.velocity = new Vector2(rb.velocity.x, velY);
    }

    public static void SetGravityScale(this Rigidbody2D rb, float g)
    {
        rb.gravityScale = g;
    }
}