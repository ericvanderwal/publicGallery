using UnityEngine;

namespace PublicGallery
{
    public class GalleryObject : MonoBehaviour
    {
        [SerializeField]
        private GameObject planeGameObject;

        [SerializeField]
        private Orientation orientation;

        public Orientation GetOrientation => orientation;

        public void SetImage(Texture2D texture2D)
        {
            Renderer renderer = planeGameObject.GetComponent<Renderer>();
            if (renderer != null) renderer.material.mainTexture = texture2D;
        }
    }
}