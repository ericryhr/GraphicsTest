using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
	Vector3 rotationSmoothVelocity;
	Vector3 currentRotation;
	float xCameraMovement;
	float yCameraMovement;
	public Camera thisCamera;

	float x;
	float y;
	float z;

	public bool lockCursor;
	public float mouseSensitivityX = 10f;
	public float mouseSensitivityY = 10f;
	public Transform target;
	public float dstFromTarget = 2;
	public Vector2 pitchMinMax = new Vector2(-40, 85);
	public float rotationSmoothTime = 0.12f;

	public LayerMask cameraCollisionLayer;

	void Start()
	{
		z = thisCamera.nearClipPlane;
		x = (Mathf.Tan(thisCamera.fieldOfView / 2) * z) / 4;
		y = x / thisCamera.aspect;

		if (lockCursor)
		{
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
		}
	}

	void LateUpdate()
	{
		xCameraMovement += Input.GetAxis("Mouse X") * mouseSensitivityX;
		yCameraMovement -= Input.GetAxis("Mouse Y") * mouseSensitivityY;
		yCameraMovement = Mathf.Clamp(yCameraMovement, pitchMinMax.x, pitchMinMax.y);

		currentRotation = Vector3.SmoothDamp(currentRotation, new Vector3(yCameraMovement, xCameraMovement), ref rotationSmoothVelocity, rotationSmoothTime);
		transform.eulerAngles = currentRotation;

		CollisionCheck();
	}

	void CollisionCheck()
	{
		transform.position = target.position - transform.forward * dstFromTarget;

		Vector3 pointLeftUp = target.position + Vector3.forward * z + Vector3.up * -y + Vector3.right * x;
		Vector3 pointLeftDown = target.position + Vector3.forward * z + Vector3.up * y + Vector3.right * x;
		Vector3 pointRightUp = target.position + Vector3.forward * z + Vector3.up * -y + Vector3.right * -x;
		Vector3 pointRightDown = target.position + Vector3.forward * z + Vector3.up * y + Vector3.right * -x;

		Vector3 direction = transform.position - target.position;

		bool centerBlocking = Physics.Raycast(target.position, direction, out RaycastHit centerInfo, dstFromTarget, cameraCollisionLayer, QueryTriggerInteraction.Ignore);
		bool leftUpBlocking = Physics.Raycast(pointLeftUp, direction, out RaycastHit leftUpInfo, dstFromTarget, cameraCollisionLayer, QueryTriggerInteraction.Ignore);
		bool leftdownBlocking = Physics.Raycast(pointLeftDown, direction, out RaycastHit leftDownInfo, dstFromTarget, cameraCollisionLayer, QueryTriggerInteraction.Ignore);
		bool rightUpBlocking = Physics.Raycast(pointRightUp, direction, out RaycastHit rightUpInfo, dstFromTarget, cameraCollisionLayer, QueryTriggerInteraction.Ignore);
		bool rightDownBlocking = Physics.Raycast(pointRightDown, direction, out RaycastHit rightDownInfo, dstFromTarget, cameraCollisionLayer, QueryTriggerInteraction.Ignore);

		//Si tots els rayos son bloquejats la camera es mou. To-do: fer que el moviment de la camera sigui lerp

		if(centerBlocking && leftdownBlocking && leftUpBlocking && rightDownBlocking && rightUpBlocking)
		{
			Debug.Log(centerInfo.transform.name);
			transform.position = centerInfo.point + (target.position - transform.position) * .01f;
		}
	}

	private void OnDrawGizmos()
	{
		float z = thisCamera.nearClipPlane;
		float x = (Mathf.Tan(thisCamera.fieldOfView / 2) * z) / 4;
		float y = x / thisCamera.aspect;

		Vector3 pointLeftUp = target.position + Vector3.forward * z + Vector3.up * -y + Vector3.right * x;
		Vector3 pointLeftDown = target.position + Vector3.forward * z + Vector3.up * y + Vector3.right * x;
		Vector3 pointRightUp = target.position + Vector3.forward * z + Vector3.up * -y + Vector3.right * -x;
		Vector3 pointRightDown = target.position + Vector3.forward * z + Vector3.up * y + Vector3.right * -x;

		Vector3 direction = transform.position - target.position;

		Gizmos.color = Color.green;
		Gizmos.DrawRay(pointLeftDown, direction);
		Gizmos.color = Color.blue;
		Gizmos.DrawRay(pointLeftUp, direction);
		Gizmos.color = Color.red;
		Gizmos.DrawRay(pointRightDown, direction);
		Gizmos.color = Color.yellow;
		Gizmos.DrawRay(pointRightUp, direction);
		Gizmos.color = Color.magenta;
		Gizmos.DrawRay(target.position, direction);
	}
}
