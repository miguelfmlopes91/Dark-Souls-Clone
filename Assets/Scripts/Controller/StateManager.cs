using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Controller
{
public class StateManager : MonoBehaviour
{
    public float Horizontal { get; set; }
    public float Vertical { get; set; }
    private float delta;
    private Animator _anim;
    private Rigidbody _rigidbody;
    private GameObject _activeModel;
    
    public void Init()
    {
        SetupAnimator();
        _rigidbody = GetComponent<Rigidbody>();
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

    public void Tick(float d)
    {
        delta = d;
    }
    // Start is called before the first frame update
    void Start()
    {
    
    }

    // Update is called once per frame
    void Update()
    {
    
    }
}
}