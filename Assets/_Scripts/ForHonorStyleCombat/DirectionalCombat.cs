using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;
using UnityEngine.UIElements;
using Unity.VisualScripting;

public class DirectionalCombat : MonoBehaviour
{
    [Header("Directional Input")]
    [SerializeField] Transform _inputPosSphere;


    [Header("Directional Output")]
    [SerializeField] Transform _outputPosSphere;
    private Vector3[] _possibleDirections = new Vector3[5];

    [Header("Values")]
    [SerializeField] float _inputVisualizationOffset = 0.7f;
    [SerializeField] float _outputVisualizationOffset = 0.4f;
    [SerializeField] float _outputVisualizationhhHeightOffset = 0.4f;

    [Header("Testing")]
    public float _timeToSwapPos = .5f;
    float _swapCounter = 0;
    private int _currentPositionIndex = -1;

    private void Awake()
    {
    }

    private void SetPossibleOutputDirection()
    {
        _possibleDirections[0] = (-transform.right + Vector3.up *_outputVisualizationhhHeightOffset).normalized;
        _possibleDirections[1] = (-transform.right + transform.up).normalized;
        _possibleDirections[2] = transform.up;
        _possibleDirections[3] = (transform.right + transform.up).normalized;
        _possibleDirections[4] = (transform.right + Vector3.up * _outputVisualizationhhHeightOffset).normalized;
    }

    private void Update()
    {
        SetPossibleOutputDirection();
        CalculateInputPosition();
        //_currentPositionIndex = 0;
        _swapCounter += Time.deltaTime;

        //if (_swapCounter >= _timeToSwapPos) SwapOutputPositionTest();
        
    }

    private void SwapOutputPosition()
    {
        _outputPosSphere.localPosition = _possibleDirections[_currentPositionIndex] * _outputVisualizationOffset;
    }

    private void LateUpdate()
    {
        GetClosestDirectionToInput();
        SwapOutputPosition();
    }

    private void GetClosestDirectionToInput()
    {
        float[] distances = _possibleDirections.Select(p => Vector3.Distance(p, _inputPosSphere.position)).ToArray();
        float closestDistance = distances.Min();
        _currentPositionIndex = distances.ToList().IndexOf(closestDistance);
    }

    private void CalculateInputPosition()
    {
        Vector3 screenPoint = Input.mousePosition;
        Vector3 mouseViewPortPos = Camera.main.ScreenToViewportPoint(screenPoint);
        Vector3 mouseNormalizedPos = mouseViewPortPos - new Vector3(0.5f, 0.5f, 0f);

        // Step 4: Get the position relative to the camera's viewport size
        Vector3 mouseCameraPos = new Vector3(
            mouseNormalizedPos.x * Camera.main.pixelWidth,
            mouseNormalizedPos.y * Camera.main.pixelHeight,
            0f
        ).normalized;

        _inputPosSphere.position = transform.position + mouseCameraPos * _inputVisualizationOffset;
    }

    private void SwapOutputPositionTest()
    {
        _swapCounter = 0;
        _currentPositionIndex++;
        _outputPosSphere.localPosition = _possibleDirections[_currentPositionIndex] * _outputVisualizationOffset;
        if (_currentPositionIndex >= _possibleDirections.Length - 1) _currentPositionIndex = -1;
    }
}
