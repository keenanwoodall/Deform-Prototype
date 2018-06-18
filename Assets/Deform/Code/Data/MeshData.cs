using UnityEngine;

[System.Serializable]
public struct MeshData
{
	public Vector3[] vertices;
	public Vector3[] baseVertices;
	public Vector3[] normals;
	public Vector4[] tangents;
	public Color[] colors;

	public int Size { get { return vertices.Length; } }
}
