using UnityEngine;

namespace DForm.DFormers
{
	[AddComponentMenu ("DForm/Position")]
	public class Position : DFormerComponent
	{
		public Vector3 offset;

		public override VertexData[] Modify (VertexData[] vertexData)
		{
			for (var vertexIndex = 0; vertexIndex < vertexData.Length; vertexIndex++)
				vertexData[vertexIndex].position += offset;

			return vertexData;
		}
	}
}