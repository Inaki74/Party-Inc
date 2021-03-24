using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Firestore;
using Firebase.Extensions;

namespace PartyInc
{
    namespace PartyFirebase.Firestore
    {
        public class Fb_FirestoreSession : MonoSingleton<Fb_FirestoreSession>
        {
            public bool SetupCompleted { get; set; }
            public Dictionary<string, object> LocalPlayerData { get; set; }
            public Dictionary<string, object> LocalPlayerSocialData { get; set; }

            private bool _gotPlayerData;
            private bool _gotPlayerSocialData;

            private void Update()
            {
                if (!SetupCompleted)
                {
                    SetupCompleted = _gotPlayerData && _gotPlayerSocialData;
                }
            }

            public T GetHighscore<T>(string gameName)
            {
                Dictionary<string, object> currentStats = (Dictionary<string, object>)LocalPlayerData[Fb_Constants.FIRESTORE_KEY_PLAYER_STATS];
                Dictionary<string, object> currentGamestats = (Dictionary<string, object>)currentStats[Fb_Constants.FIRESTORE_KEY_PLAYER_STATS_GAMESTATS];
                if (currentGamestats != null && currentGamestats.ContainsKey(gameName))
                {
                    Dictionary<string, object> currentThisGameStats = (Dictionary<string, object>)currentGamestats[gameName];

                    return (T)Convert.ChangeType(currentThisGameStats[Fb_Constants.FIRESTORE_KEY_PLAYER_STATS_GAMESTATS_RECORDSCORE], typeof(T));
                }
                else
                {
                    return default(T);
                }
            }

            public bool CheckIfHighscore<T>(string gameName, T score)
            {
                Dictionary<string, object> currentStats = (Dictionary<string, object>)LocalPlayerData[Fb_Constants.FIRESTORE_KEY_PLAYER_STATS];
                Dictionary<string, object> currentGamestats = (Dictionary<string, object>)currentStats[Fb_Constants.FIRESTORE_KEY_PLAYER_STATS_GAMESTATS];
                if (currentGamestats != null && currentGamestats.ContainsKey(gameName))
                {
                    Dictionary<string, object> currentThisGameStats = (Dictionary<string, object>)currentGamestats[gameName];
                    if (typeof(T) == typeof(int))
                    {
                        Debug.Log(currentThisGameStats[Fb_Constants.FIRESTORE_KEY_PLAYER_STATS_GAMESTATS_RECORDSCORE].ToString());
                        long currentRecord = (long)currentThisGameStats[Fb_Constants.FIRESTORE_KEY_PLAYER_STATS_GAMESTATS_RECORDSCORE];
                        return (int)Convert.ChangeType(score, typeof(int)) > currentRecord;
                    }
                    else
                    {
                        double currentRecord = (double)currentThisGameStats[Fb_Constants.FIRESTORE_KEY_PLAYER_STATS_GAMESTATS_RECORDSCORE];
                        return (float)Convert.ChangeType(score, typeof(float)) > currentRecord;
                    }
                }
                else
                {
                    return true;
                }
            }

            public void SetGameResults<T>(T score, string gameName, bool won, bool wasHighscore)
            {
                // Create the player which will be inserted.
                Fb_FirestoreStructures.FSPlayer playerWithScore = new Fb_FirestoreStructures.FSPlayer();
                Fb_FirestoreStructures.FSPlayer.FSStats stats = new Fb_FirestoreStructures.FSPlayer.FSStats();

                // Get the local data we have about records.
                // We can get local data because only our player will break its records. Local data won't be outdated.
                // This shouldn't change outside the game.
                // Even IF it changes, it should update almost instantly.
                Dictionary<string, object> currentStats = (Dictionary<string, object>)LocalPlayerData[Fb_Constants.FIRESTORE_KEY_PLAYER_STATS];
                Dictionary<string, object> currentGamestats = (Dictionary<string, object>)currentStats[Fb_Constants.FIRESTORE_KEY_PLAYER_STATS_GAMESTATS];

                // Check if we had played this game before.
                if (currentGamestats != null && currentGamestats.ContainsKey(gameName))
                {
                    // We have played.
                    // Get the stats we have from this game.
                    Dictionary<string, object> currentThisGameStats = (Dictionary<string, object>)currentGamestats[gameName];
                    long matchesPlayed = (long)currentThisGameStats[Fb_Constants.FIRESTORE_KEY_PLAYER_STATS_GAMESTATS_MATCHESPLAYED];
                    long newMatchesWon = (long)currentThisGameStats[Fb_Constants.FIRESTORE_KEY_PLAYER_STATS_GAMESTATS_MATCHESWON];
                    Timestamp recordDate = (Timestamp)currentThisGameStats[Fb_Constants.FIRESTORE_KEY_PLAYER_STATS_GAMESTATS_RECORDDATE];

                    // If we won, add one to the won matches
                    if (won)
                    {
                        newMatchesWon++;
                    }

                    // Filter on type of result
                    if(typeof(T) == typeof(int))
                    {
                        int finalRecord = 0;
                        object finalDate;
                        if (wasHighscore)
                        {
                            finalRecord = (int)Convert.ChangeType(score, typeof(int));
                            finalDate = FieldValue.ServerTimestamp;
                        }
                        else
                        {
                            long aux = (long)currentThisGameStats[Fb_Constants.FIRESTORE_KEY_PLAYER_STATS_GAMESTATS_RECORDSCORE];
                            finalRecord = (int)aux;
                            finalDate = recordDate;
                        }

                        // Add to the current stats dictionary.
                        Fb_FirestoreStructures.FSPlayer.FSStats.AddGameStatsInt(currentGamestats, gameName, (int)matchesPlayed + 1, (int)newMatchesWon, finalRecord, finalDate);
                    }
                    else
                    {
                        float finalRecord = 0;
                        object finalDate;
                        if (wasHighscore)
                        {
                            finalRecord = (float)Convert.ChangeType(score, typeof(float));
                            finalDate = FieldValue.ServerTimestamp;
                        }
                        else
                        {
                            double aux = (double)currentThisGameStats[Fb_Constants.FIRESTORE_KEY_PLAYER_STATS_GAMESTATS_RECORDSCORE];
                            finalRecord = (float)aux;
                            finalDate = recordDate;
                        }

                        // Add to the current stats dictionary.
                        Fb_FirestoreStructures.FSPlayer.FSStats.AddGameStatsFloat(currentGamestats, gameName, (int)matchesPlayed + 1, (int)newMatchesWon, finalRecord, finalDate);
                    }
                }
                else
                {
                    // First match played
                    // Base case for the DB
                    int wins = 0;

                    if (won)
                        wins = 1;

                    if (typeof(T) == typeof(int))
                    {
                        Fb_FirestoreStructures.FSPlayer.FSStats.AddGameStatsInt(currentGamestats, gameName, 1, wins, (int)Convert.ChangeType(score, typeof(int)), FieldValue.ServerTimestamp);
                    }
                    else
                    {
                        Fb_FirestoreStructures.FSPlayer.FSStats.AddGameStatsFloat(currentGamestats, gameName, 1, wins, (float)Convert.ChangeType(score, typeof(float)), FieldValue.ServerTimestamp);
                    }
                }
                stats.gamestats = currentGamestats;
                playerWithScore.stats = stats.ToDictionary();

                // Path to the game record in the DB.
                string[][] paths = new string[1][];
                paths[0] = new string[2];
                paths[0][0] = Fb_Constants.FIRESTORE_KEY_PLAYER_STATS;
                paths[0][1] = Fb_Constants.FIRESTORE_KEY_PLAYER_STATS_GAMESTATS;

                // Update our doc.
                Fb_FirestoreManager.Current.UpdateDocument(Fb_FirestoreManager.Current.Players, Auth.Fb_FirebaseAuthenticateManager.Current.Auth.CurrentUser.UserId, playerWithScore.ToDictionary(), res =>
                {
                    if (res.success)
                    {
                        Debug.Log("PlayerSession: Player stats successfully updated!");
                    }
                    else
                    {
                        Debug.LogError("Something went wrong...");
                        Debug.LogError(res.exceptions[0].Message);
                    }
                }, paths);
            }

            public void Setup()
            {
                Fb_FirestoreManager.Current.Get(Fb_FirestoreManager.Current.Players, Auth.Fb_FirebaseAuthenticateManager.Current.Auth.CurrentUser.UserId, res =>
                {
                    if (res.success)
                    {
                        Debug.Log("Got local player data!");
                        LocalPlayerData = res.data;
                        _gotPlayerData = true;
                        PlaceOnChangedListenerPlayer();
                    }
                    else
                    {
                        Debug.Log(res.exceptions[0].Message);
                    }
                });

                Fb_FirestoreManager.Current.Get(Fb_FirestoreManager.Current.PlayerSocial, Auth.Fb_FirebaseAuthenticateManager.Current.Auth.CurrentUser.UserId, res =>
                {
                    if (res.success)
                    {
                        Debug.Log("Got local player social data!");
                        LocalPlayerSocialData = res.data;
                        _gotPlayerSocialData = true;
                        PlaceOnChangedListenerSocial();
                    }
                    else
                    {
                        Debug.Log(res.exceptions[0].Message);
                    }
                });
            }

            private void PlaceOnChangedListenerPlayer()
            {
                string uid = Auth.Fb_FirebaseAuthenticateManager.Current.Auth.CurrentUser.UserId;
                if (uid == "" || uid == null)
                {
                    Debug.LogError("Firestore Session: No LocalPlayerUid detected!");
                    return;
                }

                DocumentReference myPlayerRef = Fb_FirestoreManager.Current.Players.Document(uid);
                myPlayerRef.Listen(snapshot =>
                {
                    if (snapshot.Exists)
                    {
                        Debug.Log("Updated information");
                        LocalPlayerData = snapshot.ToDictionary();
                        // Trigger an on Data update event?

                    }
                    else
                    {
                        Debug.LogError("Firestore Session: Something went wrong. It seems like your player doesn't exist anymore. Wth happened.");
                        return;
                    }
                });
            }

            private void PlaceOnChangedListenerSocial()
            {
                string uid = Auth.Fb_FirebaseAuthenticateManager.Current.Auth.CurrentUser.UserId;
                if (uid == "" || uid == null)
                {
                    Debug.LogError("Firestore Session: No LocalPlayerUid detected!");
                    return;
                }

                DocumentReference myPlayerSocialRef = Fb_FirestoreManager.Current.PlayerSocial.Document(uid);
                myPlayerSocialRef.Listen(snapshot =>
                {
                    if (snapshot.Exists)
                    {
                        Debug.Log("Updated information");
                        LocalPlayerSocialData = snapshot.ToDictionary();
                        // Trigger an on Data update event?
                    }
                    else
                    {
                        Debug.LogError("Firestore Session: Something went wrong. It seems like your player doesn't exist anymore. Wth happened.");
                        return;
                    }
                });
            }
        }
    }
}


