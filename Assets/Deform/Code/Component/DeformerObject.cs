using UnityEngine;
using Unity.Jobs;
using Unity.Burst;
using Deform.Data;

namespace Deform
{
	public class DeformerObject : MonoBehaviour
	{
		public bool updateNormals = true;
		public bool updateBounds = true;
		public MeshFilter meshFilter;

		[SerializeField] [HideInInspector]
		private Mesh originalMesh;
		private Mesh dynamicMesh;
		private MeshData originalData;
		private MeshData dynamicData;
		private NativeMeshData nativeData;

		private Deformer[] deformers;

		private void Awake ()
		{
			if (meshFilter == null)
				meshFilter = GetComponent<MeshFilter> ();

			if (originalMesh == null)
				originalMesh = meshFilter.sharedMesh;
			meshFilter.sharedMesh = dynamicMesh = Instantiate (originalMesh);

			originalData = new MeshData (dynamicMesh);
			dynamicData = new MeshData (dynamicMesh);
		}

		private void OnEnable ()
		{
			DeformerObjectManager.AddDeformerObject (this);
		}

		private void OnDisable ()
		{
			DeformerObjectManager.RemoveDeformerObject (this);
		}

		private void OnDestroy ()
		{
			nativeData.Dispose ();
		}

		public JobHandle DeformData (JobHandle dependency)
		{
			var previousHandle = dependency;
			deformers = GetComponents<Deformer> ();

			for (int i = 0; i < deformers.Length; i++)
			{
				previousHandle = deformers[i].Deform (nativeData, previousHandle);
			}

			return previousHandle;
		}

		public void ApplyData ()
		{
			if (nativeData.vertices.IsCreated)
			{
				nativeData.CopyTo (dynamicData);
				dynamicMesh.vertices = dynamicData.vertices;
				dynamicMesh.normals = dynamicData.normals;
			}

			nativeData.Dispose ();
			nativeData = new NativeMeshData (originalData);

			if (updateNormals)
				dynamicMesh.RecalculateNormals ();
			if (updateBounds)
				dynamicMesh.RecalculateBounds ();
		}
	}
}