using UnityEngine;
using UnityAsyncAwaitUtil;
using System.Threading.Tasks;

namespace Deform
{
	[RequireComponent (typeof (DeformerComponentManager))]
	[ExecuteInEditMode]
	public abstract class DeformerComponent : MonoBehaviour
	{
		public bool update = true;

		private DeformerComponentManager manager;
		protected DeformerComponentManager Manager
		{
			get
			{
				if (manager == null)
					manager = GetComponent<DeformerComponentManager> ();
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
		public abstract Chunk Modify (Chunk chunk);
		public async Task<Chunk> ModifyAsync (Chunk chunk)
		{
			await new WaitForBackgroundThread ();
			return Modify (chunk);
		}
		public virtual void PostModify () { }
	}
}