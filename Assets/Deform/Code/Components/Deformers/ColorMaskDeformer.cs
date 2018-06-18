using System.Runtime.CompilerServices;
using UnityEngine;

namespace Deform.Deformers
{
	public class ColorMaskDeformer : DeformerComponent
	{
		public ColorChannel channel;
		public bool invert;

		private Vector3[] start, end;

		public override MeshData Modify (MeshData meshData, TransformData transformData, Bounds meshBounds)
		{
			if (!invert)
			{
				start = meshData.baseVertices;
				end = meshData.vertices;
			}
			else
			{
				start = meshData.vertices;
				end = meshData.baseVertices;
			}

			for (int i = 0; i < meshData.Size; i++)
			{
				var t = GetChannel (channel, meshData.colors[i]);
				meshData.vertices[i] = (start[i] * (1f - t) + end[i] * t);
			}

			return meshData;
		}

		[MethodImplAttribute (MethodImplOptions.AggressiveInlining)]
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