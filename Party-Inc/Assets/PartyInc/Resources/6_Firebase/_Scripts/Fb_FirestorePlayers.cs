using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Firestore;
using Firebase.Extensions;
using UnityEngine.SceneManagement;
using System;

namespace PartyInc
{
    namespace PartyFirebase.Firestore
    {
        /// <summary>
        /// The Firestore code in charge of the Players collection.
        /// </summary>
        public class Fb_FirestorePlayers : MonoSingleton<Fb_FirestorePlayers>
        {
            public Dictionary<string, object> LocalPlayer = new Dictionary<string, object>();

            public void GetPlayerInformation(string userId, Action Callback = null)
            {
                CollectionReference collection = Fb_FirestoreManager.Current.Players;

                collection.Document(userId).GetSnapshotAsync().ContinueWithOnMainThread(task =>
                {
                    if(task.IsFaulted || task.IsCanceled)
                    {
                        FirestoreException error = (FirestoreException)task.Exception.Flatten().InnerException;

                        Debug.Log("Couldn't find player, something went wrong: " + error.Message);
                    }

                    if (task.IsCompleted)
                    {
                        Debug.Log("Found player! Passing it by...");
                        
                        Callback();
                    }
                });
            }

            /// <summary>
            /// Update player with new data.
            /// </summary>
            /// <param name="userId"></param>
            /// <param name="data"></param>
            //public void UpdatePlayer(string userId, Dictionary<string, object> data)
            //{
            //    CollectionReference collection = Fb_FirestoreManager.Current.Players;

            //    // Check if the player exists
            //    collection.Document(userId).GetSnapshotAsync().ContinueWithOnMainThread(task =>
            //    {
            //        if(task.IsFaulted || task.IsCanceled)
            //        {
            //            FirestoreException error = (FirestoreException)task.Exception.Flatten().InnerException;

            //            Debug.Log("Couldn't find player, something went wrong: " + error.Message);

            //            return;
            //        }


            //        if (task.IsCompleted)
            //        {
            //            Debug.Log("Found player! Updating player.");
            //        }

            //        // If found, update information
            //        collection.Document(userId).SetAsync(data, SetOptions.MergeAll).ContinueWithOnMainThread(task2 =>
            //        {
            //            if (task2.IsFaulted || task2.IsCanceled)
            //            {
            //                FirestoreException error = (FirestoreException)task2.Exception.Flatten().InnerException;

            //                Debug.Log("Couldn't update player, something went wrong: " + error.Message);

            //                return;
            //            }

            //            if (task2.IsCompleted)
            //            {
            //                Debug.Log("SUCCESS: Player updated!");

            //                //Fetch the new information
            //                GetPlayerInformation(userId);
            //            }
            //        });
            //    });
            //}

            /// <summary>
            /// Update player with new data. Will only update the paths provided.
            /// </summary>
            /// <param name="userId"></param>
            /// <param name="data"></param>
            /// <param name="paths"></param>
            //public void UpdatePlayer(string userId, Dictionary<string, object> data, FieldPath[] paths)
            //{
            //    CollectionReference collection = Fb_FirestoreManager.Current.Players;

            //collection.Document(userId).GetSnapshotAsync().ContinueWithOnMainThread(task =>
            //    {
            //    if (task.IsFaulted || task.IsCanceled)
            //    {
            //        FirestoreException error = (FirestoreException)task.Exception.Flatten().InnerException;

            //        Debug.Log("Couldn't find player, something went wrong: " + error.Message);

            //        return;
            //    }


            //    if (task.IsCompleted)
            //    {
            //        Debug.Log("Found player! Updating player.");
            //    }

            //    collection.Document(userId).SetAsync(data, SetOptions.MergeFields(paths)).ContinueWithOnMainThread(task2 =>
            //    {
            //        if (task2.IsFaulted || task2.IsCanceled)
            //        {
            //            FirestoreException error = (FirestoreException)task2.Exception.Flatten().InnerException;

            //            Debug.Log("Couldn't update player, something went wrong: " + error.Message);

            //            return;
            //        }

            //        if (task2.IsCompleted)
            //        {
            //            Debug.Log("SUCCESS: Player updated!");

            //                //Fetch the new information
            //                GetPlayerInformation(userId);
            //        }
            //    });
            //});
            //}

            /// <summary>
            /// Add a brand new player.
            /// PRE: The user adding this player is the player in the sign up page.
            /// </summary>
            /// <param name="userId"></param>
            /// <param name="data"></param>
            public void AddNewPlayer(string userId, Dictionary<string, object> data, Action Callback = null)
            {
                CollectionReference collection = Fb_FirestoreManager.Current.Players;

                // Check if the player exists already.
                collection.Document(userId).GetSnapshotAsync().ContinueWithOnMainThread((task) =>
                {
                    if (task.IsFaulted || task.IsCanceled)
                    {
                        FirestoreException error = (FirestoreException)task.Exception.Flatten().InnerException;

                        Debug.Log("Couldn't get user: " + error.Message);
                    }

                    if (task.Result.Exists)
                    {
                        Debug.Log("Player already exists, cant create another player...");

                        // Show the player that the user already exists.
                        // This would never happen as a user who has already gone through the auth process
                        // has already created his player.

                        return;
                    }

                    // If the player doesn't exist, its safe to add.
                    collection.Document(userId).SetAsync(data).ContinueWithOnMainThread((task2) =>
                    {
                        if (task2.IsFaulted || task2.IsCanceled)
                        {
                            FirestoreException error = (FirestoreException)task2.Exception.Flatten().InnerException;

                            Debug.Log("Something went wrong when creating your player: " + error.Message);
                        }

                        if (task2.IsCompleted)
                        {
                            Debug.Log("SUCCESS: Created new player!");

                            Callback();
                        }
                    });
                });
            }
        }
    }
}
