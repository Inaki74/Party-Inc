using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace PartyInc
{
    namespace PartyEditor
    {
        [CustomEditor(typeof(Mono_FacialFeatureAnimator))]
        public class Editor_FacialFeatureAnimator : Editor
        {
            public override void OnInspectorGUI()
            {
                Mono_FacialFeatureAnimator myTarget = (Mono_FacialFeatureAnimator)target;

                DrawDefaultInspector();

                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.BeginHorizontal();
                myTarget.facialFeature = (Material)EditorGUILayout.ObjectField("Facial Feature", myTarget.facialFeature, typeof(Material), true, null);
                EditorGUILayout.EndHorizontal();
                EditorGUI.EndDisabledGroup();
            }
        }
    }
}