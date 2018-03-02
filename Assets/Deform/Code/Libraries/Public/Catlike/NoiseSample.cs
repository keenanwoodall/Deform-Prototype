using UnityEngine;

namespace Catlike
{
	public struct NoiseSample
	{
		public float value;
		public Vector3 derivative;

		public static NoiseSample operator + (NoiseSample a, float b)
		{
			a.value += b;
			return a;
		}

		public static NoiseSample operator + (float a, NoiseSample b)
		{
			b.value += a;
			return b;
		}

		public static NoiseSample operator + (NoiseSample a, NoiseSample b)
		{
			a.value += b.value;
			a.derivative += b.derivative;
			return a;
		}

		public static NoiseSample operator - (NoiseSample a, float b)
		{
			a.value -= b;
			return a;
		}

		public static NoiseSample operator - (float a, NoiseSample b)
		{
			b.value = a - b.value;
			b.derivative = -b.derivative;
			return b;
		}

		public static NoiseSample operator - (NoiseSample a, NoiseSample b)
		{
			a.value -= b.value;
			a.derivative -= b.derivative;
			return a;
		}

		public static NoiseSample operator * (NoiseSample a, float b)
		{
			a.value *= b;
			a.derivative *= b;
			return a;
		}

		public static NoiseSample operator * (float a, NoiseSample b)
		{
			b.value *= a;
			b.derivative *= a;
			return b;
		}

		public static NoiseSample operator * (NoiseSample a, NoiseSample b)
		{
			a.derivative = a.derivative * b.value + b.derivative * a.value;
			a.value *= b.value;
			return a;
		}
	}
}