using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PartyInc
{
    namespace LL
    {
        public class Mono_ObstacleBoosterPlacement_LL : MonoBehaviour
        {
            [SerializeField] private GameObject _boosterPrefab;

            [SerializeField] private GameObject _booster;
            [SerializeField] private LayerMask _whatIsInvisWall;
            [SerializeField] private Transform _uXY;
            [SerializeField] private Transform _lXY;
            [SerializeField] private Transform _fpXY;

            private float _x1;
            private float _x2;
            private float _y1;
            private float _y2;

            private float _fpY;

            private void Awake()
            {
                if(Mono_ProceduralGenerator_LL.Current.BoosterCount == 0)
                {
                    _booster = Instantiate(_boosterPrefab, Vector3.zero, Quaternion.identity);
                    _booster.transform.parent = transform;
                    _booster.transform.localPosition = DecideBoosterPosition();
                }
            }

            private Vector3 DecideBoosterPosition()
            {
                Debug.Log(transform.position.y);
                Debug.Log(Mono_ProceduralGenerator_LL.Current.NextObstacleY);

                if (transform.position.y > Mono_ProceduralGenerator_LL.Current.NextObstacleY)
                {
                    _lXY.transform.localPosition = new Vector3(1.3f, -2f, 0f);

                    _fpXY.localPosition = DetermineImpactWithInvisibleWall(true);
                    _fpY = _fpXY.position.y;

                    _x1 = _lXY.localPosition.x;
                    _y2 = _lXY.localPosition.y;
                    _y1 = (60 * _uXY.localPosition.y / 100) + (40 * _fpY / 100);
                }
                else
                {
                    _uXY.transform.localPosition = new Vector3(1.3f, -2f, 0f);

                    _fpXY.localPosition = DetermineImpactWithInvisibleWall(false);
                    _fpY = _fpXY.position.y;

                    _x1 = _uXY.localPosition.x;
                    _y2 = _uXY.localPosition.y;
                    _y1 = (60*_lXY.localPosition.y/100) + (40 * _fpY/100);
                }

                _x2 = 45 * Mono_ProceduralGenerator_LL.XDifference / 100;

                if (_x1 > _x2)
                {
                    float aux = 0;
                    aux = _x1;
                    _x1 = _x2;
                    _x2 = aux;
                }

                if (_y1 > _y2)
                {
                    float aux = 0;
                    aux = _y1;
                    _y1 = _y2;
                    _y2 = aux;
                }

                return new Vector3(Random.Range(_x1, _x2), Random.Range(_y1, _y2), 0f);
            }

            private Vector3 DetermineImpactWithInvisibleWall(bool up)
            {
                RaycastHit hit;

                if (up)
                {
                    Debug.Log("UP");
                    Physics.Raycast(_uXY.position, _uXY.TransformDirection(Vector3.up), out hit, 10f, _whatIsInvisWall);
                    return transform.InverseTransformPoint(hit.point);
                }
                else
                {
                    Debug.Log("DOWN");
                    Physics.Raycast(_lXY.position, _lXY.TransformDirection(Vector3.down), out hit, 10f, _whatIsInvisWall);
                    return transform.InverseTransformPoint(hit.point);
                }
            }
        }
    }
}


