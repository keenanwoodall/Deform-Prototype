using UnityEngine;

namespace Deform.Deformers
{
	public class ColorMaskDeformer : DeformerComponent
	{
		public ColorChannel channel;

		public override VertexData[] Modify (VertexData[] vertexData)
		{
			for (var vertexIndex = 0; vertexIndex < vertexData.Length; vertexIndex++)
			{
				vertexData[vertexIndex].position = Vector3.Lerp (
					vertexData[vertexIndex].position, 
					vertexData[vertexIndex].basePosition, 
					GetChannel (channel, vertexData[vertexIndex].color));
			}

			return vertexData;
		}

		private float GetChannel (ColorChannel channel, Color color)
		{
			switch (channel)
			{
				case ColorChannel.R:
					return color.r;
				case ColorChannel.G:
					return color.g;
				case ColorChannel.B:
					return color.b;
				default:
					return color.a;
			}
		}
	}
}