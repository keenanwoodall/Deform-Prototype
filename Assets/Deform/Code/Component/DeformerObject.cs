using UnityEngine;
using Unity.Jobs;
using Deform.Data;

namespace Deform
{
	public class DeformerObject : MonoBehaviour
	{
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

		public JobHandle DeformData (JobHandle dependency = default (JobHandle))
		{
			deformers = GetComponents<Deformer> ();

			for (int i = 0; i < deformers.Length; i++)
			{
				var deformer = deformers[i];
				if (deformer != null && deformer.enabled && deformer.update)
					dependency = deformer.Deform (meshData.nativeData, dependency);
			}

			return dependency;
		}

		public JobHandle RecalculateNormalsAsync (JobHandle dependency)
		{
			return meshData.RecalculateNormalsAsync (dependency);
		}
		public void ApplyData ()
		{
			meshData.ApplyData (updateBounds);
		}
	}
}