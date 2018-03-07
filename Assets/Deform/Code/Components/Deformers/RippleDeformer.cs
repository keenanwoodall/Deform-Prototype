using UnityEngine;
using Deform.Math.Trig;

namespace Deform.Deformers
{
	public class RippleDeformer : DeformerComponent
	{
		public float speed;
		public Vector3 offset;
		public Transform axis;
		public Sin sin = new Sin () { frequency = 5f, amplitude = 0.2f };

		private TransformData axisCache;
		private float speedOffset;
		private Matrix4x4 moveSpace;
		private Matrix4x4 inverseAxisSpace;

		public override void PreModify ()
		{
			if (sin.frequency < 0.001f && sin.frequency > -0.001f)
				sin.frequency = 0.001f * Mathf.Sign (sin.frequency);

			if (axis == null)
			{
				axis = new GameObject ("RippleAxis").transform;
				axis.SetParent (transform);
				axis.localPosition = Vector3.zero;
				axis.Rotate (-90f, 0f, 0f);
			}

			axisCache = new TransformData (axis);

			speedOffset += (Manager.SyncedDeltaTime * speed) / sin.frequency;

			moveSpace = Matrix4x4.TRS (Vector3.zero, Quaternion.Inverse (axis.rotation) * transform.rotation, Vector3.one);
			inverseAxisSpace = moveSpace.inverse;
		}

		public override Chunk Modify (Chunk chunk, TransformData transformData, Bounds meshBounds)
		{
			var boundsSize = meshBounds.size.sqrMagnitude;
			var oneOverBoundsSize = 1f / boundsSize;
			for (int vertexIndex = 0; vertexIndex < chunk.vertexData.Length; vertexIndex++)
			{
				var position = moveSpace.MultiplyPoint3x4 (chunk.vertexData[vertexIndex].position);
				var sinOffset = (speedOffset + (meshBounds.center - position + (offset * boundsSize)).sqrMagnitude) * oneOverBoundsSize;
				var positionOffset = new Vector3 (0f, 0f, sin.Solve (sinOffset) * boundsSize);
				position += positionOffset;
				position = inverseAxisSpace.MultiplyPoint3x4 (position);
				chunk.vertexData[vertexIndex].position = position;
			}

			return chunk;
		}
	}
}