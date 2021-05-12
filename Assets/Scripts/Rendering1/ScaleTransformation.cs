using UnityEngine;

namespace Rendering1
{
    public class ScaleTransformation : Transformation
    {
        [SerializeField] private Vector3 _scale = default;
        
        public override Vector3 Apply(Vector3 point)
        {
            point.x *= _scale.x;
            point.y *= _scale.y;
            point.z *= _scale.z;

            return point;
        }
    }
}