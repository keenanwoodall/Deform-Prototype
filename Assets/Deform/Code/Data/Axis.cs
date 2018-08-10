using UnityEngine;
using UnityEngine.Jobs;

namespace Deform
{
	public struct Axis
	{
		public readonly Matrix4x4 axisSpace;
		public readonly Matrix4x4 inverseAxisSpace;

		public Axis (Transform deformObject, Transform axis)
		{
			axisSpace = Matrix4x4.TRS (Vector3.zero, Quaternion.Inverse (axis.rotation) * deformObject.rotation, Vector3.one);
			inverseAxisSpace = axisSpace.inverse;
		}

		public Axis (TransformAccess deformObject, TransformAccess axis)
		{
			axisSpace = Matrix4x4.TRS (Vector3.zero, Quaternion.Inverse (axis.rotation) * deformObject.rotation, Vector3.one);
			inverseAxisSpace = axisSpace.inverse;
		}
	}
}