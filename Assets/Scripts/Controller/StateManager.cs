using System.Collections;
using System.Collections.Generic;
using Enemies;
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
    [HideInInspector]public ActionManager ActionManager;
    
    [field: Header("inputs")]
    public float Horizontal { get; set; } 
    public float Vertical { get; set; }
    public float MoveAmount { get; set; }
    public Vector3 MoveDirection { get; set; }
    public bool rt, rb, lt, lb; //TODO: have some enums
    public bool RollInput { get;  set; }


    [Header("Stats")] 
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float runSpeed = 3.5f;
    [SerializeField] private float rotateSpeed = 5f;
    [SerializeField] private float toGround = 0.5f;
    [SerializeField] private float rollSpeed = 1f;

    [field: Header("States")] 
    public bool Running { get; set; }
    private bool onGround;
    public bool LockOn { get; set; }
    private bool inAction;
    public bool IsTwoHanded { get; set; }
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
    
    [Header("Other")] 
    public EnemyTarget LockOnTarget;
    public Transform LockOnTransform;
    public AnimationCurve roll_curve;
    
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


        ActionManager = GetComponent<ActionManager>();
        ActionManager.Init();
        
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

        if (!CanMove)
            return;
        
        _animatorHook.RootMotionMultiplier = 1f;//reset root motion before checking for rolls
        _animatorHook.CloseRoll();
        HandleRolls();
        

        Anim.applyRootMotion = false;
        RgBody.drag = (MoveAmount > 0 || !OnGround) ? 0f : 4f;
        
        //apply movement to the model
        float targetSpeed = moveSpeed;
        if (Running)
        {
            targetSpeed = runSpeed;
            LockOn = false;
        }
        
        if (OnGround)
            RgBody.velocity = MoveDirection * (targetSpeed * MoveAmount);


        //apply real rotation to model
        Vector3 targetDir = (LockOn == false) ? 
            MoveDirection 
            : (LockOnTransform != null) ? 
                LockOnTransform.position - transform.position 
                : MoveDirection;
        
        targetDir.y = 0;
        if (targetDir == Vector3.zero)
            targetDir = transform.forward;

        Quaternion tr = Quaternion.LookRotation(targetDir);
        Quaternion targetRotation = Quaternion.Slerp(transform.rotation, tr, Delta * MoveAmount * rotateSpeed);
        transform.rotation = targetRotation;
        
        Anim.SetBool("lockOn", LockOn);
        
        if (LockOn)
            HandleLockOnAnimations(MoveDirection);
        else
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

    private void HandleLockOnAnimations(Vector3 moveDir)
    {
        Vector3 relativeDir = transform.InverseTransformDirection(moveDir);
        float h = relativeDir.x;
        float v = relativeDir.z;
        
        Anim.SetFloat("vertical", v, 0.4f, Delta);
        Anim.SetFloat("horizontal", h, 0.4f, Delta);
    }

    private void HandleRolls()
    {
        if (!RollInput)
            return;

        float v = Vertical;
        float h = Horizontal;
        v = (MoveAmount > 0.3f) ? 1 : 0;
        h = 0;
        /*if (!LockOn)
        {
            v = (MoveAmount > 0.3f) ? 1 : 0;
            h = 0;
        }
        else
        {
            if (Mathf.Abs(v) < 0.3f)
                v = 0;
            if (Mathf.Abs(h) < 0.3f)
                h = 0;
        }*/

        if (v != 0)
        {
            if (MoveDirection == Vector3.zero)
                MoveDirection = transform.forward;
            Quaternion targetRot = Quaternion.LookRotation(MoveDirection);
            transform.rotation = targetRot;
            _animatorHook.InitForRoll();
            _animatorHook.RootMotionMultiplier = rollSpeed;
        }
        else
        {
            _animatorHook.RootMotionMultiplier = 1.3f;
        }

        Anim.SetFloat("vertical", v);
        Anim.SetFloat("horizontal", h);
        
        CanMove = false;
        inAction = true;
        Anim.CrossFade("Rolls", 0.2f);
    }

    public void DetectAction()
    {
        if (CanMove == false) return;

        if (rb == false && rt == false && lt == false && lb == false)
            return;

        
        Action action = ActionManager.GetActionSlot(this);
        if (action == null) return;
        
        string targetAnimaton = action.targetAnimation;
        if (string.IsNullOrEmpty(targetAnimaton)) return;

        CanMove = false;
        inAction = true;
        Anim.CrossFade(targetAnimaton, 0.2f);
        //_rigidbody.velocity = Vector3.zero;
    }

    public void HandleTwoHanded()
    {
        Anim.SetBool("two_handed", IsTwoHanded);
    }
}
}