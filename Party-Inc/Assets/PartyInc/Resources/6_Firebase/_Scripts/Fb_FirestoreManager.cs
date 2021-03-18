using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Firestore;
using Firebase.Extensions;
using System;
using System.Threading.Tasks;
using System.Linq;

namespace PartyInc
{
    namespace PartyFirebase.Firestore
    {
        public struct FirestoreCallResult
        {
            public bool success;
            public Dictionary<string, object> data;
            public Dictionary<string, object> oldData;
            public List<FirestoreException> exceptions;
        }

        public class Fb_FirestoreManager : MonoSingleton<Fb_FirestoreManager>
        {
            public FirebaseFirestore FsDB { get; private set; }
            public CollectionReference Players { get; private set; }
            public CollectionReference PlayerSocial { get; private set; }

            public CollectionReference Test { get; private set; }

            private bool _runStartOnce;

            private void Start()
            {
                if (_runStartOnce) return;

                _runStartOnce = true;
                Fb_FirebaseManager.Current.InitializeService(FirestoreInit);
            }

            private void FirestoreInit()
            {
                FsDB = FirebaseFirestore.DefaultInstance;

                Test = FsDB.Collection(Fb_Constants.FIRESTORE_COLLECTION_TEST);
                Players = FsDB.Collection(Fb_Constants.FIRESTORE_KEY_PLAYERS);
                PlayerSocial = FsDB.Collection(Fb_Constants.FIRESTORE_KEY_PLAYERSOCIAL);
            }

            /// <summary>
            /// ADD data to a collection with “id” document id. If id is not placed, it will AddAsync granting it its id.
            /// If id is placed, it will overwrite any document with id “id” there is. If there is no document with that id it will add a new one.
            /// The Continue is for the continuation of execution.
            /// </summary>
            /// <param name="collection"></param>
            /// <param name="data"></param>
            /// <param name="id"></param>
            /// <param name="Continue"></param>
            public void Add(CollectionReference collection, Dictionary<string, object> data, string id = "", Action<FirestoreCallResult> Continue = null)
            {
                FirestoreCallResult result = new FirestoreCallResult();
                result.exceptions = new List<FirestoreException>();

                if (id == "" || id == null)
                {
                    // No ID supplied, Firebase system will generate the id
                    Debug.Log("Adding data without supplying id.");

                    collection.AddAsync(data).ContinueWithOnMainThread(task =>
                    {
                        if (task.IsFaulted || task.IsCanceled)
                        {
                            TaskFaulted(task, result);

                            result.success = false;
                            result.data = null;
                            result.oldData = null;

                            Continue(result);
                            return;
                        }

                        if (task.IsCompleted)
                        {
                            Debug.Log("SUCCESS: Added new data via AddAsync!");

                            result.success = true;

                            Dictionary<string, object> returnData = data;

                            returnData.Add("docid", task.Result.Id);

                            result.data = returnData;
                            result.oldData = null;

                            Continue(result);
                            return;
                        }
                    });
                }
                else
                {
                    // ID supplied, overwrite
                    Debug.Log("Adding data by supplying id.");

                    // Check if the document exists already.
                    collection.Document(id).GetSnapshotAsync().ContinueWithOnMainThread((task) =>
                    {
                        if (task.IsFaulted || task.IsCanceled)
                        {
                            TaskFaulted(task, result);

                            result.success = false;
                            result.data = null;
                            result.oldData = null;

                            Continue(result);
                            return;
                        }

                        if (task.Result.Exists)
                        {
                            Debug.Log("That Data already exists, cant overwrite... If you want to overwrite existing documents use UPDATE methods instead!");

                            // Show the player that the user already exists.
                            // This would never happen as a user who has already gone through the auth process
                            // has already created his player.

                            result.success = false;
                            result.data = null;
                            result.oldData = task.Result.ToDictionary();

                            Continue(result);
                            return;
                        }

                        // If the player doesn't exist, its safe to add.
                        collection.Document(id).SetAsync(data).ContinueWithOnMainThread((task2) =>
                        {
                            if (task2.IsFaulted || task2.IsCanceled)
                            {
                                TaskFaulted(task2, result);

                                result.success = false;
                                result.data = null;
                                result.oldData = null;

                                Continue(result);
                                return;
                            }

                            if (task2.IsCompleted)
                            {
                                Debug.Log("SUCCESS: Added new data via SetAsync overwrite!");

                                result.success = true;
                                result.data = data;
                                result.oldData = null;

                                Continue(result);
                                return;
                            }
                        });
                    });
                }
            }

            /// <summary>
            /// GET data from a document of id “id” in a collection
            /// The assignation Continue must be a named function used to assign the result in unity.
            /// </summary>
            /// <param name="collection"></param>
            /// <param name="id"></param>
            /// <param name="AssignationContinue"></param>
            public void Get(CollectionReference collection, string id, Action<FirestoreCallResult> AssignationContinue)
            {
                FirestoreCallResult result = new FirestoreCallResult();
                result.exceptions = new List<FirestoreException>();

                collection.Document(id).GetSnapshotAsync().ContinueWithOnMainThread(task =>
                {
                    if (task.IsFaulted || task.IsCanceled)
                    {
                        TaskFaulted(task, result);

                        result.success = false;
                        result.data = null;
                        result.oldData = null;

                        AssignationContinue(result);
                        return;
                    }

                    if (task.IsCompleted)
                    {
                        Debug.Log("SUCCESS: Found data!");

                        result.success = true;
                        result.data = task.Result.ToDictionary();
                        result.oldData = null;

                        AssignationContinue(result);
                        return;
                    }
                });
            }

            /// <summary>
            /// UPDATE data of a document of id “id” in a collection with “data”.
            /// If paths == null, the update is a MergeAll update, it will update all fields with delta != 0 (has differences). The rest are left intact. See more below.
            /// If paths != null, the update is a MergeFields update, it will update those fields that are indicated in the jagged array paths.
            /// The Continue is used for the continuation of exectution.
            /// </summary>
            /// <param name="collection"></param>
            /// <param name="id"></param>
            /// <param name="data"></param>
            /// <param name="Continue"></param>
            /// <param name="paths"></param>
            public void UpdateDocument(CollectionReference collection, string id, Dictionary<string, object> data, Action<FirestoreCallResult> Continue = null, string[][] paths = null)
            {
                FirestoreCallResult result = new FirestoreCallResult();
                result.exceptions = new List<FirestoreException>();

                if (paths != null)
                {
                    Debug.Log("Updating with specified paths.");

                    FieldPath[] fPaths = new FieldPath[paths.Length];

                    int i = 0;
                    foreach(string[] path in paths)
                    {
                        fPaths[i] = new FieldPath(path);
                        i++;
                    }

                    collection.Document(id).GetSnapshotAsync().ContinueWithOnMainThread(task =>
                    {
                        if (task.IsFaulted || task.IsCanceled)
                        {
                            TaskFaulted(task, result);

                            result.success = false;
                            result.data = null;
                            result.oldData = null;

                            Continue(result);
                            return;
                        }


                        if (task.Result.Exists)
                        {
                            Debug.Log("Found data! Updating it.");
                            result.oldData = task.Result.ToDictionary();
                        }
                        else
                        {
                            Debug.Log("Didnt find data... Cant update!");

                            result.success = false;
                            result.data = null;
                            result.oldData = null;

                            Continue(result);
                            return;
                        }

                        collection.Document(id).SetAsync(data, SetOptions.MergeFields(fPaths)).ContinueWithOnMainThread(task2 =>
                        {
                            if (task2.IsFaulted || task2.IsCanceled)
                            {
                                TaskFaulted(task2, result);

                                result.success = false;
                                result.data = null;

                                Continue(result);
                                return;
                            }

                            if (task2.IsCompleted)
                            {
                                Debug.Log("Updated info in specified paths: ");

                                int j = 0;
                                foreach(string[] path in paths)
                                {
                                    string res = "Path " + j + ":";

                                    foreach(string s in path)
                                    {
                                        res += s + ".";
                                    }

                                    Debug.Log(res);
                                    j++;
                                }

                                Debug.Log("SUCCESS: Document of id: " + id + " updated!");

                                result.success = true;
                                result.data = data;

                                Continue(result);
                                return;
                            }
                        });
                    });
                }
                else
                {
                    Debug.Log("Updating without specified paths.");

                    collection.Document(id).GetSnapshotAsync().ContinueWithOnMainThread(task =>
                    {
                        if (task.IsFaulted || task.IsCanceled)
                        {
                            TaskFaulted(task, result);

                            result.success = false;
                            result.data = null;
                            result.oldData = null;

                            Continue(result);
                            return;
                        }


                        if (task.Result.Exists)
                        {
                            Debug.Log("Found data! Updating it.");
                            result.oldData = task.Result.ToDictionary();
                        }
                        else
                        {
                            Debug.Log("Didnt find data... Cant update!");

                            result.success = false;
                            result.data = null;
                            result.oldData = null;

                            Continue(result);
                            return;
                        }

                        // If found, update information
                        collection.Document(id).SetAsync(data, SetOptions.MergeAll).ContinueWithOnMainThread(task2 =>
                        {
                            if (task2.IsFaulted || task2.IsCanceled)
                            {
                                TaskFaulted(task2, result);

                                result.success = false;
                                result.data = null;

                                Continue(result);
                                return;
                            }

                            if (task2.IsCompleted)
                            {
                                Debug.Log("SUCCESS: Document of id: " + id + " updated!");

                                result.success = true;
                                result.data = data;

                                Continue(result);
                                return;
                            }
                        });
                    });
                }
            }

            /// <summary>
            /// UPDATES the array of type T field “name” in the document of id “id” in a collection with the data of type T.
            /// If the element is within the array already It wont add it, it doesn't admit repetitions.
            /// Can only do this with primitive T types, namely: string, number, bool or null. For more complex data, those will have their own separate methods.
            /// Admits Dictionaries(maps), but the equals condition is that every field in the dictionary is the same (absolute equality).
            /// Exhaustive list: string, char, int, long, float, double, bool, short and Dictionary<string, object>.
            /// PRE: The array in Firestore MUST be of equivalent type T.
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="collection"></param>
            /// <param name="id"></param>
            /// <param name="data"></param>
            /// <param name="fieldPath"></param>
            /// <param name="Continue"></param>
            public void UpdateUniquelyArrayPrimitive<T>(CollectionReference collection, string id, T data, string fieldPath, Action<FirestoreCallResult> Continue = null)
            {
                FirestoreCallResult result = new FirestoreCallResult();
                result.exceptions = new List<FirestoreException>();

                Type t = typeof(T);
                if(t == typeof(int) ||
                    t == typeof(long) ||
                     t == typeof(short) ||
                      t == typeof(float) ||
                       t == typeof(double) ||
                        t == typeof(string) ||
                         t == typeof(char) ||
                          t == typeof(bool) || t == typeof(Dictionary<string, object>))
                {
                    // Type admitted

                    collection.Document(id).UpdateAsync(fieldPath, FieldValue.ArrayUnion(data)).ContinueWithOnMainThread(task =>
                    {
                        if(task.IsFaulted || task.IsCanceled)
                        {
                            TaskFaulted(task, result);

                            result.success = false;
                            result.data = null;
                            result.oldData = null;

                            Continue(result);
                            return;
                        }

                        if (task.IsCompleted)
                        {
                            Debug.Log("Array of path: " + fieldPath + " updated successfuly!");

                            result.success = true;
                            result.data = null;
                            result.oldData = null;

                            Continue(result);
                            return;
                        }
                    });
                }
                else
                {
                    Debug.Log("Use of wrong method, the type T you included isn't accepted. For complex types look for the adequate method, or make it yourself!");

                    result.success = false;
                    result.data = null;
                    result.oldData = null;
                    result.exceptions.Add(new FirestoreException(FirestoreError.Cancelled, "Wrong Method, type T of " + t.ToString() + " not admitted."));

                    Continue(result);
                }
            }

            /// <summary>
            /// UPDATES the array of type T field “name” in the document of id “id” in a collection with the data "data".
            /// Use this for non-primitive types. For primitive types use the non-generic as its faster.
            /// This updates the array uniquely, that is, given an equals condition, if there is a T in the list considered equal within it,
            /// it wont add the new one. (Maintains Primary Key Condition)
            /// The EqualsPredicate is the condition provided for two maps to be equal (define PK fields).
            /// PRE: The array in Firestore MUST be of equivalent type T.
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="collection"></param>
            /// <param name="id"></param>
            /// <param name="data"></param>
            /// <param name="fieldPath"></param>
            /// <param name="EqualsPredicate"></param>
            /// <param name="Continue"></param>
            public void UpdateUniquelyArrayGeneric<T>(CollectionReference collection, string id, T data, string fieldPath, Func<T, T, bool> EqualsPredicate, Action<FirestoreCallResult> Continue = null)
            {
                FirestoreCallResult result = new FirestoreCallResult();
                result.exceptions = new List<FirestoreException>();

                Debug.Log("Updating Uniquely on Array:");
                // First search for a T that is equal (defined)
                collection.Document(id).GetSnapshotAsync().ContinueWithOnMainThread(task =>
                {
                    if(task.IsFaulted || task.IsCanceled)
                    {
                        TaskFaulted(task, result);

                        result.data = null;
                        result.success = false;
                        result.oldData = null;

                        return;
                    }

                    if (task.Result.Exists)
                    {
                        Debug.Log("Data found! Checking for equals...");
                        // There's data
                        Dictionary<string, object> broughtData = task.Result.ToDictionary();
                        result.oldData = broughtData;

                        List<object> auxList = broughtData["test"] as List<object>;
                        List<T> list = new List<T>();
                        foreach(object o in auxList)
                        {
                            list.Add((T) o );
                        }

                        foreach (T comparee in list)
                        {
                            if (EqualsPredicate(data, comparee))
                            {
                                // If found, tell the user that there is an equal object in the array
                                Debug.Log("Found the item in the array, can't add to maintain Primary Key condition!");

                                result.exceptions.Add(new FirestoreException(FirestoreError.Cancelled, "The array has an object with the same Primary Key supplied by the data object."));

                                result.success = false;
                                result.data = null;

                                Continue(result);
                                return;
                            }
                        }

                        // If not found, its safe to add
                        Debug.Log("Did not find the object in the array, its safe to add.");
                        collection.Document(id).UpdateAsync(fieldPath, FieldValue.ArrayUnion(data)).ContinueWithOnMainThread(task2 =>
                        {
                            if(task.IsFaulted || task.IsCanceled)
                            {
                                TaskFaulted(task2, result);

                                result.data = null;
                                result.success = false;

                                return;
                            }

                            if (task2.IsCompleted)
                            {
                                Debug.Log("Array of path: " + fieldPath + " updated successfuly!");

                                result.success = true;
                                result.data = null;

                                return;
                            }
                        });
                    }
                });
            }

            /// <summary>
            /// REMOVES the data "data" from array of type Dictionary field “name” in the document of id “id” in a collection.
            /// </summary>
            /// <param name="collection"></param>
            /// <param name="id"></param>
            /// <param name="data"></param>
            /// <param name="fieldPath"></param>
            /// <param name="Continue"></param>
            public void RemoveInArrayGeneric<T>(CollectionReference collection, string id, T data, string fieldPath, Func<T, T, bool> EqualsPredicate, Action<FirestoreCallResult> Continue = null)
            {
                FirestoreCallResult result = new FirestoreCallResult();
                result.exceptions = new List<FirestoreException>();

                Debug.Log("Removing from array:");
                // First search for a T that is equal (defined)
                collection.Document(id).GetSnapshotAsync().ContinueWithOnMainThread(task =>
                {
                    if (task.IsFaulted || task.IsCanceled)
                    {
                        TaskFaulted(task, result);

                        result.data = null;
                        result.success = false;
                        result.oldData = null;

                        return;
                    }

                    if (task.Result.Exists)
                    {
                        Debug.Log("Data found! Checking for equals...");
                        // There's data
                        Dictionary<string, object> broughtData = task.Result.ToDictionary();
                        result.oldData = broughtData;

                        List<object> auxList = broughtData[fieldPath] as List<object>;
                        List<T> list = new List<T>();
                        foreach (object o in auxList)
                        {
                            list.Add((T)o);
                        }

                        foreach (T comparee in list)
                        {
                            if (EqualsPredicate(data, comparee))
                            {
                                // If found, delete that element from the array using the comparee!
                                Debug.Log("Found the item in the array, deleting...");

                                collection.Document(id).UpdateAsync(fieldPath, FieldValue.ArrayRemove(comparee)).ContinueWithOnMainThread(task2 =>
                                {
                                    if(task2.IsFaulted || task2.IsCanceled)
                                    {
                                        TaskFaulted(task2, result);

                                        result.success = false;
                                        result.data = null;

                                        Continue(result);
                                        return;
                                    }

                                    if (task2.IsCompleted)
                                    {
                                        Debug.Log("Item successfully deleted in document of ID: " + id + ", on field: " + fieldPath);

                                        result.success = true;
                                        result.data = null;

                                        Continue(result);
                                        return;
                                    }
                                });
                            }
                        }
                    }
                });
            }

            /// <summary>
            /// REMOVES the data "data" from array of type T field “name” in the document of id “id” in a collection.
            /// Can only do this with primitive T types, namely: string, number, bool or null. For more complex data, those will have their own separate methods.
            /// Admits Dictionaries, but will remove dictionaries that are absolutely equal (all fields).
            /// Exhaustive list: string, char, int, long, float, double, bool, short.
            /// PRE: The array in Firestore MUST be of equivalent type T.
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="collection"></param>
            /// <param name="id"></param>
            /// <param name="data"></param>
            /// <param name="fieldPath"></param>
            /// <param name="Continue"></param>
            public void RemoveInArray<T>(CollectionReference collection, string id, T data, string fieldPath, Action<FirestoreCallResult> Continue = null)
            {
                FirestoreCallResult result = new FirestoreCallResult();
                result.exceptions = new List<FirestoreException>();

                Type t = typeof(T);
                if (t == typeof(int) ||
                    t == typeof(long) ||
                     t == typeof(short) ||
                      t == typeof(float) ||
                       t == typeof(double) ||
                        t == typeof(string) ||
                         t == typeof(char) ||
                          t == typeof(bool) || t == typeof(Dictionary<string, object>))
                {
                    // Type admitted

                    collection.Document(id).UpdateAsync(fieldPath, FieldValue.ArrayRemove(data)).ContinueWithOnMainThread(task =>
                    {
                        if (task.IsFaulted || task.IsCanceled)
                        {
                            TaskFaulted(task, result);

                            result.success = false;
                            result.data = null;
                            result.oldData = null;

                            Continue(result);
                            return;
                        }

                        if (task.IsCompleted)
                        {
                            Debug.Log("Array of path: " + fieldPath + ", element removed successfully!");

                            result.success = true;
                            result.data = null;
                            result.oldData = null;

                            Continue(result);
                            return;
                        }
                    });
                }
                else
                {
                    Debug.Log("Use of wrong method, the type T you included isn't accepted. For complex types look for the adequate method, or make it yourself!");

                    result.success = false;
                    result.data = null;
                    result.oldData = null;
                    result.exceptions.Add(new FirestoreException(FirestoreError.Cancelled, "Wrong Method, type T of " + t.ToString() + " not admitted."));

                    Continue(result);
                }
            }

            /// <summary>
            /// DELETES a document of id “id” within a collection.
            /// The Continue brings back information of wether the operation was successful or not.
            /// </summary>
            /// <param name="collection"></param>
            /// <param name="id"></param>
            /// <param name="Continue"></param>
            public void Delete(CollectionReference collection, string id, Action<FirestoreCallResult> Continue)
            {
                // TODO: Do this but in a trusted environment (Cloud functions).
            }

            private void TaskFaulted(Task task, FirestoreCallResult result)
            {
                foreach (FirestoreException e in task.Exception.Flatten().InnerExceptions)
                {
                    result.exceptions.Add(e);
                }

                if (result.exceptions.Count == 0) Debug.Log("NO FIRESTORE EXCEPTIONS FOUND: Weird.");

                FirestoreException error = (FirestoreException)task.Exception.Flatten().InnerException;

                Debug.Log("Couldn't get data, something went wrong: " + error.Message);
            }

            private void TaskFaulted(Task<DocumentSnapshot> task, FirestoreCallResult result)
            {
                foreach (FirestoreException e in task.Exception.Flatten().InnerExceptions)
                {
                    result.exceptions.Add(e);
                }

                if (result.exceptions.Count == 0) Debug.Log("NO FIRESTORE EXCEPTIONS FOUND: Weird.");

                FirestoreException error = (FirestoreException)task.Exception.Flatten().InnerException;

                Debug.Log("Couldn't get data, something went wrong: " + error.Message);
            }

            private void TaskFaulted(Task<DocumentReference> task, FirestoreCallResult result)
            {
                foreach (FirestoreException e in task.Exception.Flatten().InnerExceptions)
                {
                    result.exceptions.Add(e);
                }

                if (result.exceptions.Count == 0) Debug.Log("NO FIRESTORE EXCEPTIONS FOUND: Weird.");

                FirestoreException error = (FirestoreException)task.Exception.Flatten().InnerException;

                Debug.Log("Couldn't get data, something went wrong: " + error.Message);
            }

            /*
             QUICK REFERENCE:

             task.IsCompleted != "task finished successfully", a task can be completed and faulted.

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

             docRef.DeleteAsync();

             Dictionary<string, object> updates = new Dictionary<string, object>
             {
                 { "Capital", FieldValue.Delete }
             };
             await cityRef.UpdateAsync(updates);

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

            FIELD PATHS
                string[] paths = { "testdict", "Atr1" };
                FieldPath[] fields = { new FieldPath(paths) }; => testdict.Atr1, only takes string arrays not string lists.

                new FieldPath(paths) creates a reference to the nested Atr1 in the testdict dictionary.

            ARRAYUNION and ARRAYDELETE:

                You can use FieldValue.ArrayUnion(object) to add an element to an array if its not already in there. If its already there, it updates it.
                You use it within the UpdateAsync function of DocumentReference.
                FieldValue.ArrayDelete(object) deletes all the array elements equal to that object.

                Similarly you can use FieldValue.Increment(incAmount) to increment simple numbers.
             */
        }
    }
}


