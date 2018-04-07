using UnityEngine;

namespace Deform
{
	public static class MeshUtil
	{
		public static Mesh Copy (Mesh mesh)
		{
			return Object.Instantiate (mesh);
		}
	}
}