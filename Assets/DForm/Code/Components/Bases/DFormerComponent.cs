using UnityEngine;

namespace DForm
{
	[RequireComponent (typeof (DFormerManager))]
	public abstract class DFormerComponent : MonoBehaviour
	{
		protected DFormerManager manager { get; private set; }

		private void Awake ()
		{
			manager = GetComponent<DFormerManager> ();
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