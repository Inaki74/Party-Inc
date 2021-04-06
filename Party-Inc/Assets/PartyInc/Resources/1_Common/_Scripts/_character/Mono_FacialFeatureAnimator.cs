using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PartyInc
{
    /// <summary>
    /// A tool for animating each of the faces pieces.
    /// Meant for ease of use for animators and to use in conjunction with the Projector.
    /// </summary>
    [ExecuteInEditMode]
    [RequireComponent(typeof(MeshRenderer))]
    public class Mono_FacialFeatureAnimator : MonoBehaviour
    {
        private MeshRenderer _meshRenderer;

        [SerializeField] private Texture _expression;
        private Texture _lastExpression;

        public Material facialFeature;

        private void Update()
        {
            if (!_expression.Equals(_lastExpression))
            {
                facialFeature.mainTexture = _expression;
            }


            _lastExpression = _expression;
        }

        private void OnValidate()
        {
            if (_meshRenderer == null)
            {
                _meshRenderer = GetComponent<MeshRenderer>();
            }

            if (_meshRenderer != null)
            {
                facialFeature = _meshRenderer.sharedMaterial;
            }

            facialFeature.mainTexture = _expression;
        }
    }
}
