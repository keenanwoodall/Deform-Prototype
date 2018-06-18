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

		public override MeshData Modify (MeshData meshData, TransformData transformData, Bounds meshBounds)
		{
			for (int i = 0; i < meshData.Size; i++)
				meshData.vertices[i] = transformSpace.MultiplyPoint3x4 (meshData.vertices[i]);

			return meshData;
		}
	}
}