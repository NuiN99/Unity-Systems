using UnityEngine;

namespace NuiN.NExtensions
{
    public static class LayerExtensions
    {
        public static bool ContainsLayer(this LayerMask layerMask, GameObject layerObject)
        {
            return layerMask == (layerMask | (1 << layerObject.layer));
        }
        public static bool ContainsLayer(this LayerMask layerMask, int layer)
        {
            return layerMask == (layerMask | (1 << layer));
        }
    }

}