using UnityEngine;

namespace Deform
{
	/// <summary>
	/// This is the base class for any deformer component (taper, bend, twist etc.)
	/// Inherit from this class if you want to make your own deformer.
	/// </summary>
	[RequireComponent (typeof (DeformerComponentManager))]
	[ExecuteInEditMode]
	public abstract class DeformerComponent : MonoBehaviour
	{
		/// <summary>
		/// Vertex data won't be sent here if false.
		/// </summary>
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

		/// <summary>
		/// Called right before Modify is called on any deformer components.
		/// This is a good place to cache anything you can't access off the main thread.
		/// </summary>
		public virtual void PreModify () { }
		/// <summary>
		/// Deform the vertex data in this method by modifying the vertexData argument and returning it.
		/// This method is might be running on another thread (if Update Mode is set to UpdateAsync) so you can't access things like transforms or anything else Unity manages.
		/// If you need access to something and Unity won't let you get it, override and cache it in the PreModify method.
		/// </summary>
		/// <param name="transformData">The current transform data of the mesh.</param>
		/// <param name="vertexDataBounds">The bounds of the current vertex data.</param>
		/// <returns>The vertexData you return will be sent to the next deformer component or applied to the mesh if this is the last deformer component.</returns>
		public abstract VertexData[] Modify (VertexData[] vertexData, TransformData transformData, Bounds vertexDataBounds);
		/// <summary>
		/// Called right after Modify done being called on all deformer components.
		/// This is a good place to cleanup or reset anything after deformation has occured.
		/// </summary>
		public virtual void PostModify () { }
	}
}