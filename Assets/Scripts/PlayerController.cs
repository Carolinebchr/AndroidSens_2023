using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using System;

public class PlayerController : MonoBehaviour
{
    //Script was made by following Lurple Tutorial on "Unity - how to make a player controller with gyroscope and accelerometer"

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

    private int filenum = 0;

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
<<<<<<< Updated upstream
=======

        dataSaved = dataSaved + "\nTime: ;" + System.DateTime.Now.ToString() + " ; Acceleration: ;" + Input.acceleration.x.ToString();
>>>>>>> Stashed changes
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

    /// <summary>
    /// save the string to a file in Data folder under streamingAsset Folder 
    /// </summary>
    public void SaveData()
    {
        Directory.CreateDirectory(Application.streamingAssetsPath + "/DataLog/");
        string TxtName = Application.streamingAssetsPath + "/DataLog/" + "Data" + "("+ filenum.ToString() + ")" + ".csv";

        
        if (!File.Exists(TxtName))
        {
            File.WriteAllText(TxtName, dataSaved);
        }
        else
        {
            filenum++;
            TxtName = Application.streamingAssetsPath + "/DataLog/" + "Data" + "(" + filenum.ToString() + ")" + ".csv";
            File.WriteAllText(TxtName, dataSaved);
        }
    }

    private void FixedUpdate()
    {
        //Every 1s the dataSaved string is updated to hold the new values as well as the old values as a string 
        //this is for when we call the SavedData() the data.txt will contain from the start up until you call the function.
        dataSaved = dataSaved + "Time: " + System.DateTime.Now.ToString() + " | \n Rotation: " + _rawGyroRotation.rotation.ToString() + "| \n Postition: " + transform.position.ToString() + " |\n\n\n";
    }

}




