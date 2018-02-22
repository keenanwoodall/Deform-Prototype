using System.Collections.Generic;
using UnityEngine;

namespace Deform
{
	[RequireComponent (typeof (MeshFilter))]
	[ExecuteInEditMode]
	public class DeformerManager : DeformerManagerBase
	{
		public enum RefreshMode { Update, Pause, Stop }
		public RefreshMode refreshMode = RefreshMode.Update;

		[SerializeField, HideInInspector]
		private MeshFilter meshFilter;
		[SerializeField, HideInInspector]
		private List<DeformerComponent> deformers;

		private void Awake ()
		{
			meshFilter = GetComponent<MeshFilter> ();
			ChangeTarget (meshFilter);
		}

		private void Update ()
		{
			switch (refreshMode)
			{
				case RefreshMode.Update:
					DeformChunks ();
					ApplyChunksToTarget ();
					ResetChunks ();
					return;
				case RefreshMode.Pause:
					return;
				case RefreshMode.Stop:
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
			for (var chunkIndex = 0; chunkIndex < chunks.Length; chunkIndex++)
				for (var deformerIndex = 0; deformerIndex < deformerCount; deformerIndex++)
					chunks[chunkIndex].vertexData = deformers[deformerIndex].Modify (chunks[chunkIndex].vertexData);

			// Call Post Modify
			for (var deformerIndex = 0; deformerIndex < deformerCount; deformerIndex++)
				deformers[deformerIndex].PostModify ();
		}

		public void AddDeformer (DeformerComponent deformer)
		{
			deformers.Add (deformer);
		}

		public void RemoveDeformer (DeformerComponent deformer)
		{
			deformers.Remove (deformer);
		}
	}
}