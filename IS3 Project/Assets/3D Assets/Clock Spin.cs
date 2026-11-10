using System;
using UnityEngine;
using UnityEngine.Animations;

public class ClockSpin : MonoBehaviour
{
    public enum Hand { Hour, Minute }
    [SerializeField] Hand hand;
    float multiplier = 60; [SerializeField] float GameTimeConstant = 60f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (hand == Hand.Hour)
        {
            //edit GameTimeConstant with GameManager.GameTimeConstant or something when that happens
            multiplier = 0.5f * GameTimeConstant; // (360/12 = 30deg)/min
        }

        else { multiplier = 6f * GameTimeConstant; } // 360deg/min

    }

    // Update is called once per frame
    void Update()
    {
        float dpm;
        transform.Rotate(0f, 0f, multiplier * Time.deltaTime, Space.Self);
    }
}
