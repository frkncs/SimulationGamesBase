using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MovementScript : MonoBehaviour
{
	#region Variables

	// Public Variables

	// Private Variables
	[SerializeField] private float movementSpeed = 8f;
	[SerializeField] private float counterSpeed = 4f;
	[SerializeField] private float jumpForce = 250f;
	[SerializeField] private Transform footTrans;
	[SerializeField] private LayerMask groundLayer;

	private Rigidbody _rb;
	private CameraController _cameraController;
	private Vector3 _movementDir, _counterMovement;
	private bool _onGround;

	#endregion

	private void Awake()
	{
		Init();
	}
	
	private void Update()
	{
		_onGround = Physics.CheckSphere(footTrans.position, .2f, groundLayer);

		if (_onGround && Input.GetKeyDown(KeyCode.Space))
		{
			_rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
		}

		var verticalInput = Input.GetAxisRaw("Vertical");
		var horizontalInput = Input.GetAxisRaw("Horizontal");

		Vector3 cameraForward = Vector3.ProjectOnPlane(_cameraController.transform.forward, Vector3.up).normalized;
		Vector3 cameraRight = _cameraController.transform.right;

		_movementDir = (cameraForward * verticalInput) + (cameraRight * horizontalInput);
		_movementDir.y = 0;
		_counterMovement = new Vector3(-_rb.velocity.x * (_onGround ? counterSpeed : counterSpeed * 2), 0, -_rb.velocity.z * (_onGround ? counterSpeed : counterSpeed * 2));
	}

	private void FixedUpdate()
	{
		_rb.AddForce((_movementDir.normalized * movementSpeed) + _counterMovement);
	}

	private void Init()
	{
		SetupRigidbody();
		SetupCamera();
	}

	private void SetupCamera()
	{
		_cameraController = GetComponentInChildren<CameraController>();

		if (_cameraController == null)
		{
			throw new MissingComponentException("There is no CameraController under the player");
		}
	}

	private void SetupRigidbody()
	{
		_rb = GetComponent<Rigidbody>();
		_rb.freezeRotation = true;
	}
}
