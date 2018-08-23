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
		public enum NoiseSpace { None, Normal, Spherical}
		public NoiseSpace space;

		private float speedOffset;

		private void Update ()
		{
			speedOffset += speed * Time.deltaTime;
		}

		public override JobHandle Deform (NativeMeshData data, JobHandle dependency)
		{
			switch (space)
			{
				case NoiseSpace.None:
					return new NoneDeformJob ()
					{
						magnitude = magnitude,
						frequency = frequency,
						offset = offset + speedOffset,
						data = data
					}.Schedule (data.size, BATCH_COUNT, dependency);
				case NoiseSpace.Normal:
					return new NormalDeformJob ()
					{
						magnitude = magnitude,
						frequency = frequency,
						offset = offset + speedOffset,
						data = data
					}.Schedule (data.size, BATCH_COUNT, dependency);
				case NoiseSpace.Spherical:
					return new SphericalDeformJob ()
					{
						magnitude = magnitude,
						frequency = frequency,
						offset = offset + speedOffset,
						data = data
					}.Schedule (data.size, BATCH_COUNT, dependency);
				default:
					Debug.LogError (space.ToString () + " is not supported.");
					return new JobHandle ();
			}
		}

		[BurstCompile]
		private struct NoneDeformJob : IJobParallelFor
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

		[BurstCompile]
		private struct NormalDeformJob : IJobParallelFor
		{
			public float magnitude;
			public float frequency;
			public float offset;
			public NativeMeshData data;

			public void Execute (int index)
			{
				var position = data.vertices[index];
				var noiseOffset = data.normals[index] * noise.cnoise ((position + offset) * frequency) * magnitude;
				
				data.vertices[index] = position + noiseOffset;
			}
		}

		[BurstCompile]
		private struct SphericalDeformJob : IJobParallelFor
		{
			public float magnitude;
			public float frequency;
			public float offset;
			public NativeMeshData data;

			public void Execute (int index)
			{
				var position = data.vertices[index];
				var noiseOffset = normalize (data.vertices[index]) * noise.cnoise ((position + offset) * frequency) * magnitude;

				data.vertices[index] = position + noiseOffset;
			}
		}
	}
}