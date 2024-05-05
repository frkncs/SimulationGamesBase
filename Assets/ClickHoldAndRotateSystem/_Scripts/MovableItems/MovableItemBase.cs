using UnityEngine;

public abstract class MovableItemBase : MonoBehaviour
{
	#region Variables

	// Public Variables
	[HideInInspector] public int RotationValue; // Rotation value in the Y axis

	// Private Variables
	private Outline _outline;

	#endregion
	
	protected void Awake()
	{
		gameObject.layer = LayerMask.NameToLayer("Movable");

		_outline = gameObject.AddComponent<Outline>();
		_outline.OutlineMode = Outline.Mode.OutlineVisible;
		_outline.OutlineColor = Color.white;
		_outline.OutlineWidth = 12f;

		_outline.enabled = false;
	}
}
