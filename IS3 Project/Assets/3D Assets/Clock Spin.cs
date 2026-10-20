using System;
using UnityEngine;
using UnityEngine.Animations;

public class ClockSpin : MonoBehaviour
{
    public enum Hand {Hour, Minute}
    [SerializeField] Hand hand;
    float multiplier = 60; [SerializeField] float GameTimeConstant = 60;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        /*if (hand == Hand.Hour)
        {
            //edit GameTimeConstant with GameManager.GameTimeConstant or something when that happens
            multiplier = 5/60/60 *GameTimeConstant;
        }

        else { multiplier = 1/60 *GameTimeConstant;}*/
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate (0,multiplier*Time.deltaTime,0);
    }
}
