using UnityEngine;

namespace Deform.Deformers
{
	public class SpherifyDeformer : DeformerComponent
	{
		public float radius = 1f;
		[Range (0f, 1f)]
		public float strength = 0f;
		public Vector3 offset;

		private float oneMinusStrength;

		public override void PreModify ()
		{
			base.PreModify ();

			oneMinusStrength = 1f - strength;
		}

		public override VertexData[] Modify (VertexData[] vertexData, TransformData transformData, Bounds meshBounds)
		{
			var boundsSize = meshBounds.size.magnitude * 0.5f;
			for (int vertexIndex = 0; vertexIndex < vertexData.Length; vertexIndex++)
			{
				var a = vertexData[vertexIndex].position;
				var b = meshBounds.center + offset + ((vertexData[vertexIndex].position - meshBounds.center).normalized * (radius * boundsSize));
				vertexData[vertexIndex].position = a * oneMinusStrength + b * strength;
			}

			return vertexData;
		}
	}
}