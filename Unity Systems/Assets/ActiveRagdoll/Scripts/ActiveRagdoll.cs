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

    [SerializeField] float ragdollEnableDist = 2f;

    [SerializeField] Transform nonPhysicalRig;
    [SerializeField] Transform nonPhysicalRigCenter;

    [SerializeField] Transform physicalRigCenter;

    [SerializeField] float limbDrag = 2f;

    [SerializeField] float maxVelocity = 2.5f;

    [SerializeField] Transform phyiscalHead;
    [SerializeField] Transform physicalHips;
    
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
        
        float distApart = Vector3.Distance(nonPhysicalRigCenter.position.With(y: 0), physicalRigCenter.position.With(y: 0));
        
        if (!_fullRagdoll && distApart >= ragdollEnableDist)
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
