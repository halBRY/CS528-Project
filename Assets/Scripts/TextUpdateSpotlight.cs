using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextUpdateSpotlight : MonoBehaviour
{
    public int myID;
    public AppManager appManager;
    public DrawLinksAsMesh myConstellations;

    // Start is called before the first frame update
    void Start()
    {
        if(myID == 0)
        {
            gameObject.GetComponent<TextMesh>().text = myConstellations.constellationNames[appManager.dipperIDs[appManager.constSetID]];
        }
        else if(myID == 1)
        {
            gameObject.GetComponent<TextMesh>().text = appManager.setNames[appManager.constSetID];
        }
    }

    // Update is called once per frame
    public void UpdateText()
    {
        if(myID == 0)
        {
            gameObject.GetComponent<TextMesh>().text = myConstellations.constellationNames[appManager.dipperIDs[appManager.constSetID]];
        }
        else if(myID == 1)
        {
            gameObject.GetComponent<TextMesh>().text = appManager.setNames[appManager.constSetID];
        }
    }
}
