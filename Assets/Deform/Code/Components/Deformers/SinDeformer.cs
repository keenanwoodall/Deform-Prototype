using UnityEngine;
using Deform.Math.Trig;

namespace Deform.Deformers
{
	public class SinDeformer : DeformerComponent
	{
		public float offset;
		public float speed;
		public bool usePosition;
		public bool useRotation;
		public bool useScale;

		public Axis along = Axis.X;
		public Axis by = Axis.Y;

		public Sin sin = new Sin ();

		private Vector3 axisOffset;
		private float speedOffset;

		public override void PreModify ()
		{
			speedOffset += Manager.SyncedDeltaTime * speed;

			switch (along)
			{
				case Axis.X:
					axisOffset = Vector3.right * offset;
					break;
				case Axis.Y:
					axisOffset = Vector3.up * offset;
					break;
				case Axis.Z:
					axisOffset = Vector3.forward * offset;
					break;
			}
		}

		public override Chunk Modify (Chunk chunk, TransformData transformData, Bounds bounds)
		{
			for (var vertexIndex = 0; vertexIndex < chunk.vertexData.Length; vertexIndex++)
			{
				var samplePosition = chunk.vertexData[vertexIndex].position + axisOffset;
				if (usePosition)
					samplePosition += transformData.position;
				if (useRotation)
					samplePosition = transformData.rotation * samplePosition;
				if (useScale)
					samplePosition.Scale (transformData.localScale);
				chunk.vertexData[vertexIndex].position += Sin3D (samplePosition);
			}

			return chunk;
		}

		private Vector3 Sin3D (Vector3 sample)
		{
			var animatedOffset = offset + speedOffset;
			var byValue = 0f;
			switch (by)
			{
				case Axis.X:
					byValue = sample.x;
					break;
				case Axis.Y:
					byValue = sample.y;
					break;
				case Axis.Z:
					byValue = sample.z;
					break;
			}
			switch (along)
			{
				case Axis.X:
					return new Vector3 (sin.Solve (byValue, animatedOffset), 0f, 0f);
				case Axis.Y:
					return new Vector3 (0f, sin.Solve (byValue, animatedOffset), 0f);
				default:
					return new Vector3 (0f, 0f, sin.Solve (byValue, animatedOffset));
			}
		}
	}
}