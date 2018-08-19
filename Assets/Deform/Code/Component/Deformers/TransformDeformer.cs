using UnityEngine;
using Unity.Jobs;
using Unity.Burst;
using Unity.Mathematics;
using static Unity.Mathematics.math;
using Deform.Data;

namespace Deform.Deformers
{
	public class TransformDeformer : Deformer
	{
		public Vector3 position = Vector3.zero;
		public Vector3 rotation = Vector3.zero;
		public Vector3 scale = Vector3.one;

		private float4x4 transformMatrix;

		public override JobHandle Deform (NativeMeshData data, JobHandle dependency)
		{
			transformMatrix = Matrix4x4.TRS (position, Quaternion.Euler (rotation), scale);

			return new DeformJob
			{
				matrix = Matrix4x4.TRS (position, Quaternion.Euler (rotation), scale),
				data = data
			}.Schedule (data.size, BATCH_COUNT, dependency);
		}

		[BurstCompile]
		private struct DeformJob : IJobParallelFor
		{
			public float4x4 matrix;
			public NativeMeshData data;

			public void Execute (int index)
			{
				var vertice = data.vertices[index];
				vertice = mul (matrix, float4 (vertice, 1f)).xyz;
				data.vertices[index] = vertice;

				var normal = data.normals[index];
				normal = mul (matrix, float4 (normal, 1f)).xyz;
				data.normals[index] = normal;
			}
		}
	}
}