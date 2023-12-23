using System;
using NuiN.Movement;
using UnityEngine;

public class InspectorMovementInput : MonoBehaviour, IMovementInput
{
    [SerializeField] Vector3 direction = new Vector3(0, 0, 1);
    [SerializeField] Quaternion rotation = Quaternion.Euler(0,0,0);
    [SerializeField] bool isRunning = false;
    [SerializeField] bool jump = false;

    public Action OnJump { get; set; }

    void Update()
    {
        if (jump == true)
        {
            jump = false;
            OnJump?.Invoke();
        }
    }

    Vector3 IMovementInput.GetDirection()
    {
        return direction;
    }

    Quaternion IMovementInput.GetRotation()
    {
        return rotation;
    }

    bool IMovementInput.IsRunning()
    {
        return isRunning;
    }
}
