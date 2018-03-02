using UnityEngine;

namespace Deform
{
	public abstract class NoiseDeformerComponent : DeformerComponent
	{
		public float globalMagnitude = 1f;
		public Vector3 magnitude = Vector3.one;
		public Vector3 offset;
		public Vector3 speed;

		public bool usePosition;
		public bool useRotation;
		public bool useScale;

		private Vector3 _magnitude;
		private Vector3 speedOffset;

		public NoiseSpace space;

		public override void PreModify ()
		{
			_magnitude = magnitude * globalMagnitude;
			speedOffset += speed * Manager.SyncedDeltaTime;
		}

		protected Vector3 CalculateSampleCoordinate (VertexData vertex, TransformData transformData)
		{
			var sample = vertex.position + speedOffset + offset;
			if (usePosition)
				sample += transformData.position;
			if (useRotation)
				sample = transformData.rotation * sample;
			if (useScale)
				sample.Scale (transformData.localScale);

			return sample;
		}

		protected Vector3 TransformNoise (float noise, VertexData vertex)
		{
			switch (space)
			{
				case NoiseSpace.Local:
					return vertex.position + (noise * magnitude) - _magnitude;
				case NoiseSpace.Normal:
					return vertex.position + (Vector3.Scale (vertex.normal, _magnitude * noise));
				case NoiseSpace.Tangent:
					return vertex.position + (Vector3.Scale (vertex.tangent, _magnitude * noise));
				default:
					return Vector3.Scale (vertex.position, Vector3.one + (noise * _magnitude));
			}
		}

		protected Vector3 TransformNoiseSpace (Vector3 noise, VertexData vertex)
		{
			switch (space)
			{
				case NoiseSpace.Local:
					return vertex.position + (Vector3.Scale (noise, _magnitude)) - _magnitude;
				case NoiseSpace.Normal:
					return vertex.position + (Vector3.Scale (vertex.normal, Vector3.Scale (noise, _magnitude)));
				case NoiseSpace.Tangent:
					return vertex.position + (Vector3.Scale (vertex.tangent, Vector3.Scale (noise, _magnitude)));
				default:
					return Vector3.Scale (vertex.position, Vector3.one + Vector3.Scale (noise, _magnitude));
			}
		}
	}
}