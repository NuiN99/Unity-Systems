using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NuiN.ScriptableHarmony.Editor.Attributes;
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
        [SerializeField] float walkSpeed = 10f;
        [SerializeField] float runSpeed = 15f;

        [Header("Rotate Speed Settings")]
        [SerializeField] float walkingRotateSpeed = 5f;
        [SerializeField] float runningRotateSpeed = 7.5f;
        
        [Header("Jump Settings")]
        [SerializeField] float jumpForce = 25f;
        [SerializeField] int maxAirJumps = 1;
        [SerializeField] float downForceMult = 1f;

        [Header("Drag Settings")]
        [SerializeField] float groundDrag = 4f;
        [SerializeField] float airDrag = 0;
        [SerializeField] float forceAppliedDrag = 0.5f;
        
        [Header("Environment Settings")]
        [SerializeField] LayerMask groundMask;
        [SerializeField] float groundCheckDist = 0.25f;
        [SerializeField] float slopeCheckDist = 0.25f;
        [SerializeField] float maxSlopeAngle = 45f;
        
        public Rigidbody RB { get; set; }

        void IMovement.FixedTick()
        {
            if (RB.velocity.y < 0)
            {
                RB.AddForce(Vector3.down * downForceMult);
            }
        }

        void IMovement.Move(IMovementInput input)
        {
            Vector3 direction = input.GetDirection();
            _grounded = Physics.Raycast(feet.position, -feet.up, out RaycastHit groundHit, groundCheckDist, groundMask);
            bool onSlope = Physics.Raycast(feet.position, feet.forward, out RaycastHit slopeHit, slopeCheckDist, groundMask);

            RB.drag = _grounded ? groundDrag : airDrag;

            float speed = (input.IsRunning() ? runSpeed : walkSpeed) * Time.fixedDeltaTime;
            
            RB.AddForce(direction * speed);
        }

        void IMovement.Rotate(IMovementInput input)
        {
            Quaternion rotation = input.GetRotation();
            float rotateSpeed = (input.IsRunning() ? runningRotateSpeed : walkingRotateSpeed) * Time.fixedDeltaTime;
            
            RB.MoveRotation(Quaternion.RotateTowards(transform.rotation, rotation, rotateSpeed));
        }

        void IMovement.Jump()
        {
            if (_grounded)
            {
                _curAirJumps = 0;
                RB.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
                return;
            }

            if (_curAirJumps >= maxAirJumps) return;
            
            RB.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            _curAirJumps++;
        }
    }
}