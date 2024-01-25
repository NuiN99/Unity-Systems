using System.Collections;
using Animancer;
using NuiN.NExtensions;
using UnityEngine;

public class ActiveRagdoll : MonoBehaviour
{
    bool _fullRagdoll;

    [SerializeField] AnimancerComponent animator;
    [SerializeField] AnimationClip runAnim;
    
    [SerializeField] float globalMoveForce = 1f;
    [SerializeField] float globalRotateForce = 1f;
    [SerializeField] float damping = 0.9f;

    [SerializeField] float maxOffBalanceDist = 2f;

    [SerializeField] Transform physicalRigCenter;

    [SerializeField] float limbDrag = 2f;

    [SerializeField] float maxVelocity = 2.5f;

    [SerializeField] Rigidbody leftFoot;
    [SerializeField] Rigidbody rightFoot;

    [SerializeField] float groundCheckDist = 0.25f;
    [SerializeField] LayerMask groundMask;
    
    [SerializeField] FollowLimb[] limbs;
    
    void Start()
    {
        animator.Play(runAnim).Force();
    }

    void FixedUpdate()
    {
        if (_fullRagdoll) return;
        foreach (var limb in limbs)
        {
            limb.RB.drag = limbDrag;
            limb.MoveToTarget(globalMoveForce, globalRotateForce, damping, maxVelocity);
        }
        
        float offBalanceDist = Vector3.Distance(physicalRigCenter.position.With(y: 0), GetAverageFootPos().With(y: 0));
        bool offBalance = offBalanceDist >= maxOffBalanceDist;
        
        if (offBalance)
        {
            Ragdoll();
        }
    }

    void Ragdoll()
    {
        _fullRagdoll = true;

        foreach (var limb in limbs)
        {
            limb.RB.drag = 0f;
        }
    }

    Vector3 GetAverageFootPos()
    {
        return (leftFoot.position + rightFoot.position) / 2;
    }
}