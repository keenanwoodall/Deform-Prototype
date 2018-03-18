using UnityEngine;
using Deform.Math.Trig;

namespace Deform.Deformers
{
	public class RippleDeformer : DeformerComponent
	{
		public float speed;
		public float falloff = 1f;
		public AnimationCurve falloffCurve = AnimationCurve.EaseInOut (0f, 1f, 1f, 0f);
		public float offset;
		public Vector2 positionOffset;
		public Transform axis;
		public Sin sin = new Sin () { frequency = 1f, amplitude = 0.2f };

		private TransformData axisCache;
		private float speedOffset;
		private float finalOffset;
		private Matrix4x4 axisSpace;
		private Matrix4x4 inverseAxisSpace;

		public override void PreModify ()
		{
			if (sin.frequency < 0.001f && sin.frequency > -0.001f)
				sin.frequency = 0.001f * Mathf.Sign (sin.frequency);

			if (axis == null)
			{
				axis = new GameObject ("RippleAxis").transform;
				axis.SetParent (transform);
				axis.localPosition = Vector3.zero;
				axis.Rotate (-90f, 0f, 0f);
			}

			axisCache = new TransformData (axis);

			speedOffset += (Manager.SyncedDeltaTime * speed) / sin.frequency;
			finalOffset = speedOffset + ((sin.frequency != 0f) ? offset / sin.frequency : 0f);
			axisSpace = Matrix4x4.TRS (Vector3.zero, Quaternion.Inverse (axis.rotation) * transform.rotation, Vector3.one);
			inverseAxisSpace = axisSpace.inverse;
		}

		public override VertexData[] Modify (VertexData[] vertexData, TransformData transformData, Bounds meshBounds)
		{
			float maxWidth = float.MinValue;

			// Find the width.
			for (int vertexIndex = 0; vertexIndex < vertexData.Length; vertexIndex++)
			{
				var position = axisSpace.MultiplyPoint3x4 (vertexData[vertexIndex].position);
				var width = new Vector2 (position.x, position.y).sqrMagnitude;
				if (width > maxWidth)
					maxWidth = width;
			}

			for (int vertexIndex = 0; vertexIndex < vertexData.Length; vertexIndex++)
			{
				var position = axisSpace.MultiplyPoint3x4 (vertexData[vertexIndex].position);
				var xyMagnitude = (new Vector2 (position.x, position.y) + this.positionOffset).sqrMagnitude;
				var sinOffset = finalOffset + xyMagnitude * maxWidth;
				var positionOffset = new Vector3 (0f, 0f, sin.Solve (sinOffset) * falloffCurve.Evaluate (xyMagnitude * falloff));
				position += positionOffset;
				position = inverseAxisSpace.MultiplyPoint3x4 (position);
				vertexData[vertexIndex].position = position;
			}

			return vertexData;
		}
	}
}