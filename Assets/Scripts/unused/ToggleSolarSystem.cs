using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleSolarSystem : MonoBehaviour
{

    public GameObject models; 

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            models.SetActive(!models.activeSelf);
        }

        if (CAVE2.GetButtonDown(CAVE2.Button.ButtonUp))
        {
            models.SetActive(!models.activeSelf);
        }
    }
}
