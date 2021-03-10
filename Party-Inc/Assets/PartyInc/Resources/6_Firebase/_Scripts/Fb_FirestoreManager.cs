using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Firestore;

namespace PartyInc
{
    public class Fb_FirestoreManager : MonoSingleton<Fb_FirestoreManager>
    {
        public FirebaseFirestore FsDB { get; private set; }
        public CollectionReference Test { get; private set; }

        public override void Init()
        {
            base.Init();
        }

        private void Start()
        {
            if (Fb_FirebaseManager.Current.ConnectedToFirebaseServices)
            {
                FirestoreInit();
            }
            else
            {
                Debug.Log("We are not connected yet to Firebase services, redundant to access Firestore...");
                Debug.Log("Awaiting connection to Firebase...");
                StartCoroutine(AwaitForConnection());
            }
        }

        private IEnumerator AwaitForConnection()
        {
            yield return new WaitUntil(() => Fb_FirebaseManager.Current.ConnectedToFirebaseServices);
            FirestoreInit();
        }

        private void FirestoreInit()
        {
            FsDB = FirebaseFirestore.DefaultInstance;

            Test = FsDB.Collection(Fb_Constants.FIRESTORE_COLLECTION_TEST);

            //TestThings();
        }

        private void TestThings()
        {
            Fb_FirestoreStructures.TestCollection tc = new Fb_FirestoreStructures.TestCollection();

            List<string> list = new List<string>();

            list.Add("A");
            list.Add("F");
            list.Add("PUTO");
            list.Add("Me aburri");

            Dictionary<string, object> dic = new Dictionary<string, object>();

            Dictionary<string, object> dic2 = new Dictionary<string, object>();

            dic2.Add("Transporte", "RANDOOM");
            dic2.Add("Lista randomx", list);

            dic.Add("Atr1", 1);
            dic.Add("Atr2", 47);
            dic.Add("Atr6", dic2);

            List<Dictionary<string, object>> dicL = new List<Dictionary<string, object>>();

            dicL.Add(dic);
            dicL.Add(dic2);

            tc.testInt = 12;
            tc.testFloat = 0.1f;
            tc.testString = "Juan Alberto";
            tc.testTimestamp = FieldValue.ServerTimestamp;
            tc.testList = list;
            tc.testDict = dic;
            tc.testMapList = dicL;

            //FsAddToTest(tc.ToDictionary());
            FsSetTest("ogw3638xGCBxA1KvlCc7", tc.ToDictionary());
        }

        ///

        public void FsAddToTest(Dictionary<string,object> data)
        {
            Test.AddAsync(data).ContinueWith(task =>
            {
                if (task.IsCompleted)
                {
                    Debug.Log("Test data ADDED " + task.Id);
                }
                else
                {
                    Debug.Log("Test data NOT ADDED.");
                    Debug.LogError(task.Exception.InnerException.Message);
                }
            });
        }

        public void FsSetTest(string id, Dictionary<string, object> data)
        {
            DocumentReference docRef = Test.Document(id);

            string[] fields = { "testfloat", "testint" };
            docRef.SetAsync(data, SetOptions.MergeFields(fields)).ContinueWith(task =>
            {
                if (task.IsCompleted)
                {
                    Debug.Log("Successfully updated test document " + docRef.Id);
                }
                else
                {
                    Debug.Log("Error updating test document");
                    Debug.LogError(task.Exception.InnerException.Message);
                }
            });
        }

        /*
         QUICK REFERENCE:

         ADD / UPDATE
         db.Collection("collectionid").Document("id").SetAsync(info) <- Creates document with name provided
         db.Collection("collectionid").AddAsync(info)                <- Creates document with auto generated id by firestore

                                                                     SetAsync overwrites data if provided an existing id
                                                                     since AddAsync always generates a new id, it doesnt update

         db.Collection("collectionid").Document("id").SetAsync(info, SetOptions.MergeAll) <- Merges data rather than overwriting if the id exists

         SetOptions.MergeFields allow us to provide a list of field names, then Firestore will only modify those fields provided. The rest are left UNTOUCHED.

         GET
         db.Collection("collectionid").GetSnapshotAsync()            <- Gets the entire collection.
         Query query = collection.WhereEqualTo("f", v);              <- Does a Query that returns the references to those documents that have value v in field f.
         query.GetSnapshotAsync()                                    <- Gets the Query results.

         DELETE

         SetOptions.Overwrite:
                    Its an absolute Overwrite, if you removed attributes they WONT appear on the other side.

            E.g: Doc { atr1: 2, atr2: "hi" },
                 set to Doc { atr2: "hiya" }
                 => It will update to: Doc { atr2: "hiya" }

         SetOptions.MergeAll:
                    As the name suggests, it merges both sets of information. If some attribute changed in value, it will be added. If some attribute didnt exist, it will be added.
                    If information didnt change on some attribute, it will remain the same.

            E.g: Doc { atr1: 2, atr2: "hi", atr3: 2.2 }, 
                 set to Doc { atr1: 3, atr4: "Bam" }
                 => It will update to Doc { atr1: 3. atr2: "hi", atr3: 2.2, atr4: "Bam"}


         SetOptions.MergeFields:
                    As the name suggests, it merges both sets of information on specific fields. Basically just like MergeAll but with a subset of the set.

            E.g: Doc { atr1: 2, atr2: "hi" }
                 set to Doc { atr1:3, atr2:"bye" } with fields input of ["atr1"]
                 => It will update to Doc { atr1: 3, atr2: "hi" }, "hi" didnt change.

            NOTE: PROVIDING IN THE FIELDS INPUT A FIELD STRING THAT DOESNT EXIST IN THE DB WILL CRASH UNITY IMMEDIATELY.
         */
    }
}


