using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Controller
{
public class StateManager : MonoBehaviour
{
    [Header("Model")]
    private GameObject _activeModel;
    private Animator _anim;
    private Rigidbody _rigidbody;
    
    [field: Header("inputs")]
    public float Horizontal { get; set; } 
    public float Vertical { get; set; }
    public float MoveAmount { get; set; }
    public Vector3 MoveDirection { get; set; }

    [Header("Stats")] 
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float runSpeed = 3.5f;
    [SerializeField] private float rotateSpeed = 5f;

    [Header("States")] 
    private bool running;
    
    private float delta;
    
    public void Init()
    {
        SetupAnimator();
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.angularDrag = 999f;
        _rigidbody.drag = 4f;
        _rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }

    private void SetupAnimator()
    {
        if (_activeModel == null)
        {
            _anim = GetComponentInChildren<Animator>();
            if (_anim == null)
            {
                Debug.LogError("No model found");
            }
            else
            {
                _activeModel = _anim.gameObject;
            }
        }

        if (_anim == null)
        {
            _anim = _activeModel.GetComponent<Animator>();
        }
    }

    public void FixedTick(float d)
    {
        delta = d;
        _rigidbody.drag = (MoveAmount > 0) ? 0f : 4f;
        
        //apply movement to the model
        float targetSpeed = moveSpeed;
        if (running)
            targetSpeed = runSpeed;
        _rigidbody.velocity = MoveDirection * (targetSpeed * MoveAmount);

        //apply real rotation to model
        Vector3 targetDir = MoveDirection;
        targetDir.y = 0;
        if (targetDir == Vector3.zero)
            targetDir = transform.forward;

        Quaternion tr = Quaternion.LookRotation(targetDir);
        Quaternion targetRotation = Quaternion.Slerp(transform.rotation, tr, delta * MoveAmount * rotateSpeed);
        transform.rotation = targetRotation;
        
        HandleMovementAnimations();
    }

    private void HandleMovementAnimations()
    {
        _anim.SetFloat("vertical", MoveAmount, 0.4f, delta);
    }
}
}