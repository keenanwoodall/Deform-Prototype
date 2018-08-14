using UnityEngine;
using UnityEngine.Jobs;
using Unity.Mathematics;

namespace Deform
{
	public struct Axis
	{
		public readonly float4x4 axisSpace;
		public readonly float4x4 inverseAxisSpace;

		public Axis (Transform deformObject, Transform axis)
		{
			var matrix = Matrix4x4.TRS (Vector3.zero, Quaternion.Inverse (axis.rotation) * deformObject.rotation, Vector3.one);
			axisSpace = matrix;
			inverseAxisSpace = matrix.inverse;
		}

		public Axis (TransformAccess deformObject, TransformAccess axis)
		{
			var matrix = Matrix4x4.TRS (Vector3.zero, Quaternion.Inverse (axis.rotation) * deformObject.rotation, Vector3.one);
			axisSpace = matrix;
			inverseAxisSpace = matrix.inverse;
		}
	}
}