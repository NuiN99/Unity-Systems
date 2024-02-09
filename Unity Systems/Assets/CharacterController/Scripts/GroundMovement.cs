using NuiN.NExtensions;
using UnityEngine;

namespace NuiN.Movement
{
    public class GroundMovement : MonoBehaviour, IMovement
    {
        int _curAirJumps;
        bool _grounded;

        [Header("Dependencies")] 
        [SerializeField] Transform feet;

        [Header("Move Speed Settings")]
        [SerializeField] float moveSpeed = 0.05f;
        [SerializeField] float runSpeedMult = 1.5f;

        [SerializeField] float maxAirVelocityMagnitude = 7.5f;

        [SerializeField] float groundSpeedMult = 5f;
        [SerializeField] float groundDrag = 10f;
        [SerializeField] float airDrag;
        
        [Header("Rotate Speed Settings")]
        [SerializeField] float walkingRotateSpeed = 5f;
        [SerializeField] float runningRotateSpeed = 7.5f;

        [Header("Jump Settings")] 
        [SerializeField] SimpleTimer jumpDelay = new(0.2f);
        [SerializeField] float jumpForce = 7f;
        [SerializeField] int maxAirJumps = 1;
        [SerializeField] float downForceMult = 0.1f;
        [SerializeField] float downForceStartUpVelocity = 0.1f;

        [Header("Environment Settings")]
        [SerializeField] LayerMask groundMask;
        [SerializeField] float groundCheckDist = 0.25f;
        [SerializeField] float slopeCheckDist = 0.25f;
        //[SerializeField] float maxSlopeAngle = 45f;

        [SerializeField] Rigidbody rb;

        void Reset()
        {
            rb = GetComponent<Rigidbody>();
            GetComponent<Collider>();
        }

        void FixedUpdate()
        {
            if (rb.velocity.y <= downForceStartUpVelocity)
            {
                rb.velocity += Vector3.down * downForceMult;
            }
        }

        void IMovement.Move(IMovementInput input)
        {
            Vector3 direction = input.GetDirection().With(y: 0);

            bool running = input.IsRunning();
            float speed = (running ? moveSpeed * runSpeedMult : moveSpeed);
    
            _grounded = Physics.Raycast(feet.position, -feet.up, out RaycastHit groundHit, groundCheckDist, groundMask);
            bool onSlope = Physics.Raycast(feet.position, feet.forward, out RaycastHit slopeHit, slopeCheckDist, groundMask);

            Vector3 moveVector = direction * speed;
            Vector3 groundVelocity = rb.velocity.With(y: 0);
            Vector3 nextFrameVelocity = groundVelocity + moveVector;

            if (!_grounded)
            {
                rb.drag = airDrag;
                float maxAirVel = running ? maxAirVelocityMagnitude * runSpeedMult : maxAirVelocityMagnitude; 
                
                // only allow movement in a direction that doesnt increase players forward velocity past the max air vel
                if (nextFrameVelocity.magnitude >= maxAirVel && nextFrameVelocity.magnitude >= groundVelocity.magnitude)
                {
                    moveVector = Vector3.ProjectOnPlane(moveVector, rb.velocity.normalized);
                }
            }
            else
            {
                // account for ground drag
                moveVector *= groundSpeedMult;
                rb.drag = groundDrag;
                _curAirJumps = 0;
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
            if (!jumpDelay.Complete()) return;
            
            if (_grounded)
            {
                _curAirJumps = 0;
                rb.velocity = rb.velocity = rb.velocity.With(y: jumpForce);;
                return;
            }

            if (_curAirJumps >= maxAirJumps) return;
            _curAirJumps++;

            // only SETS y velocity when y velocity is less than potential jump force. Otherwise it would set y vel to a lower value when going faster
            if (rb.velocity.y <= jumpForce)
            {
                rb.velocity = rb.velocity = rb.velocity.With(y: jumpForce);;
            }
            else
            {
                rb.velocity += Vector3.up * jumpForce;
            }
        }
    }
}