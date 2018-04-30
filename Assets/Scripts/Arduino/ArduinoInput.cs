using System;
using System.IO.Ports;
using UnityEngine;
using UnityEngine.UI;

public enum Direction {
    Right,
    Left,
    Front
}

public class ArduinoInput : MonoBehaviour {

    public SerialPort serialPort = new SerialPort("COM3", 9600);
    public static Action<Direction> OnMotionDetected;

    void Start() { 
        serialPort.Open();
        serialPort.ReadTimeout = 50;
    }

    void Update() {
        if (serialPort.IsOpen)
        {
            try
            {
                //Debug.Log("Port Open");
            }
            catch (System.Exception)
            {
                Debug.Log("Can't Find port");
            }
        }
        string arduinoValue = serialPort.ReadLine();

        if (arduinoValue == "0") {
            Debug.Log("No Motion");
        } else if (arduinoValue == "1") {
            Debug.Log("Right Motion");
            if (OnMotionDetected != null)
                OnMotionDetected(Direction.Right);
        } else if (arduinoValue == "2") {
            Debug.Log("Left Motion");
            if (OnMotionDetected != null)
                OnMotionDetected(Direction.Left);
        } else if (arduinoValue == "3") {
            Debug.Log("Front Motion");
            if (OnMotionDetected != null)
                OnMotionDetected(Direction.Front);
        }
    }
}
