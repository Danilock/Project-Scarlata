using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExtensionRigidbody
{
    public static void ChangeHorizontalVelocity(this Rigidbody2D rgb, float newValue){
        rgb.velocity = new Vector2(newValue, rgb.velocity.y);
    }

    public static void ChangeVerticalVelocity(this Rigidbody2D rgb, float newValue){
        rgb.velocity = new Vector2(rgb.velocity.x, newValue);
    }
}
