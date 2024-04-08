using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpotlightManager : MonoBehaviour
{
    public GameObject[] myTextPanels;
    public TextUpdateSpotlight myTitle;

    public void UpdateText(int setID)
    {
        Debug.Log(setID);
        for(int i = 0; i < 6; i++)
        {
            myTextPanels[i].SetActive(false);
        }

        myTextPanels[setID].SetActive(true);
        myTitle.UpdateText();
    }
}
