using UnityEngine;

namespace Rendering1
{
    public class RotationTransformation : Transformation
    {
        [SerializeField] private Vector3 _rotation = default;
        
        public override Vector3 Apply(Vector3 point)
        {
            return point;
        }
    }
}