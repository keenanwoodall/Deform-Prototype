using Unity.Collections;
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


		public NativeMeshData (MeshData data, Allocator allocator = Allocator.Temp)
		{
			vertices = new NativeArray<Vector3> (data.vertices, allocator);
			normals = new NativeArray<Vector3> (data.normals, allocator);
			tangents = new NativeArray<Vector4> (data.tangents, allocator);
			uv = new NativeArray<Vector2> (data.uv, allocator);

			size = data.vertices.Length;
		}

		public void CopyTo (MeshData data)
		{
			vertices.CopyTo (data.vertices);
			normals.CopyTo (data.normals);
			tangents.CopyTo (data.tangents);
			uv.CopyTo (data.uv);
		}
		public void CopyTo (MeshData data, bool copyVertices = true, bool copyNormals = false, bool copyTangents = false, bool copyUv = false)
		{
			if (copyVertices)
				vertices.CopyTo (data.vertices);
			if (copyNormals)
				normals.CopyTo (data.normals);
			if (copyTangents)
				tangents.CopyTo (data.tangents);
			if (copyTangents)
				uv.CopyTo (data.uv);
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
	}
}