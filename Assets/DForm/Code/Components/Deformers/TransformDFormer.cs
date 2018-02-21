using UnityEngine;

namespace DForm.DFormers
{
	public class TransformDFormer : DFormerComponent
	{
		public Vector3 position;
		public Vector3 rotation;
		public Vector3 scale;

		private Matrix4x4 transformSpace;

		public override void PreModify ()
		{
			transformSpace = Matrix4x4.identity;
			transformSpace *= Matrix4x4.TRS (position, Quaternion.Euler (rotation), scale);
		}

		public override VertexData[] Modify (VertexData[] vertexData)
		{
			for (var vertexIndex = 0; vertexIndex < vertexData.Length; vertexIndex++)
				vertexData[vertexIndex].position = transformSpace.MultiplyPoint3x4 (vertexData[vertexIndex].position);

			return vertexData;
		}
	}
}