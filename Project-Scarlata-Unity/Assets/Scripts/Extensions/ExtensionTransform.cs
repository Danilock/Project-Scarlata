using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExtensionTransform
{
    public static void ResetRotation(this Transform tr)
    {
        tr.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
    }

    public static void SetScale(this Transform tr, float x, float y, float z)
    {
        tr.localScale = new Vector3(x, y, z);
    }
}
