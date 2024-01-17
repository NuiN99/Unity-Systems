using System;
using System.Collections;
using System.Collections.Generic;
using NuiN.Movement;
using UnityEngine;

public class PlayerMovementInput : MonoBehaviour, IMovementInput
{
    const string MOUSE_X = "Mouse X";
    const string MOUSE_Y = "Mouse Y";
    
    Vector2 _rotation;

    [SerializeField] Transform cam;
    [SerializeField] Transform headPos;
    
    [SerializeField] KeyCode sprintKey = KeyCode.LeftShift;
    [SerializeField] KeyCode jumpKey = KeyCode.Space;
    
    [SerializeField] float lookSensitivity = 20f;
    [Range(0f, 90f)][SerializeField] float yRotationLimit = 88f;

    void LateUpdate()
    {
        cam.position = headPos.position;
    }

    bool IMovementInput.ShouldJump()
    {
        return Input.GetKeyDown(jumpKey);
    }

    Vector3 IMovementInput.GetDirection()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        Vector3 forward = cam.forward;
        Vector3 right = cam.right;

        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        Vector3 desiredMoveDirection = forward * z + right * x;

        return desiredMoveDirection.normalized;
    }
    
    Quaternion IMovementInput.GetRotation()
    {
        _rotation.x += Input.GetAxis(MOUSE_X) * lookSensitivity;
        _rotation.y += Input.GetAxis(MOUSE_Y) * lookSensitivity;
        _rotation.y = Mathf.Clamp(_rotation.y, -yRotationLimit, yRotationLimit);
    
        var xQuat = Quaternion.AngleAxis(_rotation.x, Vector3.up);
        var yQuat = Quaternion.AngleAxis(_rotation.y, Vector3.left);

        Quaternion newRotation = xQuat * yQuat;
        cam.transform.rotation = newRotation;

        return Quaternion.Euler(0, newRotation.eulerAngles.y, 0);
    }

    bool IMovementInput.IsRunning()
    {
        return Input.GetKey(sprintKey);
    }
}
