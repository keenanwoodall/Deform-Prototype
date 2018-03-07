using UnityEngine;
using Deform.Math.Trig;

namespace Deform.Deformers
{
	public class RippleDeformer : DeformerComponent
	{
		public Vector3 offset;
		public float speed;
		public Transform axis;
		public Sin sin = new Sin ();

		private float speedOffset;
		private Matrix4x4 moveSpace;
		private Matrix4x4 meshSpace;

		public override void PreModify ()
		{
			if (sin.frequency < 0.001f && sin.frequency > -0.001f)
				sin.frequency = 0.001f * Mathf.Sign (sin.frequency);

			if (axis == null)
			{
				axis = new GameObject ("RadialSinAxis").transform;
				axis.SetParent (transform);
				axis.localPosition = Vector3.zero;
				axis.Rotate (-90f, 0f, 0f);
			}

			speedOffset += Manager.SyncedDeltaTime * speed / sin.frequency;

			moveSpace = Matrix4x4.TRS (Vector3.zero, Quaternion.Inverse (axis.rotation) * transform.rotation, Vector3.one);
			meshSpace = moveSpace.inverse;
		}

		public override Chunk Modify (Chunk chunk, TransformData transformData, Bounds bounds)
		{
			for (int vertexIndex = 0; vertexIndex < chunk.vertexData.Length; vertexIndex++)
			{
				var position = moveSpace.MultiplyPoint3x4 (chunk.vertexData[vertexIndex].position);
				var positionOffset = new Vector3 (0f, 0f, sin.Solve (speedOffset + (chunk.vertexData[vertexIndex].position - offset).sqrMagnitude));
				position += positionOffset;
				position = meshSpace.MultiplyPoint3x4 (position);
				chunk.vertexData[vertexIndex].position = position;
			}

			return chunk;
		}
	}
}