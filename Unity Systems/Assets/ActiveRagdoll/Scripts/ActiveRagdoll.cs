using System;
using System.Collections;
using Animancer;
using NuiN.NExtensions;
using UnityEngine;

public class ActiveRagdoll : MonoBehaviour
{
    bool _fullRagdoll = false;

    [SerializeField] AnimancerComponent animator;
    [SerializeField] AnimationClip getUpAnim;
    [SerializeField] AnimationClip runAnim;
    
    [SerializeField] float globalMoveForce = 1f;
    [SerializeField] float globalRotateForce = 1f;
    [SerializeField] float damping = 0.9f;

    [SerializeField] float maxOffBalanceDist = 2f;
    [SerializeField] float lyingDownCheckRadius = 1f;

    [SerializeField] Transform nonPhysicalRig;
    [SerializeField] Transform nonPhysicalRigCenter;

    [SerializeField] Transform physicalRigCenter;

    [SerializeField] float limbDrag = 2f;

    [SerializeField] float maxVelocity = 2.5f;

    [SerializeField] Transform phyiscalHead;
    [SerializeField] Transform physicalHips;

    [SerializeField] Transform leftFoot;
    [SerializeField] Transform rightFoot;

    [SerializeField] float groundCheckDist = 0.25f;
    [SerializeField] LayerMask groundMask;
    
    [SerializeField] PhysicalLimbTargeting[] limbs;

    

    void FixedUpdate()
    {
        foreach (var limb in limbs)
        {
            if (!_fullRagdoll)
            {
                limb.RB.drag = limbDrag;
                limb.MoveToTarget(globalMoveForce, globalRotateForce, damping, maxVelocity);
            }
            else
            {
                limb.RB.drag = 0;
            }
        }


        bool feetOnGround = Physics.Raycast(GetAverageFootPos(), Vector3.down, out RaycastHit hit, groundCheckDist, groundMask);
        float offBalanceDist = Vector3.Distance(physicalRigCenter.position.With(y: 0), GetAverageFootPos().With(y: 0));

        bool offBalance = offBalanceDist >= maxOffBalanceDist;
        bool lyingDown = Physics.OverlapSphere(physicalRigCenter.position, lyingDownCheckRadius, groundMask).Length > 0;
        
        if (!_fullRagdoll && offBalance && !lyingDown)
        {
            StartCoroutine(StunRoutine());
        }
    }

    IEnumerator StunRoutine()
    {
        _fullRagdoll = true;
        yield return new WaitForSeconds(2f);
        _fullRagdoll = false;
        
        // move rig to match physical body
        Vector3 offset = physicalRigCenter.position - nonPhysicalRigCenter.position;
        
        nonPhysicalRig.position = 
            new Vector3(nonPhysicalRig.position.x + offset.x, nonPhysicalRig.position.y, nonPhysicalRig.position.z + offset.z);

        nonPhysicalRig.rotation = GetPhysicalRotation();
        
        animator.Play(getUpAnim).Force();
    }
    
    Quaternion GetPhysicalRotation()
    {
        float angleHeadToHips = Vector3.Angle(phyiscalHead.position, physicalHips.position);
        return Quaternion.AngleAxis(angleHeadToHips, Vector3.up);
    }

    Vector3 GetAverageFootPos()
    {
        return (leftFoot.position + rightFoot.position) / 2;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(physicalRigCenter.position, lyingDownCheckRadius);
        Gizmos.DrawSphere(GetAverageFootPos(), 0.05f);
        Gizmos.color = default;
        
        Debug.DrawRay(GetAverageFootPos(), Vector3.down * groundCheckDist);
        
        // if grounded
        if (Physics.Raycast(GetAverageFootPos(), Vector3.down, out RaycastHit hit, groundCheckDist, groundMask))
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(hit.point, 0.05f);
            Gizmos.color = default;
        }
    }
}

[Serializable]
public class PhysicalLimbTargeting
{
    [field: SerializeField] public Rigidbody RB { get; private set; }
    [field: SerializeField] public Transform Target { get; private set; }
    [field: SerializeField] public float MoveForce { get; private set; } = 25;
    [field: SerializeField] public float RotateForce { get; private set; } = 25;

    public void MoveToTarget(float moveMult, float rotateMult, float damping, float maxVelocity)
    {
        Vector3 direction = Target.position - RB.position;
        if (RB.velocity.magnitude < maxVelocity)
        {
            RB.velocity += direction * (MoveForce * moveMult * Time.fixedDeltaTime * RB.position.y);
        }
        
        Quaternion targetRotation = Target.rotation * Quaternion.Inverse(RB.rotation);
        RB.AddTorque(targetRotation.eulerAngles * (RotateForce * rotateMult *  Time.fixedDeltaTime));

        RB.velocity *= damping;
    }
}
