using System;
using UnityEngine;

namespace NuiN.Movement
{
    public interface IMovementInput
    {
        Action OnJump { get; set; }
        
        Vector3 GetDirection();
        Quaternion GetRotation();
        bool IsRunning();
    }
}