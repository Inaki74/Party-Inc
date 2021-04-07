using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Storage;
using Firebase.Extensions;
using UnityEngine.UI;
using System;

namespace PartyInc
{
    namespace PartyFirebase.Storage
    {
        public struct FireStorageCallResult
        {
            public bool success;
            public byte[] payload;
            public List<StorageException> exceptions;
        }

        public class Fb_FirebaseStorageManager : MonoSingleton<Fb_FirebaseStorageManager>
        {
            private const string MainBucketURL = "gs://partyinc-testing.appspot.com";

            private FirebaseStorage _storage;
            public FirebaseStorage Storage
            {
                get
                {
                    return _storage;
                }
                private set
                {
                    _storage = value;
                }
            }

            private StorageReference _mainBucket;

            [SerializeField] private Text _errorText;
            private bool _runStartOnce;

            public bool StorageInitialized { get; private set; }

            private void Start()
            {
                StorageInitialized = false;
                if (_runStartOnce) return;

                _runStartOnce = true;
                Fb_FirebaseManager.Current.InitializeService(StorageInit);
            }

            private void StorageInit()
            {
                _storage = FirebaseStorage.DefaultInstance;

                _mainBucket = _storage.GetReferenceFromUrl(MainBucketURL);
            }

            public void UploadByteArrayAtURL(byte[] data, string url, Action<FireStorageCallResult> Callback = null)
            {
                FireStorageCallResult result = new FireStorageCallResult();
                result.exceptions = new List<StorageException>();

                StorageReference urlReference = _mainBucket.Child(url);

                urlReference.PutBytesAsync(data).ContinueWithOnMainThread(task =>
                {
                    if(task.IsFaulted || task.IsCanceled)
                    {
                        StorageException error = (StorageException)(task.Exception.Flatten().InnerException as StorageException);
                        ManageErrorCodes(error);

                        if (error.ErrorCode == StorageException.ErrorRetryLimitExceeded)
                        {
                            // Retry
                            Debug.Log("An unknown error happened, trying again...");
                            UploadByteArrayAtURL(data, url, Callback);

                            return;
                        }

                        result.exceptions.Add(error);
                        result.success = false;
                        result.payload = null;
                        Debug.Log("Upload failed: " + error.ToString());

                        Callback(result);
                        return;
                    }

                    if (task.IsCompleted)
                    {
                        StorageMetadata metadata = task.Result;
                        string md5Hash = metadata.Md5Hash;
                        Debug.Log("Finished uploading...");
                        Debug.Log("md5 hash = " + md5Hash);

                        result.success = true;
                        result.payload = null;

                        Callback(result);
                        return;
                    }
                });
            }

            public void DownloadByteArrayFromURL(string url, long expectedSize, Action<FireStorageCallResult> Callback = null)
            {
                FireStorageCallResult result = new FireStorageCallResult();
                result.exceptions = new List<StorageException>();

                StorageReference urlReference = _mainBucket.Child(url);

                urlReference.GetBytesAsync(expectedSize).ContinueWithOnMainThread(task =>
                {
                    if (task.IsFaulted || task.IsCanceled)
                    {
                        StorageException error = (StorageException)(task.Exception.Flatten().InnerException as StorageException);
                        ManageErrorCodes(error);

                        if (error.ErrorCode == StorageException.ErrorRetryLimitExceeded)
                        {
                            // Retry
                            Debug.Log("An unknown error happened, trying again...");
                            DownloadByteArrayFromURL(url, expectedSize, Callback);

                            return;
                        }

                        result.exceptions.Add(error);
                        result.success = false;
                        result.payload = null;
                        Debug.Log("Download failed: " + error.ToString());

                        Callback(result);
                        return;
                    }

                    if (task.IsCompleted)
                    {
                        Debug.Log("Download completed!");

                        result.success = true;
                        result.payload = task.Result;

                        Callback(result);
                        return;
                    }
                });
            }

            public void DeleteResourceOnURL(string url, Action<FireStorageCallResult> Callback = null)
            {
                FireStorageCallResult result = new FireStorageCallResult();
                result.exceptions = new List<StorageException>();

                StorageReference urlReference = _mainBucket.Child(url);

                urlReference.DeleteAsync().ContinueWithOnMainThread(task =>
                {
                    if (task.IsFaulted || task.IsCanceled)
                    {
                        StorageException error = (StorageException)(task.Exception.Flatten().InnerException as StorageException);
                        ManageErrorCodes(error);

                        if (error.ErrorCode == StorageException.ErrorRetryLimitExceeded)
                        {
                            // Retry
                            Debug.Log("An unknown error happened, trying again...");
                            DeleteResourceOnURL(url, Callback);

                            return;
                        }

                        result.exceptions.Add(error);
                        result.success = false;
                        result.payload = null;
                        Debug.Log("Deletion failed: " + error.ToString());

                        Callback(result);
                        return;
                    }

                    if (task.IsCompleted)
                    {
                        Debug.Log("Deletion completed!");

                        result.success = true;
                        result.payload = null;

                        Callback(result);
                        return;
                    }
                });
            }

            public void SetErrorMessage(string message, Color state)
            {
                _errorText.color = state;
                _errorText.text = message;

                StopCoroutine(ResetErrorMessage());
                StartCoroutine(ResetErrorMessage());
            }

            private IEnumerator ResetErrorMessage()
            {
                yield return new WaitForSeconds(5f);

                _errorText.text = "";
            }

            public void ManageErrorCodes(StorageException errorCode)
            {
                switch (errorCode.ErrorCode)
                {
                    // https://firebase.google.com/docs/storage/unity/handle-errors
                    case StorageException.ErrorRetryLimitExceeded: // 
                        SetErrorMessage("An unknown error happened, trying again...", Color.yellow);
                        break;
                    case StorageException.ErrorUnknown: // 
                        SetErrorMessage("And unknown error happened, try again later!", Color.yellow);
                        break;
                    case StorageException.ErrorBucketNotFound: // 
                        SetErrorMessage("Fatal error, please contact Support.", Color.red);
                        break;
                    case StorageException.ErrorNotAuthorized: // 
                        SetErrorMessage("You cant do this right now.", Color.red);
                        break;
                    case StorageException.ErrorObjectNotFound: // 
                        SetErrorMessage("Couldn't find what you wanted.", Color.red);
                        break;
                    case StorageException.ErrorQuotaExceeded: // 
                        SetErrorMessage("Fatal error, please contact Support.", Color.red);
                        break;
                    case StorageException.ErrorCanceled: // 
                        SetErrorMessage("The action was cancelled.", Color.yellow);
                        break;
                }
            }

            /*
            Local Reference:

            Storage Reference:

            // Create a child reference
            // imagesRef now points to "images"
            StorageReference imagesRef = storageRef.Child("images");

            // Child references can also take paths delimited by '/' such as:
            // "images/space.jpg".
            StorageReference spaceRef = imagesRef.Child("space.jpg");
            // spaceRef now points to "images/space.jpg"
            // imagesRef still points to "images"

            // This is equivalent to creating the full referenced
            StorageReference spaceRefFull = storage.GetReferenceFromUrl(
            "gs://<your-cloud-storage-bucket>/images/space.jpg");

            // Parent allows us to move to the parent of a reference
            // imagesRef now points to 'images'
            StorageReference imagesRef = spaceRef.Parent;

            // Root allows us to move all the way back to the top of our bucket
            // rootRef now points to the root
            StorageReference rootRef = spaceRef.Root;

            // Reference's path is: "images/space.jpg"
            // This is analogous to a file path on disk
            string path = spaceRef.Path;

            // Reference's name is the last segment of the full path: "space.jpg"
            // This is analogous to the file name
            string name = spaceRef.Name;

            // Reference's bucket is the name of the storage bucket where files are stored
            string bucket = spaceRef.Bucket;
             */
        }
    }
}



