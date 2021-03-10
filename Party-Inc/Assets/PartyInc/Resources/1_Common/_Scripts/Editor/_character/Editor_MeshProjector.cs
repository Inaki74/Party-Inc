using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace PartyInc
{
    namespace PartyEditor
    {
        [CustomEditor(typeof(Mono_MeshProjector))]
        public class Editor_MeshProjector : Editor
        {
            public override void OnInspectorGUI()
            {
                Mono_MeshProjector myTarget = (Mono_MeshProjector)target;

                DrawDefaultInspector();

                if (GUILayout.Button("Project"))
                {
                    myTarget.Project();
                }

                if (GUILayout.Button("Undo"))
                {
                    myTarget.Undo();
                }
            }
        }
    }
}
