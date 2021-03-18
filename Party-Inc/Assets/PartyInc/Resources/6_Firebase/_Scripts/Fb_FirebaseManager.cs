using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Analytics;
using Firebase.Extensions;
using Firebase;
using System;

namespace PartyInc
{
    namespace PartyFirebase
    {
        public class Fb_FirebaseManager : MonoSingleton<Fb_FirebaseManager>
        {
            private FirebaseApp _app;
            public FirebaseApp App
            {
                get
                {
                    return _app;
                }
                private set
                {
                    _app = value;
                }
            }

            private bool _connectedToFirebaseServices;

            public override void Init()
            {
                base.Init();
                FirebaseInit();
            }

            private void FirebaseInit()
            {
                // Initialize Analytics
                FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
                {
                    if (task.IsCompleted)
                    {
                        _app = FirebaseApp.DefaultInstance;

                        // Setup firebase services basics
                        FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);
                        Debug.Log("Analytics set up successfully.");

                        _connectedToFirebaseServices = true;
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

            public void InitializeService(Action initMethod)
            {
                if (_connectedToFirebaseServices)
                {
                    initMethod();
                }
                else
                {
                    Debug.Log("We are not connected yet to Firebase services...");
                    Debug.Log("Awaiting connection to Firebase...");
                    StartCoroutine(AwaitForConnection(initMethod));
                }
            }

            private IEnumerator AwaitForConnection(Action initMethod)
            {
                yield return new WaitUntil(() => _connectedToFirebaseServices);
                initMethod();
            }
        }
    }

       
}


