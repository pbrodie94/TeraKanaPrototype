using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMove : MonoBehaviour 
{
	//Player Movement Shit
	[Header("Attributes")]
	private float speed = 6;
    public float walkSpeed = 6;
	public float sprintSpeed = 8;
	public float jumpHeight = 8;

	public bool sprinting = false;

	private Vector3 moveDirection = Vector3.zero;
	private CharacterController controller;
	private Transform cam;

    void Start()
	{
        controller = gameObject.GetComponent<CharacterController>();
        cam = GameObject.Find("Main Camera").transform;
    }

	void Update()
	{
		//Movement
		//Get input
		float v = Input.GetAxis(InputManager.Vertical);
        float h = Input.GetAxis(InputManager.Horizontal);

		if (Input.GetButtonDown(InputManager.Sprint))
			sprinting = true;

		if (controller.velocity.magnitude < 0.1f)
			sprinting = false;

		speed = sprinting ? speed = sprintSpeed : speed = walkSpeed;

        GetDirection(v, h);
        ApplyMovement();
		
    }

	void GetDirection (float v, float h)
	{
		moveDirection = new Vector3 (h, 0, v);
		moveDirection = transform.TransformDirection(moveDirection);

        if (Input.GetButtonDown(InputManager.Jump))
		{
			moveDirection.y = jumpHeight;
		}

	}

	void ApplyMovement ()
	{
		moveDirection += Physics.gravity;
		controller.Move(moveDirection * speed * Time.deltaTime);
	}

}
