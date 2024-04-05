using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AppManager : MonoBehaviour
{
    public GameObject myPlayer;
    public GameObject myHeadTrack;
    public DrawLinksAsMesh myConstellations;
    public PointCloudCustomVertData myStars;

    public GameObject StaticGUI;
    public GameObject TimeGUI;
    public GameObject ScaleGUI;
    public GameObject DistGUI;

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

    public Material darkSky;
    public Material highlightSky;

    public TextAsset[] constSets;

    private int actionMode = 0;
    private int lastTimeDirection = 0;

    public int highlightID = 0;

    private bool isPaused = true;
    private bool activeColorScale = true;

    // Start is called before the first frame update
    void Start()
    {
        timeText = TimeGUI.GetComponent<TMP_Text>();
        timeString = "Present";

        scaleText = ScaleGUI.GetComponent<TMP_Text>();
        scaleText.text = "1 ft. = 1 parsec";

        distText = DistGUI.GetComponent<TMP_Text>();
        distString = " ft. away from Sol";
    }

    // Update is called once per frame
    void Update()
    {
        distance = Vector3.Distance(myHeadTrack.transform.position, new Vector3(0f, 1f, 0f)); //Sol location
        distText.text = ((distance * myStars.getScaleFactor()) / 10) + distString;

        switch(actionMode)
        {
            case 0:
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
                break;

            case 1:
                float myScale = myStars.getScaleFactor();
                float adjustedScale = (myScale/0.30478512648f) / 10;
                scaleText.text = adjustedScale + " ft. = 1 parsec";

                break;
            default:
                break;
        }

        //cave button right
        // 0 - forward in time
        // 1 - increase scale
        // 2 - cycle through constellations
        // 3 - toggle GUI elements
        if(Input.GetKeyDown(KeyCode.X))
        {
            switch (actionMode)
            {
                case 0:
                    myStars.updateFrameNumber(0);
                    myConstellations.updateFrameNumber(0);
                    lastTimeDirection = 0;
                    break;

                case 1: 
                    myStars.updateScaleFactor(0);
                    myConstellations.updateScaleFactor(0);
                    break;

                case 2:
                    UpdateHighlightID(0);
                    Debug.Log("Highlight number " + highlightID);
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
        {
            switch (actionMode)
            {
                case 0:
                    myStars.updateFrameNumber(1);
                    myConstellations.updateFrameNumber(1);
                    lastTimeDirection = 1;
                    break;

                case 1: 
                    myStars.updateScaleFactor(1);
                    myConstellations.updateScaleFactor(1);
                    break;

                case 2:
                    UpdateHighlightID(1);
                    Debug.Log("Highlight number " + highlightID);
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
        // 2 - cycle through constellations
        // 3 - reset position
        if(Input.GetKeyDown(KeyCode.C))
        {
            switch (actionMode)
            {
                case 0:
                    isPaused = false;
                    break;

                case 1: 
                    Debug.Log("Change GUI element for distance from Sol");
                    break;

                case 2:
                    UpdateHighlightID(1);
                    Debug.Log("Highlight number " + highlightID);
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
        // 1 - ...???
        // 2 - cycle through constellations
        // 3 - update color scale
        if(Input.GetKeyDown(KeyCode.V))
        {
            switch (actionMode)
            {
                case 0:
                    isPaused = true;
                    break;

                case 1: 
                    Debug.Log("Change GUI element for distance from Sol");
                    break;

                case 2:
                    UpdateHighlightID(1);
                    Debug.Log("Highlight number " + highlightID);
                    break;

                case 3:
                    myStars.updateColorScale();
                    UpdateColorGUI();
                    break;
                
                default:
                    break;
            }
        }

        if(!isPaused)
        {
            myStars.updateFrameNumber(lastTimeDirection);
            myConstellations.updateFrameNumber(lastTimeDirection);
        }

        //Highlight the chosen constellation
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            myConstellations.showHighlight(59);
        }

        //Deselect constellation
        if(Input.GetKeyDown(KeyCode.Alpha2))
        {
            myConstellations.hideHighlight();
        }

        if(distance < 30)
        {
            RenderSettings.skybox = darkSky;
        }
        else
        {
            RenderSettings.skybox = darkSky;
        }
    }

    public void UpdateHighlightID(int direction)
    {
        int maxID = myConstellations.constellationSubMeshIds.Length;
        
        if(direction == 0)
        {
            highlightID++; 
        }
        else
        {
            highlightID--;
        }

        if(highlightID >= maxID)
        {
            highlightID = 0;
        }
        else if(highlightID < 0)
        {
            highlightID = maxID;
        }
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
        myConstellations.clearMeshData();
        myConstellations.DrawConstellations(constSets[0].name);
    }

    public void UpdateConstSetKorean(bool toggle)
    {
        myConstellations.clearMeshData();
        myConstellations.DrawConstellations(constSets[1].name);
    }

    public void UpdateConstSetArabic(bool toggle)
    {
        myConstellations.clearMeshData();
        myConstellations.DrawConstellations(constSets[2].name);
    }

    public void UpdateConstSetChinese(bool toggle)
    {
        myConstellations.clearMeshData();
        myConstellations.DrawConstellations(constSets[3].name);
    }

    public void UpdateConstSetRomanian(bool toggle)
    {
        myConstellations.clearMeshData();
        myConstellations.DrawConstellations(constSets[4].name);
    }

    public void UpdateConstSetSami(bool toggle)
    {
        myConstellations.clearMeshData();
        myConstellations.DrawConstellations(constSets[5].name);
    }

    public void UpdateConstSetXhosa(bool toggle)
    {
        myConstellations.clearMeshData();
        myConstellations.DrawConstellations(constSets[6].name);
    }

    public void UpdateConstSetZulu(bool toggle)
    {
        myConstellations.clearMeshData();
        myConstellations.DrawConstellations(constSets[7].name);
    }

    public void UpdateActionTime(bool toggle)
    {
        actionMode = 0;
    }

    public void UpdateActionScale(bool toggle)
    {
        actionMode = 1;
        isPaused = true;
    }

    public void UpdateActionHighlight(bool toggle)
    {
        actionMode = 2;
        isPaused = true;
    }

    public void UpdateActionQuick(bool toggle)
    {
        actionMode = 3;
        isPaused = true;
    }

    public void ToggleAllGUI()
    {
        TimeGUI.SetActive(!TimeGUI.activeSelf);
        ScaleGUI.SetActive(!ScaleGUI.activeSelf);
        DistGUI.SetActive(!DistGUI.activeSelf);
        ColorGUI.SetActive(!ColorGUI.activeSelf);
    }
}
