using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using System;

public class PlayerController : MonoBehaviour
{
    //movement
    [SerializeField] private float speed = 3.3f;
    private float rotSpeed = 90f;
    private Vector3 moveDirection = Vector3.zero;
    [SerializeField] private CharacterController controller;

    //rotation
    private float _initialYAngle = 0f;
    private float _appliedGyroAngle = 0f;
    private float _calibrationYAngle = 0f;

    private Transform _rawGyroRotation;
    private float _tempSmoothing;


    //settings
    private float _smoothing = 0.1f;


    public string dataSaved;

    private void Update()
    {
        //movement
        Vector3 move = new Vector3(Input.acceleration.x * speed * Time.deltaTime,0,-Input.acceleration.z * speed * Time.deltaTime);
        Vector3 rotMovement = transform.TransformDirection(move);
        controller.Move(rotMovement);

        //rotation
        ApplyGyroRotation();
        ApplyCalibration();

        transform.rotation = Quaternion.Slerp(transform.rotation,_rawGyroRotation.rotation,_smoothing);
    }

    private IEnumerator Start()
    {
        Input.gyro.enabled = true;
        Application.targetFrameRate= 60;
        _initialYAngle = transform.eulerAngles.y;
        _rawGyroRotation = new GameObject("GyroRaw").transform;
        _rawGyroRotation.position= transform.position;
        _rawGyroRotation.rotation= transform.rotation;

        yield return new WaitForSeconds(1);

        StartCoroutine(CalibrateYAngle());
    }

    private IEnumerator CalibrateYAngle()
    {
        _tempSmoothing = _smoothing;
        _smoothing= 1f;
        _calibrationYAngle = _appliedGyroAngle - _initialYAngle;

        yield return null;
        _smoothing = _tempSmoothing;
    }
    private void ApplyGyroRotation()
    {
        _rawGyroRotation.rotation = Input.gyro.attitude;
        _rawGyroRotation.Rotate(0f, 0f, 180f, Space.Self);
        _rawGyroRotation.Rotate(90f, 180f, 0f, Space.World);
        _appliedGyroAngle = _rawGyroRotation.eulerAngles.y;

    }
    private void ApplyCalibration()
    {
        _rawGyroRotation.Rotate(0f, -_calibrationYAngle, 0f, Space.World);

    }
    public void SetEnable(bool value)
    {
        enabled = true;
        StartCoroutine(CalibrateYAngle());
    }

    public void SaveData()
    {
        Directory.CreateDirectory(Application.streamingAssetsPath + "/DataLog/");
        string TxtName = Application.streamingAssetsPath + "/DataLog/" + "Data" + ".txt";
        

        if (!File.Exists(TxtName))
        {
            File.WriteAllText(TxtName, dataSaved);
        }
    }

    private void Awake()
    {
        Time.fixedDeltaTime = 1 / 10;
    }

    private void FixedUpdate()
    {
        dataSaved = dataSaved + "\nTime: " + System.DateTime.Now.ToString() + " | Position: " + transform.position.ToString() + " |";
    }
}

