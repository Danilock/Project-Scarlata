using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MyArray))]
public class MyArrayEditor : Editor
{
    private List<Vector3> _newPositions = new List<Vector3>();
    protected virtual void OnSceneGUI()
    {
        MyArray myArrayTarget = (MyArray)target;

        _newPositions = myArrayTarget.SomePositions;

        EditorGUI.BeginChangeCheck();

        if (myArrayTarget.SomePositions == null)
            return;

        for (int i = 0; i < myArrayTarget?.SomePositions.Count; i++)
        {
            var currentTarget = myArrayTarget.SomePositions[i] + myArrayTarget.transform.position;

            Handles.DrawSolidDisc(currentTarget, myArrayTarget.transform.forward, myArrayTarget.DrawSize);

            _newPositions[i] = Handles.PositionHandle(myArrayTarget.SomePositions[i] + myArrayTarget.transform.position, Quaternion.identity);
            _newPositions[i] -= myArrayTarget.transform.position;

            if (myArrayTarget.SomePositions.Count > 1)
            {
                Handles.DrawLine(currentTarget, myArrayTarget.SomePositions[(i + 1) % myArrayTarget.SomePositions.Count] + myArrayTarget.transform.position);
            }
        }

        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(myArrayTarget, "MyArrayUpdate");

            myArrayTarget.SomePositions = _newPositions;
        }
    }
}
