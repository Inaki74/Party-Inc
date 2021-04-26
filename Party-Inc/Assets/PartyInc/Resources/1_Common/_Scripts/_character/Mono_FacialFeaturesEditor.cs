using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PartyInc
{
    /// <summary>
    /// The editor of facial features.
    /// It contains a bunch of sliders to modify the face of the character.
    /// </summary>
    //[RequireComponent(typeof(Mono_MeshProjector))]
    [ExecuteInEditMode]
    public class Mono_FacialFeaturesEditor : MonoBehaviour
    {
        private Mono_MeshProjector _meshProjector;

        [SerializeField] private Transform _leftEyeSocketTransform;
        [SerializeField] private Transform _rightEyeSocketTransform;
        [SerializeField] private Transform _mouthTransform;

        public const float EYE_HEIGHT_LOW = -4.0f;
        public const float EYE_HEIGHT_HI = 1.0f;
        public const float EYE_SEPARATION_LOW = -0.9f;
        public const float EYE_SEPARATION_HI = 2.0f;
        public const float EYE_ROTATION_LOW = -25.0f;
        public const float EYE_ROTATION_HI = 25.0f;
        public const float EYE_SCALE_LOW = 0.5f;
        public const float EYE_SCALE_HI = 1.5f;
        public const float EYE_SQUASH_LOW = 0.5f;
        public const float EYE_SQUASH_HI = 1.5f;

        public const float MOUTH_HEIGHT_LOW = -2.0f;
        public const float MOUTH_HEIGHT_HI = 4.0f;
        public const float MOUTH_SCALE_LOW = 0.5f;
        public const float MOUTH_SCALE_HI = 1.5f;
        public const float MOUTH_SQUASH_LOW = 0.5f;
        public const float MOUTH_SQUASH_HI = 1.5f;

        // SLIDERS
        [Header("SLIDERS")]
        [Header("Eye Sockets")]
        [Range(EYE_HEIGHT_LOW, EYE_HEIGHT_HI)]
        [SerializeField] private float _eyeHeight;
        private float _lastEyeHeight;

        public float EyeHeight { set { _eyeHeight = value; } }

        [Range(EYE_SEPARATION_LOW, EYE_SEPARATION_HI)]
        [SerializeField] private float _eyeSeparation;
        private float _lastEyeSeparation;

        public float EyeSeparation { set { _eyeSeparation = value; } }

        [Range(EYE_ROTATION_LOW, EYE_ROTATION_HI)]
        [SerializeField] private float _eyeRotation;
        private float _lastEyeRotation;

        public float EyeRotation { set { _eyeRotation = value; } }

        [Range(EYE_SCALE_LOW, EYE_SCALE_HI)]
        [SerializeField] private float _eyeScale;
        private float _lastEyeScale;

        public float EyeScale { set { _eyeScale = value; } }

        [Range(EYE_SQUASH_LOW, EYE_SQUASH_HI)]
        [SerializeField] private float _eyeSquash;
        private float _lastEyeSquash;

        public float EyeSquash { set { _eyeSquash = value; } }

        [Header("Mouth")]
        [Range(MOUTH_HEIGHT_LOW, MOUTH_HEIGHT_HI)]
        [SerializeField] private float _mouthHeight;
        private float _lastMouthHeight;

        public float MouthHeight { set { _mouthHeight = value; } }

        [Range(MOUTH_SCALE_LOW, MOUTH_SCALE_HI)]
        [SerializeField] private float _mouthScale;
        private float _lastMouthScale;

        public float MouthScale { set { _mouthScale = value; } }

        [Range(MOUTH_SQUASH_LOW, MOUTH_SQUASH_HI)]
        [SerializeField] private float _mouthSquash;
        private float _lastMouthSquash;

        public float MouthSquash { set { _mouthSquash = value; } }

        // Start is called before the first frame update
        void Start()
        {
            return;
            // Initialize everything that is necessary.
            Init();
        }

        private void Init()
        {
            // Initialize sliders
            _lastEyeHeight = _eyeHeight;
            _lastMouthHeight = _mouthHeight;
            _lastEyeSeparation = _eyeSeparation;
            _lastEyeRotation = _eyeRotation;
            _lastEyeScale = _eyeScale;
            _lastMouthScale = _mouthScale;
            _lastEyeSquash = _eyeSquash;
            _lastMouthSquash = _mouthSquash;

            if (_meshProjector == null)
            {
                _meshProjector = GetComponent<Mono_MeshProjector>();
            }
        }

        // Update is called once per frame
        void Update()
        {
            return;

            if (_meshProjector == null) Init();

            // Slider to change eye _eyeHeight
            if (_eyeHeight != _lastEyeHeight)
            {
                // Undoes projection
                _meshProjector.Undo();

                // Applies changes
                _leftEyeSocketTransform.localPosition = new Vector3(_leftEyeSocketTransform.localPosition.x, _leftEyeSocketTransform.localPosition.y, _eyeHeight);
                _rightEyeSocketTransform.localPosition = new Vector3(_leftEyeSocketTransform.localPosition.x, _leftEyeSocketTransform.localPosition.y, _eyeHeight);

                // Re projects
                _meshProjector.Project();
            }

            // Slider to change eye _eyeSeparation
            if (_eyeSeparation != _lastEyeSeparation)
            {
                _meshProjector.Undo();
                _leftEyeSocketTransform.localPosition = new Vector3(-_eyeSeparation, _leftEyeSocketTransform.localPosition.y, _leftEyeSocketTransform.localPosition.z);
                _rightEyeSocketTransform.localPosition = new Vector3(_eyeSeparation, _leftEyeSocketTransform.localPosition.y, _leftEyeSocketTransform.localPosition.z);
                _meshProjector.Project();
            }

            // Slider to change eye _eyeRotations
            if (_eyeRotation != _lastEyeRotation)
            {
                _meshProjector.Undo();
                _leftEyeSocketTransform.localRotation = Quaternion.Euler(new Vector3(_leftEyeSocketTransform.localRotation.eulerAngles.x, -_eyeRotation, _leftEyeSocketTransform.localRotation.eulerAngles.z));
                _rightEyeSocketTransform.localRotation = Quaternion.Euler(new Vector3(_rightEyeSocketTransform.localRotation.eulerAngles.x, _eyeRotation, _rightEyeSocketTransform.localRotation.eulerAngles.x));
                _meshProjector.Project();
            }

            // Slider to change _eyeHeight of mouth
            if (_mouthHeight != _lastMouthHeight)
            {
                _meshProjector.Undo();
                _mouthTransform.localPosition = new Vector3(_mouthTransform.localPosition.x, _mouthTransform.localPosition.y, _mouthHeight);
                _meshProjector.Project();
            }

            // Slider to change the mouths scaling and _eyeSquashing
            if (_mouthSquash != _lastMouthSquash || _mouthScale != _lastMouthScale)
            {
                _meshProjector.Undo();
                _mouthTransform.localScale = new Vector3(_mouthScale, _mouthTransform.localScale.y, _mouthSquash * _mouthScale);
                _meshProjector.Project();
            }

            // Slider to change the eyes scaling and _eyeSquashing
            if (_eyeScale != _lastEyeScale || _eyeSquash != _lastEyeSquash)
            {
                _meshProjector.Undo();
                _leftEyeSocketTransform.localScale = new Vector3(_eyeScale, _leftEyeSocketTransform.localScale.y, _eyeScale * _eyeSquash);
                _rightEyeSocketTransform.localScale = new Vector3(_eyeScale, _rightEyeSocketTransform.localScale.y, _eyeScale * _eyeSquash);
                _meshProjector.Project();
            }

            // Register the last value of each slider so as to detect change.
            _lastEyeHeight = _eyeHeight;
            _lastMouthHeight = _mouthHeight;
            _lastEyeSeparation = _eyeSeparation;
            _lastEyeRotation = _eyeRotation;
            _lastEyeScale = _eyeScale;
            _lastMouthScale = _mouthScale;
            _lastEyeSquash = _eyeSquash;
            _lastMouthSquash = _mouthSquash;
        }
    }
}


