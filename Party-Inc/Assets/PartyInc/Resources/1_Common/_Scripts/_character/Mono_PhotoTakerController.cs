using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace PartyInc
{
    public class Mono_PhotoTakerController : MonoBehaviour
    {
        private static string path;

        [SerializeField] private Camera _photoCamera;
        private bool _photoTakenOnFrame;

        [SerializeField] private int _width;
        [SerializeField] private int _height;

        private void Awake()
        {
            path = Application.dataPath + "/PartyInc/Resources/5_Experimental/Inaki/HEReIAM.jpg";
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

                byte[] photoEncoded = myPhoto.EncodeToJPG();
                System.IO.File.WriteAllBytes(path, photoEncoded);
                print("Photo taken in: " + path);

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


