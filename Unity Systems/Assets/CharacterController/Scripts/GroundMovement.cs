using System.Collections;
using NuiN.NExtensions;
using UnityEngine;

namespace NuiN.Movement
{
    public class GroundMovement : MonoBehaviour, IMovement
    {
        int _curAirJumps;
        bool _grounded;
        bool _jumpOnCooldown;

        [Header("Dependencies")] 
        [SerializeField] Transform feet;

        [Header("Move Speed Settings")]
        [SerializeField] float moveSpeed = 10f;
        [SerializeField] float runSpeedMult = 1.5f;

        [SerializeField] float maxAirVelocityMagnitude = 10f;

        [SerializeField] float groundSpeedMult = 5f;
        [SerializeField] float groundDrag = 5f;
        [SerializeField] float airDrag = 0.05f;
        
        [Header("Rotate Speed Settings")]
        [SerializeField] float walkingRotateSpeed = 5f;
        [SerializeField] float runningRotateSpeed = 7.5f;
        
        [Header("Jump Settings")]
        [SerializeField] float jumpForce = 25f;
        [SerializeField] int maxAirJumps = 1;
        [SerializeField] SerializedWaitForSeconds jumpDelay = new(0.25f);
        [SerializeField] float downForceMult = 1f;

        [Header("Environment Settings")]
        [SerializeField] LayerMask groundMask;
        [SerializeField] float groundCheckDist = 0.25f;
        [SerializeField] float slopeCheckDist = 0.25f;
        //[SerializeField] float maxSlopeAngle = 45f;

        [SerializeField] Rigidbody rb;
        [SerializeField] Collider col;

        void Reset()
        {
            rb = GetComponent<Rigidbody>();
            col = GetComponent<Collider>();
        }

        void FixedUpdate()
        {
            if (rb.velocity.y < 0)
            {
                rb.AddForce(Vector3.down * downForceMult);
            }
        }

        void IMovement.Move(IMovementInput input)
        {
            Vector3 direction = input.GetDirection();

            bool running = input.IsRunning();
            float speed = (running ? moveSpeed * runSpeedMult : moveSpeed);
            
            _grounded = Physics.Raycast(feet.position, -feet.up, out RaycastHit groundHit, groundCheckDist, groundMask);
            bool onSlope = Physics.Raycast(feet.position, feet.forward, out RaycastHit slopeHit, slopeCheckDist, groundMask);

            Vector3 moveVector = direction * speed;
            Vector3 newVelocity = rb.velocity.With(y: 0) + moveVector;

            if (!_grounded)
            {
                rb.drag = airDrag;
                float maxAirVel = running ? maxAirVelocityMagnitude * runSpeedMult : maxAirVelocityMagnitude; 
                if (newVelocity.magnitude >= maxAirVel)
                {
                    Vector3 clampedVector = Vector3.ClampMagnitude(newVelocity, maxAirVel);
                    Vector3 allowedVector = clampedVector - rb.velocity.With(y: 0);

                    moveVector = allowedVector;
                }
            }
            else
            {
                moveVector *= groundSpeedMult;
                rb.drag = groundDrag;
            }
            
            rb.velocity += moveVector;
        }

        void IMovement.Rotate(IMovementInput input)
        {
            Quaternion rotation = input.GetRotation();
            float rotateSpeed = input.IsRunning() ? runningRotateSpeed : walkingRotateSpeed;

            transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, rotateSpeed);
        }

        void IMovement.Jump()
        {
            if (_jumpOnCooldown) return;
            StartCoroutine(JumpDelayRoutine());
            
            if (_grounded)
            {
                _curAirJumps = 0;
                rb.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);
                return;
            }

            if (_curAirJumps >= maxAirJumps) return;

            rb.velocity += Vector3.up * jumpForce;
            _curAirJumps++;
        }

        IEnumerator JumpDelayRoutine()
        {
            _jumpOnCooldown = true;
            yield return jumpDelay.Wait;
            _jumpOnCooldown = false;
        }
    }
}