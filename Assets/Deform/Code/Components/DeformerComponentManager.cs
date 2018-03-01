using System.Collections.Generic;
using UnityEngine;

namespace Deform
{
	[RequireComponent (typeof (MeshFilter))]
	[ExecuteInEditMode]
	public class DeformerComponentManager : DeformerBase
	{
		public UpdateMode updateMode = UpdateMode.UpdateInstant;
		public NormalsCalculationMode normalsCalculation = NormalsCalculationMode.Unity;

		[HideInInspector, SerializeField]
		private float smoothingAngle = 60f;
		public float SmoothingAngle
		{
			get { return smoothingAngle; }
			set { smoothingAngle = Mathf.Clamp (value, 0f, 180f); }
		}

		[SerializeField, HideInInspector]
		private List<DeformerComponent> deformers = new List<DeformerComponent> ();

		private void Awake ()
		{
			DiscardChanges ();
			ChangeTarget (GetComponent<MeshFilter> ());
#if UNITY_EDITOR
			if (!Application.isPlaying || (Application.isPlaying && Time.frameCount == 0))
				UpdateMeshInstant (normalsCalculation, SmoothingAngle);
#else
			UpdateMeshInstant (normalsCalculation, SmoothingAngle);
#endif
		}

		public void Update ()
		{
			switch (updateMode)
			{
				case UpdateMode.UpdateInstant:
					UpdateMeshInstant (normalsCalculation, smoothingAngle, true);
					return;
				case UpdateMode.UpdateAsync:
					UpdateAsync ();
					return;
				case UpdateMode.UpdateFrameSplit:
					UpdateFrameSplit ();
					return;
				case UpdateMode.Pause:
					return;
				case UpdateMode.Stop:
					ResetChunks ();
					ApplyChunksToTarget (NormalsCalculationMode.Original, smoothingAngle);
					return;
			}
		}

		private void OnDestroy ()
		{
			deformers.Clear ();
		}

		private void UpdateAsync ()
		{
#if UNITY_EDITOR
			if (Application.isPlaying)
				UpdateMeshAsync (normalsCalculation, SmoothingAngle);
			else
				UpdateFrameSplit ();
#else
			UpdateMeshAsync (normalsCalculation, SmoothingAngle);
#endif
		}
		private void UpdateFrameSplit ()
		{
			if (deformChunkIndex >= chunks.Length)
			{
				UpdateSyncedTime ();
				UpdateChunkTransformData ();
				ApplyChunksToTarget (normalsCalculation, smoothingAngle);
				ResetChunks ();
				deformChunkIndex = 0;
			}
			DeformChunk (deformChunkIndex);
			deformChunkIndex++;
		}

		private void NotifyPreModify ()
		{
			for (var deformerIndex = 0; deformerIndex < deformers.Count; deformerIndex++)
			{
				if (deformers[deformerIndex].update)
					deformers[deformerIndex].PreModify ();
			}
		}
		private void NotifyPostModify ()
		{
			for (var deformerIndex = 0; deformerIndex < deformers.Count; deformerIndex++)
				if (deformers[deformerIndex].update)
					deformers[deformerIndex].PostModify ();
		}

		protected override void DeformChunks ()
		{
			NotifyPreModify ();

			for (var deformerIndex = 0; deformerIndex < deformers.Count; deformerIndex++)
			{
				if (deformers[deformerIndex].update)
				{
					for (var chunkIndex = 0; chunkIndex < chunks.Length; chunkIndex++)
						chunks[chunkIndex] = deformers[deformerIndex].Modify (chunks[chunkIndex]);
				}
			}

			NotifyPostModify ();
		}

		protected override void DeformChunk (int index)
		{
			if (index == 0)
				NotifyPreModify ();

			// Modify chunk
			for (var deformerIndex = 0; deformerIndex < deformers.Count; deformerIndex++)
				if (deformers[deformerIndex].update)
					chunks[index] = deformers[deformerIndex].Modify (chunks[index]);

			if (index == chunks.Length - 1)
				NotifyPostModify ();
		}

		public void AddDeformer (DeformerComponent deformer)
		{
			if (deformers == null)
				deformers = new List<DeformerComponent> ();
			if (!deformers.Contains (deformer))
				deformers.Add (deformer);
		}

		public void RemoveDeformer (DeformerComponent deformer)
		{
			if (deformers == null)
				return;
			if (deformers.Contains (deformer))
				deformers.Remove (deformer);
		}

		public void RefreshDeformerOrder ()
		{
			var oldDeformers = new List<DeformerComponent> ();
			oldDeformers.AddRange (deformers);
			deformers.Clear ();

			var currentDeformers = GetComponents<DeformerComponent> ();

			for (int i = 0; i < currentDeformers.Length; i++)
				if (oldDeformers.Contains (currentDeformers[i]))
					deformers.Add (currentDeformers[i]);
		}

		public List<DeformerComponent> GetDeformers ()
		{
			return deformers;
		}
	}
}