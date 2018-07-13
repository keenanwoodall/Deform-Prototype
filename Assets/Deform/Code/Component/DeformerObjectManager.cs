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

			JobHandle previousHandle = new JobHandle ();
			for (int i = 0; i < deformerObjects.Count; i++)
				previousHandle = deformerObjects[i].DeformData (previousHandle);
			previousHandle.Complete ();
			for (int i = 0; i < deformerObjects.Count; i++)
				deformerObjects[i].ApplyData ();
		}

		public static void AddDeformerObject (DeformerObject deformerObject)
		{
			deformerObjects.Add (deformerObject);
		}
		public static void RemoveDeformerObject (DeformerObject deformerObject)
		{
			deformerObjects.Remove (deformerObject);
		}
	}
}