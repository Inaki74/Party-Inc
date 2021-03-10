using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PartyInc
{
    public interface IFb_FirestoreMapStructure
    {
        Dictionary<string, object> ToDictionary();
    }
}
