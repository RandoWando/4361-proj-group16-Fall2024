using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Shmoove
{
    public class ThirdPersonController : MonoBehaviour
    {
        [Header("Inputs")]
        //inputs from the input controller
        private PlayerControls playerControls;
        private InputAction jump;
        private InputAction move;
        private InputAction sprint;

        [Header("Scene Objects")]
        [SerializeField]
        private Camera playerCamera;
        private Rigidbody rb;

        //movement fields
        [Header("Movement")]
        [SerializeField]
        public float walkSpeed = 6f;
        [SerializeField]
        public float sprintSpeed = 10f;
        
        private float moveSpeed;
        [SerializeField]
        public float groundDrag = 1f;
        //private bool comboActive;     // REMOVE IF EVENTUALLY UNUSED

        [Header("Jumping")]
        [SerializeField]
        private float jumpForce = 12f;
        [SerializeField]
        private float gravityMultiplier = 3f;
        [SerializeField]
        private int airJump = 1;

        private Vector3 moveDirection = Vector3.zero;

        // Player movement tracker
        public movementState state;
        public enum movementState
        {
            walking,
            sprinting,
            //combo,
            inAir
        }

        // Player debuff state tracker
        public statusEffects status;
        public enum statusEffects
        {
            None,
            noControl,
            noAbilities,
            lowGrav,
            extraJump
        }

        // singular startup initialization. Only called once at start
        private void Awake()
        {
            rb = this.GetComponent<Rigidbody>();
        }

        // can be called multiple times, whenever script is loaded
        private void OnEnable()
        {
            // locking cursor
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            // using the jump and move controls, just initializes them
            playerControls = new PlayerControls();
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

            // called for requisite movement speed handling
            movementHandler();

            // called to provide movement to the player
            playerMove();

            // called to face the character where we're inputting to move to
            LookAt();

            // applying drag based on player situation
            if (IsGrounded())
            {
                rb.drag = groundDrag;
            }
            else
            {
                rb.drag = 0;
            }

        }

        private void playerMove()
        {
            moveDirection = Vector3.zero;

            // movement from player input in 2 dimensions
            moveDirection += move.ReadValue<Vector2>().x * GetCameraRight(playerCamera);
            moveDirection += move.ReadValue<Vector2>().y * GetCameraForward(playerCamera);
            // normalizing so the input is = 1
            moveDirection = moveDirection.normalized;

            Vector3 horizontalVelocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);

            // adding movement, limited air control in else 
            if (IsGrounded())
                rb.AddForce(moveDirection, ForceMode.Impulse);
            else
                rb.AddForce(moveDirection, ForceMode.Impulse);

            Debug.Log($"Current Speed: {Math.Abs(rb.velocity.x) + Math.Abs(rb.velocity.z)}");

            // capping horizontal speeds
            horizontalVelocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            if (horizontalVelocity.magnitude > moveSpeed)
            {
                horizontalVelocity = horizontalVelocity.normalized * moveSpeed;
                rb.velocity = new Vector3(horizontalVelocity.x, rb.velocity.y, horizontalVelocity.z);
            }

            // when we're in the air and falling
            if (!IsGrounded())
            {
                // applying increased gravity for snappier and cleaner feel
                Vector3 extraGravity = (Physics.gravity * gravityMultiplier) - Physics.gravity;
                rb.AddForce(extraGravity, ForceMode.Acceleration);
            }

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
                Vector3 jumpForce = Vector3.up * this.jumpForce;
                rb.AddForce(jumpForce, ForceMode.Impulse);
            }
            else if (context.performed && airJump > 0)
            {
                Vector3 jumpForce = Vector3.up * this.jumpForce;
                rb.AddForce(jumpForce, ForceMode.Impulse);
                airJump--;
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

        // more consolidated way to handle the 3 movespeeds
        private void movementHandler()
        {
            // state  sprinting
            if (IsGrounded() && sprint.IsPressed())
            {
                state = movementState.sprinting; moveSpeed = sprintSpeed;
            }
            // state  walking
            else if (IsGrounded())
            {
                state = movementState.walking; moveSpeed = walkSpeed;
            }
            // special extra movestate for maybe something REMOVE IF EVENTUALLY UNUSED
            //else if (comboActive)
            //{
            //    state = movementState.combo; moveSpeed = 1.5f * sprintSpeed;
            //}
            // state  inAir
            else
            {
                state = movementState.inAir;
            }
        }

        // debuff
        private void debuffHandler(float time)
        {
            switch (status)
            {
                case (statusEffects.None):
                    {
                        
                        break;
                    }
                case (statusEffects.noControl):
                    {
                        break;
                    }
                case (statusEffects.noAbilities):
                {

                        break;
                }
                case (statusEffects.lowGrav):
                {

                        break;
                }
                case (statusEffects.extraJump):
                {

                        break;
                }


            }



        }

    }
}
