using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine.Serialization;

namespace PublicGallery
{
    /// <summary>
    /// Download texture 2d images and place on prefab 3d object frames
    /// within the scene a preselected locations
    /// </summary>
    public class DownloadManager : MonoBehaviour
    {
        [Header("Setup")]
        [Space(3)]
        [SerializeField]
        [Tooltip("List of URLs to download 2d images from the web")]
        private List<string> urlsToDownload;

        [SerializeField]
        [Tooltip("")]
        private bool randomizeMaterialSelection;

        [FormerlySerializedAs("paintingLocations")]
        [SerializeField]
        [Tooltip(
            "An array of locations for frame prefabs with images to be optionally places. These can be randomized or in order")]
        private List<GameObject> openArtLocations;

        [SerializeField]
        [Tooltip("Should the paintings be places randomly? If not, will be places in order")]
        private bool randomizePaintingPlacementOrder;

        [SerializeField]
        [Tooltip(
            "An array of frame prefab gameobjects. It should include Portrait, landscape and square frames if you expect those shapes in your gallery")]
        private GameObject[] framePrefabs;

        [SerializeField]
        [Tooltip("")]
        private bool randomizeFrameSelection;

        [Space(20)]
        [Header("Debug Only")]
        [Space(3)]
        [SerializeField]
        private bool enableDebug;

        [SerializeField]
        private List<Texture2D> unplacedTextures;

        [SerializeField]
        private List<GameObject> resultArtFinalArts;

        [SerializeField]
        private List<GameObject> usedArtPositions;

        [SerializeField]
        private List<Texture2D> placedTextures;


        void Start()
        {
            unplacedTextures = new List<Texture2D>();
            usedArtPositions = new List<GameObject>();
            placedTextures = new List<Texture2D>();
            resultArtFinalArts = new List<GameObject>();
            StartCoroutine(DownloadUrls());
        }

        IEnumerator DownloadUrls()
        {
            if (enableDebug) Debug.Log("Beginning images download");

            foreach (string url in urlsToDownload)
            {
                UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
                yield return request.SendWebRequest();

                if (request.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError("Failed to download URL: " + url);
                }
                else
                {
                    Texture2D texture = DownloadHandlerTexture.GetContent(request);
                    unplacedTextures.Add(texture);
                    if (enableDebug) Debug.Log("Downloaded texture " + unplacedTextures.Count);
                }
            }

            if (enableDebug) Debug.Log("Downloaded a total textures of: " + unplacedTextures.Count);

            PlaceImages();
        }


        /// <summary>
        /// Place image within Scene. If no more materials or no more placement location exist
        /// the placement of images will stop. 
        /// </summary>
        public void PlaceImages()
        {
            if (enableDebug) Debug.Log("Placing Image start");

            int totalCount = unplacedTextures.Count;

            // using the total number of images to place, iterate through
            for (var index = 0; index < totalCount; index++)
            {
                // get next texture
                Texture2D texture2D = GetNextTexture();

                // if no more textures, stop placing
                if (texture2D == null) return;

                // get next placement location
                GameObject placeLocation = GetNextPlaceLocation();

                // if no more place locations, stop placing
                if (placeLocation == null) return;

                // get matching frame prefab
                GameObject framePrefab = GetFramePrefab(texture2D);

                // no matching frames, must be a problem, stop placing to avoid error
                if (framePrefab == null) return;

                // instantiate the art and frame together. Place in the scene.
                var success = CreateArt(texture2D, framePrefab, placeLocation);
            }
        }

        /// <summary>
        /// Get the next free placement location within the gallery
        /// </summary>
        /// <returns></returns>
        private GameObject GetNextPlaceLocation()
        {
            GameObject location = null;
            if (openArtLocations.Count == 0) return null;

            if (randomizePaintingPlacementOrder)
            {
                location = openArtLocations[Random.Range(0, openArtLocations.Count - 1)];
            }
            else
            {
                location = openArtLocations[0];
            }

            // remove location from list
            openArtLocations.Remove(location);
            usedArtPositions.Add(location);


            return location;
        }

        /// <summary>
        /// Get the next free texture available
        /// </summary>
        /// <returns></returns>
        private Texture2D GetNextTexture()
        {
            // no more textures
            if (unplacedTextures.Count == 0) return null;
            Texture2D texture2D = null;

            if (randomizeMaterialSelection)
            {
                texture2D = unplacedTextures[Random.Range(0, unplacedTextures.Count - 0)];
            }
            else
            {
                texture2D = unplacedTextures[0];
            }

            // remove material from list
            unplacedTextures.Remove(texture2D);
            placedTextures.Add(texture2D);

            return texture2D;
        }

        /// <summary>
        /// Get a frame prefab of the correct orientation
        /// </summary>
        /// <param name="texture2D"></param>
        /// <returns></returns>
        private GameObject GetFramePrefab(Texture2D texture2D)
        {
            Orientation orientation = Helper.GetOrientation(texture2D);
            if (enableDebug) Debug.Log("material orientation is " + orientation);

            foreach (var frame in framePrefabs)
            {
                GalleryObject galleryObject = frame.GetComponent<GalleryObject>();
                if (galleryObject == null) break;

                if (galleryObject != null && galleryObject.GetOrientation == Helper.GetOrientation(texture2D))
                {
                    if (enableDebug) Debug.Log("Found a matching orientation prefab ");
                    return galleryObject.gameObject;
                }
            }

            return null;
        }


        /// <summary>
        /// Instantiate and place frame in the correct location with art. Return the placed art gameobject completed
        /// </summary>
        /// <param name="texture2D"></param>
        /// <param name="framePrefab"></param>
        private bool CreateArt(Texture2D texture2D, GameObject framePrefab, GameObject location)
        {
            // get prefab
            GameObject placedFrameObject = Instantiate(framePrefab, location.transform.position,
                location.transform.rotation);

            // if there is a problem, return fail
            if (placedFrameObject == null) return false;

            // set image
            GalleryObject placedGalleryObject = placedFrameObject.GetComponent<GalleryObject>();
            placedGalleryObject.SetImage(texture2D);

            // list of all placed object for possible future reference
            resultArtFinalArts.Add(placedFrameObject);

            return placedFrameObject;
        }
    }
}