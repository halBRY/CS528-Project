/************************************************************************************************
Adapted from NavModeUI
*************************************************************************************************/
 
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class constSetUI : MonoBehaviour {

    public Toggle walkButton;
    public Toggle driveButton;
    public Toggle freeflyButton;

    public CAVE2WandNavigator navController;

    // Use this for initialization
    void Start () {
        navController = GetComponentInParent<CAVE2WandNavigator>();

        if(navController.navMode == CAVE2WandNavigator.NavigationMode.Walk)
        {
            walkButton.isOn = true;
        }
        else if (navController.navMode == CAVE2WandNavigator.NavigationMode.Drive)
        {
            driveButton.isOn = true;
        }
        else if (navController.navMode == CAVE2WandNavigator.NavigationMode.Freefly)
        {
            freeflyButton.isOn = true;
        }
    }

    void Update()
    {
        //UpdateButtons();
    }

    public void UpdateNavButtons()
    {
        walkButton.SetIsOnWithoutNotify(false);
        driveButton.SetIsOnWithoutNotify(false);
        freeflyButton.SetIsOnWithoutNotify(false);

        switch (navController.navMode)
        {
            case (CAVE2WandNavigator.NavigationMode.Walk):
                walkButton.SetIsOnWithoutNotify(true);
                break;
            case (CAVE2WandNavigator.NavigationMode.Drive):
                driveButton.SetIsOnWithoutNotify(true);
                break;
            case (CAVE2WandNavigator.NavigationMode.Freefly):
                freeflyButton.SetIsOnWithoutNotify(true);
                break;
        }
    }
}
