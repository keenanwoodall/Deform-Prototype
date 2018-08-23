using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;
using UnityEngine;

namespace Deform.Data
{
	public struct NativeMeshData
	{
		public NativeArray<float3> vertices;
		public NativeArray<float3> normals;
		public NativeArray<float4> tangents;
		public NativeArray<float2> uv;

		public readonly int size;

		public NativeMeshData (ManagedMeshData data, Allocator allocator)
		{
			var length = data.size;
			vertices = new NativeArray<float3> (length, allocator, NativeArrayOptions.UninitializedMemory);
			normals = new NativeArray<float3> (length, allocator, NativeArrayOptions.UninitializedMemory);
			tangents = new NativeArray<float4> (length, allocator, NativeArrayOptions.UninitializedMemory);
			uv = new NativeArray<float2> (length, allocator, NativeArrayOptions.UninitializedMemory);

			size = data.vertices.Length;
			
			CopyVector3ArrayIntoNativeFloat3Array (data.vertices, vertices);
			CopyVector3ArrayIntoNativeFloat3Array (data.normals, normals);
			CopyVector4ArrayIntoNativeFloat4Array (data.tangents, tangents);
			CopyVector2ArrayIntoNativeFloat2Array (data.uv, uv);
		}

		public void CopyFrom (ManagedMeshData data)
		{
			CopyVector3ArrayIntoNativeFloat3Array (data.vertices, vertices);
			CopyVector3ArrayIntoNativeFloat3Array (data.normals, normals);
			CopyVector4ArrayIntoNativeFloat4Array (data.tangents, tangents);
			CopyVector2ArrayIntoNativeFloat2Array (data.uv, uv);
		}

		public void CopyTo (ManagedMeshData data)
		{
			CopyNativeFloat3ArrayIntoVector3Array (data.vertices, vertices);
			CopyNativeFloat3ArrayIntoVector3Array (data.normals, normals);
			CopyNativeFloat4ArrayIntoVector4Array (data.tangents, tangents);
			CopyNativeFloat2ArrayIntoVector2Array (data.uv, uv);
		}

		public void Dispose ()
		{
			if (vertices.IsCreated)
				vertices.Dispose ();
			if (normals.IsCreated)
				normals.Dispose ();
			if (tangents.IsCreated)
				tangents.Dispose ();
			if (uv.IsCreated)
				uv.Dispose ();
		}


		private unsafe void CopyVector2ArrayIntoNativeFloat2Array (Vector2[] managed, NativeArray<float2> unmanaged)
		{
			fixed (void* managedBufferPointer = managed)
			{
				UnsafeUtility.MemCpy (NativeArrayUnsafeUtility.GetUnsafeBufferPointerWithoutChecks (unmanaged), managedBufferPointer, managed.Length * (long)UnsafeUtility.SizeOf<Vector2> ());
			}
		}
		private unsafe void CopyNativeFloat2ArrayIntoVector2Array (Vector2[] managed, NativeArray<float2> unmanaged)
		{
			fixed (void* managedBufferPointer = managed)
			{
				UnsafeUtility.MemCpy (managedBufferPointer, NativeArrayUnsafeUtility.GetUnsafeBufferPointerWithoutChecks (unmanaged), managed.Length * (long)UnsafeUtility.SizeOf<Vector2> ());
			}
		}

		private unsafe void CopyVector3ArrayIntoNativeFloat3Array (Vector3[] managed, NativeArray<float3> unmanaged)
		{
			fixed (void* managedBufferPointer = managed)
			{
				UnsafeUtility.MemCpy (NativeArrayUnsafeUtility.GetUnsafeBufferPointerWithoutChecks (unmanaged), managedBufferPointer, managed.Length * (long)UnsafeUtility.SizeOf<Vector3> ());
			}
		}
		private unsafe void CopyNativeFloat3ArrayIntoVector3Array(Vector3[] managed, NativeArray<float3> unmanaged)
		{
			fixed (void* managedBufferPointer = managed)
			{
				UnsafeUtility.MemCpy (managedBufferPointer, NativeArrayUnsafeUtility.GetUnsafeBufferPointerWithoutChecks (unmanaged), managed.Length * (long)UnsafeUtility.SizeOf<Vector3> ());
			}
		}

		private unsafe void CopyVector4ArrayIntoNativeFloat4Array (Vector4[] managed, NativeArray<float4> unmanaged)
		{
			fixed (void* managedBufferPointer = managed)
			{
				UnsafeUtility.MemCpy (NativeArrayUnsafeUtility.GetUnsafeBufferPointerWithoutChecks (unmanaged), managedBufferPointer, managed.Length * (long)UnsafeUtility.SizeOf<Vector4> ());
			}
		}
		private unsafe void CopyNativeFloat4ArrayIntoVector4Array(Vector4[] managed, NativeArray<float4> unmanaged)
		{
			fixed (void* managedBufferPointer = managed)
			{
				UnsafeUtility.MemCpy (managedBufferPointer, NativeArrayUnsafeUtility.GetUnsafeBufferPointerWithoutChecks (unmanaged), managed.Length * (long)UnsafeUtility.SizeOf<Vector4> ());
			}
		}
	}
}