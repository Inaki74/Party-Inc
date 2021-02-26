using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace PartyInc
{
    namespace LL
    {
        public class Mono_ProceduralGenerator_LL : MonoSingleton<Mono_ProceduralGenerator_LL>
        {
            [SerializeField] private Mono_Camera_Synchronizer_LL _sync;
            [SerializeField] private GameObject _obstacle;

            private GameObject _newest;

            private int _baseCaseCountMax;
            private int _baseCaseCount = 0;
            private int _amountOfObstaclesUpAtOnce = 6;

            public float NextObstacleY { get; private set; }

            public bool BaseCase { get; private set; }

            public const float XDifference = 7f;
            private int _boosterCount = 3;

            private void Start()
            {
                if (!PhotonNetwork.IsMasterClient) return;
                SpawnObstacle();
            }

            public override void Init()
            {
                base.Init();
                Mono_ObstacleDeathCount_LL.onObstacleDied += SpawnObstacle;
                Mono_ObstaclePassCheck_LL.onGateRenderedBaseCase += SpawnObstacle;

                BaseCase = true;
                _baseCaseCountMax = _amountOfObstaclesUpAtOnce;
            }

            private void OnDestroy()
            {
                Mono_ObstacleDeathCount_LL.onObstacleDied -= SpawnObstacle;
                Mono_ObstaclePassCheck_LL.onGateRenderedBaseCase -= SpawnObstacle;
            }

            private void SpawnObstacle()
            {
                if (!PhotonNetwork.IsMasterClient) return;

                if (_baseCaseCount == _baseCaseCountMax)
                {
                    BaseCase = false;
                }
                _baseCaseCount++;

                _boosterCount--;
                float y = NextObstacleY;
                _newest = PhotonNetwork.InstantiateRoomObject("_utils/" + _obstacle.name, new Vector3(_sync.SpawningPoint.transform.position.x, y, 0f), Quaternion.identity);
                NextObstacleY = Random.Range(-3f, 3f);
                _newest.GetComponent<Mono_ObstacleBoosterPlacement_LL>().PlaceBooster(_boosterCount);

                if(_boosterCount == 0)
                {
                    _boosterCount = 3;
                }

                ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable();
                props.Add("DeathCount", _amountOfObstaclesUpAtOnce);
                PhotonNetwork.SetPlayerCustomProperties(props);
            }
        }
    }
}



