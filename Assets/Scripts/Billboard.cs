/*******************************************
Written by Hal Brynteson for CS 528 
*******************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    void Update()
    {
        transform.rotation = Quaternion.Euler(-90f, Camera.main.transform.rotation.eulerAngles.y, 0f);
    }
}
