using System.Collections.Generic;
using UnityEngine;

namespace Deform
{
	[ExecuteInEditMode]
	public class DeformerComponentManager : DeformerBase
	{
		public UpdateMode updateMode = UpdateMode.UpdateAsync;
		public NormalsCalculationMode normalsCalculation = NormalsCalculationMode.Unity;

		[SerializeField, HideInInspector]
		private List<DeformerComponent> deformers = new List<DeformerComponent> ();

		private void Start ()
		{
			var mf = GetComponent<MeshFilter> ();
			if (mf != null)
				SetTarget (mf);
			else
			{
				var smr = GetComponent<SkinnedMeshRenderer> ();
				if (smr != null)
					SetTarget (smr);
				else
					return;
			}

			UpdateInstant ();
		}

		public void Update ()
		{
			if (target == null && skinnedTarget == null)
				return;

			switch (updateMode)
			{
				case UpdateMode.UpdateInstant:
					UpdateInstant ();
					return;
				case UpdateMode.UpdateAsync:
					UpdateAsync ();
					return;
				case UpdateMode.Pause:
					return;
				case UpdateMode.Stop:
					ResetVertexData ();
					ApplyVertexDataToTarget (NormalsCalculationMode.Original);
					return;
			}
		}

		private void OnDestroy ()
		{
			deformers.Clear ();
		}

		protected override void DeformVertexData ()
		{
			// I'm not threading savvy, but I have a hunch that using all these locks isn't the ideal solution.
			lock (deformers)
			{
				RemoveNullDeformers ();

				for (var deformerIndex = 0; deformerIndex < deformers.Count; deformerIndex++)
				{
					lock (deformers[deformerIndex])
					{
						if (deformers[deformerIndex].update)
						{
							meshData = deformers[deformerIndex].Modify (meshData, SyncedTransform, MeshDataUtil.GetBounds (meshData));
						}
					}
				}
			}

			NotifyPostModify ();
		}

		private void UpdateInstant ()
		{
			UpdateTransformData ();
			UpdateSyncedTime ();
			NotifyPreModify ();
			UpdateMeshInstant (normalsCalculation);
			NotifyPostModify ();
		}

		private void UpdateAsync ()
		{
			UpdateTransformData ();
			UpdateSyncedTime ();
			NotifyPreModify ();
#if UNITY_EDITOR
			if (Application.isPlaying)
				UpdateMeshAsync (normalsCalculation, NotifyPostModify);
			else
				UpdateMeshInstant (normalsCalculation);
#else
			UpdateMeshAsync (normalsCalculation, NotifyPostModify);
#endif
		}

		private void NotifyPreModify ()
		{
			RemoveNullDeformers ();
			for (var deformerIndex = 0; deformerIndex < deformers.Count; deformerIndex++)
			{
				if (deformers[deformerIndex].update)
					deformers[deformerIndex].PreModify ();
			}
		}
		private void NotifyPostModify ()
		{
			RemoveNullDeformers ();
			for (var deformerIndex = 0; deformerIndex < deformers.Count; deformerIndex++)
				if (deformers[deformerIndex].update)
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

		public void RefreshDeformerOrder ()
		{
			lock (deformers)
			{
				var currentDeformers = GetComponents<DeformerComponent> ();

				var oldDeformers = new List<DeformerComponent> ();
				oldDeformers.AddRange (deformers);
				deformers = new List<DeformerComponent> ();

				for (int i = 0; i < currentDeformers.Length; i++)
					if (oldDeformers.Contains (currentDeformers[i]))
						deformers.Add (currentDeformers[i]);
			}
		}

		public List<DeformerComponent> GetDeformers ()
		{
			return deformers;
		}

		private void RemoveNullDeformers ()
		{
			for (int deformerIndex = 0; deformerIndex < deformers.Count; deformerIndex++)
			{
				if (deformers[deformerIndex] == null)
				{
					deformers.RemoveAt (deformerIndex);
					deformerIndex--;
				}
			}
		}
	}
}
