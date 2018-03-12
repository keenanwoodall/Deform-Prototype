using System.Collections.Generic;
using UnityEngine;

namespace Deform
{
	[ExecuteInEditMode]
	public class DeformerComponentManager : DeformerBase
	{
		public UpdateMode updateMode = UpdateMode.UpdateAsync;
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

		private void Start ()
		{
			var mf = GetComponent<MeshFilter> ();
			if (mf != null)
				SetTarget (mf);
			else
			{
				var sf = GetComponent<SkinnedMeshRenderer> ();
				if (sf != null)
					SetTarget (sf);
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
					ApplyVertexDataToTarget (NormalsCalculationMode.Original, smoothingAngle);
					return;
			}
		}

		private void OnDestroy ()
		{
			deformers.Clear ();
		}

		protected override void DeformVertexData ()
		{
			lock (vertexData)
			{
				lock (deformers)
				{
					RemoveNullDeformers ();

					for (var deformerIndex = 0; deformerIndex < deformers.Count; deformerIndex++)
					{
						lock (deformers[deformerIndex])
						{
							if (deformers[deformerIndex].update)
							{
								vertexData = deformers[deformerIndex].Modify (vertexData, SyncedTransform, VertexDataUtil.GetBounds (vertexData));
							}
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