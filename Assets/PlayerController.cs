using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	CharacterController controller;
	public float speed = 8f;
	public float gravity = -12f;
	public Transform playerCamera;

	public float turnSmoothTime = 0.2f;
	float turnSmoothSpeed;
	float velocityY;

    // Start is called before the first frame update
    void Start()
    {
		controller = GetComponent<CharacterController>();   
    }

	private void Update()
	{
		velocityY += Time.deltaTime * gravity;

		Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
		if (input != Vector2.zero)
		{
			Rotate(input);

			Vector3 movement = transform.forward * speed + Vector3.up * velocityY;

			controller.Move(movement * Time.deltaTime);
		}
		else
		{
			Vector3 movement = Vector3.up * velocityY;

			controller.Move(movement * Time.deltaTime);
		}

		if (controller.isGrounded)
		{
			velocityY = 0;
		}
	}

	void Rotate(Vector2 input)
	{
		//Rotate the character
		float targetRotation = Mathf.Atan2(input.x, input.y) * Mathf.Rad2Deg + playerCamera.eulerAngles.y;
		transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref turnSmoothSpeed, turnSmoothTime);
	}
}
