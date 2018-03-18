using UnityEngine;

namespace Deform.Deformers
{
	public class TextureMaskDeformer : DeformerComponent
	{
		[Range (0f, 1f)]
		public float strength = 1f;
		public ColorChannel channel;
		public Texture2D texture;
		private Color[] colors;
		private int width;
		private int height;
		private int channelIndex;

		public override void PreModify ()
		{
			base.PreModify ();
			if (texture != null)
			{
				colors = texture.GetPixels ();
				width = texture.width;
				height = texture.height;
			}
			else
			{
				colors = new Color[4];
				width = 2;
				height = 2;
			}
			channelIndex = (int)channel;
		}

		public override VertexData[] Modify (VertexData[] vertexData, TransformData transformData, Bounds meshBounds)
		{
			Vector2 uv;
			Vector2Int pixel;
			Color color;
			for (var vertexIndex = 0; vertexIndex < vertexData.Length; vertexIndex++)
			{
				uv = vertexData[vertexIndex].uv;
				pixel = new Vector2Int ((int)(uv.x * width), (int)(uv.y * height));
				color = colors[pixel.x + width * pixel.y];
				var a = vertexData[vertexIndex].position;
				var b = vertexData[vertexIndex].basePosition;
				var t = color[channelIndex] * strength;
				vertexData[vertexIndex].position = a * (1f - t) + b * t;
			}

			return vertexData;
		}
	}
}