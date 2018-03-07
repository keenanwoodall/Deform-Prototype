using UnityEngine;

namespace Deform.Deformers
{
	public class TransformDeformer : DeformerComponent
	{
		public Vector3 position = Vector3.zero;
		public Vector3 rotation = Vector3.zero;
		public Vector3 scale = Vector3.one;

		private Matrix4x4 transformSpace;

		public override void PreModify ()
		{
			transformSpace = Matrix4x4.TRS (position, Quaternion.Euler (rotation), scale);
		}

		public override VertexData[] Modify (VertexData[] vertexData, TransformData transformData, Bounds meshBounds)
		{
			for (int vertexIndex = 0; vertexIndex < vertexData.Length; vertexIndex++)
				vertexData[vertexIndex].position = transformSpace.MultiplyPoint3x4 (vertexData[vertexIndex].position);

			return vertexData;
		}
	}
}