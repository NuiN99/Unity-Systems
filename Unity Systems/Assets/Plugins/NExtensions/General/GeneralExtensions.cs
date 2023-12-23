namespace NExtensions.General.Extensions
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    
    public static class GeneralExtensions
    {
        public static Vector3 With(this Vector3 v3, float? x = null, float? y = null, float? z = null) =>
            new Vector3(x ?? v3.x, y ?? v3.y, z ?? v3.z);
        
        public static Vector3 Add(this Vector3 v3, float x = 0, float y = 0, float z = 0) =>
             new Vector3(v3.x + x, v3.y + y, v3.z + z);

        public static Transform Set(this Transform transform, float? x = null, float? y = null, float? z = null)
        {
            transform.position = new Vector3(x ?? transform.position.x, y ?? transform.position.y, z ?? transform.position.z);
            return transform;
        }
        public static Transform Add(this Transform transform, float x = 0, float y = 0, float z = 0)
        {
            transform.position += new Vector3(x, y, z);
            return transform;
        }
        
        public static GameObject Set(this GameObject gameObject, float? x = null, float? y = null, float? z = null)
        {
            Transform transform = gameObject.transform;
            transform.position = new Vector3(x ?? transform.position.x, y ?? transform.position.y, z ?? transform.position.z);
            return gameObject;
        }
        public static GameObject Add(this GameObject gameObject, float x = 0, float y = 0, float z = 0)
        {
            gameObject.transform.position += new Vector3(x, y, z);
            return gameObject;
        }
    }
}
