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
				lastHandle = deformerObjects[i].DeformData (lastHandle);
		}
		private void LateUpdate ()
		{
			lastHandle.Complete ();

			for (int i = 0; i < deformerObjects.Count; i++)
				deformerObjects[i].ApplyData ();
		}

		private void OnDestroy ()
		{
			lastHandle.Complete ();
		}

		public static void AddDeformerObject (DeformerObject deformerObject)
		{
			if (instance == null)
				print ("Deformer Object Manager missing from scene. Deformer objects will not update.");
			deformerObjects.Add (deformerObject);
		}
		public static void RemoveDeformerObject (DeformerObject deformerObject)
		{
			deformerObjects.Remove (deformerObject);
		}
	}
}