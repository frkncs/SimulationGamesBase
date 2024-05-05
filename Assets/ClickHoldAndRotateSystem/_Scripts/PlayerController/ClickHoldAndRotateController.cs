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
	[SerializeField] private float zOffsetForHoldObject = 6f;
	[SerializeField] private float yOffsetForHoldObject = 2f;
	[SerializeField] private float reachDistance = 4f;
	[SerializeField] private float pickUpForce = 100f;
	
	private Camera _mainCam;
	private LayerMask _movableLayer;
	private MovableItemBase _lastLookedMovable;
	private MovableItemBase _currentlyLookingMovable;
	private Transform _currentlyLookingMovableTrans;
	private Rigidbody _currentlyLookingMovableRb;
	private Tween _rotateObjectTween;
	
	private int _lastHighligtedObjectInstanceId = -1;
	private bool _isHoldingObject;

	#endregion
	
	private void Awake()
	{
		_movableLayer = LayerMask.GetMask("Movable");
		_mainCam = Camera.main;
	}

	private void Update()
	{
		StartRaycast();

		PickupAndMove();

		RotateMovable();
	}

	private void PickupAndMove()
	{
		if (_currentlyLookingMovableTrans != null)
		{
			if (Input.GetKeyDown(KeyCode.E) && !_isHoldingObject)
			{
				PickupMovable();
			}
			else if (Input.GetKeyDown(KeyCode.E) && _isHoldingObject)
			{
				DropMovable();
			}
		}
	}
	
	private void RotateMovable()
	{
		if (Input.GetKeyDown(KeyCode.R) && _currentlyLookingMovable != null)
		{
			if (_rotateObjectTween != null)
			{
				_rotateObjectTween.Kill();
			}
			
			if (Input.GetKey(KeyCode.LeftShift))
			{
				_currentlyLookingMovable.RotationValue += 45;
			}
			else
			{
				_currentlyLookingMovable.RotationValue += 90;
			}
			
			_rotateObjectTween = _currentlyLookingMovableTrans.DORotate(new Vector3(0, _currentlyLookingMovable.RotationValue, 0), 0.15f);
		}
	}
	private void FixedUpdate()
	{
		if (_currentlyLookingMovableTrans != null)
		{
			MoveMovable();
		}
	}
	
	private void MoveMovable()
	{
		Vector3 forwardHoldObjectPos = transform.position + (_mainCam.transform.forward * zOffsetForHoldObject) + (Vector3.up * yOffsetForHoldObject);
		
		if (_isHoldingObject && (forwardHoldObjectPos - _currentlyLookingMovableTrans.position).sqrMagnitude >= .01f)
		{
			Vector3 moveDir = forwardHoldObjectPos - _currentlyLookingMovableTrans.position;
			_currentlyLookingMovableRb.AddForce(moveDir * pickUpForce);
		}
	}
	
	private void PickupMovable()
	{
		_isHoldingObject = true;
		
		_currentlyLookingMovableRb.useGravity = false;
		_currentlyLookingMovableRb.drag = 10;
		_currentlyLookingMovableRb.constraints = RigidbodyConstraints.FreezeRotation;

		_currentlyLookingMovableTrans.DORotate(new Vector3(0, _currentlyLookingMovable.RotationValue, 0), .2f, RotateMode.Fast);
	}
	
	private void DropMovable()
	{
		_isHoldingObject = false;
		
		_currentlyLookingMovableRb.useGravity = true;
		_currentlyLookingMovableRb.drag = 1;
		_currentlyLookingMovableRb.constraints = RigidbodyConstraints.None;
	}

	private void StartRaycast()
	{
		if (_isHoldingObject)
		{
			return;
		}
		
		if (Physics.Raycast(_mainCam.transform.position, _mainCam.transform.forward, out RaycastHit hitInfo, reachDistance, _movableLayer))
		{
			if (_currentlyLookingMovableTrans == hitInfo.transform)
			{
				return;
			}

			int instanceId = hitInfo.transform.GetInstanceID();

			if (_lastHighligtedObjectInstanceId != -1 && instanceId != _lastHighligtedObjectInstanceId)
			{
				_lastLookedMovable.SetOutlineVisibility(false);
			}

			_lastHighligtedObjectInstanceId = instanceId;
			_currentlyLookingMovableTrans = hitInfo.transform;
			_currentlyLookingMovable = _currentlyLookingMovableTrans.GetComponent<MovableItemBase>();
			_currentlyLookingMovableRb = _currentlyLookingMovableTrans.GetComponent<Rigidbody>();
			_lastLookedMovable = _currentlyLookingMovable;

			_currentlyLookingMovable.SetOutlineVisibility(true);
		}
		else
		{
			if (_lastLookedMovable != null)
			{
				_lastLookedMovable.SetOutlineVisibility(false);
			}
			_currentlyLookingMovableTrans = null;
			_currentlyLookingMovableRb = null;
			_currentlyLookingMovable = null;
		}
	}
}
