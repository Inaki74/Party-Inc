using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Analytics;
using Firebase;

namespace PartyInc
{
    public class Fb_FirebaseManager : MonoSingleton<Fb_FirebaseManager>
    {
        public bool ConnectedToFirebaseServices { get; private set; }

        // Start is called before the first frame update
        void Start()
        {
            FirebaseInit();
        }

        private void FirebaseInit()
        {
            // Initialize Analytics
            FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(continuationAction: task =>
            {
                if (task.IsCompleted)
                {
                    // Setup firebase services basics
                    FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);
                    Debug.Log("Analytics set up successfully.");

                    ConnectedToFirebaseServices = true;
                }
                else
                {
                    // Something went wrong.
                    Debug.LogError("ERROR: Failed to connect to Firebase.");
                    Debug.LogError(task.Result.ToString());
                    Debug.LogError(task.Exception.InnerException.Message);
                }
            });
        }
    }
}


