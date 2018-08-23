using UnityEngine;
using Unity.Jobs;
using UnityEngine.Jobs;
using Unity.Mathematics;
using static Unity.Mathematics.math;
using Unity.Burst;
using Deform.Data;
using Unity.Collections;

namespace Deform.Deformers
{
	public class NoiseDeformer : Deformer
	{

		public float magnitude = 0.5f;
		public float frequency = 1f;
		public float speed = 0f;
		public float offset = 0f;

		private float speedOffset;

		private void Update ()
		{
			speedOffset += speed * Time.deltaTime / ((frequency == 0f) ? 1f : frequency);
		}

		public override JobHandle Deform (NativeMeshData data, JobHandle dependency)
		{
			return new DeformJob ()
			{
				magnitude = magnitude,
				frequency = frequency,
				offset = offset + speedOffset,
				data = data
			}.Schedule (data.size, BATCH_COUNT, dependency);
		}

		[BurstCompile]
		private struct DeformJob : IJobParallelFor
		{
			public float magnitude;
			public float frequency;
			public float offset;
			public NativeMeshData data;

			public void Execute (int index)
			{
				var position = data.vertices[index];

				var noiseOffset = float3 
				(
					noise.cnoise (float3 (position.y, position.z, offset) * frequency),
					noise.cnoise (float3 (position.x, position.z, offset) * frequency),
					noise.cnoise (float3 (position.x, position.y, offset) * frequency)
				) * magnitude;

				data.vertices[index] = position + noiseOffset;
			}
		}
	}
}