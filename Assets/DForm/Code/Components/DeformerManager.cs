using System.Collections.Generic;
using UnityEngine;

namespace Deform
{
	[RequireComponent (typeof (MeshFilter))]
	[ExecuteInEditMode]
	public class DeformerManager : DeformerManagerBase
	{
		public enum UpdateMode { Update, Pause, Stop }
		public UpdateMode updateMode = UpdateMode.Update;

		[SerializeField, HideInInspector]
		private List<DeformerComponent> deformers = new List<DeformerComponent> ();

		private void Awake ()
		{
			// Return if already initialized.
			if (target != null && originalMesh != null)
				return;

			target = GetComponent<MeshFilter> ();
			ChangeTarget (target);
		}

		private void Update ()
		{
			UpdateMesh ();
		}

		public void UpdateMesh ()
		{
			switch (updateMode)
			{
				case UpdateMode.Update:
					DeformChunks ();
					ApplyChunksToTarget ();
					ResetChunks ();
					return;
				case UpdateMode.Pause:
					return;
				case UpdateMode.Stop:
					ResetChunks ();
					ApplyChunksToTarget ();
					return;
			}
		}

		private void DeformChunks ()
		{
			var deformerCount = deformers.Count;

			// Call Pre Modify
			for (var deformerIndex = 0; deformerIndex < deformerCount; deformerIndex++)
				deformers[deformerIndex].PreModify ();
			
			// Modify chunks
			for (var deformerIndex = 0; deformerIndex < deformerCount; deformerIndex++)
				if (deformers[deformerIndex].update)
					for (var chunkIndex = 0; chunkIndex < chunks.Length; chunkIndex++)
						chunks[chunkIndex].vertexData = deformers[deformerIndex].Modify (chunks[chunkIndex].vertexData);

			// Call Post Modify
			for (var deformerIndex = 0; deformerIndex < deformerCount; deformerIndex++)
				deformers[deformerIndex].PostModify ();
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

		public List<DeformerComponent> GetDeformers ()
		{
			return deformers;
		}
	}
}