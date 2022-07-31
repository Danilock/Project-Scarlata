using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

namespace Rewriters
{
    public class PlaceObjectEditorWindow : EditorWindow
    {
        List<string> _options = new List<string> { "Jose", "Lucas" };
        Animator _animator;
        int _selectionIndex;
        // Add menu named "My Window" to the Window menu
        [MenuItem("Tools/Level Builder")]
        static void Init()
        {
            // Get existing open window or if none, make a new one:
            PlaceObjectEditorWindow window = (PlaceObjectEditorWindow)EditorWindow.GetWindow(typeof(PlaceObjectEditorWindow));
            window.Show();
        }

        void OnGUI()
        {
            _animator = (Animator)EditorGUILayout.ObjectField(_animator, typeof(Animator), true);

            if (_animator != null)
                DebugAnimatorNames(_animator);
        }

        private void DebugAnimatorNames(Animator animator)
        {
            _options.Clear();

            foreach(var parameter in animator.parameters)
            {
                _options.Add(parameter.name);
            }

            _selectionIndex = EditorGUILayout.Popup("Select", _selectionIndex, _options.ToArray());
        }
    }
}
