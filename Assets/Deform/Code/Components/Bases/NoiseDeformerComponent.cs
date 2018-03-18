using UnityEngine;
using System.Runtime.CompilerServices;

namespace Deform
{
	/// <summary>
	/// This class inherits from DeformerComponent and it's sole purpose is to assist in the making of
	/// noise deformers. Of course, you don't have to use this method to make noise deformers, but it does
	/// take care of scaling/offsetting sample coordinates and transforming your noise vector into different
	/// noise spaces.
	/// </summary>
	public abstract class NoiseDeformerComponent : DeformerComponent
	{
		public bool abs;
		public bool usePosition;
		public bool useRotation;
		public bool useScale;

		public float globalMagnitude = 1f;
		public Vector3 magnitude = Vector3.one;
		public Vector3 offset;
		public Vector3 speed;

		private Vector3 _magnitude;
		private Vector3 speedOffset;

		public NoiseSpace space;

		public override void PreModify ()
		{
			_magnitude = magnitude * globalMagnitude;

			var frequency = GetFrequency ();
			if (frequency < 0.0001f && frequency > -0.0001f)
				frequency = Mathf.Sign (frequency) * 0.0001f;

			speedOffset += speed * Manager.SyncedDeltaTime / frequency;
		}

		/// <summary>
		/// Send a vertex to get a correctly offset sample coordinate to plugin into your noise function of choice.
		/// </summary>
		/// <returns>Returns a coordinate to plug into your noise.</returns>
		[MethodImplAttribute (MethodImplOptions.AggressiveInlining)]
		protected Vector3 CalculateSampleCoordinate (VertexData vertex, TransformData transformData)
		{
			var sample = vertex.position + speedOffset + offset;
			if (useRotation)
				sample = transformData.rotation * sample;
			if (usePosition)
				sample += transformData.position;
			if (useScale)
				sample.Scale (transformData.localScale);

			return sample;
		}

		/// <summary>
		/// Converts your noise sample into your desired space (Local, Normal, Tangent, Spherical)
		/// based on the 'space' property included in this class.
		/// </summary>
		[MethodImplAttribute (MethodImplOptions.AggressiveInlining)]
		protected Vector3 TransformNoise (float noise, VertexData vertex)
		{
			if (abs)
				noise = Mathf.Abs (noise);
			switch (space)
			{
				case NoiseSpace.Local:
					return vertex.position + (noise * _magnitude) - _magnitude;
				case NoiseSpace.Normal:
					return vertex.position + (Vector3.Scale (vertex.normal, _magnitude * noise));
				case NoiseSpace.Tangent:
					return vertex.position + (Vector3.Scale (vertex.tangent, _magnitude * noise));
				default:
					return Vector3.Scale (vertex.position, Vector3.one + (noise * _magnitude));
			}
		}

		/// <summary>
		/// Converts your noise vector into your desired space (Local, Normal, Tangent, Spherical)
		/// based on the 'space' property included in this class.
		/// </summary>
		[MethodImplAttribute (MethodImplOptions.AggressiveInlining)]
		protected Vector3 TransformNoise (Vector3 noise, VertexData vertex)
		{
			if (abs)
			{
				noise.x = Mathf.Abs (noise.x);
				noise.y = Mathf.Abs (noise.y);
				noise.z = Mathf.Abs (noise.z);
			}
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

		/// <summary>
		/// Override this and simply return your frequency variable. 
		/// This class needs access to it so that speedOffset can be scaled based on frequency. 
		/// This prevent speedOffset being frequency dependant (appearing to move slower/faster based on frequency)
		/// </summary>
		protected abstract float GetFrequency ();
	}
}