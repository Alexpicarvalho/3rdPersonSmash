using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenjiBallBall : MonoBehaviour
{
    [SerializeField] float _startSpeed;
    [SerializeField] float _rotationSmootheness;
    private float _currentSpeed;

    [SerializeField] Transform _chaseTarget;
    void Start()
    {
        _currentSpeed = _startSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.LookRotation((_chaseTarget.position - transform.position).normalized);
        transform.position += transform.forward * _currentSpeed * Time.deltaTime;
    }
}
