namespace NExtensions.General.Utilities
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    
    public static class GeneralUtils
    {
        public static Vector3 Direction(Vector3 start, Vector3 end, float magnitude = 1f) =>
            (end - start).normalized * magnitude;

        public static float DirectionAngle(Vector3 direction) => 
            Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
    }
}

