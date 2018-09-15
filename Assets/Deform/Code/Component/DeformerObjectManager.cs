using UnityEngine;
using Unity.Jobs;
using System.Collections.Generic;

namespace Deform
{
	public class DeformerObjectManager : MonoBehaviour
	{
		public bool update = true;

		private bool handlesCompleted;
		private JobHandle lastHandle;
		private List<JobHandle> normalHandles = new List<JobHandle> ();
		private List<DeformerObject> deformerObjects = new List<DeformerObject> ();

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
			if (!update && handlesCompleted)
				return;

			CompleteHandles ();
			handlesCompleted = true;

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