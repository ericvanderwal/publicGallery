using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PublicGallery
{
    public static class Helper
    {
        /// <summary>
        /// Returns the orientation of a texture 2d image
        /// </summary>
        /// <param name="texture2D"></param>
        /// <returns></returns>
        public static Orientation GetOrientation(Texture2D texture2D)
        {
            float aspectRatio = (float)texture2D.width / texture2D.height;

            if (aspectRatio > 1f)
            {
                return Orientation.Landscape;
            }
            else
                if (aspectRatio < 1f)
                {
                    return Orientation.Portrait;
                }
                else
                {
                    return Orientation.Square;
                }
        }
    }
}