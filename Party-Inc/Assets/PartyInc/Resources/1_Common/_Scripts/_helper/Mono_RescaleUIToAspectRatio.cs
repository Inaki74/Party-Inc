using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PartyInc
{
    public class Mono_RescaleUIToAspectRatio : MonoBehaviour
    {
        [SerializeField] private GameObject _objectToScale;

        // Start is called before the first frame update
        void Start()
        {
            float originalAspect = 1125f / 2436f;
            float currentAspect = (float)Screen.width / (float)Screen.height;

            _objectToScale.transform.localScale = new Vector3(_objectToScale.transform.localScale.x * currentAspect / originalAspect,
                                                      _objectToScale.transform.localScale.y * currentAspect / originalAspect,
                                                      _objectToScale.transform.localScale.z * currentAspect / originalAspect);

            _objectToScale.transform.position = new Vector3(_objectToScale.transform.position.x * currentAspect / originalAspect,
                                                      _objectToScale.transform.position.y * currentAspect / originalAspect,
                                                      _objectToScale.transform.position.z * currentAspect / originalAspect);
        }
    }
}


