using UnityEditor;
using UnityEngine;

namespace Deform
{
	public static class MeshUtil
	{
		public static void Save (Mesh mesh, string name)
		{
			var copy = Object.Instantiate (mesh);
			AssetDatabase.CreateAsset (copy, AssetDatabase.GenerateUniqueAssetPath (string.Format ("Assets/{0}.asset", name)));
		}

		public static Mesh Copy (Mesh mesh)
		{
			return Object.Instantiate (mesh);
		}
	}
}