using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppManager : MonoBehaviour
{
    public GameObject myPlayer;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Return))
        {
            myPlayer.transform.position = new Vector3(0f, 0.52f, -6f);
            myPlayer.transform.rotation = Quaternion.identity;
        }
    }
}
