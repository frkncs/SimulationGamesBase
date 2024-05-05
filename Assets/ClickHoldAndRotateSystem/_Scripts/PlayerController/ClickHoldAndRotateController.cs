using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;
using UnityEngine.UI;
using DG.Tweening;

public class ClickHoldAndRotateController : MonoBehaviour
{
	#region Variables

	// Public Variables

	// Private Variables
	//[SerializeField] private Transform holdArea;
	[SerializeField] private float zOffsetForHoldObject = 6f;
	[SerializeField] private float yOffsetForHoldObject = 2f;
	[SerializeField] private float reachDistance = 4f;
	[SerializeField] private float pickUpForce = 100f;
	private Camera _mainCam;
	private LayerMask _movableLayer;

	private Dictionary<int, Outline> _cachedOutlineComponents; //instanceId, outlineComponent
	private Transform _currentlyLookingObjectTrans;
	private Rigidbody _currentlyLookingObjectRb;
	private int _lastHighligtedObjectInstanceId = -1;
	private bool _isHoldingObject;

	#endregion
	
	private void Awake()
	{
		_movableLayer = LayerMask.GetMask("Movable");
		_mainCam = Camera.main;
		_cachedOutlineComponents = new();
	}

	private void Update()
	{
		StartRaycast();

		PickupAndMove();

		RotateMovable();
	}

	private void PickupAndMove()
	{
		if (_currentlyLookingObjectTrans != null)
		{
			if (Input.GetMouseButtonDown(0))
			{
				PickupMovable();
			}
			else if (Input.GetMouseButtonUp(0))
			{
				DropMovable();
			}
		}
	}

	private void FixedUpdate()
	{
		if (_currentlyLookingObjectTrans != null)
		{
			if (Input.GetMouseButton(0))
			{
				MoveMovable();
			}
		}
	}
	
	private void MoveMovable()
	{
		Vector3 forwardHoldObjectPos = transform.position + (_mainCam.transform.forward * zOffsetForHoldObject) + (Vector3.up * yOffsetForHoldObject);
		
		if ((forwardHoldObjectPos - _currentlyLookingObjectTrans.position).sqrMagnitude >= .01f)
		{
			Vector3 moveDir = forwardHoldObjectPos - _currentlyLookingObjectTrans.position;
			_currentlyLookingObjectRb.AddForce(moveDir * pickUpForce);
		}
	}
	
	private void PickupMovable()
	{
		_isHoldingObject = true;
		
		_currentlyLookingObjectRb.useGravity = false;
		_currentlyLookingObjectRb.drag = 10;
		_currentlyLookingObjectRb.constraints = RigidbodyConstraints.FreezeRotation;

		_currentlyLookingObjectTrans.DORotate(new Vector3(0, 0, 0), .2f, RotateMode.Fast);	
	}
	
	private void DropMovable()
	{
		_isHoldingObject = false;
		
		_currentlyLookingObjectRb.useGravity = true;
		_currentlyLookingObjectRb.drag = 1;
		_currentlyLookingObjectRb.constraints = RigidbodyConstraints.None;

		_currentlyLookingObjectRb.transform.parent = null;
	}

	private void StartRaycast()
	{
		if (_isHoldingObject)
		{
			return;
		}
		
		if (Physics.Raycast(_mainCam.transform.position, _mainCam.transform.forward, out RaycastHit hitInfo, reachDistance, _movableLayer))
		{
			if (_currentlyLookingObjectTrans == hitInfo.transform)
			{
				return;
			}

			int instanceId = hitInfo.transform.GetInstanceID();

			if (_lastHighligtedObjectInstanceId != -1 && instanceId != _lastHighligtedObjectInstanceId)
			{
				_cachedOutlineComponents[_lastHighligtedObjectInstanceId].enabled = false;
			}

			if (_cachedOutlineComponents.TryGetValue(instanceId, out Outline outline))
			{
				outline.enabled = true;
			}
			else
			{
				var outlineComp = hitInfo.transform.GetComponent<Outline>();
				outlineComp.enabled = true;

				_cachedOutlineComponents.Add(instanceId, outlineComp);
			}

			_lastHighligtedObjectInstanceId = instanceId;
			_currentlyLookingObjectTrans = hitInfo.transform;
			_currentlyLookingObjectRb = _currentlyLookingObjectTrans.GetComponent<Rigidbody>();
		}
		else
		{
			if (_lastHighligtedObjectInstanceId != -1)
			{
				_cachedOutlineComponents[_lastHighligtedObjectInstanceId].enabled = false;
			}
			_currentlyLookingObjectTrans = null;
		}
	}
}
