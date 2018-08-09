using UnityEngine;
using Unity.Jobs;
using Unity.Burst;
using Deform.Data;

namespace Deform.Deformers
{
	public class TransformDeformer : Deformer
	{
		public Vector3 position = Vector3.zero;
		public Vector3 rotation = Vector3.zero;
		public Vector3 scale = Vector3.one;

		private Quaternion quaternionRotation;
		private Matrix4x4 transformMatrix;

		public override JobHandle Deform (NativeMeshData data, JobHandle dependency)
		{
			quaternionRotation = Quaternion.Euler (rotation);
			transformMatrix = Matrix4x4.TRS (position, quaternionRotation, scale);
			return new DeformJob (transformMatrix, quaternionRotation, data).Schedule (data.size, BATCH_COUNT, dependency);
		}

		[BurstCompile]
		private struct DeformJob : IJobParallelFor
		{
			public readonly Matrix4x4 matrix;
			public readonly Quaternion quaternionRotation;
			public NativeMeshData data;

			public DeformJob (Matrix4x4 matrix, Quaternion quaternionRotation, NativeMeshData data)
			{
				this.matrix = matrix;
				this.quaternionRotation = quaternionRotation;
				this.data = data;
			}

			public void Execute (int index)
			{
				var vertice = data.vertices[index];
				vertice = matrix.MultiplyPoint3x4 (vertice);
				data.vertices[index] = vertice;

				var normal = data.normals[index];
				normal = quaternionRotation * normal;
				data.normals[index] = normal;
			}
		}
	}
}