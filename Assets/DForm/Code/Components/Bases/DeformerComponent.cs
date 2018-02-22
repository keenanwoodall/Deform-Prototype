using UnityEngine;

namespace Deform
{
	[RequireComponent (typeof (DeformerManager))]
	[ExecuteInEditMode]
	public abstract class DeformerComponent : MonoBehaviour
	{
		protected DeformerManager manager { get; private set; }

		private void Awake ()
		{
			manager = GetComponent<DeformerManager> ();
			manager.AddDeformer (this);
		}

		private void OnDestroy ()
		{
			manager.RemoveDeformer (this);
		}

		public virtual void PreModify () { }
		public abstract VertexData[] Modify (VertexData[] vertexData);
		public virtual void PostModify () { }
	}
}