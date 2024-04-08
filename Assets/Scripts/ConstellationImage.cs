/*******************************************
Written by Hal Brynteson for CS 528 
*******************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstellationImage : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        transform.LookAt(new Vector3(0,1,0));
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(new Vector3(0,1,0));
    }
}
