using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using Rewriters.AI;

[CustomEditor(typeof(TargetDetection)), CanEditMultipleObjects]
public class BoundsExampleEditor : Editor
{
    private BoxBoundsHandle _boundsHandle = new BoxBoundsHandle();
    private SphereBoundsHandle _sphereHandle = new SphereBoundsHandle();
    private SphereBoundsHandle _reachedDistanceSphere = new SphereBoundsHandle();
    private Vector3 _newTargetPosition;

    // the OnSceneGUI callback uses the Scene view camera for drawing handles by default
    protected virtual void OnSceneGUI()
    {
        TargetDetection targetDetection = (TargetDetection)target;

        // copy the target object's data to the handle
        _boundsHandle.center = targetDetection.transform.position + targetDetection.Offset;
        _boundsHandle.size = targetDetection.Bounds.size;

        _sphereHandle.radius = targetDetection.Radius;
        _sphereHandle.center = targetDetection.transform.position + targetDetection.Offset;

        _reachedDistanceSphere.center = targetDetection.transform.position;
        _reachedDistanceSphere.radius = targetDetection.ReachDistance;

        // draw the handle
        EditorGUI.BeginChangeCheck();

        if (targetDetection.Type == TargetDetectionType.Box)
            _boundsHandle.DrawHandle();

        if (targetDetection.Type == TargetDetectionType.Sphere)
            _sphereHandle.DrawHandle();

        _reachedDistanceSphere.DrawHandle();

        _newTargetPosition = Handles.PositionHandle(targetDetection.Offset + targetDetection.transform.position, Quaternion.identity);
        _newTargetPosition -= targetDetection.transform.position;

        if (EditorGUI.EndChangeCheck())
        {
            // record the target object before setting new values so changes can be undone/redone
            Undo.RecordObject(targetDetection, "Process Target Detection");

            targetDetection.Offset = _newTargetPosition;

            // copy the handle's updated data back to the target object
            Bounds newBounds = new Bounds();
            newBounds.center = _boundsHandle.center;
            newBounds.size = _boundsHandle.size;

            targetDetection.Bounds = newBounds;

            targetDetection.Radius = _sphereHandle.radius;
            targetDetection.SphereCenter = _sphereHandle.center;
        }
    }
}
