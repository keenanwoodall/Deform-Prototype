using UnityEngine;

namespace Deform.Data
{
	public class MeshData
	{
		[SerializeField] [HideInInspector]
		private Mesh originalMesh;
		private Mesh dynamicMesh;

		private ManagedMeshData originalData;
		public ManagedMeshData dynamicData { get; private set; }
		public NativeMeshData nativeData { get; private set; }

		public MeshData (MeshFilter meshFilter)
		{
			if (originalMesh == null)
				originalMesh = meshFilter.sharedMesh;
			meshFilter.sharedMesh = dynamicMesh = GameObject.Instantiate (originalMesh);

			dynamicMesh.MarkDynamic ();

			originalData = new ManagedMeshData (dynamicMesh);
			dynamicData = new ManagedMeshData (dynamicMesh);
			nativeData = new NativeMeshData (originalData, Unity.Collections.Allocator.Persistent);
		}

		public void ApplyData (bool updateNormals, bool updateBounds)
		{
			if (nativeData.vertices.IsCreated)
			{
				nativeData.CopyTo (dynamicData);
				dynamicMesh.vertices = dynamicData.vertices;
				dynamicMesh.normals = dynamicData.normals;
				dynamicMesh.tangents = dynamicData.tangents;
				dynamicMesh.uv = dynamicData.uv;
			}

			nativeData.CopyFrom (originalData);

			if (updateNormals)
				dynamicMesh.RecalculateNormals ();
			if (updateBounds)
				dynamicMesh.RecalculateBounds ();
		}

		public void Dispose ()
		{
			GameObject.Destroy (dynamicMesh);
			nativeData.Dispose ();
		}
	}
}