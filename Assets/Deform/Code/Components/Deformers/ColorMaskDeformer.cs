using UnityEngine;

namespace Deform.Deformers
{
	public class ColorMaskDeformer : DeformerComponent
	{
		public ColorChannel channel;
		public bool invert;

		private Vector3[] start, end;

		public override VertexData[] Modify (VertexData[] vertexData)
		{
			if (!invert)
			{
				start = VertexDataUtil.GetBasePositions (vertexData);
				end = VertexDataUtil.GetPositions (vertexData);
			}
			else
			{
				end = VertexDataUtil.GetBasePositions (vertexData);
				start = VertexDataUtil.GetPositions (vertexData);
			}

			for (var vertexIndex = 0; vertexIndex < vertexData.Length; vertexIndex++)
			{
				vertexData[vertexIndex].position = Vector3.Lerp (
					start[vertexIndex], 
					end[vertexIndex], 
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