using UnityEngine;

namespace DForm
{
	[RequireComponent (typeof (DFormManager))]
	public abstract class DFormerComponent : MonoBehaviour
	{
		protected DFormManager manager { get; private set; }

		private void Awake ()
		{
			manager = GetComponent<DFormManager> ();
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