using UnityEngine;

namespace DFNoise
{
	[System.Serializable]
	public class DFNoise3D
	{
		[Range (-1f, 1f)]
		public float _frequency = 0.2f;

		private Vector3 _noiseOffset = new Vector3 (77.7f, 33.3f, 11.1f);
		private float _epsilon = 0.0001f;

		float GetNoise (Vector3 p)
		{
			return Perlin.Noise (p * _frequency);
		}

		public Vector3 GetGradient (Vector3 p)
		{
			var n0 = GetNoise (p);
			var nx = GetNoise (p + Vector3.right * _epsilon);
			var ny = GetNoise (p + Vector3.up * _epsilon);
			var nz = GetNoise (p + Vector3.forward * _epsilon);
			return new Vector3 (nx - n0, ny - n0, nz - n0) / _epsilon;
		}

		public Vector3 GetDFNoise (Vector3 p)
		{
			var g1 = GetGradient (p);
			var g2 = GetGradient (p + _noiseOffset);
			return Vector3.Cross (g1, g2);
		}
	}
}