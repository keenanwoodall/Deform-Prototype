using UnityEngine;

namespace DForm
{
	[RequireComponent (typeof (DFormManager))]
	public abstract class DFormerComponent : MonoBehaviour
	{
		public virtual void PreModify () { }
		public abstract VertexData[] Modify (VertexData[] vertexData);
		public virtual void PostModify () { }
	}
}