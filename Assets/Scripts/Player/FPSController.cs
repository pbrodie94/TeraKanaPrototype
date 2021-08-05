using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSController : MonoBehaviour
{
    [Header("Player Attributes")]
    private float speed;
    [SerializeField] private float crouchSpeed = 4;
    [SerializeField] private float walkSpeed = 6;
    [SerializeField] private float runSpeed = 10;
    [SerializeField] private float airSpeed = 0;
    [SerializeField] private float jumpForce = 12;

    [Header("Controller")]
    [SerializeField] private bool airControl = true;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundMask;

    [Header("Audio")]
    [SerializeField] private AudioSource playerAudio;
    [SerializeField] private AudioClip[] footStepsDirt;

    [Range(0, 1)] [SerializeField] private float movementSmoothing = 0.5f;
    private Vector3 movementVelocity = Vector3.zero;

    /*[Range(0, 1)]*/ [SerializeField] private float crouchSmoothing = 0.2f;

    private bool aiming = false;

    private CapsuleCollider capsuleCollider;
    private float standingSize = 0;
    private float wantedSize = 0;
    private float sizeVelocity = 0;

    private bool crouching = false;
    private bool sprinting = false;
    private bool jump = false;

    private float vertical = 0;
    private float horizontal = 0;

    private Light flashlight;

    private Rigidbody rb;
    private Animator anim;

    private void Awake()
    {
        playerAudio = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        flashlight = GetComponentInChildren<Light>();
        speed = walkSpeed;

        capsuleCollider = GetComponent<CapsuleCollider>();
        standingSize = capsuleCollider.height;
        wantedSize = standingSize;
    }

    private void Update()
    {
        //Movement
        //Get input
        vertical = Input.GetAxis(InputManager.Vertical);
        horizontal = Input.GetAxis(InputManager.Horizontal);

        if (Input.GetButtonDown(InputManager.Jump) && IsGrounded())
        {
            jump = true;

            if (crouching)
            {
                Crouch();
            }
        }

        if (Input.GetButtonDown(InputManager.Sprint))
        {
            Sprint();
        }

        if (Input.GetButtonDown(InputManager.Crouch) && IsGrounded())
        {
            Crouch();
        }

        if (Input.GetButtonDown(InputManager.Flashlight))
        {
            flashlight.enabled = !flashlight.enabled;
        }

        anim.SetFloat("Speed", rb.velocity.magnitude);
        anim.SetBool("Sprinting", sprinting);
        anim.SetBool("Aiming", aiming);
        anim.SetBool("Crouched", crouching);
        anim.SetBool("Grounded", IsGrounded());
    }

    private void FixedUpdate()
    {
        if (GameManager.instance.IsPaused())
            return;

        Move();

        capsuleCollider.height = Mathf.Lerp(capsuleCollider.height, wantedSize, crouchSmoothing * Time.deltaTime);
    }

    public void Move()
    {
        Vector3 targetVelocity = Vector3.zero;

        if (IsGrounded() || airControl)
        {
            targetVelocity = new Vector3(horizontal, 0, vertical);

            if (targetVelocity.magnitude > 1)
            {
                targetVelocity.Normalize();
            }

            if (sprinting && targetVelocity.magnitude < 0.5f)
            {
                Sprint();
            }

            targetVelocity *= speed;
        }

        targetVelocity.y = rb.velocity.y;
        targetVelocity = transform.TransformDirection(targetVelocity);

        rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref movementVelocity, movementSmoothing);

        if (jump)
        {
            rb.AddForce(Vector3.up * jumpForce);
        }

        jump = false;
    }

    public bool IsGrounded()
    {
        Collider col = GetComponent<Collider>();
        Vector3 groundCheck = col.bounds.center;
        groundCheck.y -= col.bounds.extents.y;

        Collider[] colliders = Physics.OverlapSphere(groundCheck, groundCheckRadius);

        for (int i = 0; i < colliders.Length; ++i)
        {
            if (colliders[i].gameObject != gameObject)
            {
                return true;
            }
        }

        return false;
    }

    private void Sprint()
    {
        sprinting = !sprinting;
        crouching = false;

        wantedSize = standingSize;

        if (sprinting)
        {
            speed = runSpeed;
        } else
        {
            speed = walkSpeed;
        }
    }

    private void Crouch()
    {
        sprinting = false;
        crouching = !crouching;

        if (crouching)
        {
            wantedSize = standingSize / 2;
            speed = crouchSpeed;

        } else
        {
            wantedSize = standingSize;
            speed = walkSpeed;
        }
    }

    public Vector3 GetVelocity()
    {
        return rb.velocity;
    }

    public bool IsSprinting()
    {
        return sprinting;
    }

    public void SetSprinting(bool sprint)
    {
        if (sprint != sprinting)
        {
            Sprint();
        }
    }

    public void SetAiming(bool aim)
    {
        aiming = aim;
    }

    public void Footstep()
    {
        if (playerAudio && footStepsDirt.Length > 0)
        {
            playerAudio.PlayOneShot(footStepsDirt[Random.Range(0, footStepsDirt.Length)]);
        }
    }
}
