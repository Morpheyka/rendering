using UnityEngine;

namespace Rendering1
{
    public class PositionTransformation : Transformation
    {
        [SerializeField] private Vector3 _position = default;
        
        public override Vector3 Apply(Vector3 point)
        {
            return point + _position;
        }
    }
}