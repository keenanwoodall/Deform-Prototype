using System;
using System.Collections.Generic;
using UnityEngine;

namespace Deform
{
	public delegate void DeformCompleteCallback ();

	[ExecuteInEditMode]
	public abstract class DeformerBase : MonoBehaviour
	{
		public DeformCompleteCallback onDeformComplete;
		[SerializeField, HideInInspector]
		protected MeshFilter target;
		[SerializeField, HideInInspector]
		protected SkinnedMeshRenderer skinnedTarget;
		[SerializeField, HideInInspector]
		protected VertexData[] vertexData;
		[SerializeField, HideInInspector]
		protected Mesh originalMesh;
		[SerializeField, HideInInspector]
		protected Mesh deformMesh;

		[SerializeField, HideInInspector]
		private List<Vector3> originalNormals = new List<Vector3> ();

		protected bool asyncUpdateInProgress { get; private set; }
		
		public int VertexCount { get { return originalMesh.vertexCount; } }
		public float SyncedTime { get; private set; }
		public float SyncedDeltaTime { get; private set; }
		public TransformData SyncedTransform { get; private set; }
		public Bounds Bounds { get; private set; }
		public SkinnedMeshRenderer SkinnedTarget { get { return skinnedTarget; } }
		public MeshFilter Target { get { return target; } }

		private void OnDestroy ()
		{
			DiscardChanges ();
		}

		protected void SetTarget (MeshFilter meshFilter, bool recreateVertexData = true)
		{
			// Assign the target.
			skinnedTarget = null;
			target = meshFilter;

			// If it's not null, the object was probably duplicated
			if (originalMesh == null)
				originalMesh = MeshUtil.Copy (target.sharedMesh);
			else
				originalMesh = MeshUtil.Copy (originalMesh);


			// Change the mesh to one we can modify.
			deformMesh = target.sharedMesh = MeshUtil.Copy (originalMesh);

			// Cache the original bounds.
			Bounds = originalMesh.bounds;

			// Cache the original normals.
			deformMesh.GetNormals (originalNormals);

			deformMesh.name = transform.name + " Deform Mesh";
			originalMesh.name = "Original";

			// Create chunk data.
			if (recreateVertexData)
				RecreateVertexData ();
		}

		protected void SetTarget (SkinnedMeshRenderer skinnedMesh, bool recreateVertexData = true)
		{
			target = null;
			// Assign the target.
			skinnedTarget = skinnedMesh;

			// If it's not null, the object was probably duplicated
			if (originalMesh == null)
				// Store the original mesh.
				originalMesh = MeshUtil.Copy (skinnedTarget.sharedMesh);
			else
				originalMesh = MeshUtil.Copy (originalMesh);

			// Change the mesh to one we can modify.
			deformMesh = skinnedTarget.sharedMesh = MeshUtil.Copy (originalMesh);

			Bounds = originalMesh.bounds;
			// Cache the original normals.
			originalMesh.GetNormals (originalNormals);

			// Create chunk data.
			if (recreateVertexData)
				RecreateVertexData ();
		}

		/// <summary>
		/// Returns (and optionally copies) the deformMesh. Depending on when you call this method, the mesh may not actually be deformed.
		/// If you want to ensure you get the deformed mesh, call this when the onDeformComplete delegate is invoked.
		/// </summary>
		/// <param name="copy">Instantiate a copy of the deformMesh?</param>
		public Mesh GetCurrentMesh (bool copy)
		{
			if (copy)
				return MeshUtil.Copy (deformMesh);
			return deformMesh;
		}

		/// <summary>
		/// Deforms vertexData and applies to mesh instantly.
		/// </summary>
		public void UpdateMeshInstant (NormalsCalculationMode normalsCalculation, float smoothingAngle)
		{
			// Don't update if another update is in progress.
			if (asyncUpdateInProgress)
				return;

			DeformVertexData ();
			ApplyVertexDataToTarget (normalsCalculation, smoothingAngle);
			ResetVertexData ();

			if (onDeformComplete != null)
				onDeformComplete.Invoke ();
		}

		/// <summary>
		/// Deforms the vertexData on another thread and then applies to the mesh.
		/// </summary>
		public async void UpdateMeshAsync (NormalsCalculationMode normalsCalculation, float smoothingAngle, Action onComplete = null)
		{
#if UNITY_EDITOR
			if (!Application.isPlaying)
			{
				Debug.LogError ("UpdateMeshAsync doesn't work in edit-mode");
				return;
			}
#endif
			if (asyncUpdateInProgress)
				return;

			asyncUpdateInProgress = true;
			await new WaitForBackgroundThread ();
			DeformVertexData ();
			await new WaitForUpdate ();
			asyncUpdateInProgress = false;

			// We have to handle the scenario in which the update starts in play mode and finishes in edit mode.
			if (!Application.isPlaying)
				return;

			ApplyVertexDataToTarget (normalsCalculation, smoothingAngle);
			ResetVertexData ();

			if (onComplete != null)
				onComplete.Invoke ();

			if (onDeformComplete != null)
				onDeformComplete.Invoke ();
		}

		/// <summary>
		/// Updates the normals of the mesh.
		/// </summary>
		/// <param name="normalsCalculation"></param>
		/// <param name="smoothingAngle"></param>
		public void UpdateNormals (NormalsCalculationMode normalsCalculation, float smoothingAngle)
		{
			switch (normalsCalculation)
			{
				case NormalsCalculationMode.Unity:
					deformMesh.RecalculateNormals ();
					break;
				case NormalsCalculationMode.Smooth:
					deformMesh.RecalculateNormals (smoothingAngle);
					break;
				case NormalsCalculationMode.Maintain:
					break;
				case NormalsCalculationMode.Original:
					deformMesh.SetNormals (originalNormals);
					break;
			}
		}

		/// <summary>
		/// Updates the SyncedTime and SyncedDeltaTime property.
		/// </summary>
		public void UpdateSyncedTime ()
		{
			SyncedDeltaTime = Time.time - SyncedTime;
			SyncedTime = Time.time;
		}

		/// <summary>
		/// Updates the SyncedTransform property.
		/// </summary>
		public void UpdateTransformData ()
		{
			SyncedTransform = new TransformData (transform);
		}

		/// <summary>
		/// Sets vertexData to the vertexData of the original mesh.
		/// </summary>
		public void RecreateVertexData ()
		{
			vertexData = VertexDataUtil.GetVertexData (originalMesh);
		}

		/// <summary>
		/// Applies the vertexData to the deform mesh.
		/// </summary>
		protected void ApplyVertexDataToTarget (NormalsCalculationMode normalsCalculation, float smoothingAngle)
		{
			VertexDataUtil.ApplyVertexData (vertexData, deformMesh);
			UpdateNormals (normalsCalculation, smoothingAngle);

			deformMesh.RecalculateBounds ();
		}

		/// <summary>
		/// This is called by UpdateInstant and UpdateAsync. Change the vertexData in here.
		/// </summary>
		protected abstract void DeformVertexData ();

		/// <summary>
		/// Sets the position of each vertex to it's base position.
		/// </summary>
		protected void ResetVertexData ()
		{
			VertexDataUtil.ResetVertexData (vertexData);
		}

		/// <summary>
		/// Sets the deform mesh back to the original mesh.
		/// </summary>
		public void DiscardChanges ()
		{
			if (originalMesh != null)
				deformMesh = Instantiate (originalMesh);
		}
	}
}