using UnityEngine;
using Unity.Jobs;
using UnityEngine.Jobs;
using Unity.Burst;
using static Unity.Mathematics.math;
using Deform.Data;

namespace Deform.Deformers
{
	public class SinDeformer : Deformer
	{
		public float amplitude = 0.1f;
		public float frequency = 0.5f;
		public float offset = 0f;
		public float speed = 0f;
		public Transform by;
		public Transform along;

		private float speedOffset;

		private void Update ()
		{
			speedOffset += speed * Time.deltaTime;
		}

		public override JobHandle Deform (NativeMeshData data, JobHandle dependency)
		{
			var job = new DeformJob (amplitude, frequency, speedOffset + offset, new Axis (transform, by), new Axis (transform, along), data);
			return job.Schedule (data.size, BATCH_COUNT, dependency);
		}

		[BurstCompile]
		private struct DeformJob : IJobParallelFor
		{
			public readonly float amplitude;
			public readonly float frequency;
			public readonly float offset;
			public readonly Axis by, along;
			private NativeMeshData data;

			public DeformJob (float amplitude, float frequency, float offset, Axis by, Axis along, NativeMeshData data)
			{
				this.amplitude = amplitude;
				this.frequency = frequency;
				this.offset = offset;
				this.by = by;
				this.along = along;
				this.data = data;
			}

			public void Execute (int index)
			{
				var t = mul (by.axisSpace, float4 (data.vertices[index], 1f)).z;
				var position = mul (along.axisSpace, float4 (data.vertices[index], 1f));
				position.z += Mathf.Sin ((t + offset) * frequency) * amplitude;
				data.vertices[index] = mul (along.inverseAxisSpace, position).xyz;
			}
		}
	}
}