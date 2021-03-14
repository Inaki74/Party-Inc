using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PartyInc
{
    namespace PartyFirebase.Firestore
    {
        public interface IFb_FirestoreMapStructure
        {
            Dictionary<string, object> ToDictionary();
        }
    }
     
}
