using UnityEngine;

public class CameraController : MonoBehaviour
{
	#region Variables

	// Public Variables

	// Private Variables
	[SerializeField] private Transform playerTransform;
	[SerializeField] private float sensitivity = 2.0f; // Mouse sensitivity
	[SerializeField] private float maxYAngle = 80.0f; // Maximum vertical angle
	
	private Vector2 _currentRotation = Vector2.zero;

	#endregion
	
	private void OnApplicationFocus(bool focusStatus) {
		Cursor.visible = !focusStatus;
		Cursor.lockState = focusStatus ? CursorLockMode.Locked : CursorLockMode.None;
	}

	private void Update()
	{
		// Get mouse input
		float mouseX = Input.GetAxis("Mouse X");
		float mouseY = -Input.GetAxis("Mouse Y");

		// Calculate rotation
		_currentRotation.x += mouseX * sensitivity;
		_currentRotation.y += mouseY * sensitivity;

		// Clamp vertical rotation
		_currentRotation.y = Mathf.Clamp(_currentRotation.y, -maxYAngle, maxYAngle);

		// Apply rotation to the camera
		transform.localRotation = Quaternion.Euler(_currentRotation.y, 0, 0);
		playerTransform.localRotation = Quaternion.Euler(0, _currentRotation.x, 0);
	}
}
