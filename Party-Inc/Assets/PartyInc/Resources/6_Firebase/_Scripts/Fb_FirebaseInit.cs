using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Analytics;
using Firebase;

namespace PartyInc
{
    public class Fb_FirebaseInit : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(continuationAction: task =>
            {
                FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);
                Debug.Log("Analytics set up successfully.");
            });
        }
    }
}