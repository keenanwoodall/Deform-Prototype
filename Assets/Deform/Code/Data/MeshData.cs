using UnityEngine;
using Unity.Jobs;
using static Unity.Mathematics.math;
using Unity.Burst;

namespace Deform.Data
{
	public class MeshData
	{
		[SerializeField] [HideInInspector]
		private Mesh originalMesh;
		private Mesh dynamicMesh;

		private ManagedMeshData originalData;
		public ManagedMeshData dynamicData { get; private set; }
		public NativeMeshData nativeData { get; private set; }

		public MeshData (MeshFilter meshFilter)
		{
			if (originalMesh == null)
				originalMesh = meshFilter.sharedMesh;
			meshFilter.sharedMesh = dynamicMesh = GameObject.Instantiate (originalMesh);

			dynamicMesh.MarkDynamic ();

			originalData = new ManagedMeshData (dynamicMesh);
			dynamicData = new ManagedMeshData (dynamicMesh);
			nativeData = new NativeMeshData (originalData, Unity.Collections.Allocator.Persistent);
		}

		public JobHandle RecalculateNormalsAsync (JobHandle dependency)
		{
			var resetHandle = new ResetNormalsJob () { data = nativeData }.Schedule (nativeData.size, 256, dependency);
			var calcHandle = new RecalculateNormalsJob () { data = nativeData }.Schedule (resetHandle);
			var normHandle = new NormalizeNormalsJob () { data = nativeData }.Schedule (nativeData.size, 256, calcHandle);

			return normHandle;
		}

		public void ApplyData (bool updateBounds)
		{
			if (nativeData.vertices.IsCreated)
			{
				nativeData.CopyTo (dynamicData);
				dynamicMesh.vertices = dynamicData.vertices;
				dynamicMesh.normals = dynamicData.normals;
				dynamicMesh.tangents = dynamicData.tangents;
				dynamicMesh.uv = dynamicData.uv;
			}

			nativeData.CopyFrom (originalData);

			if (updateBounds)
				dynamicMesh.RecalculateBounds ();
		}

		public void Dispose ()
		{
			GameObject.Destroy (dynamicMesh);
			nativeData.Dispose ();
		}
		[BurstCompile]
		private struct ResetNormalsJob : IJobParallelFor
		{
			public NativeMeshData data;

			public void Execute (int index)
			{
				data.normals[index] = float3 (0);
			}
		}
		[BurstCompile]
		private struct RecalculateNormalsJob : IJob
		{
			public NativeMeshData data;

			public void Execute ()
			{
				for (int i = 0; i < data.triangles.Length; i+=3)
				{
					// vertex index from triangle
					var t0 = data.triangles[i];
					var t1 = data.triangles[i + 1];
					var t2 = data.triangles[i + 2];
					// vertex
					var v0 = data.vertices[t0];
					var v1 = data.vertices[t1];
					var v2 = data.vertices[t2];
					// triangle normal
					var n = float3
					(
						v0.y * v1.z - v0.y * v2.z - v1.y * v0.z + v1.y * v2.z + v2.y * v0.z - v2.y * v1.z,
						-v0.x * v1.z + v0.x * v2.z + v1.x * v0.z - v1.x * v2.z - v2.x * v0.z + v2.x * v1.z,
						v0.x * v1.y - v0.x * v2.y - v1.x * v0.y + v1.x * v2.y + v2.x * v0.y - v2.x * v1.y
					);
					// add normal
					data.normals[t0] += n;
					data.normals[t1] += n;
					data.normals[t2] += n;
				}
			}
		}
		[BurstCompile]
		private struct NormalizeNormalsJob : IJobParallelFor
		{
			public NativeMeshData data;

			public void Execute (int index)
			{
				data.normals[index] = normalize (data.normals[index]);
			}
		}
	}
}