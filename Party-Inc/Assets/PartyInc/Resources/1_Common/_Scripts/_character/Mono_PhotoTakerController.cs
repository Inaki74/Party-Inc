using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace PartyInc
{
    using PartyFirebase.Storage;
    using PartyFirebase.Auth;

    public class Mono_PhotoTakerController : MonoBehaviour
    {
        private static string path;

        [SerializeField] private Camera _photoCamera;
        private bool _photoTakenOnFrame;

        [SerializeField] private int _width;
        [SerializeField] private int _height;

        private void Awake()
        {
            path = "player-data/{0}/images/account-image.jpg";
            Camera.onPostRender += RunPostRender;
        }

        private void OnDestroy()
        {
            Camera.onPostRender -= RunPostRender;
        }

        private void RunPostRender(Camera cam)
        {
            if (_photoTakenOnFrame && cam.Equals(_photoCamera))
            {
                _photoTakenOnFrame = false;
                RenderTexture myPhotoRendered = _photoCamera.targetTexture;

                Texture2D myPhoto = new Texture2D(myPhotoRendered.width, myPhotoRendered.height, TextureFormat.ARGB32, false);
                Rect rectangle = new Rect(0, 0, myPhotoRendered.width, myPhotoRendered.height);

                myPhoto.ReadPixels(rectangle, 0, 0);

                // Wait for confirm (Shows two buttons, confirm , retake)
                // On yes save photo and send it to DB
                // On no, cleanup

                byte[] photoEncoded = myPhoto.EncodeToJPG();

                string url = string.Format(path, Fb_FirebaseAuthenticateManager.Current.Auth.CurrentUser.UserId); //
                Fb_FirebaseStorageManager.Current.UploadByteArrayAtURL(photoEncoded, url, res =>
                {
                    if (res.success)
                    {
                        Debug.Log("Upload successful!");
                        Debug.Log("URL: " + url);
                    }
                    else
                    {
                        Debug.Log("Upload failed!");
                        Debug.Log(res.exceptions[0].Message);
                    }
                });

                RenderTexture.ReleaseTemporary(myPhotoRendered);
                _photoCamera.targetTexture = null;
            }
        }

        public void TakePhotoWithInternalData()
        {

            _photoCamera.targetTexture = RenderTexture.GetTemporary(94, 86, 16); // 94x86 is the resolution that works here for this photo marker. Didnt do it adaptive. BASICALLY DONT CHANGE THIS.
            _photoTakenOnFrame = true;
        }

        public void TakePhoto(int width, int height)
        {
            _photoTakenOnFrame = true;
            _photoCamera.targetTexture = RenderTexture.GetTemporary(width, height, 16);
        }
    }
}


