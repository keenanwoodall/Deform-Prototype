using System.Collections.Generic;
using UnityEngine;

namespace Deform
{
	[RequireComponent (typeof (MeshFilter))]
	[ExecuteInEditMode]
	public class DeformerComponentManager : DeformerBase
	{
		public UpdateMode updateMode = UpdateMode.UpdateFrameSplit;
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
			ChangeTarget (GetComponent<MeshFilter> (), false);
			RecreateChunks (updateMode == UpdateMode.UpdateInstant || updateMode == UpdateMode.UpdateAsync);

			UpdateMeshInstant (normalsCalculation, SmoothingAngle);
		}

		public void Update ()
		{
			switch (updateMode)
			{
				case UpdateMode.UpdateInstant:
					UpdateInstant ();
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

		private void UpdateInstant ()
		{
			UpdateTransformData ();
			UpdateSyncedTime ();
			NotifyPreModify ();
			UpdateMeshInstant (normalsCalculation, smoothingAngle);
			NotifyPostModify ();
		}

		private void UpdateAsync ()
		{
			UpdateTransformData ();
			UpdateSyncedTime ();
			NotifyPreModify ();
#if UNITY_EDITOR
			if (Application.isPlaying)
				UpdateMeshAsync (normalsCalculation, SmoothingAngle, NotifyPostModify);
			else
				UpdateMeshInstant (normalsCalculation, SmoothingAngle);
#else
			UpdateMeshAsync (normalsCalculation, SmoothingAngle, NotifyPostModify);
#endif
		}

		private void UpdateFrameSplit ()
		{
			DeformChunk (deformChunkIndex);
			deformChunkIndex++;

			// Updating the last chunk?
			if (deformChunkIndex >= chunks.Length)
			{
				UpdateSyncedTime ();
				UpdateTransformData ();
				ApplyChunksToTarget (normalsCalculation, smoothingAngle);
				ResetChunks ();
				deformChunkIndex = 0;
				NotifyPostModify ();
			}
			// Updating the first chunk?
			else if (deformChunkIndex == 0)
				NotifyPreModify ();
		}

		private void NotifyPreModify ()
		{
			for (var deformerIndex = 0; deformerIndex < deformers.Count; deformerIndex++)
				if (deformers[deformerIndex].update)
					deformers[deformerIndex].PreModify ();
		}
		private void NotifyPostModify ()
		{
			for (var deformerIndex = 0; deformerIndex < deformers.Count; deformerIndex++)
				if (deformers[deformerIndex].update)
					deformers[deformerIndex].PostModify ();
		}

		protected override void DeformChunks ()
		{
			lock (chunks)
			{
				lock (deformers)
				{
					for (var deformerIndex = 0; deformerIndex < deformers.Count; deformerIndex++)
					{
						lock (deformers[deformerIndex])
						{
							if (deformers[deformerIndex].update)
							{
								for (var chunkIndex = 0; chunkIndex < chunks.Length; chunkIndex++)
								{
									chunks[chunkIndex] = deformers[deformerIndex].Modify (chunks[chunkIndex], SyncedTransform, Bounds);
								}
							}
						}
					}
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
					chunks[index] = deformers[deformerIndex].Modify (chunks[index], SyncedTransform, Bounds);

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
			// Not sure why locking the deformers list doesn't prevent this method from changing it while the other thread works on it
			// For now this if statement seems to prevent an error.
			//if (asyncUpdateInProgress)
			//	return;

			lock (deformers)
			{
				var oldDeformers = new List<DeformerComponent> ();
				oldDeformers.AddRange (deformers);
				deformers = new List<DeformerComponent> ();

				var currentDeformers = GetComponents<DeformerComponent> ();

				for (int i = 0; i < currentDeformers.Length; i++)
					if (oldDeformers.Contains (currentDeformers[i]))
						deformers.Add (currentDeformers[i]);
			}
		}

		public List<DeformerComponent> GetDeformers ()
		{
			return deformers;
		}
	}
}