using UnityEngine;
using Unity.Jobs;
using Deform.Data;

namespace Deform
{
	public class DeformerObject : MonoBehaviour
	{
		public bool updateNormals = true;
		public bool updateBounds = true;

		[SerializeField] [HideInInspector]
		private MeshFilter meshFilter;

		private MeshData meshData;
		private Deformer[] deformers;

		private void Awake ()
		{
			if (meshFilter == null)
				meshFilter = GetComponent<MeshFilter> ();

			meshData = new MeshData (meshFilter);
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
			meshData.Dispose ();
		}

		public JobHandle DeformData (JobHandle dependency)
		{
			var previousHandle = dependency;
			deformers = GetComponents<Deformer> ();

			for (int i = 0; i < deformers.Length; i++)
			{
				previousHandle = deformers[i].Deform (meshData.nativeData, previousHandle);
			}

			return previousHandle;
		}

		public void ApplyData ()
		{
			meshData.ApplyData (updateNormals, updateBounds);
		}
	}
}