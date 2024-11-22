using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PartyInc
{
    namespace Hub
    {
        [RequireComponent(typeof(RectTransform))]
        public class Mono_ModalScreen : MonoBehaviour
        {
            [SerializeField] private GameObject _dimBackground;

            [SerializeField] private Vector2 _finalLocalPosition;
            private Vector2 _originalLocalPosition;

            private RectTransform _thisRectTransform;


            // Start is called before the first frame update
            void Start()
            {
                _thisRectTransform = GetComponent<RectTransform>();
                _originalLocalPosition = transform.localPosition;
            }

            private void OnEnable()
            {
                LeanTween.moveLocalY(gameObject, _finalLocalPosition.y, 0.3f).setEaseOutExpo();
            }

            private void OnCompleteLeanTween()
            {
                gameObject.SetActive(false);
            }

            public void OnCloseModal()
            {
                LTDescr moveEvent = LeanTween.moveLocalY(gameObject, _originalLocalPosition.y, 0.2f).setEaseOutExpo();
                _dimBackground.SetActive(false);
                moveEvent.setOnComplete(OnCompleteLeanTween);
                //moveEvent.callOnCompletes();
            }
        }
    }
}


