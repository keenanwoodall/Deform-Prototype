using UnityEngine;

namespace Deform
{
	[RequireComponent (typeof (DeformerManager))]
	[ExecuteInEditMode]
	public abstract class DeformerComponent : MonoBehaviour
	{
		public bool update = true;

		private DeformerManager manager;
		protected DeformerManager Manager
		{
			get
			{
				if (manager == null)
					manager = GetComponent<DeformerManager> ();
				return manager;
			}
		}

		private void Awake ()
		{
			Manager.AddDeformer (this);
		}

		private void OnDestroy ()
		{
			Manager.RemoveDeformer (this);
		}

		public virtual void PreModify () { }
		public abstract VertexData[] Modify (VertexData[] vertexData);
		public virtual void PostModify () { }
	}
}