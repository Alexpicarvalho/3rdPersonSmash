using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

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
    bool _canMove = true;

    [Header("Ground Checking")]
    [SerializeField] float _raycastDistance;
    [SerializeField] float _jumpInputCooldown = .1f;
    bool _grounded;
    float _timeSinceLastJump = 0;

    [Header("Dodge Dash")]
    [SerializeField] float _dodgeCooldown;
    [SerializeField] float _dodgeSpeed;
    [SerializeField] float _dodgeDuration;
    [SerializeField] AnimationCurve _dodgeSpeedCurve;
    float _timeSinceLastDodge = 0;
    [SerializeField] string _dodgeLayerName;

    private Animator _anim;


    // Start is called before the first frame update
    void Start()
    {
        Physics.gravity = Vector3.down * 20f;
        _rb = GetComponent<Rigidbody>();
        _anim = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        float xMov = Input.GetAxis("Horizontal");
        float zMov = Input.GetAxis("Vertical");

        if (_canMove)
        {
            _moveDirection = new Vector3(xMov, 0.0f, zMov).normalized;
        }

        transform.rotation = Quaternion.LookRotation(_moveDirection);
        _timeSinceLastJump += Time.deltaTime;
        _timeSinceLastDodge += Time.deltaTime;

        CheckGrounded();
        if (Input.GetKeyDown(KeyCode.Space) && _timeSinceLastJump >= _jumpInputCooldown && _currentJumpIndex < _maxJumps - 1)
        {
            _anim.SetTrigger("Jump");
            Debug.Log("Current jump : " + _currentJumpIndex);
            _rb.Sleep();
            _rb.WakeUp();
            _rb.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse);
            _currentJumpIndex++;
        }
        if (Input.GetButtonDown("Fire2") && _timeSinceLastDodge >= _dodgeCooldown) StartCoroutine(Dodge());
        //Debug.Log(_grounded);
    }

    private void CheckGrounded()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, _raycastDistance))
        {
            //Debug.Log(hit.collider.name);
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
        if (_moveDirection.magnitude > 0 && _canMove)
        {
            _anim.SetBool("Runing", true);
            if (_grounded) _rb.MovePosition(transform.position + _speed * Time.deltaTime * _moveDirection);
            else _rb.MovePosition(transform.position + _airSpeed * Time.deltaTime * _moveDirection);
        }
        else _anim.SetBool("Runing", false);
    }

    IEnumerator Dodge()
    {
        _canMove = false;
        _timeSinceLastDodge = 0;
        float startTime = Time.time;
        _rb.useGravity = false;
        _currentJumpIndex--;

        while (Time.time < startTime + _dodgeDuration)
        {
            _rb.MovePosition(transform.position + (_dodgeSpeedCurve.Evaluate(Time.time - startTime) / _dodgeDuration) * _dodgeSpeed * Time.deltaTime * _moveDirection);
            yield return null;
        }

        _rb.useGravity = true;
        _canMove = true;

    }
}
