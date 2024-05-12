using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

public abstract class MovableItemBase : MonoBehaviour
{
	#region Variables

	// Public Variables
	[HideInInspector] public Rigidbody Rigidbody => _rb;
	[HideInInspector] public int RotationValue; // Rotation value in the Y axis

	// Private Variables
	[SerializeField] private LayerMask placableLayers;
	[SerializeField] private Material placableTriggerMat, notPlacableTriggerMat;
	private Material _defaultMaterial;
	private MeshRenderer _meshRenderer;
	private Rigidbody _rb;
	private Outline _outline;
	private MaterialPropertyBlock _placableTriggerMaterialPropBlock;
	private Vector3 _colliderTriggerSize;

	private bool _canPlace;
	private bool _canCheckPlace;

	#endregion

	protected void Awake()
	{
		gameObject.layer = LayerMask.NameToLayer("Movable");

		_placableTriggerMaterialPropBlock = new MaterialPropertyBlock();
		
		_rb = GetComponent<Rigidbody>();
		_meshRenderer = GetComponent<MeshRenderer>();

		_defaultMaterial = _meshRenderer.material;

		_colliderTriggerSize = _meshRenderer.bounds.size * 1.15f;

		_outline = gameObject.AddComponent<Outline>();
		_outline.OutlineMode = Outline.Mode.OutlineVisible;
		_outline.OutlineColor = Color.white;
		_outline.OutlineWidth = 12f;

		_outline.enabled = false;
	}

	public void SetOutlineVisibility(bool visibility)
	{
		_outline.enabled = visibility;
	}
	
	public void PlaceMovable()
	{
		_rb.constraints = RigidbodyConstraints.FreezeAll;
		_meshRenderer.material = _defaultMaterial;
		_canCheckPlace = false;

		// Snap to ground
		if (Physics.BoxCast(transform.position + (Vector3.up * _meshRenderer.bounds.extents.y), _meshRenderer.bounds.extents, Vector3.down, out RaycastHit hitInfo, Quaternion.identity, 5f))
		{
			var placePoint = transform.position;
			placePoint.y = hitInfo.point.y + _meshRenderer.bounds.extents.y;
			
			transform.position = placePoint;
		}
	}

	public void DropMovable()
	{
		_rb.useGravity = true;
		_rb.drag = 1;
		_rb.constraints = RigidbodyConstraints.None;
		_meshRenderer.material = _defaultMaterial;
		_canCheckPlace = false;
	}

	public void PickupMovable()
	{
		_rb.useGravity = false;
		_rb.drag = 10;
		_rb.constraints = RigidbodyConstraints.FreezeRotation;
		_canCheckPlace = true;
		StartCoroutine(CheckCanPlaceCor());
	}
	
	private IEnumerator CheckCanPlaceCor()
	{
		while(_canCheckPlace)
		{
			var colliders = Physics.OverlapBox(transform.position, _meshRenderer.bounds.extents * 1.25f, transform.rotation, placableLayers)
			.Where(o => o.transform != transform).ToArray();

			_canPlace = colliders.Length > 0;

			if (_canPlace)
			{
				_meshRenderer.material = placableTriggerMat;
			}
			else
			{
				_meshRenderer.material = notPlacableTriggerMat;
			}
			
			yield return null;
		}
	}
	
	public bool CheckCanPlace()
	{
		return _canPlace;
	}
}
