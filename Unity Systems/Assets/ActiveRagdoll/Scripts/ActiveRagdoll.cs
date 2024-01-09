using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveRagdoll : MonoBehaviour
{
    bool _fullRagdoll = false;
    
    [SerializeField] float globalMoveForce = 1f;
    [SerializeField] float globalRotateForce = 1f;

    [SerializeField] Transform center;
    [SerializeField] float ragdollEnableDist = 2f;

    [SerializeField] Transform nonPhysicalRig;
    [SerializeField] float yOffset = 1f;
    
    [SerializeField] PhysicalLimbTargeting[] limbs;
    
    Vector3 _averagePhysicalPos;

    void FixedUpdate()
    {
        if (_fullRagdoll) return;
        
        Vector3 sumPos = default;
        foreach (var limb in limbs)
        {
            limb.MoveToTarget(globalMoveForce, globalRotateForce);
            sumPos += limb.RB.position;
        }
        
        _averagePhysicalPos = sumPos / limbs.Length;
        float distFromAvg = Vector3.Distance(center.position, _averagePhysicalPos);

        nonPhysicalRig.position = new Vector3(_averagePhysicalPos.x, nonPhysicalRig.position.y, 0);

        /*if (distFromAvg >= ragdollEnableDist)
        {
            _fullRagdoll = true;
        }*/
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
    public void MoveToTarget(float moveMult, float rotateMult)
    {
        float dist = Vector3.Distance(Target.position, RB.position);
        dist *= dist;
        
        Vector3 direction = Target.position - RB.position;
        RB.AddForce(direction * (MoveForce * moveMult * dist * Time.fixedDeltaTime));
        
        float rotationFactor = Mathf.Clamp01(1.0f - dist / _maxDist);
        Quaternion targetRotation = Target.rotation * Quaternion.Inverse(RB.rotation);
        RB.AddTorque(targetRotation.eulerAngles * (RotateForce * rotateMult * rotationFactor *  Time.fixedDeltaTime));
    }
}
