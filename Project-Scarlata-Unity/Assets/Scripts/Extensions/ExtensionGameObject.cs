using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public static class ExtensionGameObject
{
    public static bool CompareTag(this GameObject obj, string[] tags)
    {
        return tags.Contains(obj.tag);
    }
    
    public static bool CompareTag(this Collider2D obj, string[] tags)
    {
        return tags.Contains(obj.tag);
    }
}
