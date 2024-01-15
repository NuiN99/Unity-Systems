using UnityEngine;

namespace NuiN.Movement
{
    public interface IMovement
    {
        Rigidbody RB { get; set; }
        void Assign(Rigidbody rb) => RB = rb;
        void Move(IMovementInput input);
        void Rotate(IMovementInput input);
        void Jump();
    }
}