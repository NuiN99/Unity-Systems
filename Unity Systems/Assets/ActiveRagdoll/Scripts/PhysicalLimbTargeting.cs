using System;
using UnityEngine;

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
            RB.velocity += direction * (MoveForce * moveMult * RB.position.y);
        }
        
        Quaternion targetRotation = Target.rotation * Quaternion.Inverse(RB.rotation);
        RB.AddTorque(targetRotation.eulerAngles * (RotateForce * rotateMult));

        RB.velocity *= damping;
    }
}