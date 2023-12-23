using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NuiN.ScriptableHarmony.Editor.Attributes;
using UnityEngine;

namespace NuiN.Movement
{
    public class MovementController : MonoBehaviour
    {
        IMovement _movement;
        IMovementInput _input;
        
        List<MovementConstraint> _activeConstraints = new();

        [SerializeField] Rigidbody rb;
        
        [field: SerializeField, ReadOnlyPlayMode] public bool CanMove { get; private set; } = true;
        [field: SerializeField, ReadOnlyPlayMode] public bool CanRotate { get; private set; } = true;
        [field: SerializeField, ReadOnlyPlayMode] public bool IsRunning { get; private set; } = false;
        
        bool ConstraintApplied => _activeConstraints.Count > 0;

        void Reset()
        {
            rb = rb ? rb : GetComponent<Rigidbody>();
        }

        void Awake()
        {
            _movement = GetComponent<IMovement>();
            if(_movement == null) Debug.LogError($"Missing Movement component on {gameObject}", gameObject);
            
            _input = GetComponent<IMovementInput>();
            if (_input == null) Debug.LogError($"Missing MovementInput on {gameObject.name}", gameObject);
            
            _movement?.Assign(rb);
        }

        void OnEnable()
        {
            _input.OnJump += _movement.Jump;
        }
        void OnDisable()
        {
            _input.OnJump -= _movement.Jump;
        }

        void FixedUpdate()
        {
            IsRunning = _input.IsRunning(); // only for debugging
            
            _movement.FixedTick();
            if(CanMove) _movement.Move(_input);
            if(CanRotate) _movement.Rotate(_input);
        }
        
        public void ApplyConstraint(float duration, MovementConstraint constraint)
        {
            StartCoroutine(ApplyConstraintRoutine(duration, constraint));
        }

        IEnumerator ApplyConstraintRoutine(float duration, MovementConstraint constraint)
        {
            _activeConstraints.Add(constraint);
            EvaluateConstraints();
            
            yield return new WaitForSeconds(duration);
            
            _activeConstraints.Remove(constraint);
            EvaluateConstraints();
        }

        void EvaluateConstraints()
        {
            // only allow move and rotate if every element on the stack allows it
            CanMove = _activeConstraints.All(constraint => constraint.canMove);
            CanRotate = _activeConstraints.All(constraint => constraint.canRotate);
        }
    }
}



