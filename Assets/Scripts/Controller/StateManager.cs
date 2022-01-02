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
    public bool rt, rb, lt, lb; //TODO: have some enums
    
    [Header("Stats")] 
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float runSpeed = 3.5f;
    [SerializeField] private float rotateSpeed = 5f;
    [SerializeField] private float toGround = 0.5f;

    [field: Header("States")] 
    public bool Running { get; set; }
    private bool onGround;
    private bool lockOn;
    private bool inAction;
    private bool canMove;
    public bool OnGround
    {
        get
        {
            bool r = false;
            Vector3 origin = transform.position + (Vector3.up * toGround);
            float dist = toGround + 0.3f;
            RaycastHit hit;
            //Debug.DrawRay(origin, Vector3.down * dist);
            if (Physics.Raycast(origin, Vector3.down, out hit, dist, ignoreLayers))
            {
                r = true;
                Vector3 targetPosition = hit.point;
                transform.position = targetPosition;
            }
            
            return r;
        }
        set => onGround = value;
    }
    
    private float delta;
    private float _actionDelay;
    [HideInInspector] public LayerMask ignoreLayers;
    
    
    public void Init()
    {
        SetupAnimator();
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.angularDrag = 999f;
        _rigidbody.drag = 4f;
        _rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        gameObject.layer = 8;
        ignoreLayers = ~(1 << 9);
        
        _anim.SetBool("onGround", true);
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
        
        DetectAction();

        if (inAction)
        {
            _actionDelay += delta;
            if (_actionDelay > 0.3f)
            {
                inAction = false;
                _actionDelay = 0f;
            }
            else
            {
                return;
            }
        }
        
        canMove = _anim.GetBool("canMove");

        if (canMove == false)
        {
            return;
        }

        _rigidbody.drag = (MoveAmount > 0 || !OnGround) ? 0f : 4f;
        
        //apply movement to the model
        float targetSpeed = moveSpeed;
        if (Running)
        {
            targetSpeed = runSpeed;
            lockOn = false;
        }
        
        if (OnGround)
            _rigidbody.velocity = MoveDirection * (targetSpeed * MoveAmount);

        if (!lockOn)
        {
            //apply real rotation to model
            Vector3 targetDir = MoveDirection;
            targetDir.y = 0;
            if (targetDir == Vector3.zero)
                targetDir = transform.forward;

            Quaternion tr = Quaternion.LookRotation(targetDir);
            Quaternion targetRotation = Quaternion.Slerp(transform.rotation, tr, delta * MoveAmount * rotateSpeed);
            transform.rotation = targetRotation;
        }
        
        HandleMovementAnimations();
    }

    public void Tick(float d)
    {
        delta = d;
        _anim.SetBool("onGround", OnGround);
    }

    private void HandleMovementAnimations()
    {
        _anim.SetBool("run", Running);
        _anim.SetFloat("vertical", MoveAmount, 0.4f, delta);
    }

    public void DetectAction()
    {
        if (canMove == false) return;

        if (rb == false && rt == false && lt == false && lb == false)
            return;

        string targetAnimaton = null;

        if (rb)
            targetAnimaton = "oh_attack_1";
        if (rt)
            targetAnimaton = "oh_attack_2";
        if (lt)
            targetAnimaton = "oh_attack_3";
        if (lb)
            targetAnimaton = "th_attack_1";

        if (string.IsNullOrEmpty(targetAnimaton))
            return;

        canMove = false;
        inAction = true;
        _anim.CrossFade(targetAnimaton, 0.2f);
        _rigidbody.velocity = Vector3.zero;
    }
}
}