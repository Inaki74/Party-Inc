using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PartyInc
{
    namespace Hub
    {
        public struct AssetsStoreData
        {
            public int baseprice;
            public int premiumprice;
            public string storename;
            public string assetid;
            public int type;
        }

        public class Mono_CharacterStoreCache : MonoBehaviour
        {
            public bool GotData { get; private set; }

            private List<AssetsStoreData>[] _allAssetsStoreData = new List<AssetsStoreData>[24];

            // Start is called before the first frame update
            void Start()
            {

            }

            // Update is called once per frame
            void Update()
            {

            }

            private void GetAllAssetsStoreData()
            {

            }
        }
    }
}


