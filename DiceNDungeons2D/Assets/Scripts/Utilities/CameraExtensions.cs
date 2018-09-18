using UnityEngine;

namespace Utilities
{
    public static class CameraExtensions  {
        public static Vector3 WorldToUIPoint(this Camera camera, Vector3 pos)
        {
            var uiPos = camera.WorldToScreenPoint(pos);
            uiPos.Set(uiPos.x, Screen.height - uiPos.y, uiPos.z);

            return uiPos;
        }
    }
}
