using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMove : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float _speed;
    [SerializeField] private float _airSpeed;
    [SerializeField] private float _jumpForce;
    [SerializeField] private int _maxJumps;
    private Rigidbody _rb;
    private int _currentJumpIndex = 0;
    Vector3 _moveDirection;

    [Header("Ground Checking")]
    [SerializeField] float _raycastDistance;
    bool _grounded;

    private Animator _anim;

    // Start is called before the first frame update
    void Start()
    {
        Physics.gravity = Vector3.down * 20f;
        _rb = GetComponent<Rigidbody>();
        _anim = GetComponent<Animator>();
    }

    private void Update()
    {
        float xMov = Input.GetAxis("Horizontal");
        float zMov = Input.GetAxis("Vertical");

        _moveDirection = new Vector3(xMov, 0.0f, zMov).normalized;
        transform.rotation = Quaternion.LookRotation(_moveDirection);


        CheckGrounded();
        if (Input.GetKeyDown(KeyCode.Space) && _currentJumpIndex < _maxJumps - 1)
        {
            Debug.Log("Current jump : " + _currentJumpIndex);
            if (_currentJumpIndex > 0) _anim.SetTrigger("SecondJump");
            _rb.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse);
            _currentJumpIndex++;
        }
        //Debug.Log(_grounded);
    }

    private void CheckGrounded()
    {
        if(Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, _raycastDistance))
        {
           Debug.Log(hit.collider.name);
            _grounded = true;
            _currentJumpIndex = 0;
        }
        else _grounded = false; 
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, Vector3.down * _raycastDistance);
    }
    void FixedUpdate()
    {
        if(_moveDirection.magnitude > 0)
        {
            if(_grounded) _rb.MovePosition(transform.position +_speed * Time.deltaTime * _moveDirection);
            else _rb.MovePosition(transform.position + _airSpeed * Time.deltaTime * _moveDirection);
        }
        
        
    }
}
