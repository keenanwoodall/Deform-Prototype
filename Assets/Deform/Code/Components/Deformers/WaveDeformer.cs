using UnityEngine;

namespace Deform.Deformers
{
	public class WaveDeformer : DeformerComponent
	{
		[Range (0f, 1f)]
		public float steepness = 0.25f;
		public float waveLength = 2f;
		public float speed = 1f;
		public float offset = 0f;
		public Transform axis;

		private Matrix4x4 axisSpace;
		private Matrix4x4 inverseAxisSpace;

		public override void PreModify ()
		{
			base.PreModify ();

			if (axis == null)
			{
				axis = new GameObject ("WaveAxis").transform;
				axis.SetParent (transform);
				axis.localPosition = Vector3.zero;
			}

			axisSpace = Matrix4x4.TRS (Vector3.zero, Quaternion.Inverse (axis.rotation) * transform.rotation, axis.localScale);
			inverseAxisSpace = axisSpace.inverse;

			if (waveLength < 0.001f)
				waveLength = 0.001f;
		}
		public override MeshData Modify (MeshData meshData, TransformData transformData, Bounds vertexDataBounds)
		{
			var k = 2f * Mathf.PI / waveLength;
			var c = Mathf.Sqrt (9.8f / k);
			var a = steepness / k;

			for (int i = 0; i < meshData.Size; i++)
			{
				var position = axisSpace.MultiplyPoint3x4 (meshData.vertices[i]);
				var f = k * (position.x - c * Manager.SyncedTime * speed) + offset;
				position.x += a * Mathf.Cos (f);
				position.y += a * Mathf.Sin (f);

				meshData.vertices[i] = inverseAxisSpace.MultiplyPoint3x4 (position);
			}

			return meshData;
		}
	}
}