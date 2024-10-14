using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Shmoove
{
    public class ThirdPersonController : MonoBehaviour
    {
        //inputs from the input controller
        private PlayerControls playerControls;
        private InputAction jump;
        private InputAction move;
        private InputAction sprint;

        //movement fields
        private Rigidbody rb;
        [SerializeField]
        private float movementForce = 100f;
        [SerializeField]
        private float jumpForce = 12f;
        [SerializeField]
        private float walkSpeed = 6f;
        [SerializeField]
        private float sprintSpeed = 14f;
        [SerializeField]
        private float gravityMultiplier = 3f;
        [SerializeField]
        private int airJump = 1;


        private Vector3 forceDirection = Vector3.zero;

        [SerializeField]
        private Camera playerCamera;

        // singular startup initialization. Only called once at start
        private void Awake()
        {
            rb = this.GetComponent<Rigidbody>();
            playerControls = new PlayerControls();
        }

        // can be called multiple times, whenever script is loaded
        private void OnEnable()
        {
            // locking cursor
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            // using the jump and move controls, just initializes them
            playerControls.Player.Jump.performed += DoJump;
            move = playerControls.Player.Move;
            sprint = playerControls.Player.Sprint;
            playerControls.Player.Enable();
        }

        // If I read more documentation I could guess what this is. don't remove tho
        private void OnDisable()
        {
            playerControls.Player.Jump.performed -= DoJump;
            playerControls.Player.Disable();
        }

        // called many times a second
        private void FixedUpdate()
        {
            // Determine current max speed based on sprint input
            float currentMaxSpeed = sprint.IsPressed() ? sprintSpeed : walkSpeed;

            // movement from player input in 2 dimensions
            forceDirection += move.ReadValue<Vector2>().x * GetCameraRight(playerCamera) * movementForce;
            forceDirection += move.ReadValue<Vector2>().y * GetCameraForward(playerCamera) * movementForce;


            // adding movement before setting to zero so we stop when it's let go
            rb.AddForce(forceDirection, ForceMode.Acceleration);
            forceDirection = Vector3.zero;

            // when we're in the air and falling
            if (!IsGrounded())
            {
                // applying increased gravity for snappier and cleaner feel
                Vector3 extraGravity = (Physics.gravity * gravityMultiplier) - Physics.gravity;
                rb.AddForce(extraGravity, ForceMode.Acceleration);
            }

            // capping horizontal speeds
            Vector3 horizontalVelocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            if (horizontalVelocity.magnitude > currentMaxSpeed)
            {
                horizontalVelocity = horizontalVelocity.normalized * currentMaxSpeed;
                rb.velocity = new Vector3(horizontalVelocity.x, rb.velocity.y, horizontalVelocity.z);
            }

            // called to face the character where we're inputting to move to
            LookAt();

            // Debug log to verify sprint functionality
            //Debug.Log($"Current Speed: {rb.velocity.magnitude}, Max Speed: {currentMaxSpeed}, Sprinting: {sprint.IsPressed()}");
        }

        // function for implementing the playermodel looking in a direction
        private void LookAt()
        {
            Vector3 direction = rb.velocity;
            direction.y = 0f;

            if (move.ReadValue<Vector2>().sqrMagnitude > .1f && direction.sqrMagnitude > .1f)
                this.rb.rotation = Quaternion.LookRotation(direction, Vector3.up);
            else
                rb.angularVelocity = Vector3.zero;
        }

        // returning normalized right direction from the camera so we can move based on camera pov
        private Vector3 GetCameraRight(Camera playerCamera)
        {
            Vector3 right = playerCamera.transform.right;
            right.y = 0;
            return right.normalized;
        }

        // returning normalized forward direction from the camera so we can move based on camera pov
        private Vector3 GetCameraForward(Camera playerCamera)
        {
            Vector3 forward = playerCamera.transform.forward;
            forward.y = 0;
            return forward.normalized;
        }

        // applying jump to player when called
        private void DoJump(InputAction.CallbackContext context)
        {
            if (context.performed && IsGrounded())
            {
                rb.AddForce((Vector3.up * jumpForce), ForceMode.Impulse);
            }
            else if (context.performed && airJump > 0)
            {
                rb.AddForce((Vector3.up * jumpForce), ForceMode.Impulse);
                airJump -= 1;
            }
        }

        // raycast based method of determining if the player is on the ground right now
        private bool IsGrounded()
        {
            //Debug.Log("IsGrounded check initiated");

            // spherecast dimensions
            float spherecastRadius = 1f;
            float spherecastHeight = 1f;
            Vector3 capsuleBottom = transform.position + Vector3.up * (spherecastRadius - 0.1f);
            
            
            if (Physics.SphereCast(capsuleBottom, spherecastRadius, Vector3.down, out RaycastHit hit, spherecastHeight))
            {
                //Debug.Log($"Grounded: Hit {hit.collider.gameObject.name} at distance {hit.distance}");
                airJump = 1;
                return true;
            }
            else
            {
                //Debug.Log($"Not grounded");
                return false;
            }
        }
    }
}
