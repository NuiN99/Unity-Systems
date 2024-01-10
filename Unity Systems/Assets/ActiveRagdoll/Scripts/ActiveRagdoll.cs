using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveRagdoll : MonoBehaviour
{
    bool _fullRagdoll = false;
    
    [SerializeField] float globalMoveForce = 1f;
    [SerializeField] float globalRotateForce = 1f;
    [SerializeField] float damping = 0.9f;

    [SerializeField] Transform center;
    [SerializeField] float ragdollEnableDist = 2f;

    [SerializeField] Transform nonPhysicalRig;
    [SerializeField] float yOffset = 1f;
    
    [SerializeField] float limbDrag = 2f;

    [SerializeField] float maxVelocity = 2.5f;
    
    [SerializeField] PhysicalLimbTargeting[] limbs;
    
    Vector3 _averagePhysicalPos;

    void FixedUpdate()
    {
        Vector3 sumPos = default;
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
            
            sumPos += limb.RB.position;
        }
        
        _averagePhysicalPos = sumPos / limbs.Length;
        float distFromAvg = Vector3.Distance(center.position, _averagePhysicalPos);

        nonPhysicalRig.position = new Vector3(_averagePhysicalPos.x, nonPhysicalRig.position.y, _averagePhysicalPos.z);
        
        if (!_fullRagdoll && distFromAvg >= ragdollEnableDist)
        {
            StartCoroutine(StunRoutine());
        }
    }

    IEnumerator StunRoutine()
    {
        _fullRagdoll = true;
        yield return new WaitForSeconds(2f);
        _fullRagdoll = false;
    }
}

[Serializable]
public class PhysicalLimbTargeting
{
    [field: SerializeField] public Rigidbody RB { get; private set; }
    [field: SerializeField] public Transform Target { get; private set; }
    [field: SerializeField] public float MoveForce { get; private set; } = 25;
    [field: SerializeField] public float RotateForce { get; private set; } = 25;

    float _maxDist = 5f;
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
