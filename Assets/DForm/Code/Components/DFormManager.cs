using UnityEngine;

namespace DForm
{
	[RequireComponent (typeof (MeshFilter))]
	[ExecuteInEditMode]
	public class DFormManager : DFormManagerBase
	{
		[SerializeField, HideInInspector]
		private MeshFilter meshFilter;
		[SerializeField, HideInInspector]
		private DFormerComponent[] deformers;

		private void Awake ()
		{
			meshFilter = GetComponent<MeshFilter> ();
			ChangeTarget (meshFilter);
			UpdateDeformerReferences ();
		}

		private void Update ()
		{
#if UNITY_EDITOR
			// Update references in the editor
			if (!Application.isPlaying)
				UpdateDeformerReferences ();
#endif
			DeformChunks ();
			ApplyChunksToTarget ();
			ResetChunks ();
		}

		private void UpdateDeformerReferences ()
		{
			deformers = GetComponents<DFormerComponent> ();
		}

		private void DeformChunks ()
		{
			// Call Pre Modify
			for (var deformerIndex = 0; deformerIndex < deformers.Length; deformerIndex++)
				deformers[deformerIndex].PreModify ();
			
			// Modify chunks
			for (var chunkIndex = 0; chunkIndex < chunks.Length; chunkIndex++)
				for (var deformerIndex = 0; deformerIndex < deformers.Length; deformerIndex++)
					chunks[chunkIndex].vertexData = deformers[deformerIndex].Modify (chunks[chunkIndex].vertexData);

			// Call Post Modify
			for (var deformerIndex = 0; deformerIndex < deformers.Length; deformerIndex++)
				deformers[deformerIndex].PostModify ();
		}
	}
}