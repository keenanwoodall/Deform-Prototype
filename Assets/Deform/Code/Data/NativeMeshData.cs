using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

namespace Deform.Data
{
	public struct NativeMeshData
	{
		public NativeArray<Vector3> vertices;
		public NativeArray<Vector3> normals;
		public NativeArray<Vector4> tangents;
		public NativeArray<Vector2> uv;

		public readonly int size;

		public NativeMeshData (MeshData data, Allocator allocator)
		{
			var length = data.size;
			vertices = new NativeArray<Vector3> (length, allocator, NativeArrayOptions.UninitializedMemory);
			normals = new NativeArray<Vector3> (length, allocator, NativeArrayOptions.UninitializedMemory);
			tangents = new NativeArray<Vector4> (length, allocator, NativeArrayOptions.UninitializedMemory);
			uv = new NativeArray<Vector2> (length, allocator, NativeArrayOptions.UninitializedMemory);

			size = data.vertices.Length;
			
			GetNativeVector3Arrays (data.vertices, vertices);
			GetNativeVector3Arrays (data.normals, normals);
			GetNativeVector4Arrays (data.tangents, tangents);
			GetNativeVector2Arrays (data.uv, uv);
		}

		public void CopyFrom (MeshData data)
		{
			GetNativeVector3Arrays (data.vertices, vertices);
			GetNativeVector3Arrays (data.normals, normals);
			GetNativeVector4Arrays (data.tangents, tangents);
			GetNativeVector2Arrays (data.uv, uv);
		}

		public void CopyTo (MeshData data)
		{
			SetNativeVector3Array (data.vertices, vertices);
			SetNativeVector3Array (data.normals, normals);
			SetNativeVector4Array (data.tangents, tangents);
			SetNativeVector2Array (data.uv, uv);
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

		private unsafe void GetNativeVector2Arrays (Vector2[] input, NativeArray<Vector2> output)
		{
			fixed (void* vertexBufferPointer = input)
			{
				UnsafeUtility.MemCpy (NativeArrayUnsafeUtility.GetUnsafeBufferPointerWithoutChecks (output),
					vertexBufferPointer, input.Length * (long)UnsafeUtility.SizeOf<Vector2> ());
			}
		}
		unsafe void SetNativeVector2Array (Vector2[] vertexArray, NativeArray<Vector2> vertexBuffer)
		{
			fixed (void* vertexArrayPointer = vertexArray)
			{
				UnsafeUtility.MemCpy (vertexArrayPointer, NativeArrayUnsafeUtility.GetUnsafeBufferPointerWithoutChecks (vertexBuffer), vertexArray.Length * (long)UnsafeUtility.SizeOf<Vector2> ());
			}
		}
		private unsafe void GetNativeVector3Arrays (Vector3[] input, NativeArray<Vector3> output)
		{
			fixed (void* vertexBufferPointer = input)
			{
				UnsafeUtility.MemCpy (NativeArrayUnsafeUtility.GetUnsafeBufferPointerWithoutChecks (output),
					vertexBufferPointer, input.Length * (long)UnsafeUtility.SizeOf<Vector3> ());
			}
		}
		unsafe void SetNativeVector3Array (Vector3[] vertexArray, NativeArray<Vector3> vertexBuffer)
		{
			fixed (void* vertexArrayPointer = vertexArray)
			{
				UnsafeUtility.MemCpy (vertexArrayPointer, NativeArrayUnsafeUtility.GetUnsafeBufferPointerWithoutChecks (vertexBuffer), vertexArray.Length * (long)UnsafeUtility.SizeOf<Vector3> ());
			}
		}
		private unsafe void GetNativeVector4Arrays (Vector4[] input, NativeArray<Vector4> output)
		{
			fixed (void* vertexBufferPointer = input)
			{
				UnsafeUtility.MemCpy (NativeArrayUnsafeUtility.GetUnsafeBufferPointerWithoutChecks (output),
					vertexBufferPointer, input.Length * (long)UnsafeUtility.SizeOf<Vector4> ());
			}
		}
		unsafe void SetNativeVector4Array (Vector4[] vertexArray, NativeArray<Vector4> vertexBuffer)
		{
			fixed (void* vertexArrayPointer = vertexArray)
			{
				UnsafeUtility.MemCpy (vertexArrayPointer, NativeArrayUnsafeUtility.GetUnsafeBufferPointerWithoutChecks (vertexBuffer), vertexArray.Length * (long)UnsafeUtility.SizeOf<Vector4> ());
			}
		}
	}
}