using Photon.Pun;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
	private CharacterController controller;

	// GroundCheck
	public bool isGrounded;
	public Transform groundCheck;
	public float groundDistance = 0.4f;
	public LayerMask groundMask;

	private Vector3 velocity;
	public float gravity = -9.81f;
	public float moveSpeed = 5f;
	public float runSpeed = 10f;
	public float jumpHeight = 1.5f;
	public float coyoteTime = 0.1f;
	public float jumpBufferTime = 0.12f;

	private float coyoteTimeCounter;
	private float jumpBufferCounter;

	public PhotonView photonView;

	void Reset()
	{
		controller = GetComponent<CharacterController>();
		controller.center = new Vector3(0f, -0.35f, 0f);
	}

	void Awake()
	{
		controller = GetComponent<CharacterController>();
		controller.center = new Vector3(0f, -0.35f, 0f);
	}

	void Update()
	{
		if(PhotonNetwork.InRoom && !photonView.IsMine)
		{
			return;
		}
		// Mirar si estic tocant el terra
		if (groundCheck != null)
		{
			isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
		}
		else
		{
			isGrounded = controller.isGrounded;
		}

		if (isGrounded)
		{
			coyoteTimeCounter = coyoteTime;
		}
		else
		{
			coyoteTimeCounter -= Time.deltaTime;
		}

		if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
		{
			jumpBufferCounter = jumpBufferTime;
			Debug.Log("Salt input detectat: Space");
		}
		else
		{
			jumpBufferCounter -= Time.deltaTime;
		}

		if (isGrounded && velocity.y < 0f)
		{
			velocity.y = -2f;
		}

		Vector2 input = Vector2.zero;

		if (Keyboard.current != null)
		{
			if (Keyboard.current.aKey.isPressed)
			{
				input.x -= 1f;
			}

			if (Keyboard.current.dKey.isPressed)
			{
				input.x += 1f;
			}

			if (Keyboard.current.wKey.isPressed)
			{
				input.y += 1f;
			}

			if (Keyboard.current.sKey.isPressed)
			{
				input.y -= 1f;
			}
		}

		input = Vector2.ClampMagnitude(input, 1f);

		float currentSpeed = moveSpeed;
		if (Keyboard.current != null && Keyboard.current.leftShiftKey.isPressed)
		{
			currentSpeed = runSpeed;
		}

		Vector3 move = transform.right * input.x + transform.forward * input.y;
		controller.Move(move * currentSpeed * Time.deltaTime);

		if (jumpBufferCounter > 0f && coyoteTimeCounter > 0f)
		{
			Debug.Log("Salt executat");
			velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
			jumpBufferCounter = 0f;
			coyoteTimeCounter = 0f;
		}

		// Gravetat
		// Formula de velocitat = acceleració * temps^2
		velocity.y += gravity * Time.deltaTime;
		controller.Move(velocity * Time.deltaTime);
	}
}
