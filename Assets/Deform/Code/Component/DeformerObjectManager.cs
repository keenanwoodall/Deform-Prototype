using UnityEngine;
using Unity.Jobs;
using System.Collections.Generic;

namespace Deform
{
	public class DeformerObjectManager : MonoBehaviour
	{
		private static DeformerObjectManager instance;
		private static List<DeformerObject> deformerObjects = new List<DeformerObject> ();

		public bool update = true;
		private JobHandle lastHandle;
		private List<JobHandle> normalHandles = new List<JobHandle> ();

		private void Awake ()
		{
			if (instance == null)
				instance = this;
			else
				Destroy (gameObject);
		}
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
			CompleteHandles ();
			Destroy (instance);
		}

		private void CompleteHandles ()
		{
			lastHandle.Complete ();
			foreach (var normalCalculationHandle in normalHandles)
				normalCalculationHandle.Complete ();
			normalHandles = new List<JobHandle> ();
		}

		public static void AddDeformerObject (DeformerObject deformerObject)
		{
			EnsureInstance ();
			deformerObjects.Add (deformerObject);
		}
		public static void RemoveDeformerObject (DeformerObject deformerObject)
		{
			EnsureInstance ();
			deformerObjects.Remove (deformerObject);
		}

		protected static void EnsureInstance ()
		{
			if (instance == null)
				instance = new GameObject ("DeformerManager").AddComponent<DeformerObjectManager> ();
		}
	}
}