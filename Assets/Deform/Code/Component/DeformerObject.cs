using UnityEngine;
using Unity.Jobs;
using Deform.Data;

namespace Deform
{
	public class DeformerObject : MonoBehaviour
	{
		public bool updateBounds = true;
		[SerializeField]
		private DeformerObjectManager manager; // changing this after OnEnable won't do anything

		[SerializeField] [HideInInspector]
		private MeshFilter meshFilter;

		private MeshData meshData;
		private Deformer[] deformers;

		private void Awake ()
		{
			if (meshFilter == null)
				meshFilter = GetComponent<MeshFilter> ();

			meshData = new MeshData (meshFilter);

			if (manager == null)
			{
				Debug.Log ("Manager reference null. Attempting to find one...");
				manager = FindObjectOfType<DeformerObjectManager> ();
				if (manager == null)
					Debug.LogError ("Manager not found in scene. Create one and assign it to the manager field for this deformer object to be processed.");
				else
				{

				}
				Debug.Log ("Manager found. For better performance, assign the reference manually.");
			}
		}

		private void OnEnable ()
		{
			if (manager)
				manager.AddDeformerObject (this);
		}

		private void OnDisable ()
		{
			if (manager)
				manager.RemoveDeformerObject (this);
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