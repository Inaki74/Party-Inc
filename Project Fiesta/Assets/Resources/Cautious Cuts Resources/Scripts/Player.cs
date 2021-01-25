using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FiestaTime
{
    namespace CC
    {
        /// <summary>
        /// This is the 'cutting' of the cutting system.
        /// </summary>
        public class Player : MonoBehaviour
        {
            [SerializeField] private LineRenderer _lr;

            [SerializeField] private Color _c1 = Color.yellow;
            [SerializeField] private Color _c2 = Color.red;

            [SerializeField] LayerMask _whatIsLog;

            private int _vertexCount;
            private int _maxVertexCount = 7;

            // Start is called before the first frame update
            void Start()
            {
                if(_lr == null)
                {
                    _lr = GetComponent<LineRenderer>();
                }

                _lr.startColor = _c1;
                _lr.endColor = _c2;
                _lr.startWidth = 0.3f;
                _lr.endWidth = 0f;
                _lr.positionCount = 0;
            }

            // Update is called once per frame
            void Update()
            {
                if (Application.isMobilePlatform)
                {
                    TakeInputMobile();
                }
                else
                {

                }
            }

            private void TakeInputMobile()
            {
                if(Input.touchCount > 0)
                {
                    Touch t = Input.touches[0];

                    if(t.phase == TouchPhase.Moved)
                    {
                        RenderLine(t);
                    }

                    int posCount = _lr.positionCount;
                    Vector3[] lrPositions = new Vector3[posCount];
                    _lr.GetPositions(lrPositions);

                    for(int i = 1; i < posCount; i++)
                    {
                        Vector3 pos = lrPositions[i];
                        Vector3 lastPos = lrPositions[i - 1];
                        float dist = Vector3.Distance(pos, lastPos);
                        Vector3 direction = pos - lastPos;

                        for(int j = 0; j < 30; j++)
                        {
                            float ratio = dist / 30;
                            Vector3 newPos = lastPos + direction * ratio * j;
                            RaycastHit hit;
                            if (CheckForLog(Camera.main.WorldToScreenPoint(newPos), out hit))
                            {
                                Debug.Log("HIT LOG");
                            }
                        }
                    }

                    if (t.phase == TouchPhase.Ended)
                    {
                        ResetLine();
                    }
                }
            }

            private bool CheckForLog(Vector3 startPosition, out RaycastHit hit)
            {
                if (Physics.Raycast(Camera.main.ScreenToWorldPoint(startPosition), Camera.main.transform.TransformDirection(Vector3.back), out hit, 100f, _whatIsLog))
                {
                    Debug.DrawRay(Camera.main.ScreenToWorldPoint(startPosition), Camera.main.transform.TransformDirection(Vector3.back) * hit.distance, Color.red, 0f);
                    return true;
                }
                else
                {
                    Debug.DrawRay(Camera.main.ScreenToWorldPoint(startPosition), Camera.main.transform.TransformDirection(Vector3.back) * 100f, Color.green, 0f);
                    return false;
                }
            }

            private void RenderLine(Touch t)
            {
                // Create that sort of 'limited' rendering.
                if (_lr.positionCount == _maxVertexCount)
                {
                    // Moves all the front points to the back
                    for (int i = 1; i <= _lr.positionCount - 1; i++)
                    {
                        Vector3 position = _lr.GetPosition(i);
                        _lr.SetPosition(i - 1, position);
                    }

                    // Add the position to the end of the vector.
                    Vector3 tPos = new Vector3(t.position.x, t.position.y, 5);
                    _lr.SetPosition(_maxVertexCount - 1, Camera.main.ScreenToWorldPoint(tPos));
                }
                else
                {
                    _lr.positionCount = _vertexCount + 1;
                    Vector3 tPos = new Vector3(t.position.x, t.position.y, 5);
                    _lr.SetPosition(_vertexCount, Camera.main.ScreenToWorldPoint(tPos));
                    _vertexCount++;
                }
            }

            private void ResetLine()
            {
                _lr.positionCount = 0;
                _vertexCount = 0;
            }
        }
    }
}


