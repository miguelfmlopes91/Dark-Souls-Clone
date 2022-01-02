using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Controller
{
public class StateManager : MonoBehaviour
{
    [Header("Model")]
    private GameObject _activeModel;
    public Animator Anim { get; private set; }
    public Rigidbody RgBody { get; private set; }
    private AnimatorHook _animatorHook;
    
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
    public bool CanMove { get; private set; }
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

    public float Delta { get; private set; }
    private float _actionDelay;
    [HideInInspector] public LayerMask ignoreLayers;
    
    
    public void Init()
    {
        SetupAnimator();
        RgBody = GetComponent<Rigidbody>();
        RgBody.angularDrag = 999f;
        RgBody.drag = 4f;
        RgBody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        _animatorHook = _activeModel.AddComponent<AnimatorHook>();
        _animatorHook.Init(this);
        
        gameObject.layer = 8;
        ignoreLayers = ~(1 << 9);
        
        Anim.SetBool("onGround", true);
    }

    private void SetupAnimator()
    {
        if (_activeModel == null)
        {
            Anim = GetComponentInChildren<Animator>();
            if (Anim == null)
            {
                Debug.LogError("No model found");
            }
            else
            {
                _activeModel = Anim.gameObject;
            }
        }

        if (Anim == null)
        {
            Anim = _activeModel.GetComponent<Animator>();
        }
    }

    public void FixedTick(float d)
    {
        Delta = d;
        
        DetectAction();

        if (inAction)
        {
            Anim.applyRootMotion = true;
            _actionDelay += Delta;
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
        
        CanMove = Anim.GetBool("canMove");

        if (CanMove == false)
        {
            return;
        }

        Anim.applyRootMotion = false;
        
        RgBody.drag = (MoveAmount > 0 || !OnGround) ? 0f : 4f;
        
        //apply movement to the model
        float targetSpeed = moveSpeed;
        if (Running)
        {
            targetSpeed = runSpeed;
            lockOn = false;
        }
        
        if (OnGround)
            RgBody.velocity = MoveDirection * (targetSpeed * MoveAmount);

        if (!lockOn)
        {
            //apply real rotation to model
            Vector3 targetDir = MoveDirection;
            targetDir.y = 0;
            if (targetDir == Vector3.zero)
                targetDir = transform.forward;

            Quaternion tr = Quaternion.LookRotation(targetDir);
            Quaternion targetRotation = Quaternion.Slerp(transform.rotation, tr, Delta * MoveAmount * rotateSpeed);
            transform.rotation = targetRotation;
        }
        
        HandleMovementAnimations();
    }

    public void Tick(float d)
    {
        Delta = d;
        Anim.SetBool("onGround", OnGround);
    }

    private void HandleMovementAnimations()
    {
        Anim.SetBool("run", Running);
        Anim.SetFloat("vertical", MoveAmount, 0.4f, Delta);
    }

    public void DetectAction()
    {
        if (CanMove == false) return;

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

        CanMove = false;
        inAction = true;
        Anim.CrossFade(targetAnimaton, 0.2f);
        //_rigidbody.velocity = Vector3.zero;
    }
}
}