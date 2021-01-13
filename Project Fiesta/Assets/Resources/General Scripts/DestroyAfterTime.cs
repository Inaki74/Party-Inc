using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterTime : MonoBehaviour
{
    [SerializeField] private float _timeToDestroy;

    private float _timeElapsed = 0f;

    // Update is called once per frame
    void Update()
    {
        _timeElapsed += Time.deltaTime;

        if(_timeElapsed >= _timeToDestroy)
        {
            Destroy(gameObject);
        }
    }
}
