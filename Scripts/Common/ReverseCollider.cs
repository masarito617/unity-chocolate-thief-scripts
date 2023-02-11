using System.Linq;
using UnityEngine;

namespace Chocolate.Common
{
    /// <summary>
    /// コライダーを反転させる
    /// 移動範囲の制御で使用
    /// </summary>
    public class ReverseCollider : MonoBehaviour
    {
        public bool removeExistingColliders = true;

        private void Start()
        {
            CreateInvertedMeshCollider();
        }

        private void CreateInvertedMeshCollider()
        {
            if (removeExistingColliders)
                RemoveExistingColliders();

            InvertMesh();
            gameObject.AddComponent<MeshCollider>();
        }

        private void RemoveExistingColliders()
        {
            var colliders = GetComponents<Collider>();
            foreach (var c in colliders)
            {
                DestroyImmediate(c);
            }

        }

        private void InvertMesh()
        {
            var mesh = GetComponent<MeshFilter>().mesh;
            mesh.triangles = mesh.triangles.Reverse().ToArray();
        }
    }
}
