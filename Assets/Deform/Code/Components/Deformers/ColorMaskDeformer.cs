using UnityEngine;

namespace Deform.Deformers
{
	public class ColorMaskDeformer : DeformerComponent
	{
		public ColorChannel channel;
		public bool invert;

		private Vector3[] start, end;

		public override Chunk Modify (Chunk chunk, TransformData transformData, Bounds bounds)
		{
			if (!invert)
			{
				start = VertexDataUtil.GetBasePositions (chunk.vertexData);
				end = VertexDataUtil.GetPositions (chunk.vertexData);
			}
			else
			{
				end = VertexDataUtil.GetBasePositions (chunk.vertexData);
				start = VertexDataUtil.GetPositions (chunk.vertexData);
			}

			for (int vertexIndex = 0; vertexIndex < chunk.vertexData.Length; vertexIndex++)
			{
				chunk.vertexData[vertexIndex].position = Vector3.Lerp (
					start[vertexIndex], 
					end[vertexIndex], 
					GetChannel (channel, chunk.vertexData[vertexIndex].color));
			}

			return chunk;
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