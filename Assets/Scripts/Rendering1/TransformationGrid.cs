using System;
using UnityEngine;

namespace Rendering1
{
    public class TransformationGrid : MonoBehaviour
    {
        [Header("Point")]
        [SerializeField] private PrimitiveType _prefab = PrimitiveType.Cube;
        [SerializeField] private Vector3 _pointSize = Vector3.one;
        
        [Header("Grid")]
        [SerializeField] private uint _resolution = 10;

        private Transform[] _grid = Array.Empty<Transform>();
        private Transformation[] _transformation = null; 

        private void Awake()
        {
            _transformation = GetComponents<Transformation>();
            _grid = new Transform[_resolution * _resolution * _resolution];

            for (int z = 0, i = 0; z < _resolution; z++)
                for (var y = 0; y < _resolution; y++)
                    for (var x = 0; x < _resolution; i++, x++)
                        _grid[i] = CreatePoint(x,y,z);
        }

        private Transform CreatePoint(int x, int y, int z)
        {
            var instance = GameObject.CreatePrimitive(_prefab);
            var point = instance.transform;
            point.SetParent(transform);
            point.localPosition = CalculateCoordinates(x, y, z);
            point.localScale = _pointSize;
            point.GetComponent<Renderer>().material.color = new Color(
                (float) x / _resolution,
                (float) y / _resolution,
                (float) z / _resolution);

            return point;
        }

        private Vector3 CalculateCoordinates(int x, int y, int z)
        {
            return new Vector3(
                x - (_resolution - 1) * 0.5f,
                y - (_resolution - 1) * 0.5f,
                z - (_resolution - 1) * 0.5f);
        }

        private void Update()
        {
            for (int z = 0, i = 0; z < _resolution; z++)
                for (var y = 0; y < _resolution; y++)
                    for (var x = 0; x < _resolution; i++, x++)
                        _grid[i].localPosition = TransformPoint(x, y, z);
        }

        private Vector3 TransformPoint(int x, int y, int z)
        {
            var coordinates = CalculateCoordinates(x, y, z);

            for (var i = 0; i < _transformation.Length; i++)
                _transformation[i].Apply(coordinates);

            return coordinates;
        }
    }
}