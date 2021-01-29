using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FiestaTime
{
    namespace CC
    {
        public class UIManager : MonoSingleton<UIManager>
        {
            [SerializeField] private Text _heightText;
            [SerializeField] private Text _angleText;
            [SerializeField] private Text _totalText;

            // Start is called before the first frame update
            void Start()
            {

            }

            // Update is called once per frame
            void Update()
            {

            }

            private void DisplayScore(float height, float angle, float total)
            {
                _heightText.text = "H: " + height.ToString("0.00") + "%";
                _angleText.text = "A: " + angle.ToString("0.00") + "%";
                _totalText.text = "TOTAL: " + total.ToString("0.00") + "%";
            }

            public override void Init()
            {
                base.Init();

                Player.onLogSlicedScore += DisplayScore;
            }
        }
    }
}


