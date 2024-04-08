using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AppManager : MonoBehaviour
{
    public GameObject myPlayer;
    public DrawLinksAsMesh myConstellations;
    public PointCloudCustomVertData myStars;
    public SpotlightManager spotlight;

    public GameObject menu;

    public GameObject StaticGUI;
    public GameObject TimeGUI;
    public GameObject ScaleGUI;
    public GameObject DistGUI;
    public GameObject HighlightGUI;
    public GameObject highlight;

    public GameObject SpotlightGUI;

    public TextUpdate numberText;
    public TextUpdate nameText;
    public TextUpdate nameTextOther;

    public TextUpdateSpotlight dipperText;
    public TextUpdateSpotlight setText;

    public GameObject ColorGUI;
    public GameObject SpectGUI;
    public GameObject ExoGUI;

    private TMP_Text timeText;
    private TMP_Text scaleText;
    private TMP_Text distText;

    private string timeString;
    private string scaleString;
    private string distString;

    public float distance;
    private float scaledDistance;

    public Material darkSky;
    public Material highlightSky;

    public TextAsset[] constSets;
    public TextAsset[] constNames;

    public Vector3 dipperPos = new Vector3(-1.05f, 0.5668f, 0.1284f);
    public Vector3 dipperRot = new Vector3(-57.56f, -80.049f, -14.351f);

    private int actionMode = 0;
    private int lastTimeDirection = 0;
    private int tempActionMode = 0;

    public int highlightID = 0;
    public int constSetID = 0;

    public int[] dipperIDs;
    public string[] setNames;

    private bool isPaused = true;
    private bool activeColorScale = true;
    private bool distanceIsFeet = true;
    private bool isSpotlight = false;

    // Start is called before the first frame update
    void Start()
    {
        timeText = TimeGUI.GetComponent<TMP_Text>();
        timeString = "Present";

        scaleText = ScaleGUI.GetComponent<TMP_Text>();
        scaleText.text = "1 ft. = 1 parsec";

        distText = DistGUI.GetComponent<TMP_Text>();
        distString = " ft. from Sol";

        //Show skycultures with big dipper
        dipperIDs = new int[6] {38, 218, 2, 38, 0, 1};
        setNames = new string[6] {"Modern", "Korean", "Indigenous Arabic", "Traditional Chinese", "Romanian", "Sami"};
    }

    // Update is called once per frame
    void Update()
    {
        distance = Vector3.Distance(myPlayer.transform.position, new Vector3(0f, 1f, 0f)); //Sol location

        if(distanceIsFeet)
        {
            scaledDistance = distance * 0.30479999024f;
        }
        else
        {
            scaledDistance = (distance * 0.30479999024f) * myStars.getLiveScale();
        }

        distText.text = string.Format("{0:#.00}{1}", scaledDistance, distString);

        float myFrame = myStars.getFrameNumber();
    
        if(myFrame == 0)
        {
            timeString = "Present";
        }
        else if(myFrame > 0)
        {
            timeString = (int) myFrame + " years ahead";
        }
        else if(myFrame < 0)
        {
            timeString = Mathf.Abs( (int) myFrame) + " years ago";
        }

        timeText.text = timeString;

        float myScale = myStars.getLiveScale();
        scaleText.text = string.Format("1ft. = {0:#.00} pc", myScale);
        //scaleText.text = "1 ft. = " + myScale + " parsec";

        if(Input.GetKeyDown(KeyCode.M))
        //if(CAVE2.GetButtonDown(CAVE2.Button.Button5))
        {
            //Debug.Log("Setting mode to 3, holding mode " + actionMode);
            tempActionMode = actionMode;
            actionMode = 3;
        }

        
        if(Input.GetKeyUp(KeyCode.M))
        //if(CAVE2.GetButtonUp(CAVE2.Button.Button5))
        {
            //Debug.Log("Setting action mode back to " + tempActionMode);
            actionMode = tempActionMode;
        }

        //cave button right
        // 0 - forward in time
        // 1 - increase scale
        // 2 - cycle through constellations
        // 3 - toggle GUI elements
        if(Input.GetKeyDown(KeyCode.X))
        //if(CAVE2.GetButtonDown(CAVE2.Button.ButtonRight))
        {
            switch (actionMode)
            {
                case 0:
                    myStars.updateFrameNumber(0);
                    myConstellations.updateFrameNumber(0);
                    lastTimeDirection = 0;
                    break;

                case 1: 
                    myStars.updateLiveScale(0);
                    myConstellations.updateLiveScale(0);
                    break;

                case 2:
                    if(!isSpotlight)
                    {
                        UpdateHighlightID(0);
                        numberText.UpdateText();
                        nameText.UpdateText();
                        nameTextOther.UpdateText();
                    }
                    //Debug.Log("Highlight number " + highlightID);
                    break;

                case 3:
                    ToggleAllGUI();
                    break;

                default:
                    break;
            }
        }

        //cave button left
        // 0 - backwards in time
        // 1 - decrease scale
        // 2 - cycle through constellations
        // 3 - toggle constellation lines elements
        if(Input.GetKeyDown(KeyCode.Z))
        //if(CAVE2.GetButtonDown(CAVE2.Button.ButtonLeft))
        {
            switch (actionMode)
            {
                case 0:
                    myStars.updateFrameNumber(1);
                    myConstellations.updateFrameNumber(1);
                    lastTimeDirection = 1;
                    break;

                case 1: 
                    myStars.updateLiveScale(1);
                    myConstellations.updateLiveScale(1);
                    break;

                case 2:
                    if(!isSpotlight)
                    {
                        UpdateHighlightID(1);
                        numberText.UpdateText();
                        nameText.UpdateText();
                        nameTextOther.UpdateText();
                    }
                    //Debug.Log("Highlight number " + highlightID);
                    break;

                case 3:
                    myConstellations.hideLines();
                    break;
                
                default:
                    break;
            }
        }

        //cave button up
        // 0 - play
        // 1 - change distance from sol units
        // 2 - if spotlighting, cycle through skycultures
        // 3 - reset position
        if(Input.GetKeyDown(KeyCode.C))
        //if(CAVE2.GetButtonDown(CAVE2.Button.ButtonUp))
        {
            switch (actionMode)
            {
                case 0:
                    isPaused = false;
                    break;

                case 1: 
                    distanceIsFeet = !distanceIsFeet;
                    if(distanceIsFeet)
                    {
                        distString = " ft. from Sol";
                    }
                    else
                    {
                        distString = " pc. from Sol";
                    }
                    break;

                case 2:
                    UpdateConstID();
                    dipperText.UpdateText();
                    setText.UpdateText();
                    spotlight.UpdateText(constSetID);
                    //Debug.Log("Highlight number " + highlightID);
                    break;

                case 3:
                    myPlayer.transform.position = new Vector3(0f, 0.52f, -6f);
                    myPlayer.transform.rotation = Quaternion.identity;
                    break;
                
                default:
                    break;
            }
        }

        //cave button down
        // 0 - play
        // 1 - reset scale to default
        // 2 - if spotlighting, jump to pos
        // 3 - update color scale
        if(Input.GetKeyDown(KeyCode.V))
        //if(CAVE2.GetButtonDown(CAVE2.Button.ButtonDown))
        {
            switch (actionMode)
            {
                case 0:
                    isPaused = true;
                    break;

                case 1: 
                    myStars.ResetLiveScale();
                    myConstellations.ResetLiveScale();
                    break;

                case 2:
                    //UpdateHighlightID(1);
                    //Debug.Log("Highlight number " + highlightID);
                    if(isSpotlight)
                    {
                        myPlayer.transform.position = dipperPos;
                        myPlayer.transform.eulerAngles = dipperRot;
                    }
                    break;

                case 3:
                    myStars.updateColorScale();
                    UpdateColorGUI();
                    break;
                
                default:
                    break;
            }
        }
        
        if(isSpotlight)
        {
            highlightID = dipperIDs[constSetID];
        }

        if(actionMode == 2)
        {
            numberText.UpdateText();
            nameText.UpdateText();
            nameTextOther.UpdateText();
            myConstellations.showHighlight(highlightID);
        }
        
        //Disable action buttons when menu is open
        if(menu.GetComponent<OMenuManager>().openMenus > 0)
        {
            isPaused = true;
        }

        if(!isPaused)
        {
            myStars.updateFrameNumber(lastTimeDirection);
            myConstellations.updateFrameNumber(lastTimeDirection);
        }

        /*
        if(distance < 30)
        {
            RenderSettings.skybox = darkSky;
        }
        else
        {
            RenderSettings.skybox = darkSky;
        }*/
    }

    public void UpdateHighlightID(int direction)
    {
        int maxID = myConstellations.constellationNames.Length;
        
        if(direction == 0)
        {
            highlightID++; 

            if(highlightID >= maxID)
            {
                highlightID = 0;
            }
        }
        else
        {
            highlightID--;
            if(highlightID < 0)
            {
                highlightID = maxID - 1;
            }
        }
    }

    public void UpdateConstID()
    {
        constSetID++; 

        if(constSetID > 5)
        {
            constSetID = 0;
        }

        myConstellations.clearMeshData();
        myConstellations.DrawConstellations(constSets[constSetID].name, constNames[constSetID].name);

    }

    public void UpdateColorGUI()
    {
        activeColorScale = !activeColorScale;
        if(activeColorScale)
        {
            ExoGUI.SetActive(false);
            SpectGUI.SetActive(true);
        }
        else
        {
            ExoGUI.SetActive(true);
            SpectGUI.SetActive(false);
        }
    }

    public void UpdateConstSetModern(bool toggle)
    {
        Debug.Log("Push");
        myConstellations.clearMeshData();
        myConstellations.DrawConstellations(constSets[0].name, constNames[0].name);
        constSetID = 0;
    }

    public void UpdateConstSetKorean(bool toggle)
    {
        myConstellations.clearMeshData();
        myConstellations.DrawConstellations(constSets[1].name, constNames[1].name);
        constSetID = 1;
    }

    public void UpdateConstSetArabic(bool toggle)
    {
        myConstellations.clearMeshData();
        myConstellations.DrawConstellations(constSets[2].name, constNames[2].name);
        constSetID = 2;
    }

    public void UpdateConstSetChinese(bool toggle)
    {
        myConstellations.clearMeshData();
        myConstellations.DrawConstellations(constSets[3].name, constNames[3].name);
        constSetID = 3;
    }

    public void UpdateConstSetRomanian(bool toggle)
    {
        myConstellations.clearMeshData();
        myConstellations.DrawConstellations(constSets[4].name, constNames[4].name);
        constSetID = 4;
    }

    public void UpdateConstSetSami(bool toggle)
    {
        myConstellations.clearMeshData();
        myConstellations.DrawConstellations(constSets[5].name, constNames[5].name);
        constSetID = 5;
    }

    public void UpdateConstSetXhosa(bool toggle)
    {
        myConstellations.clearMeshData();
        myConstellations.DrawConstellations(constSets[6].name, constNames[6].name);
        constSetID = 6;
    }

    public void UpdateConstSetZulu(bool toggle)
    {
        myConstellations.clearMeshData();
        myConstellations.DrawConstellations(constSets[7].name, constNames[7].name);
        constSetID = 7;
    }

    public void UpdateActionTime(bool toggle)
    {
        actionMode = 0;
        myConstellations.hideHighlight();
    }

    public void UpdateActionScale(bool toggle)
    {
        actionMode = 1;
        isPaused = true;
        myConstellations.hideHighlight();
    }

    public void UpdateActionHighlight(bool toggle)
    {
        actionMode = 2;
        isPaused = true;
        HighlightGUI.SetActive(true);
        highlight.SetActive(true);
    }

    public void UpdateActionQuick(bool toggle)
    {
        actionMode = 3;
        isPaused = true;
        myConstellations.hideHighlight();
    }

    public void constellationSpotlight(bool toggle)
    {
        isSpotlight = toggle;
        actionMode = 2;
        //Debug.Log(isSpotlight);
        HighlightGUI.SetActive(true);
        highlight.SetActive(false);
        SpotlightGUI.SetActive(true);
        spotlight.gameObject.SetActive(true);
        spotlight.UpdateText(0);
        constSetID = 0;
        myConstellations.clearMeshData();
        myConstellations.DrawConstellations(constSets[0].name, constNames[0].name);
    }

    public void ToggleAllGUI()
    {
        TimeGUI.SetActive(!TimeGUI.activeSelf);
        ScaleGUI.SetActive(!ScaleGUI.activeSelf);
        DistGUI.SetActive(!DistGUI.activeSelf);
        ColorGUI.SetActive(!ColorGUI.activeSelf);

        if(actionMode == 2 || tempActionMode == 2)
        {
            HighlightGUI.SetActive(!HighlightGUI.activeSelf);
            if(isSpotlight)
            {
                SpotlightGUI.SetActive(!SpotlightGUI);
                spotlight.gameObject.SetActive(!spotlight);
            }
        }
        else
        {
            HighlightGUI.SetActive(false);
            SpotlightGUI.SetActive(false);
        }
    }
}
