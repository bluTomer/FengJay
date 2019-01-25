using UnityEngine;

namespace Scripts
{
    public static class Utils
    {
        public static void DestroyGameObject(this Component obj)
        {
            if (Application.isPlaying)
            {
                Object.Destroy(obj.gameObject);
            }
            else
            {
                Object.DestroyImmediate(obj.gameObject);
            }
        }
    }
}