using UnityEngine;

namespace Deform
{
	[RequireComponent (typeof (DeformerManager))]
	public abstract class DeformerComponent : MonoBehaviour
	{
		protected DeformerManager manager { get; private set; }

		private void Awake ()
		{
			manager = GetComponent<DeformerManager> ();
			manager.UpdateDeformerReferences ();
		}

		private void OnDestroy ()
		{
			manager.UpdateDeformerReferences ();
		}

		public virtual void PreModify () { }
		public abstract VertexData[] Modify (VertexData[] vertexData);
		public virtual void PostModify () { }
	}
}