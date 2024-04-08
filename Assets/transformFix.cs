using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class transformFix : MonoBehaviour
{
    public GameObject myPlayer;

    public Vector3 newRot;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       gameObject.transform.position = myPlayer.transform.position;
       newRot = myPlayer.transform.rotation.eulerAngles;
       newRot.y = gameObject.transform.eulerAngles.y;
       gameObject.transform.eulerAngles = newRot;
    }
}
