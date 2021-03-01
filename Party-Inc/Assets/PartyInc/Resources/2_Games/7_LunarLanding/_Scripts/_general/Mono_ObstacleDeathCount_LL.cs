using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace PartyInc
{
    namespace LL
    {
        public class Mono_ObstacleDeathCount_LL : MonoBehaviourPun, IPunInstantiateMagicCallback
        {
            public delegate void ActionObstacleDeath();
            public static event ActionObstacleDeath onObstacleDied;

            private int _currentDeathCount;

            [SerializeField] private int _deathCount;

            private void Awake()
            {
                Mono_ObstaclePassCheck_LL.onGateRendered += GateRendered;
            }

            private void OnDestroy()
            {
                Mono_ObstaclePassCheck_LL.onGateRendered -= GateRendered;
            }

            private void GateRendered()
            {
                _currentDeathCount--;

                if(_currentDeathCount <= 0f)
                {
                    onObstacleDied.Invoke();
                    PhotonNetwork.Destroy(gameObject);
                }
            }

            public void OnPhotonInstantiate(PhotonMessageInfo info)
            {
                StartCoroutine(WaitForDeathCount(info));
            }

            private IEnumerator WaitForDeathCount(PhotonMessageInfo info)
            {
                yield return new WaitUntil(() => info.Sender.CustomProperties.ContainsKey("DeathCount"));

                _currentDeathCount = (int)info.Sender.CustomProperties["DeathCount"];
            }
        }
    }
}

