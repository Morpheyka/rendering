using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

namespace Rendering0
{
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    public class Grid : MonoBehaviour
    {
        [SerializeField] private Vector2Int _size = default;

        private MeshFilter _filter = null;
        private MeshRenderer _renderer = null;

        private const int CHUNK_SIZE = 65535;

        private void Awake()
        {
            _filter = GetComponent<MeshFilter>();
            _renderer = GetComponent<MeshRenderer>();
        }

        private void Start()
        {
            Generate();
        }

        private void Generate()
        {
            var width = _size.x;
            var height = _size.y;

            var topLeftX = (width - 1) * -0.5f;
            var topLeftZ = (height - 1) * 0.5f;

            var terrain = new MeshData(width, height);

            for (int y = 0, vertexIndex = 0; y <= height; y++)
            {
                for (var x = 0; x <= width; vertexIndex++, x++)
                {
                    terrain.vertices[vertexIndex] = new Vector3(topLeftX + x, topLeftZ - y);
                    terrain.uvs[vertexIndex] = new Vector2(x / (float) width, y / (float) height);

                    if (x < width && y < height)
                    {
                        terrain.AddTriangle(vertexIndex, vertexIndex + width + 1, vertexIndex + 1);
                        terrain.AddTriangle(vertexIndex + 1, vertexIndex + width + 1, vertexIndex + width + 2);
                    }
                }
            }

            _filter.mesh = terrain.CreateMesh();
            _filter.mesh.name = "Procedural Mesh";
            _renderer.material = new Material(Shader.Find("Unlit/Color"));
        }

        private struct MeshData
        {
            public readonly Vector3[] vertices;
            private readonly int[] triangles;
            public readonly Vector2[] uvs;

            private int triangleIndex;

            public MeshData(int width, int height)
            {
                vertices = new Vector3[(width + 1) * (height + 1)];
                uvs = new Vector2[(width + 1) * (height + 1)];
                triangles = new int[width * height * 6];
                triangleIndex = 0;
            }

            public void AddTriangle(int a, int b, int c)
            {
                triangles[triangleIndex] = a;
                triangles[triangleIndex + 1] = b;
                triangles[triangleIndex + 2] = c;
                triangleIndex += 3;
            }

            public Mesh CreateMesh()
            {
                var mesh = new Mesh
                {
                    vertices = vertices,
                    indexFormat = vertices.Length > CHUNK_SIZE ? IndexFormat.UInt32 : IndexFormat.UInt16,
                    subMeshCount = Mathf.CeilToInt((float) triangles.Length / CHUNK_SIZE),
                    uv = uvs,
                };

                var subMeshTriangles = triangles.ToList();

                for (var i = 0; i < mesh.subMeshCount; i++)
                {
                    var subMeshTrianglesCount =
                        subMeshTriangles.Count < CHUNK_SIZE ? subMeshTriangles.Count : CHUNK_SIZE;
                    var targetTriangles = subMeshTriangles.GetRange(0, subMeshTrianglesCount).ToArray();

                    mesh.SetTriangles(targetTriangles, i);
                    subMeshTriangles.RemoveRange(0, subMeshTrianglesCount);
                }

                mesh.RecalculateBounds();
                mesh.RecalculateNormals();
                mesh.Optimize();

                return mesh;
            }
        }
    }
}
