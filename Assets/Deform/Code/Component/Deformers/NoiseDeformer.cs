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
		// General noise settings
		public float magnitude = 0.1f;
		public float frequency = 1f;
		public float speed = .1f;
		public Vector3 offset = Vector3.zero;
		public enum NoiseSpace { Derivative, Local, Normal, Color }
		public NoiseSpace noiseSpace;
		[Header ("Local Noise Space Settings")]
		public Vector3 localNoiseDirection = Vector3.up;
		[Header ("Color Noise Space Settings")]
		public bool alphaInfluence = true;

		private float speedOffset;

		private void Update ()
		{
			speedOffset += speed * Time.deltaTime / ((frequency == 0f) ? 1f : frequency);
		}

		public override JobHandle Deform (NativeMeshData data, JobHandle dependency)
		{
			switch (noiseSpace)
			{
				case NoiseSpace.Derivative:
					return new DerivativeDeformJob ()
					{
						magnitude = magnitude,
						frequency = frequency,
						offset = offset + (Vector3.one * speedOffset),
						data = data
					}.Schedule (data.size, BATCH_COUNT, dependency);
				case NoiseSpace.Local:
					return new LocalDeformJob ()
					{
						magnitude = magnitude,
						frequency = frequency,
						offset = offset + (Vector3.one * speedOffset),
						direction = localNoiseDirection,
						data = data
					}.Schedule (data.size, BATCH_COUNT, dependency);
				case NoiseSpace.Normal:
					return new NormalDeformJob ()
					{
						magnitude = magnitude,
						frequency = frequency,
						offset = offset + (Vector3.one * speedOffset),
						data = data
					}.Schedule (data.size, BATCH_COUNT, dependency);
				case NoiseSpace.Color:
					return new ColorDeformJob ()
					{
						magnitude = magnitude,
						frequency = frequency,
						offset = offset + (Vector3.one * speedOffset),
						alphaInfluence = alphaInfluence,
						data = data
					}.Schedule (data.size, BATCH_COUNT, dependency);
				default:
					throw new System.Exception (string.Format ("{0} is not a suppported noise space.", noiseSpace.ToString ()));
			}
		}

		[BurstCompile]
		private struct DerivativeDeformJob : IJobParallelFor
		{
			public float magnitude;
			public float frequency;
			public float3 offset;
			public NativeMeshData data;

			public void Execute (int index)
			{
				var position = data.vertices[index];

				var noiseOffset = float3 
				(
					noise.cnoise (float3 (position.y + offset.y, position.z + offset.z, offset.x) * frequency),
					noise.cnoise (float3 (position.x + offset.x, position.z + offset.z, offset.y) * frequency),
					noise.cnoise (float3 (position.x + offset.x, position.y + offset.y, offset.z) * frequency)
				) * magnitude;

				data.vertices[index] = position + noiseOffset;
			}
		}

		[BurstCompile]
		private struct LocalDeformJob : IJobParallelFor
		{
			public float magnitude;
			public float frequency;
			public float3 offset;
			public float3 direction;
			public NativeMeshData data;

			public void Execute (int index)
			{
				var position = data.vertices[index];

				var noiseOffset = (direction * noise.cnoise (float3 (position.x + offset.x, position.y + offset.y, position.z + offset.z) * frequency)) * magnitude;

				data.vertices[index] = position + noiseOffset;
			}
		}

		[BurstCompile]
		private struct NormalDeformJob : IJobParallelFor
		{
			public float magnitude;
			public float frequency;
			public float3 offset;
			public NativeMeshData data;

			public void Execute (int index)
			{
				var position = data.vertices[index];

				var noiseOffset = (data.normals[index] * noise.cnoise (float3 (position.x + offset.x, position.y + offset.y, position.z + offset.z) * frequency)) * magnitude;

				data.vertices[index] = position + noiseOffset;
			}
		}
		[BurstCompile]
		private struct ColorDeformJob : IJobParallelFor
		{
			public float magnitude;
			public float frequency;
			public float3 offset;
			public bool alphaInfluence;
			public NativeMeshData data;

			public void Execute (int index)
			{
				var position = data.vertices[index];
				var color = data.colors[index];
				var noiseOffset = color.xyz * noise.cnoise (float3 (position.x + offset.x, position.y + offset.y, position.z + offset.z) * frequency) * magnitude * (alphaInfluence ? color.w : 1f);

				data.vertices[index] = position + noiseOffset;
			}
		}
	}
}