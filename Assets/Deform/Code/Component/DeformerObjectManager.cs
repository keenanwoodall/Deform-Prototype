using UnityEngine;
using Unity.Jobs;
using System.Collections.Generic;

namespace Deform
{
	public class DeformerObjectManager : MonoBehaviour
	{
		private List<DeformerObject> deformerObjects = new List<DeformerObject> ();

		public bool update = true;
		private JobHandle lastHandle;
		private List<JobHandle> normalHandles = new List<JobHandle> ();

		private void Update ()
		{
			if (!update)
				return;

			for (int i = 0; i < deformerObjects.Count; i++)
			{
				var deformerObject = deformerObjects[i];
				lastHandle = deformerObject.DeformData ();
				normalHandles.Add (deformerObjects[i].RecalculateNormalsAsync (lastHandle));
			}
		}
		private void LateUpdate ()
		{
			CompleteHandles ();
			for (int i = 0; i < deformerObjects.Count; i++)
			{
				var deformerObject = deformerObjects[i];
				deformerObject.ApplyData ();
			}
		}

		private void OnDestroy ()
		{
			if (this.isActiveAndEnabled)
				CompleteHandles ();
		}

		private void CompleteHandles ()
		{
			lastHandle.Complete ();
			foreach (var normalCalculationHandle in normalHandles)
				normalCalculationHandle.Complete ();
			normalHandles = new List<JobHandle> ();
		}

		public void AddDeformerObject (DeformerObject deformerObject)
		{
			deformerObjects.Add (deformerObject);
		}
		public void RemoveDeformerObject (DeformerObject deformerObject)
		{
			deformerObjects.Remove (deformerObject);
		}
	}
}