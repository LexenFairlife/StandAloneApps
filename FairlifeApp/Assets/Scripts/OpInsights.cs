using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Data.SqlClient;
using System.Data;

/// <summary>
/// Developed by Lexen Sterenberg
/// Updated by Asad Sheikh in December 2020
/// 
/// Source repository: https://github.com/fairlife-code/FairlifeApp
/// </summary>
public class OpInsights : MonoBehaviour
{
    #region Class Variables
    
    [System.Serializable] public class Hour {
        public string hourText, product;
        public DateTime hour, hourUTC;
        public int cases, eqCases, scrap, dtMinutes, accMinutes, baseID, materialID;
        public float oem;
    }
    
    [System.Serializable] public class DowntimeEntry {
        public string display;
        public DateTime hourUTC, hourLocal;
        public int dtMinutes;
        public string initials, level1, level2, level3, level4, level5, opNotes, engNotes;
    }
    
    [System.Serializable] public class ReasonCode {
        public string display;
        public int ID;
        public string level1, level2, level3, level4, level5, notes;
    }
    
    public string site = "CPS", equipment = "Filler";
    public int line;
    public int sectionID = -2;
    public bool debug = true;
    List <Hour> hours = new List <Hour> ();
    List <DowntimeEntry> downtimeEntries = new List <DowntimeEntry> ();
    public List <ReasonCode> reasonCodes = new List <ReasonCode> ();
    float reasonDowmloadTimer = 3600;
    int hourID;
    float hourDownloadTimer = 60;
    public int entry;
    /// <summary> Level of the downtime description </summary>
    string level1 = "", level2 = "", level3 = "", level4 = "", level5 = "";
    public List <int> reasonCodeSQLIDs = new List <int> ();
    public int reasonCodeSQLID;
    int reasonCodeID = 0;
    string userInitials = "";

    GameObject buttonContainer;
    public GameObject loadingBlocker, uploadingIcon;
    public Text title;
    /// <summary> Number of minutes accounted for by downtime entries operators have made for the selected hour </summary>
    public Text accountedDowntime;
    public Text baseText, materialText;
    float accountedDowntimeFloat = 0;
    int unaccountedForMinutes = 0;
    public Text buttonQuestion, shiftEffeciency;
    public List<RectTransform> buttons;
    List<Text> buttonTexts;
    public RectTransform saveButton, cancelButton, backButton;
    //public RectTransform FGIDOverride;
    public GameObject copyLastHourButton;
    //public InputField FGIDInput, bottleVolInput, BPCInput, fgNotesInput;
    //public Text validationMsg;
    public List<Vector2> buttonPoses = new List<Vector2> { };
    int activeButtons = 0;

    public List<GameObject> entryButtons;
    List<Text> entryTopLineTexts;//, entryBottomLineTexts;
    List<InputField> entryOpNotes, entryEngNotes;
    List<InputField> entryMinutesText;
    /// <summary> Stores the machine's current out put in units per minute</summary>
    public List<Text> hourReportStatusText;
    /// <summary> Stores the machine's current output in units per hour</summary>
    public List<Text> hourReportGphText;
    public Text shiftHoursAndProd, shiftCases, shiftEq, shiftScrap, shiftEf;
    public Text pastEntriesHours, pastEntriesDT, pastEntriesOp, pastEntriesReasons;
    public InputField notes, minutes, initials, opComsSend;
    public Text initialsButton, lineButtonText, selectedHourText, opComsRecieve;
    public Button selectedHourButton, refreshButton;
    public RectTransform selectedHourRect;
    /// <summary>Oldest hour with unaccounted for downtime within a 25-hour span of the current time (for the selected site/line)</summary>
    DateTime lastUnaccountedForHour;
    int lastUnaccountedForHourID;
    /// <summary>User-specified hour. Can be the same time or prior to lastUnaccountedHour</summary>
    DateTime selectedHour;

    int dtMinutes = 0, accountedDtMinutes = 0;
    int baseID, materialID, packSize;
    float bottleSize; string ml;
    List<int> materialIDs = new List<int>();
    List<string> materialDescriptions = new List<string>();
    List<string> opsComsListA = new List<string>();
    List<string> opsComsListB = new List<string>();
    string opsComsString;
    List<float> bottleSizes = new List<float>();
    List<int> packSizes = new List<int>();
    public float opComsCountdown = 30;
    float inputTimer = 0;
    public float downtimeEntriesDownloadTimer = 60;

    bool firstStart = true;
    int i1, j1, j2, k2, level = 0, saveInt; //k1, saveInt2
    string query;
    SqlDataReader reader;
    private string connectionString = @"Data Source = tcp:172.16.5.217; user id = cipapp;password = bestinclass&1;Connection Timeout=10;";
    List<string> buttonString = new List<string> { "Line1", "Line2", "Line3", "Line6" };
    float saveDelay = 0;
    string[] splitString;

    DateTime currentTime;
    DateTime lastTime;
    float frameTime;
    bool fiveMinuteRunning;

    private int _runningCoroutines;
    public int runningCoroutines
    {
        get { return _runningCoroutines; }
        set
        {
            loadingBlocker.SetActive(value != 0);
            _runningCoroutines = value;
        }
    }

    #region buttonVectors
    Vector2 buttonPos1 = new Vector2(-85f, -30f);
    //Vector2 buttonSize1 = new Vector2(1740, 1010);
    Vector2 buttonPos1_9 = new Vector2(-520, 225);
    Vector2 buttonPos2_9 = new Vector2(-85, 225);
    Vector2 buttonPos3_9 = new Vector2(350, 225);
    Vector2 buttonPos4_9 = new Vector2(-520, -30);
    Vector2 buttonPos5_9 = new Vector2(-85, -30);
    Vector2 buttonPos6_9 = new Vector2(350, -30);
    Vector2 buttonPos7_9 = new Vector2(-520, -280);
    Vector2 buttonPos8_9 = new Vector2(-85, -280);
    Vector2 buttonPos9_9 = new Vector2(350, -280);
    Vector2 buttonSize9 = new Vector2(425, 225);

    Vector2 buttonPos01_16 = new Vector2(-750, 350);
    Vector2 buttonPos02_16 = new Vector2(-305, 350);
    Vector2 buttonPos03_16 = new Vector2(135, 350);
    Vector2 buttonPos04_16 = new Vector2(575, 350);
    Vector2 buttonPos05_16 = new Vector2(-750, 90);
    Vector2 buttonPos06_16 = new Vector2(-305, 90);
    Vector2 buttonPos07_16 = new Vector2(135, 90);
    Vector2 buttonPos08_16 = new Vector2(575, 90);
    Vector2 buttonPos09_16 = new Vector2(-750, -170);
    Vector2 buttonPos10_16 = new Vector2(-305, -170);
    Vector2 buttonPos11_16 = new Vector2(135, -170);
    Vector2 buttonPos12_16 = new Vector2(575, -170);
    Vector2 buttonPos13_16 = new Vector2(-750, -425);
    Vector2 buttonPos14_16 = new Vector2(-305, -425);
    Vector2 buttonPos15_16 = new Vector2(135, -425);
    Vector2 buttonPos16_16 = new Vector2(575, -425);
    Vector2 buttonSize16 = new Vector2(400, 200);

    Vector2 buttonPos01_25 = new Vector2(-780, 365);
    Vector2 buttonPos02_25 = new Vector2(-430, 365);
    Vector2 buttonPos03_25 = new Vector2(-80, 365);
    Vector2 buttonPos04_25 = new Vector2(270, 365);
    Vector2 buttonPos05_25 = new Vector2(620, 365);
    Vector2 buttonPos06_25 = new Vector2(-780, 165);
    Vector2 buttonPos07_25 = new Vector2(-430, 165);
    Vector2 buttonPos08_25 = new Vector2(-80, 165);
    Vector2 buttonPos09_25 = new Vector2(270, 165);
    Vector2 buttonPos10_25 = new Vector2(620, 165);
    Vector2 buttonPos11_25 = new Vector2(-780, -35);
    Vector2 buttonPos12_25 = new Vector2(-430, -35);
    Vector2 buttonPos13_25 = new Vector2(-80, -35);
    Vector2 buttonPos14_25 = new Vector2(270, -35);
    Vector2 buttonPos15_25 = new Vector2(620, -35);
    Vector2 buttonPos16_25 = new Vector2(-780, -235);
    Vector2 buttonPos17_25 = new Vector2(-430, -235);
    Vector2 buttonPos18_25 = new Vector2(-80, -235);
    Vector2 buttonPos19_25 = new Vector2(270, -235);
    Vector2 buttonPos20_25 = new Vector2(620, -235);
    Vector2 buttonPos21_25 = new Vector2(-780, -430);
    Vector2 buttonPos22_25 = new Vector2(-430, -430);
    Vector2 buttonPos23_25 = new Vector2(-80, -430);
    Vector2 buttonPos24_25 = new Vector2(270, -430);
    Vector2 buttonPos25_25 = new Vector2(620, -430);
    Vector2 buttonSize25 = new Vector2(325, 190);

    Vector2 buttonPos01_36 = new Vector2(-810, 380);
    Vector2 buttonPos02_36 = new Vector2(-519, 380);
    Vector2 buttonPos03_36 = new Vector2(-228, 380);
    Vector2 buttonPos04_36 = new Vector2(63, 380);
    Vector2 buttonPos05_36 = new Vector2(355, 380);
    Vector2 buttonPos06_36 = new Vector2(645, 380);
    Vector2 buttonPos07_36 = new Vector2(-810, 212);
    Vector2 buttonPos08_36 = new Vector2(-519, 212);
    Vector2 buttonPos09_36 = new Vector2(-228, 212);
    Vector2 buttonPos10_36 = new Vector2(63, 212);
    Vector2 buttonPos11_36 = new Vector2(355, 212);
    Vector2 buttonPos12_36 = new Vector2(645, 212);
    Vector2 buttonPos13_36 = new Vector2(-810, 46);
    Vector2 buttonPos14_36 = new Vector2(-519, 46);
    Vector2 buttonPos15_36 = new Vector2(-228, 46);
    Vector2 buttonPos16_36 = new Vector2(63, 46);
    Vector2 buttonPos17_36 = new Vector2(355, 46);
    Vector2 buttonPos18_36 = new Vector2(645, 46);
    Vector2 buttonPos19_36 = new Vector2(-810, -119);
    Vector2 buttonPos20_36 = new Vector2(-519, -119);
    Vector2 buttonPos21_36 = new Vector2(-228, -119);
    Vector2 buttonPos22_36 = new Vector2(63, -119);
    Vector2 buttonPos23_36 = new Vector2(355, -119);
    Vector2 buttonPos24_36 = new Vector2(645, -119);
    Vector2 buttonPos25_36 = new Vector2(-810, -284);
    Vector2 buttonPos26_36 = new Vector2(-519, -284);
    Vector2 buttonPos27_36 = new Vector2(-228, -284);
    Vector2 buttonPos28_36 = new Vector2(63, -284);
    Vector2 buttonPos29_36 = new Vector2(355, -284);
    Vector2 buttonPos30_36 = new Vector2(645, -284);
    Vector2 buttonPos31_36 = new Vector2(-810, -450);
    Vector2 buttonPos32_36 = new Vector2(-519, -450);
    Vector2 buttonPos33_36 = new Vector2(-228, -450);
    Vector2 buttonPos34_36 = new Vector2(63, -450);
    Vector2 buttonPos35_36 = new Vector2(355, -450);
    Vector2 buttonPos36_36 = new Vector2(645, -450);
    Vector2 buttonSize36 = new Vector2(275, 155);

    Vector2 buttonPos01_49 = new Vector2(-830, 395);
    Vector2 buttonPos02_49 = new Vector2(-580, 395);
    Vector2 buttonPos03_49 = new Vector2(-330, 395);
    Vector2 buttonPos04_49 = new Vector2(-80, 395);
    Vector2 buttonPos05_49 = new Vector2(170, 395);
    Vector2 buttonPos06_49 = new Vector2(420, 395);
    Vector2 buttonPos07_49 = new Vector2(670, 395);
    Vector2 buttonPos08_49 = new Vector2(-830, 255);
    Vector2 buttonPos09_49 = new Vector2(-580, 255);
    Vector2 buttonPos10_49 = new Vector2(-330, 255);
    Vector2 buttonPos11_49 = new Vector2(-80, 255);
    Vector2 buttonPos12_49 = new Vector2(170, 255);
    Vector2 buttonPos13_49 = new Vector2(420, 255);
    Vector2 buttonPos14_49 = new Vector2(670, 255);
    Vector2 buttonPos15_49 = new Vector2(-830, 111);
    Vector2 buttonPos16_49 = new Vector2(-580, 111);
    Vector2 buttonPos17_49 = new Vector2(-330, 111);
    Vector2 buttonPos18_49 = new Vector2(-80, 111);
    Vector2 buttonPos19_49 = new Vector2(170, 111);
    Vector2 buttonPos20_49 = new Vector2(420, 111);
    Vector2 buttonPos21_49 = new Vector2(670, 111);
    Vector2 buttonPos22_49 = new Vector2(-830, -33);
    Vector2 buttonPos23_49 = new Vector2(-580, -33);
    Vector2 buttonPos24_49 = new Vector2(-330, -33);
    Vector2 buttonPos25_49 = new Vector2(-80, -33);
    Vector2 buttonPos26_49 = new Vector2(170, -33);
    Vector2 buttonPos27_49 = new Vector2(420, -33);
    Vector2 buttonPos28_49 = new Vector2(670, -33);
    Vector2 buttonPos29_49 = new Vector2(-830, -176);
    Vector2 buttonPos30_49 = new Vector2(-580, -176);
    Vector2 buttonPos31_49 = new Vector2(-330, -176);
    Vector2 buttonPos32_49 = new Vector2(-80, -176);
    Vector2 buttonPos33_49 = new Vector2(170, -176);
    Vector2 buttonPos34_49 = new Vector2(420, -176);
    Vector2 buttonPos35_49 = new Vector2(670, -176);
    Vector2 buttonPos36_49 = new Vector2(-830, -320);
    Vector2 buttonPos37_49 = new Vector2(-580, -320);
    Vector2 buttonPos38_49 = new Vector2(-330, -320);
    Vector2 buttonPos39_49 = new Vector2(-80, -320);
    Vector2 buttonPos40_49 = new Vector2(170, -320);
    Vector2 buttonPos41_49 = new Vector2(420, -320);
    Vector2 buttonPos42_49 = new Vector2(670, -320);
    Vector2 buttonPos43_49 = new Vector2(-830, -465);
    Vector2 buttonPos44_49 = new Vector2(-580, -465);
    Vector2 buttonPos45_49 = new Vector2(-330, -465);
    Vector2 buttonPos46_49 = new Vector2(-80, -465);
    Vector2 buttonPos47_49 = new Vector2(170, -465);
    Vector2 buttonPos48_49 = new Vector2(420, -465);
    Vector2 buttonPos49_49 = new Vector2(670, -465);
    Vector2 buttonSize49 = new Vector2(235, 130);

    Vector2 buttonPos01_64 = new Vector2(-842, 395);
    Vector2 buttonPos02_64 = new Vector2(-626, 395);
    Vector2 buttonPos03_64 = new Vector2(-408, 395);
    Vector2 buttonPos04_64 = new Vector2(-192, 395);
    Vector2 buttonPos05_64 = new Vector2(24, 395);
    Vector2 buttonPos06_64 = new Vector2(242, 395);
    Vector2 buttonPos07_64 = new Vector2(458, 395);
    Vector2 buttonPos08_64 = new Vector2(676, 395);
    Vector2 buttonPos09_64 = new Vector2(-842, 271);
    Vector2 buttonPos10_64 = new Vector2(-626, 271);
    Vector2 buttonPos11_64 = new Vector2(-408, 271);
    Vector2 buttonPos12_64 = new Vector2(-192, 271);
    Vector2 buttonPos13_64 = new Vector2(24, 271);
    Vector2 buttonPos14_64 = new Vector2(242, 271);
    Vector2 buttonPos15_64 = new Vector2(458, 271);
    Vector2 buttonPos16_64 = new Vector2(676, 271);
    Vector2 buttonPos17_64 = new Vector2(-842, 147);
    Vector2 buttonPos18_64 = new Vector2(-626, 147);
    Vector2 buttonPos19_64 = new Vector2(-408, 147);
    Vector2 buttonPos20_64 = new Vector2(-192, 147);
    Vector2 buttonPos21_64 = new Vector2(24, 147);
    Vector2 buttonPos22_64 = new Vector2(242, 147);
    Vector2 buttonPos23_64 = new Vector2(458, 147);
    Vector2 buttonPos24_64 = new Vector2(676, 147);
    Vector2 buttonPos25_64 = new Vector2(-842, 23);
    Vector2 buttonPos26_64 = new Vector2(-626, 23);
    Vector2 buttonPos27_64 = new Vector2(-408, 23);
    Vector2 buttonPos28_64 = new Vector2(-192, 23);
    Vector2 buttonPos29_64 = new Vector2(24, 23);
    Vector2 buttonPos30_64 = new Vector2(242, 23);
    Vector2 buttonPos31_64 = new Vector2(458, 23);
    Vector2 buttonPos32_64 = new Vector2(676, 23);
    Vector2 buttonPos33_64 = new Vector2(-842, -101);
    Vector2 buttonPos34_64 = new Vector2(-626, -101);
    Vector2 buttonPos35_64 = new Vector2(-408, -101);
    Vector2 buttonPos36_64 = new Vector2(-192, -101);
    Vector2 buttonPos37_64 = new Vector2(24, -101);
    Vector2 buttonPos38_64 = new Vector2(242, -101);
    Vector2 buttonPos39_64 = new Vector2(458, -101);
    Vector2 buttonPos40_64 = new Vector2(676, -101);
    Vector2 buttonPos41_64 = new Vector2(-842, -225);
    Vector2 buttonPos42_64 = new Vector2(-626, -225);
    Vector2 buttonPos43_64 = new Vector2(-408, -225);
    Vector2 buttonPos44_64 = new Vector2(-192, -225);
    Vector2 buttonPos45_64 = new Vector2(24, -225);
    Vector2 buttonPos46_64 = new Vector2(242, -225);
    Vector2 buttonPos47_64 = new Vector2(458, -225);
    Vector2 buttonPos48_64 = new Vector2(676, -225);
    Vector2 buttonPos49_64 = new Vector2(-842, -349);
    Vector2 buttonPos50_64 = new Vector2(-626, -349);
    Vector2 buttonPos51_64 = new Vector2(-408, -349);
    Vector2 buttonPos52_64 = new Vector2(-192, -349);
    Vector2 buttonPos53_64 = new Vector2(24, -349);
    Vector2 buttonPos54_64 = new Vector2(242, -349);
    Vector2 buttonPos55_64 = new Vector2(458, -349);
    Vector2 buttonPos56_64 = new Vector2(676, -349);
    Vector2 buttonPos57_64 = new Vector2(-842, -473);
    Vector2 buttonPos58_64 = new Vector2(-626, -473);
    Vector2 buttonPos59_64 = new Vector2(-408, -473);
    Vector2 buttonPos60_64 = new Vector2(-192, -473);
    Vector2 buttonPos61_64 = new Vector2(24, -473);
    Vector2 buttonPos62_64 = new Vector2(242, -473);
    Vector2 buttonPos63_64 = new Vector2(458, -473);
    Vector2 buttonPos64_64 = new Vector2(676, -473);

    Vector2 buttonSize64 = new Vector2(210, 115);
    #endregion
    #endregion
    
    void Awake() {
        lastTime = DateTime.Now;
        currentTime = DateTime.Now;
        if (site == "CPS") {
            connectionString = @"Data Source = tcp:172.16.5.217; user id = cipapp;password = bestinclass&1;Connection Timeout=10;";
            buttonString = new List<string> { "Line1", "Line2", "Line3", "Line6" };
        } else if (site == "GYR") {
            connectionString = @"Data Source = tcp:172.16.5.217; user id = cipapp;password = bestinclass&1;Connection Timeout=10;";
            buttonString = new List<string> { "Line1", "Line2"};
        }
        
        buttonContainer = transform.Find("Panel/Buttons").gameObject;
        buttonTexts = new List <Text> ();
        for (i1 = 0; i1 < buttons.Count; i1++) {
            buttonPoses.Add (Vector2.zero);
            buttonTexts.Add (buttons [i1].gameObject.GetComponentInChildren<Text>());
        }
        
        //Entries
        entryTopLineTexts = new List <Text> ();
        entryMinutesText = new List <InputField> ();
        entryOpNotes = new List <InputField> ();
        entryEngNotes = new List <InputField> ();
        for (i1 = 0; i1 < entryButtons.Count; i1++) {
            entryTopLineTexts.Add (entryButtons [i1].transform.parent.Find ("Top Line"         ).GetComponent <Text> ());
            entryMinutesText.Add  (entryButtons [i1].transform.parent.Find ("Minutes"          ).GetComponent <InputField> ());
            entryOpNotes.Add      (entryButtons [i1].transform.parent.Find ("Operator Notes"   ).GetComponent <InputField> ());
            entryEngNotes.Add     (entryButtons [i1].transform.parent.Find ("Engineering Notes").GetComponent <InputField> ());
        }

        /*MANUAL FGID ENTRY CODE
        FGIDInput = buttonContainer.gameObject.transform.Find("FGID Input").GetComponent<InputField>();
        validationMsg = buttonContainer.gameObject.transform.Find("validationMsg").GetComponent<Text>();
        FGIDOverride = buttonContainer.gameObject.transform.Find("FGID Override").GetComponent<RectTransform>();
        */

        sectionID = -2;
        CenterButtons(Vector2.zero);
        saveButton.gameObject.SetActive(false);
        cancelButton.gameObject.SetActive(false);
        backButton.gameObject.SetActive(false);
        buttonContainer.SetActive(true);
        buttonString = new List<string> { "Line1", "Line2", "Line3", "Line6" };
        ChangeButtonCount();
        Application.targetFrameRate = 60;
        StartCoroutine (RecieveOpsComs());
    }


    void Update() {
        lastTime = currentTime;
        currentTime = DateTime.Now;
        frameTime = (float)(currentTime - lastTime).TotalMilliseconds / 1000;
        
        inputTimer += frameTime;
        
        //Animate buttons sliding from mid-screen to their proper positions
        if (buttonContainer.activeSelf) {
            for (i1 = 0; i1 < activeButtons; i1++) {
                buttons[i1].anchoredPosition = Vector2.Lerp(buttons[i1].anchoredPosition, buttonPoses[i1], .25f);
            }
        }
        
        if (downtimeEntriesDownloadTimer > 0 && inputTimer > 60) {
            downtimeEntriesDownloadTimer -= frameTime;
            if (downtimeEntriesDownloadTimer <= 0) {
                StartCoroutine (DownloadDowntimeEntrys ());
                //Download reason codes
            }
        }

        if (saveDelay > 0 && inputTimer > 2) {
            saveDelay -= frameTime;
            if (saveDelay <= 0) {
                if (sectionID == 1) {
                    saveDelay = 5;
                } else { 
                    StartCoroutine(Save());
                }
            }
        }
        
        if (hourDownloadTimer > 0 && inputTimer > 2) {
            if (hourID == 0 && hourDownloadTimer > 60) {
                hourDownloadTimer = 60;
            }
            hourDownloadTimer -= frameTime;
            if (hourDownloadTimer <= 0) {
                if (hourID == 0) { hourDownloadTimer = 60; } else { hourDownloadTimer = 300; }
                StartCoroutine (DownloadHours ());
            }
        }
        
        if (reasonDowmloadTimer > 0 && inputTimer > 300) {
            reasonDowmloadTimer -= frameTime;
            if (reasonDowmloadTimer <= 0) {
                StartCoroutine (DownloadReasonCodes ());
            }
        }

        //if selected hour is the current hour
        if (hours.Count > hourID && (currentTime - hours [hourID].hour).TotalMinutes <= 60) {
            //once per minute
            if (currentTime.Second == 0) {
                if (!fiveMinuteRunning) {
                    fiveMinuteRunning = true;
                    //StartCoroutine(UpdateHourlyReport());
                }
            } else if (fiveMinuteRunning) {
                fiveMinuteRunning = false;
            }
        }

        if (runningCoroutines == 0 && (currentTime.Second % 30 == 0)) {
            refreshButton.image.color = new Color(0.504717f, 0.8422842f, 1f, 1f);
        }

        if (Input.GetKeyDown(KeyCode.F1)) {
            Button(-3); //Initials
            saveButton.gameObject.SetActive(true);
            cancelButton.gameObject.SetActive(false);
            backButton.gameObject.SetActive(false);
        }
        if (accountedDowntime.color == Color.white && Input.GetKeyDown(KeyCode.BackQuote)) {
            Button(-4); //Select Hour
        }

        if ((Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))) {
            if (buttonContainer.gameObject.activeSelf && saveButton.gameObject.activeSelf) {
                Button(-1); //Save
            } else if (opComsSend.text.Length > 0) {
                StartCoroutine(SendOpsComs());
            }
        }

        if (opComsCountdown > 0) {
            opComsCountdown -= frameTime;
        } else {
            StartCoroutine(RecieveOpsComs());
            opComsCountdown = 10;
            //AddMinutes (); // This is just here to catch any misses
        }
        
        if (sectionID == -1) {
            if (initials.text.Length >= 3) {
                SaveInitials();
            }
        }

        /*
		if (Input.GetKeyDown (KeyCode.Alpha1)) {		Button (0);} else
		if (Input.GetKeyDown (KeyCode.Alpha2)) {		Button (1);} else
		if (Input.GetKeyDown (KeyCode.Alpha3)) {		Button (2);} else
		if (Input.GetKeyDown (KeyCode.Alpha4)) {		Button (3);} else
		if (Input.GetKeyDown (KeyCode.Alpha5)) {		Button (4);} else
		if (Input.GetKeyDown (KeyCode.Alpha6)) {		Button (5);} else
		if (Input.GetKeyDown (KeyCode.Alpha7)) {		Button (6);} else
		if (Input.GetKeyDown (KeyCode.Alpha8)) {		Button (7);} else
		if (Input.GetKeyDown (KeyCode.Alpha9)) {		Button (8);} else
		if (Input.GetKeyDown (KeyCode.Alpha0)) {		Button (9);}
		*/
    }

    /// <summary>
    /// Setter method for int entry - used by downtime entry scrollrect
    /// </summary>
    /// <param name="inputInt">Index of the in-focus entry</param>
    public void ChangeEntry(int inputInt) {
        entry = inputInt;
    }

    /// <summary>
    /// Deletes a downtime entry from the scrollrect - used by downtime entry scrollrect
    /// </summary>
    /// <param name="inputInt">The index of the entry to be removed</param>
    public void RemoveEntry(int inputInt) {
        for (int i = 0; i < entryButtons.Count; i++) {
            if (i < inputInt) {
                // Nothing to see here
            } else if (i >= inputInt) {
                if (entryButtons [i+1].activeSelf) {
                    entryTopLineTexts[i].text = "";
                    entryOpNotes     [i].text = "";
                    entryEngNotes    [i].text = "";
                    entryMinutesText [i].text = "";
                    entryButtons     [i].SetActive(true);
                    entryButtons     [i + 1].transform.parent.gameObject.SetActive(false);
                    break;
                } else if (!entryButtons [i+1].activeSelf) {
                    entryTopLineTexts[i].text = entryTopLineTexts[i+1].text;
                    entryOpNotes     [i].text = entryOpNotes     [i+1].text;
                    entryEngNotes    [i].text = entryEngNotes    [i+1].text;
                    entryMinutesText [i].text = entryMinutesText [i+1].text;
                }
            }
        }
        MinutesTextChanged ();
        CheckCopyButton ();
    }

    /// <summary>
    /// Tallies up the minutes accounted for by user's downtime entries
    /// If the accounted minutes aren't equal to the total minutes of downtime, sets the downtime minutes text color to red
    /// If the accounted minutes equal the total minutes, sets the downtime minutes text color to white
    /// </summary>
    public void AddMinutes() {
        accountedDowntimeFloat = 0;
        for (j2 = 0; j2 < entryMinutesText.Count; j2++) {
            if (entryMinutesText[j2].text != "" && !entryButtons [j2].activeSelf) {
                accountedDowntimeFloat += float.Parse(entryMinutesText[j2].text);
            } else {
                break;
            }
        }
        accountedDtMinutes = Mathf.RoundToInt(accountedDowntimeFloat);
        accountedDowntime.text = accountedDtMinutes + " / " + hours [hourID].dtMinutes.ToString() + " Downtime Minutes";
        unaccountedForMinutes = hours [hourID].dtMinutes - accountedDtMinutes;
        if (hours [hourID].accMinutes != hours [hourID].dtMinutes || accountedDtMinutes != hours [hourID].dtMinutes) {
            if (hourID == 0) {
                selectedHourButton.image.color = new Color(0.504717f, 0.8422842f, 1f, 1f);
            } else {
                selectedHourButton.image.color = Color.gray;
            }
            if (Mathf.RoundToInt(accountedDowntimeFloat) != hours [hourID].dtMinutes) {
                accountedDowntime.color = Color.red;
            } else {
                accountedDowntime.color = new Color(0.504717f, 0.8422842f, 1f, 1f);
            }
        } else {
            accountedDowntime.color = Color.white;
            selectedHourButton.image.color = new Color(0.504717f, 0.8422842f, 1f, 1f);
        }
    }


    /// <summary>
    /// Setter method for int sectionID
    /// </summary>
    /// <param name="inputInt">the value to assign to sectionID</param>
    public void ChangeSection(int inputInt) {
        sectionID = inputInt;
    }

    /// <summary>
    /// Called whenever a button is clicked. Determines which actions to call by determining the active screen (via sectionID) and the buttonID
    /// </summary>
    /// <remarks>
    /// buttonIDs 0 through 63 identify the buttons 0-63 in the buttonContainer. Their purposes change dynamically throughout the program
    /// buttonIDs -6 and -2 correspond to Back and Cancel
    /// buttonID -1 identifies the Save button in the buttonContainer
    /// buttonID -3 identifies the user button on the main screen
    /// buttonID -4 identifies SelectedHour in the Entries section
    /// buttonID -5 identifies the MaterialID button on the main screen
    /// buttonID -7 identifies the CopyLastHour button in the Entries section
    /// </remarks>
    /// <param name="buttonID">Identifying value of the button which initiated the method call</param>
    public void Button(int buttonID) {
        inputTimer = 0;
        runningCoroutines++;
        //We need to have a section to add minutes and we should make the notes manditory?
        //Debug.Log(sectionID + " : " + buttonID);
        buttonQuestion.text = "";
        //if the user is on the material select screen
        if (sectionID == -3){
            /*Manual FGID code
            if(buttonID == -5)
            {
                GetFGIDOverride();
            }
            else */
            if (buttonID <= -2)
            {
                sectionID = 0;
                buttonContainer.SetActive(false);
            }
            else
            {
                SelectMaterialID(buttonID);
                //StartCoroutine(UpdateHourlyReport());
            }
            //FGIDOverride.gameObject.SetActive(false);
        } else if (sectionID == -2) { //else if user is on the line select screen
            if (buttonID <= -2) {
                UnityEngine.SceneManagement.SceneManager.LoadScene(0);
            } else {
                StartCoroutine (SelectLine(buttonID));
            }
        } else if (sectionID == -1) {  //else if user is on the initials entry screen
            if (buttonID <= -2) {
                UnityEngine.SceneManagement.SceneManager.LoadScene(0);
            } else if (buttonID == -1) {
                if (initials.text.Length >= 2) {
                    SaveInitials();
                }
                //else initials missing, ignore request to continue
            }
        } else if (sectionID == 0) { //Main Screen
            if (buttonID == -7) { //if copyLastHour button, copy the last hour
                CopyLastHourNew ();
                //StartCoroutine(CopyLastHour());
            } else if (buttonID == -5) { //else if material ID
                //If button is MaterialID and there are multiple material options, take user to the material selection screen
                if (materialDescriptions.Count > 0) {
                    sectionID = -3;
                    buttonString.Clear();
                    buttonString.AddRange(materialDescriptions);
                    //FGIDOverride.gameObject.SetActive(true);
                    cancelButton.gameObject.SetActive(true);
                    backButton.gameObject.SetActive(true);
                    saveButton.gameObject.SetActive(false);
                    buttonQuestion.gameObject.SetActive(true);
                    buttonQuestion.text = $"Which product is/was running during the hour of {lastUnaccountedForHour}?";
                    CenterButtons(materialText.rectTransform.anchoredPosition);
                    ChangeButtonCount();
                    buttonContainer.gameObject.SetActive(true);
                }
                /* //else prompt user to manually enter an FGID
                else
                {  
                    GetFGIDOverride();
                }*/
            }
            //else if SelectedHour button, save current entries and let the user set a new selectedHour
            else if (((hourID == 0 || accountedDowntime.color == Color.white) && buttonID == -4)/* || (entryTopLineTexts[0].text == "" && buttonID == -4)*/) { // Select Hour Button
                sectionID = 2;
                if (saveDelay > 0)
                {   //This checks to see if we were waiting on saving.  If we were, we want to save now, before we move to another hour.
                    saveDelay = -1;
                    StartCoroutine(Save());
                }
                GetLastAccountedForHourNew ();
                //StartCoroutine(GetLastAccountedForHour());
            }
            //else if userButton, take the user back to the initials entry screen
            else if (buttonID == -3) {
                cancelButton.gameObject.SetActive(false);
                backButton.gameObject.SetActive(false);
                saveButton.gameObject.SetActive(true);
                buttonContainer.SetActive(true);
                for (j1 = 0; j1 < buttons.Count; j1++)
                {
                    buttons[j1].gameObject.SetActive(false);
                }
                sectionID = -1;
                initials.text = "";
                initials.gameObject.SetActive(true);
                initials.ActivateInputField();
                runningCoroutines--;
                return;
            }
        } else if (sectionID == 1) {   //New Entry
            if (buttonID <= -6 && level > 1) {
                level--;
                CenterButtons(backButton.anchoredPosition);
                if (level == 1) {
                    entryTopLineTexts[entry].text = "";
                } else if (level == 2) {
                    entryTopLineTexts[entry].text = level1;
                }
                PopulateButtonsWithReasonCodes ();
                saveButton.gameObject.SetActive(false);
                minutes.gameObject.SetActive(false);
                notes.gameObject.SetActive(false);
            }
            //else if cancel button was clicked, return to main screen
            else if (buttonID <= -2)
            {
                buttonContainer.SetActive(false);
                notes.gameObject.SetActive(false);
                minutes.gameObject.SetActive(false);
                entryButtons[entry].SetActive(true);
                entryTopLineTexts[entry].text = "";
                entryOpNotes[entry].text = "";
                notes.text = "";
                minutes.text = "";
                sectionID = 0;
            }
            //else if save button was clicked
            else if (buttonID <= -1)
            {
                //if entry for minutes was valid, return to main screen and save as new downtime entry
                if (minutes.text != "" && float.Parse(minutes.text) <= 60 && float.Parse(minutes.text) >= 0)
                {
                    buttonContainer.SetActive(false);
                    notes.gameObject.SetActive(false);
                    minutes.gameObject.SetActive(false);
                    entryMinutesText[entry].text = minutes.text;
                    entryOpNotes[entry].text = notes.text;
                    entryButtons[entry].SetActive(false);
                    notes.text = "";
                    minutes.text = "";
                    if (entry < 9)
                    {
                        entryButtons[entry + 1].transform.parent.gameObject.SetActive(true);
                    }
                    title.text = "Operator Insights*";
                    sectionID = 0;
                    copyLastHourButton.SetActive(false);
                    MinutesTextChanged ();
                }
                //else ignore request to save
                else
                {
                    runningCoroutines--;
                    return;
                }
            } else if (level == 1) { //else progress level by one, update buttons
                level = 2;
                level1 = buttonString [buttonID];
                reasonCodeSQLID = reasonCodeSQLIDs [buttonID];
                entryTopLineTexts[entry].text = level1;
                CenterButtons(buttons[buttonID].anchoredPosition);
                buttonQuestion.text = entryTopLineTexts[entry].text;//"What was the cause of the downtime?";
                PopulateButtonsWithReasonCodes ();
                //StartCoroutine(EntryQuery());
            } else if (level == 2) {
                level = 3;
                level2 = buttonString[buttonID];
                reasonCodeSQLID = reasonCodeSQLIDs [buttonID];
                entryTopLineTexts[entry].text = level1 + " > " + level2;
                CenterButtons(buttons[buttonID].anchoredPosition);
                buttonQuestion.text = entryTopLineTexts[entry].text;
                PopulateButtonsWithReasonCodes ();
                //StartCoroutine(EntryQuery());
            } else if (level == 3) {
                level = 4;
                level3 = buttonString[buttonID];
                reasonCodeSQLID = reasonCodeSQLIDs [buttonID];
                entryTopLineTexts[entry].text = level1 + " > " + level2 + " > " + level3;
                CenterButtons(buttons[buttonID].anchoredPosition);
                buttonQuestion.text = entryTopLineTexts[entry].text;
                PopulateButtonsWithReasonCodes ();
                //StartCoroutine(EntryQuery());
            } else if (level == 4) {
                level = 5;
                level4 = buttonString[buttonID];
                reasonCodeSQLID = reasonCodeSQLIDs [buttonID];
                entryTopLineTexts[entry].text = level1 + " > " + level2 + " > " + level3 + " > " + level4;              
                CenterButtons(buttons[buttonID].anchoredPosition);
                buttonQuestion.text = entryTopLineTexts[entry].text;
                PopulateButtonsWithReasonCodes ();
                //StartCoroutine(EntryQuery());
            } else if (level == 5) {
                level = 6;
                level5 = buttonString[buttonID];
                reasonCodeSQLID = reasonCodeSQLIDs [buttonID];
                entryTopLineTexts[entry].text = level1 + " > " + level2 + " > " + level3 + " > " + level4 + " > " + level5;
                buttonQuestion.text = entryTopLineTexts[entry].text;
                DisplayNotes();
            }
        } else if (sectionID == 2) {   //Select Hour
            sectionID = 0;
            buttonContainer.SetActive(false);
            if (buttonID >= 0 && buttonID < 12) {
                hourID = buttonID + lastUnaccountedForHourID;
                selectedHour = lastUnaccountedForHour + new TimeSpan(buttonID * -1, 0, 0);
                selectedHourText.text = selectedHour.ToString("MM-dd  HH:mm");
                PopulateStatesNew ();
                StartCoroutine(CheckMaterialID());
                UpdateCurrentDowntimeEntrysFromList ();
                UpdateDowntimeEntrysText ();
            }
        }
        runningCoroutines--;
        #region inactive code
        /*MANUAL FGID ENTRY CODE
        //if user is on the manual FGID entry screen
        else if (sectionID == 3)
        {
            if (buttonID == -2)
            {
                sectionID = 0;
                FGIDInput.gameObject.SetActive(false);
                cancelButton.gameObject.SetActive(false);
                backButton.gameObject.SetActive(false);
                saveButton.gameObject.SetActive(false);
                validationMsg.gameObject.SetActive(false);
                buttonContainer.SetActive(false);

            }
            else if (buttonID == -6) {
                sectionID = 0;
                FGIDInput.gameObject.SetActive(false);
                cancelButton.gameObject.SetActive(false);
                backButton.gameObject.SetActive(false);
                saveButton.gameObject.SetActive(false);
                validationMsg.gameObject.SetActive(false);
                Button(-5);
            }
            else if (buttonID == -1 && FGIDInput.text.Length > 4 && int.Parse(FGIDInput.text) >= 0)
            {
                StartCoroutine(LookupMaterialID(int.Parse(FGIDInput.text)));
            }
            else
            {
                validationMsg.text = "FGID must be a positive 5 or 6 digit entry.";
                validationMsg.gameObject.SetActive(true);
            }
        }
        //if user is on BottleVol/BPC entry screen
        else if (sectionID == 4)
        {
            if (buttonID == -2 || buttonID == -3)
            {
                sectionID = 0;
                bottleVolInput.gameObject.SetActive(false);
                BPCInput.gameObject.SetActive(false);
                cancelButton.gameObject.SetActive(false);
                backButton.gameObject.SetActive(false);
                saveButton.gameObject.SetActive(false);
                validationMsg.gameObject.SetActive(false);
                buttonContainer.SetActive(false);
            }
            else if (buttonID == -1 && bottleVolInput.text.Length > 0 && int.Parse(bottleVolInput.text) > 0 && BPCInput.text.Length > 0 && int.Parse(BPCInput.text) > 0)
            {
                SetFGData(int.Parse(bottleVolInput.text), int.Parse(BPCInput.text));
                sectionID = 0;
                bottleVolInput.gameObject.SetActive(false);
                BPCInput.gameObject.SetActive(false);
                cancelButton.gameObject.SetActive(false);
                backButton.gameObject.SetActive(false);
                saveButton.gameObject.SetActive(false);
                validationMsg.gameObject.SetActive(false);
                buttonContainer.SetActive(false);
            }
            else
            {
                validationMsg.text = "Both entries must be greater than 0.";
                validationMsg.gameObject.SetActive(true);
            }
        }*/
        #endregion 
    }


    /// <summary>
    /// Clears user entries from the entryOpNotes and entryMinutesText input fields
    /// Deactivates all the entrybutton containers other than the first one
    /// </summary>
    public void ResetEntries() {
        entryButtons[0].SetActive(true);
        entryTopLineTexts[0].text = "";
        for (j1 = 0; j1 < entryButtons.Count; j1++) {
            entryTopLineTexts[j1].text = "";
            entryOpNotes[j1].text = "";
            entryEngNotes[j1].text = "";
            entryMinutesText[j1].text = "";
            if (j1 > 0) {
                entryButtons[j1].SetActive(true);
                entryButtons[j1].transform.parent.gameObject.SetActive(false);
            }
        }
        AddMinutes();
    }


    /// <summary>
    /// Sets the control buttons to active, deactivates all other buttonContainer buttons
    /// </summary>
    public void DisplayNotes() {
        minutes.text = (hours [hourID].dtMinutes - accountedDtMinutes) > 0 ? (hours [hourID].dtMinutes - accountedDtMinutes).ToString() : "0";
        saveButton.gameObject.SetActive(true);
        notes.gameObject.SetActive(true);
        minutes.gameObject.SetActive(true);
        foreach (RectTransform b in buttons) {
            b.gameObject.SetActive(false);
        }
    }


    /// <summary>
    /// Sets all buttons' anchored position to the vector parameter
    /// </summary>
    /// <param name="pos">The vector position to center all buttons on</param>
    public void CenterButtons(Vector2 pos) {
        for (j1 = 0; j1 < buttons.Count; j1++) {
            buttons[j1].anchoredPosition = pos;
        }
    }


    /// <summary>
    /// Determines the number of buttons which are currently active, sets the active buttons vector positions accordingly
    /// Sets buttonContainer to active
    /// </summary>

    public void ChangeButtonCount() {
        inputTimer = 0;
        activeButtons = buttonString.Count;
        if        (buttonString.Count ==  1) { buttonPoses[0] = buttonPos1;
        } else if (buttonString.Count ==  2) { buttonPoses[0] = buttonPos4_9; buttonPoses[1] = buttonPos6_9;
        } else if (buttonString.Count ==  3) { buttonPoses[0] = buttonPos2_9; buttonPoses[1] = buttonPos7_9; buttonPoses[2] = buttonPos9_9;
        } else if (buttonString.Count ==  4) { buttonPoses[0] = buttonPos1_9; buttonPoses[1] = buttonPos3_9; buttonPoses[2] = buttonPos7_9; buttonPoses[3] = buttonPos9_9;
        } else if (buttonString.Count ==  5) { buttonPoses[0] = buttonPos1_9; buttonPoses[1] = buttonPos3_9; buttonPoses[2] = buttonPos5_9; buttonPoses[3] = buttonPos7_9; buttonPoses[4] = buttonPos9_9;
        } else if (buttonString.Count ==  6) { buttonPoses[0] = buttonPos1_9; buttonPoses[1] = buttonPos2_9; buttonPoses[2] = buttonPos3_9; buttonPoses[3] = buttonPos7_9; buttonPoses[4] = buttonPos8_9; buttonPoses[5] = buttonPos9_9;
        } else if (buttonString.Count ==  7) { buttonPoses[0] = buttonPos1_9; buttonPoses[1] = buttonPos2_9; buttonPoses[2] = buttonPos3_9; buttonPoses[3] = buttonPos5_9; buttonPoses[4] = buttonPos7_9; buttonPoses[5] = buttonPos8_9; buttonPoses[6] = buttonPos9_9;
        } else if (buttonString.Count ==  8) { buttonPoses[0] = buttonPos1_9; buttonPoses[1] = buttonPos2_9; buttonPoses[2] = buttonPos3_9; buttonPoses[3] = buttonPos4_9; buttonPoses[4] = buttonPos6_9; buttonPoses[5] = buttonPos7_9; buttonPoses[6] = buttonPos8_9; buttonPoses[7] = buttonPos9_9;
        } else if (buttonString.Count ==  9) { buttonPoses[0] = buttonPos1_9; buttonPoses[1] = buttonPos2_9; buttonPoses[2] = buttonPos3_9; buttonPoses[3] = buttonPos4_9; buttonPoses[4] = buttonPos5_9; buttonPoses[5] = buttonPos6_9; buttonPoses[6] = buttonPos7_9; buttonPoses[7] = buttonPos8_9; buttonPoses[8] = buttonPos9_9;
        } else if (buttonString.Count == 10) { buttonPoses[0] = buttonPos02_16; buttonPoses[1] = buttonPos03_16; buttonPoses[2] = buttonPos05_16; buttonPoses[3] = buttonPos06_16; buttonPoses[4] = buttonPos07_16; buttonPoses[5] = buttonPos08_16; buttonPoses[6] = buttonPos09_16; buttonPoses[7] = buttonPos10_16; buttonPoses[8] = buttonPos11_16; buttonPoses[9] = buttonPos12_16;
        } else if (buttonString.Count == 11) { buttonPoses[0] = buttonPos01_16; buttonPoses[1] = buttonPos02_16; buttonPoses[2] = buttonPos03_16; buttonPoses[3] = buttonPos05_16; buttonPoses[4] = buttonPos06_16; buttonPoses[5] = buttonPos07_16; buttonPoses[6] = buttonPos08_16; buttonPoses[7] = buttonPos09_16; buttonPoses[8] = buttonPos10_16; buttonPoses[9] = buttonPos11_16; buttonPoses[10] = buttonPos12_16;
        } else if (buttonString.Count == 12) { buttonPoses[0] = buttonPos01_16; buttonPoses[1] = buttonPos02_16; buttonPoses[2] = buttonPos03_16; buttonPoses[3] = buttonPos04_16; buttonPoses[4] = buttonPos05_16; buttonPoses[5] = buttonPos06_16; buttonPoses[6] = buttonPos07_16; buttonPoses[7] = buttonPos08_16; buttonPoses[8] = buttonPos09_16; buttonPoses[9] = buttonPos10_16; buttonPoses[10] = buttonPos11_16; buttonPoses[11] = buttonPos12_16;
        } else if (buttonString.Count == 13) { buttonPoses[0] = buttonPos02_16; buttonPoses[1] = buttonPos05_16; buttonPoses[2] = buttonPos06_16; buttonPoses[3] = buttonPos07_16; buttonPoses[4] = buttonPos08_16; buttonPoses[5] = buttonPos09_16; buttonPoses[6] = buttonPos10_16; buttonPoses[7] = buttonPos11_16; buttonPoses[8] = buttonPos12_16; buttonPoses[9] = buttonPos13_16; buttonPoses[10] = buttonPos14_16; buttonPoses[11] = buttonPos15_16; buttonPoses[12] = buttonPos16_16;
        } else if (buttonString.Count == 14) { buttonPoses[0] = buttonPos02_16; buttonPoses[1] = buttonPos03_16; buttonPoses[2] = buttonPos05_16; buttonPoses[3] = buttonPos06_16; buttonPoses[4] = buttonPos07_16; buttonPoses[5] = buttonPos08_16; buttonPoses[6] = buttonPos09_16; buttonPoses[7] = buttonPos10_16; buttonPoses[8] = buttonPos11_16; buttonPoses[9] = buttonPos12_16; buttonPoses[10] = buttonPos13_16; buttonPoses[11] = buttonPos14_16; buttonPoses[12] = buttonPos15_16; buttonPoses[13] = buttonPos16_16;
        } else if (buttonString.Count == 15) { buttonPoses[0] = buttonPos01_16; buttonPoses[1] = buttonPos02_16; buttonPoses[2] = buttonPos03_16; buttonPoses[3] = buttonPos05_16; buttonPoses[4] = buttonPos06_16; buttonPoses[5] = buttonPos07_16; buttonPoses[6] = buttonPos08_16; buttonPoses[7] = buttonPos09_16; buttonPoses[8] = buttonPos10_16; buttonPoses[9] = buttonPos11_16; buttonPoses[10] = buttonPos12_16; buttonPoses[11] = buttonPos13_16; buttonPoses[12] = buttonPos14_16; buttonPoses[13] = buttonPos15_16; buttonPoses[14] = buttonPos16_16;
        } else if (buttonString.Count == 16) { buttonPoses[0] = buttonPos01_16; buttonPoses[1] = buttonPos02_16; buttonPoses[2] = buttonPos03_16; buttonPoses[3] = buttonPos04_16; buttonPoses[4] = buttonPos05_16; buttonPoses[5] = buttonPos06_16; buttonPoses[6] = buttonPos07_16; buttonPoses[7] = buttonPos08_16; buttonPoses[8] = buttonPos09_16; buttonPoses[9] = buttonPos10_16; buttonPoses[10] = buttonPos11_16; buttonPoses[11] = buttonPos12_16; buttonPoses[12] = buttonPos13_16; buttonPoses[13] = buttonPos14_16; buttonPoses[14] = buttonPos15_16; buttonPoses[15] = buttonPos16_16;
        } else if (buttonString.Count == 17) { buttonPoses[0] = buttonPos03_25; buttonPoses[1] = buttonPos07_25; buttonPoses[2] = buttonPos08_25; buttonPoses[3] = buttonPos09_25; buttonPoses[4] = buttonPos11_25; buttonPoses[5] = buttonPos12_25; buttonPoses[6] = buttonPos13_25; buttonPoses[7] = buttonPos14_25; buttonPoses[8] = buttonPos15_25; buttonPoses[9] = buttonPos16_25; buttonPoses[10] = buttonPos17_25; buttonPoses[11] = buttonPos18_25; buttonPoses[12] = buttonPos19_25; buttonPoses[13] = buttonPos20_25; buttonPoses[14] = buttonPos22_25; buttonPoses[15] = buttonPos23_25; buttonPoses[16] = buttonPos24_25;
        } else if (buttonString.Count == 18) { buttonPoses[0] = buttonPos02_25; buttonPoses[1] = buttonPos03_25; buttonPoses[2] = buttonPos04_25; buttonPoses[3] = buttonPos06_25; buttonPoses[4] = buttonPos07_25; buttonPoses[5] = buttonPos08_25; buttonPoses[6] = buttonPos09_25; buttonPoses[7] = buttonPos10_25; buttonPoses[8] = buttonPos11_25; buttonPoses[9] = buttonPos12_25; buttonPoses[10] = buttonPos13_25; buttonPoses[11] = buttonPos14_25; buttonPoses[12] = buttonPos15_25; buttonPoses[13] = buttonPos16_25; buttonPoses[14] = buttonPos17_25; buttonPoses[15] = buttonPos18_25; buttonPoses[16] = buttonPos19_25; buttonPoses[17] = buttonPos20_25;
        } else if (buttonString.Count == 19) { buttonPoses[0] = buttonPos02_25; buttonPoses[1] = buttonPos04_25; buttonPoses[2] = buttonPos06_25; buttonPoses[3] = buttonPos07_25; buttonPoses[4] = buttonPos08_25; buttonPoses[5] = buttonPos09_25; buttonPoses[6] = buttonPos10_25; buttonPoses[7] = buttonPos11_25; buttonPoses[8] = buttonPos12_25; buttonPoses[9] = buttonPos13_25; buttonPoses[10] = buttonPos14_25; buttonPoses[11] = buttonPos15_25; buttonPoses[12] = buttonPos16_25; buttonPoses[13] = buttonPos17_25; buttonPoses[14] = buttonPos18_25; buttonPoses[15] = buttonPos19_25; buttonPoses[16] = buttonPos20_25; buttonPoses[17] = buttonPos22_25; buttonPoses[18] = buttonPos24_25;
        } else if (buttonString.Count == 20) { buttonPoses[0] = buttonPos02_25; buttonPoses[1] = buttonPos04_25; buttonPoses[2] = buttonPos06_25; buttonPoses[3] = buttonPos07_25; buttonPoses[4] = buttonPos08_25; buttonPoses[5] = buttonPos09_25; buttonPoses[6] = buttonPos10_25; buttonPoses[7] = buttonPos11_25; buttonPoses[8] = buttonPos12_25; buttonPoses[9] = buttonPos13_25; buttonPoses[10] = buttonPos14_25; buttonPoses[11] = buttonPos15_25; buttonPoses[12] = buttonPos16_25; buttonPoses[13] = buttonPos17_25; buttonPoses[14] = buttonPos18_25; buttonPoses[15] = buttonPos19_25; buttonPoses[16] = buttonPos20_25; buttonPoses[17] = buttonPos21_25; buttonPoses[18] = buttonPos23_25; buttonPoses[19] = buttonPos25_25;
        } else if (buttonString.Count == 21) { buttonPoses[0] = buttonPos01_25; buttonPoses[1] = buttonPos02_25; buttonPoses[2] = buttonPos03_25; buttonPoses[3] = buttonPos04_25; buttonPoses[4] = buttonPos05_25; buttonPoses[5] = buttonPos07_25; buttonPoses[6] = buttonPos08_25; buttonPoses[7] = buttonPos09_25; buttonPoses[8] = buttonPos12_25; buttonPoses[9] = buttonPos13_25; buttonPoses[10] = buttonPos14_25; buttonPoses[11] = buttonPos16_25; buttonPoses[12] = buttonPos17_25; buttonPoses[13] = buttonPos18_25; buttonPoses[14] = buttonPos19_25; buttonPoses[15] = buttonPos20_25; buttonPoses[16] = buttonPos21_25; buttonPoses[17] = buttonPos22_25; buttonPoses[18] = buttonPos23_25; buttonPoses[19] = buttonPos24_25; buttonPoses[20] = buttonPos25_25;
        } else if (buttonString.Count == 22) { buttonPoses[0] = buttonPos01_25; buttonPoses[1] = buttonPos02_25; buttonPoses[2] = buttonPos03_25; buttonPoses[3] = buttonPos04_25; buttonPoses[4] = buttonPos05_25; buttonPoses[5] = buttonPos06_25; buttonPoses[6] = buttonPos07_25; buttonPoses[7] = buttonPos08_25; buttonPoses[8] = buttonPos09_25; buttonPoses[9] = buttonPos10_25; buttonPoses[10] = buttonPos12_25; buttonPoses[11] = buttonPos14_25; buttonPoses[12] = buttonPos16_25; buttonPoses[13] = buttonPos17_25; buttonPoses[14] = buttonPos18_25; buttonPoses[15] = buttonPos19_25; buttonPoses[16] = buttonPos20_25; buttonPoses[17] = buttonPos21_25; buttonPoses[18] = buttonPos22_25; buttonPoses[19] = buttonPos23_25; buttonPoses[20] = buttonPos24_25; buttonPoses[21] = buttonPos25_25;
        } else if (buttonString.Count == 23) { buttonPoses[0] = buttonPos01_25; buttonPoses[1] = buttonPos02_25; buttonPoses[2] = buttonPos03_25; buttonPoses[3] = buttonPos04_25; buttonPoses[4] = buttonPos05_25; buttonPoses[5] = buttonPos06_25; buttonPoses[6] = buttonPos07_25; buttonPoses[7] = buttonPos08_25; buttonPoses[8] = buttonPos09_25; buttonPoses[9] = buttonPos10_25; buttonPoses[10] = buttonPos12_25; buttonPoses[11] = buttonPos13_25; buttonPoses[12] = buttonPos14_25; buttonPoses[13] = buttonPos16_25; buttonPoses[14] = buttonPos17_25; buttonPoses[15] = buttonPos18_25; buttonPoses[16] = buttonPos19_25; buttonPoses[17] = buttonPos20_25; buttonPoses[18] = buttonPos21_25; buttonPoses[19] = buttonPos22_25; buttonPoses[20] = buttonPos23_25; buttonPoses[21] = buttonPos24_25; buttonPoses[22] = buttonPos25_25;
        } else if (buttonString.Count == 24) { buttonPoses[0] = buttonPos01_25; buttonPoses[1] = buttonPos02_25; buttonPoses[2] = buttonPos03_25; buttonPoses[3] = buttonPos04_25; buttonPoses[4] = buttonPos05_25; buttonPoses[5] = buttonPos06_25; buttonPoses[6] = buttonPos07_25; buttonPoses[7] = buttonPos08_25; buttonPoses[8] = buttonPos09_25; buttonPoses[9] = buttonPos10_25; buttonPoses[10] = buttonPos11_25; buttonPoses[11] = buttonPos12_25; buttonPoses[12] = buttonPos13_25; buttonPoses[13] = buttonPos14_25; buttonPoses[14] = buttonPos15_25; buttonPoses[15] = buttonPos16_25; buttonPoses[16] = buttonPos17_25; buttonPoses[17] = buttonPos18_25; buttonPoses[18] = buttonPos19_25; buttonPoses[19] = buttonPos20_25; buttonPoses[20] = buttonPos21_25; buttonPoses[21] = buttonPos22_25; buttonPoses[22] = buttonPos24_25; buttonPoses[23] = buttonPos25_25;
        } else if (buttonString.Count == 25) { buttonPoses[0] = buttonPos01_25; buttonPoses[1] = buttonPos02_25; buttonPoses[2] = buttonPos03_25; buttonPoses[3] = buttonPos04_25; buttonPoses[4] = buttonPos05_25; buttonPoses[5] = buttonPos06_25; buttonPoses[6] = buttonPos07_25; buttonPoses[7] = buttonPos08_25; buttonPoses[8] = buttonPos09_25; buttonPoses[9] = buttonPos10_25; buttonPoses[10] = buttonPos11_25; buttonPoses[11] = buttonPos12_25; buttonPoses[12] = buttonPos13_25; buttonPoses[13] = buttonPos14_25; buttonPoses[14] = buttonPos15_25; buttonPoses[15] = buttonPos16_25; buttonPoses[16] = buttonPos17_25; buttonPoses[17] = buttonPos18_25; buttonPoses[18] = buttonPos19_25; buttonPoses[19] = buttonPos20_25; buttonPoses[20] = buttonPos21_25; buttonPoses[21] = buttonPos22_25; buttonPoses[22] = buttonPos23_25; buttonPoses[23] = buttonPos24_25; buttonPoses[24] = buttonPos25_25;
        } else if (buttonString.Count == 26) { buttonPoses[0] = buttonPos03_36; buttonPoses[1] = buttonPos04_36; buttonPoses[2] = buttonPos08_36; buttonPoses[3] = buttonPos09_36; buttonPoses[4] = buttonPos10_36; buttonPoses[5] = buttonPos11_36; buttonPoses[6] = buttonPos13_36; buttonPoses[7] = buttonPos14_36; buttonPoses[8] = buttonPos15_36; buttonPoses[9] = buttonPos16_36; buttonPoses[10] = buttonPos17_36; buttonPoses[11] = buttonPos18_36; buttonPoses[12] = buttonPos19_36; buttonPoses[13] = buttonPos20_36; buttonPoses[14] = buttonPos21_36; buttonPoses[15] = buttonPos22_36; buttonPoses[16] = buttonPos23_36; buttonPoses[17] = buttonPos24_36; buttonPoses[18] = buttonPos26_36; buttonPoses[19] = buttonPos27_36; buttonPoses[20] = buttonPos28_36; buttonPoses[21] = buttonPos29_36; buttonPoses[22] = buttonPos32_36; buttonPoses[23] = buttonPos33_36; buttonPoses[24] = buttonPos34_36; buttonPoses[25] = buttonPos35_36;
        } else if (buttonString.Count == 27) { buttonPoses[0] = buttonPos08_36; buttonPoses[1] = buttonPos09_36; buttonPoses[2] = buttonPos10_36; buttonPoses[3] = buttonPos13_36; buttonPoses[4] = buttonPos14_36; buttonPoses[5] = buttonPos15_36; buttonPoses[6] = buttonPos16_36; buttonPoses[7] = buttonPos17_36; buttonPoses[8] = buttonPos18_36; buttonPoses[9] = buttonPos19_36; buttonPoses[10] = buttonPos20_36; buttonPoses[11] = buttonPos21_36; buttonPoses[12] = buttonPos22_36; buttonPoses[13] = buttonPos23_36; buttonPoses[14] = buttonPos24_36; buttonPoses[15] = buttonPos25_36; buttonPoses[16] = buttonPos26_36; buttonPoses[17] = buttonPos27_36; buttonPoses[18] = buttonPos28_36; buttonPoses[19] = buttonPos29_36; buttonPoses[20] = buttonPos30_36; buttonPoses[21] = buttonPos31_36; buttonPoses[22] = buttonPos32_36; buttonPoses[23] = buttonPos33_36; buttonPoses[24] = buttonPos34_36; buttonPoses[25] = buttonPos35_36; buttonPoses[26] = buttonPos36_36;
        } else if (buttonString.Count == 28) { buttonPoses[0] = buttonPos08_36; buttonPoses[1] = buttonPos09_36; buttonPoses[2] = buttonPos10_36; buttonPoses[3] = buttonPos11_36; buttonPoses[4] = buttonPos13_36; buttonPoses[5] = buttonPos14_36; buttonPoses[6] = buttonPos15_36; buttonPoses[7] = buttonPos16_36; buttonPoses[8] = buttonPos17_36; buttonPoses[9] = buttonPos18_36; buttonPoses[10] = buttonPos19_36; buttonPoses[11] = buttonPos20_36; buttonPoses[12] = buttonPos21_36; buttonPoses[13] = buttonPos22_36; buttonPoses[14] = buttonPos23_36; buttonPoses[15] = buttonPos24_36; buttonPoses[16] = buttonPos25_36; buttonPoses[17] = buttonPos26_36; buttonPoses[18] = buttonPos27_36; buttonPoses[19] = buttonPos28_36; buttonPoses[20] = buttonPos29_36; buttonPoses[21] = buttonPos30_36; buttonPoses[22] = buttonPos31_36; buttonPoses[23] = buttonPos32_36; buttonPoses[24] = buttonPos33_36; buttonPoses[25] = buttonPos34_36; buttonPoses[26] = buttonPos35_36; buttonPoses[27] = buttonPos36_36;
        } else if (buttonString.Count == 29) { buttonPoses[0] = buttonPos07_36; buttonPoses[1] = buttonPos08_36; buttonPoses[2] = buttonPos09_36; buttonPoses[3] = buttonPos10_36; buttonPoses[4] = buttonPos11_36; buttonPoses[5] = buttonPos13_36; buttonPoses[6] = buttonPos14_36; buttonPoses[7] = buttonPos15_36; buttonPoses[8] = buttonPos16_36; buttonPoses[9] = buttonPos17_36; buttonPoses[10] = buttonPos18_36; buttonPoses[11] = buttonPos19_36; buttonPoses[12] = buttonPos20_36; buttonPoses[13] = buttonPos21_36; buttonPoses[14] = buttonPos22_36; buttonPoses[15] = buttonPos23_36; buttonPoses[16] = buttonPos24_36; buttonPoses[17] = buttonPos25_36; buttonPoses[18] = buttonPos26_36; buttonPoses[19] = buttonPos27_36; buttonPoses[20] = buttonPos28_36; buttonPoses[21] = buttonPos29_36; buttonPoses[22] = buttonPos30_36; buttonPoses[23] = buttonPos31_36; buttonPoses[24] = buttonPos32_36; buttonPoses[25] = buttonPos33_36; buttonPoses[26] = buttonPos34_36; buttonPoses[27] = buttonPos35_36; buttonPoses[28] = buttonPos36_36;
        } else if (buttonString.Count == 30) { buttonPoses[0] = buttonPos07_36; buttonPoses[1] = buttonPos08_36; buttonPoses[2] = buttonPos09_36; buttonPoses[3] = buttonPos10_36; buttonPoses[4] = buttonPos11_36; buttonPoses[5] = buttonPos12_36; buttonPoses[6] = buttonPos13_36; buttonPoses[7] = buttonPos14_36; buttonPoses[8] = buttonPos15_36; buttonPoses[9] = buttonPos16_36; buttonPoses[10] = buttonPos17_36; buttonPoses[11] = buttonPos18_36; buttonPoses[12] = buttonPos19_36; buttonPoses[13] = buttonPos20_36; buttonPoses[14] = buttonPos21_36; buttonPoses[15] = buttonPos22_36; buttonPoses[16] = buttonPos23_36; buttonPoses[17] = buttonPos24_36; buttonPoses[18] = buttonPos25_36; buttonPoses[19] = buttonPos26_36; buttonPoses[20] = buttonPos27_36; buttonPoses[21] = buttonPos28_36; buttonPoses[22] = buttonPos29_36; buttonPoses[23] = buttonPos30_36; buttonPoses[24] = buttonPos31_36; buttonPoses[25] = buttonPos32_36; buttonPoses[26] = buttonPos33_36; buttonPoses[27] = buttonPos34_36; buttonPoses[28] = buttonPos35_36; buttonPoses[29] = buttonPos36_36;
        } else if (buttonString.Count == 31) { buttonPoses[0] = buttonPos01_36; buttonPoses[1] = buttonPos07_36; buttonPoses[2] = buttonPos08_36; buttonPoses[3] = buttonPos09_36; buttonPoses[4] = buttonPos10_36; buttonPoses[5] = buttonPos11_36; buttonPoses[6] = buttonPos12_36; buttonPoses[7] = buttonPos13_36; buttonPoses[8] = buttonPos14_36; buttonPoses[9] = buttonPos15_36; buttonPoses[10] = buttonPos16_36; buttonPoses[11] = buttonPos17_36; buttonPoses[12] = buttonPos18_36; buttonPoses[13] = buttonPos19_36; buttonPoses[14] = buttonPos20_36; buttonPoses[15] = buttonPos21_36; buttonPoses[16] = buttonPos22_36; buttonPoses[17] = buttonPos23_36; buttonPoses[18] = buttonPos24_36; buttonPoses[19] = buttonPos25_36; buttonPoses[20] = buttonPos26_36; buttonPoses[21] = buttonPos27_36; buttonPoses[22] = buttonPos28_36; buttonPoses[23] = buttonPos29_36; buttonPoses[24] = buttonPos30_36; buttonPoses[25] = buttonPos31_36; buttonPoses[26] = buttonPos32_36; buttonPoses[27] = buttonPos33_36; buttonPoses[28] = buttonPos34_36; buttonPoses[29] = buttonPos35_36; buttonPoses[30] = buttonPos36_36;/*Fix this column*/
        } else if (buttonString.Count == 32) { buttonPoses[0] = buttonPos02_36; buttonPoses[1] = buttonPos03_36; buttonPoses[2] = buttonPos04_36; buttonPoses[3] = buttonPos05_36; buttonPoses[4] = buttonPos07_36; buttonPoses[5] = buttonPos08_36; buttonPoses[6] = buttonPos09_36; buttonPoses[7] = buttonPos10_36; buttonPoses[8] = buttonPos11_36; buttonPoses[9] = buttonPos12_36; buttonPoses[10] = buttonPos13_36; buttonPoses[11] = buttonPos14_36; buttonPoses[12] = buttonPos15_36; buttonPoses[13] = buttonPos16_36; buttonPoses[14] = buttonPos17_36; buttonPoses[15] = buttonPos18_36; buttonPoses[16] = buttonPos19_36; buttonPoses[17] = buttonPos20_36; buttonPoses[18] = buttonPos21_36; buttonPoses[19] = buttonPos22_36; buttonPoses[20] = buttonPos23_36; buttonPoses[21] = buttonPos24_36; buttonPoses[22] = buttonPos25_36; buttonPoses[23] = buttonPos26_36; buttonPoses[24] = buttonPos27_36; buttonPoses[25] = buttonPos28_36; buttonPoses[26] = buttonPos29_36; buttonPoses[27] = buttonPos30_36; buttonPoses[28] = buttonPos32_36; buttonPoses[29] = buttonPos33_36; buttonPoses[30] = buttonPos34_36; buttonPoses[31] = buttonPos35_36;
        } else if (buttonString.Count == 33) { buttonPoses[0] = buttonPos02_36; buttonPoses[1] = buttonPos03_36; buttonPoses[2] = buttonPos04_36; buttonPoses[3] = buttonPos05_36; buttonPoses[4] = buttonPos07_36; buttonPoses[5] = buttonPos08_36; buttonPoses[6] = buttonPos09_36; buttonPoses[7] = buttonPos10_36; buttonPoses[8] = buttonPos11_36; buttonPoses[9] = buttonPos12_36; buttonPoses[10] = buttonPos13_36; buttonPoses[11] = buttonPos14_36; buttonPoses[12] = buttonPos15_36; buttonPoses[13] = buttonPos16_36; buttonPoses[14] = buttonPos17_36; buttonPoses[15] = buttonPos18_36; buttonPoses[16] = buttonPos19_36; buttonPoses[17] = buttonPos20_36; buttonPoses[18] = buttonPos21_36; buttonPoses[19] = buttonPos22_36; buttonPoses[20] = buttonPos23_36; buttonPoses[21] = buttonPos24_36; buttonPoses[22] = buttonPos25_36; buttonPoses[23] = buttonPos26_36; buttonPoses[24] = buttonPos27_36; buttonPoses[25] = buttonPos28_36; buttonPoses[26] = buttonPos29_36; buttonPoses[27] = buttonPos30_36; buttonPoses[28] = buttonPos31_36; buttonPoses[29] = buttonPos32_36; buttonPoses[30] = buttonPos33_36; buttonPoses[31] = buttonPos34_36; buttonPoses[32] = buttonPos35_36;
        } else if (buttonString.Count == 34) { buttonPoses[0] = buttonPos02_36; buttonPoses[1] = buttonPos03_36; buttonPoses[2] = buttonPos04_36; buttonPoses[3] = buttonPos05_36; buttonPoses[4] = buttonPos07_36; buttonPoses[5] = buttonPos08_36; buttonPoses[6] = buttonPos09_36; buttonPoses[7] = buttonPos10_36; buttonPoses[8] = buttonPos11_36; buttonPoses[9] = buttonPos12_36; buttonPoses[10] = buttonPos13_36; buttonPoses[11] = buttonPos14_36; buttonPoses[12] = buttonPos15_36; buttonPoses[13] = buttonPos16_36; buttonPoses[14] = buttonPos17_36; buttonPoses[15] = buttonPos18_36; buttonPoses[16] = buttonPos19_36; buttonPoses[17] = buttonPos20_36; buttonPoses[18] = buttonPos21_36; buttonPoses[19] = buttonPos22_36; buttonPoses[20] = buttonPos23_36; buttonPoses[21] = buttonPos24_36; buttonPoses[22] = buttonPos25_36; buttonPoses[23] = buttonPos26_36; buttonPoses[24] = buttonPos27_36; buttonPoses[25] = buttonPos28_36; buttonPoses[26] = buttonPos29_36; buttonPoses[27] = buttonPos30_36; buttonPoses[28] = buttonPos31_36; buttonPoses[29] = buttonPos32_36; buttonPoses[30] = buttonPos33_36; buttonPoses[31] = buttonPos34_36; buttonPoses[32] = buttonPos35_36; buttonPoses[33] = buttonPos36_36;
        } else if (buttonString.Count == 35) { buttonPoses[0] = buttonPos01_36; buttonPoses[1] = buttonPos02_36; buttonPoses[2] = buttonPos03_36; buttonPoses[3] = buttonPos04_36; buttonPoses[4] = buttonPos05_36; buttonPoses[5] = buttonPos07_36; buttonPoses[6] = buttonPos08_36; buttonPoses[7] = buttonPos09_36; buttonPoses[8] = buttonPos10_36; buttonPoses[9] = buttonPos11_36; buttonPoses[10] = buttonPos12_36; buttonPoses[11] = buttonPos13_36; buttonPoses[12] = buttonPos14_36; buttonPoses[13] = buttonPos15_36; buttonPoses[14] = buttonPos16_36; buttonPoses[15] = buttonPos17_36; buttonPoses[16] = buttonPos18_36; buttonPoses[17] = buttonPos19_36; buttonPoses[18] = buttonPos20_36; buttonPoses[19] = buttonPos21_36; buttonPoses[20] = buttonPos22_36; buttonPoses[21] = buttonPos23_36; buttonPoses[22] = buttonPos24_36; buttonPoses[23] = buttonPos25_36; buttonPoses[24] = buttonPos26_36; buttonPoses[25] = buttonPos27_36; buttonPoses[26] = buttonPos28_36; buttonPoses[27] = buttonPos29_36; buttonPoses[28] = buttonPos30_36; buttonPoses[29] = buttonPos31_36; buttonPoses[30] = buttonPos32_36; buttonPoses[31] = buttonPos33_36; buttonPoses[32] = buttonPos34_36; buttonPoses[33] = buttonPos35_36; buttonPoses[34] = buttonPos36_36;
        } else if (buttonString.Count == 36) { buttonPoses[0] = buttonPos01_36; buttonPoses[1] = buttonPos02_36; buttonPoses[2] = buttonPos03_36; buttonPoses[3] = buttonPos04_36; buttonPoses[4] = buttonPos05_36; buttonPoses[5] = buttonPos06_36; buttonPoses[6] = buttonPos07_36; buttonPoses[7] = buttonPos08_36; buttonPoses[8] = buttonPos09_36; buttonPoses[9] = buttonPos10_36; buttonPoses[10] = buttonPos11_36; buttonPoses[11] = buttonPos12_36; buttonPoses[12] = buttonPos13_36; buttonPoses[13] = buttonPos14_36; buttonPoses[14] = buttonPos15_36; buttonPoses[15] = buttonPos16_36; buttonPoses[16] = buttonPos17_36; buttonPoses[17] = buttonPos18_36; buttonPoses[18] = buttonPos19_36; buttonPoses[19] = buttonPos20_36; buttonPoses[20] = buttonPos21_36; buttonPoses[21] = buttonPos22_36; buttonPoses[22] = buttonPos23_36; buttonPoses[23] = buttonPos24_36; buttonPoses[24] = buttonPos25_36; buttonPoses[25] = buttonPos26_36; buttonPoses[26] = buttonPos27_36; buttonPoses[27] = buttonPos28_36; buttonPoses[28] = buttonPos29_36; buttonPoses[29] = buttonPos30_36; buttonPoses[30] = buttonPos31_36; buttonPoses[31] = buttonPos32_36; buttonPoses[32] = buttonPos33_36; buttonPoses[33] = buttonPos34_36; buttonPoses[34] = buttonPos35_36; buttonPoses[35] = buttonPos36_36;
        } else if (buttonString.Count == 37) { buttonPoses[0] = buttonPos03_49; buttonPoses[1] = buttonPos04_49; buttonPoses[2] = buttonPos05_49; buttonPoses[3] = buttonPos09_49; buttonPoses[4] = buttonPos10_49; buttonPoses[5] = buttonPos11_49; buttonPoses[6] = buttonPos12_49; buttonPoses[7] = buttonPos13_49; buttonPoses[8] = buttonPos15_49; buttonPoses[9] = buttonPos16_49; buttonPoses[10] = buttonPos17_49; buttonPoses[11] = buttonPos18_49; buttonPoses[12] = buttonPos19_49; buttonPoses[13] = buttonPos20_49; buttonPoses[14] = buttonPos21_49; buttonPoses[15] = buttonPos22_49; buttonPoses[16] = buttonPos23_49; buttonPoses[17] = buttonPos24_49; buttonPoses[18] = buttonPos25_49; buttonPoses[19] = buttonPos26_49; buttonPoses[20] = buttonPos27_49; buttonPoses[21] = buttonPos28_49; buttonPoses[22] = buttonPos29_49; buttonPoses[23] = buttonPos30_49; buttonPoses[24] = buttonPos31_49; buttonPoses[25] = buttonPos32_49; buttonPoses[26] = buttonPos33_49; buttonPoses[27] = buttonPos34_49; buttonPoses[28] = buttonPos35_49; buttonPoses[29] = buttonPos37_49; buttonPoses[30] = buttonPos38_49; buttonPoses[31] = buttonPos39_49; buttonPoses[32] = buttonPos40_49; buttonPoses[33] = buttonPos41_49; buttonPoses[34] = buttonPos45_49; buttonPoses[35] = buttonPos46_49; buttonPoses[36] = buttonPos47_49;
        } else if (buttonString.Count == 38) { buttonPoses[0] = buttonPos03_49; buttonPoses[1] = buttonPos05_49; buttonPoses[2] = buttonPos08_49; buttonPoses[3] = buttonPos09_49; buttonPoses[4] = buttonPos10_49; buttonPoses[5] = buttonPos11_49; buttonPoses[6] = buttonPos12_49; buttonPoses[7] = buttonPos13_49; buttonPoses[8] = buttonPos14_49; buttonPoses[9] = buttonPos15_49; buttonPoses[10] = buttonPos16_49; buttonPoses[11] = buttonPos17_49; buttonPoses[12] = buttonPos18_49; buttonPoses[13] = buttonPos19_49; buttonPoses[14] = buttonPos20_49; buttonPoses[15] = buttonPos21_49; buttonPoses[16] = buttonPos22_49; buttonPoses[17] = buttonPos23_49; buttonPoses[18] = buttonPos24_49; buttonPoses[19] = buttonPos25_49; buttonPoses[20] = buttonPos26_49; buttonPoses[21] = buttonPos27_49; buttonPoses[22] = buttonPos28_49; buttonPoses[23] = buttonPos29_49; buttonPoses[24] = buttonPos30_49; buttonPoses[25] = buttonPos31_49; buttonPoses[26] = buttonPos32_49; buttonPoses[27] = buttonPos33_49; buttonPoses[28] = buttonPos34_49; buttonPoses[29] = buttonPos35_49; buttonPoses[30] = buttonPos37_49; buttonPoses[31] = buttonPos38_49; buttonPoses[32] = buttonPos39_49; buttonPoses[33] = buttonPos40_49; buttonPoses[34] = buttonPos41_49; buttonPoses[35] = buttonPos45_49; buttonPoses[36] = buttonPos46_49; buttonPoses[37] = buttonPos47_49;
        } else if (buttonString.Count == 39) { buttonPoses[0] = buttonPos03_49; buttonPoses[1] = buttonPos04_49; buttonPoses[2] = buttonPos05_49; buttonPoses[3] = buttonPos08_49; buttonPoses[4] = buttonPos09_49; buttonPoses[5] = buttonPos10_49; buttonPoses[6] = buttonPos11_49; buttonPoses[7] = buttonPos12_49; buttonPoses[8] = buttonPos13_49; buttonPoses[9] = buttonPos14_49; buttonPoses[10] = buttonPos15_49; buttonPoses[11] = buttonPos16_49; buttonPoses[12] = buttonPos17_49; buttonPoses[13] = buttonPos18_49; buttonPoses[14] = buttonPos19_49; buttonPoses[15] = buttonPos20_49; buttonPoses[16] = buttonPos21_49; buttonPoses[17] = buttonPos22_49; buttonPoses[18] = buttonPos23_49; buttonPoses[19] = buttonPos24_49; buttonPoses[20] = buttonPos25_49; buttonPoses[21] = buttonPos26_49; buttonPoses[22] = buttonPos27_49; buttonPoses[23] = buttonPos28_49; buttonPoses[24] = buttonPos29_49; buttonPoses[25] = buttonPos30_49; buttonPoses[26] = buttonPos31_49; buttonPoses[27] = buttonPos32_49; buttonPoses[28] = buttonPos33_49; buttonPoses[29] = buttonPos34_49; buttonPoses[30] = buttonPos35_49; buttonPoses[31] = buttonPos37_49; buttonPoses[32] = buttonPos38_49; buttonPoses[33] = buttonPos39_49; buttonPoses[34] = buttonPos40_49; buttonPoses[35] = buttonPos41_49; buttonPoses[36] = buttonPos45_49; buttonPoses[37] = buttonPos46_49; buttonPoses[38] = buttonPos47_49;
        } else if (buttonString.Count == 40) { buttonPoses[0] = buttonPos03_49; buttonPoses[1] = buttonPos05_49; buttonPoses[2] = buttonPos08_49; buttonPoses[3] = buttonPos09_49; buttonPoses[4] = buttonPos10_49; buttonPoses[5] = buttonPos11_49; buttonPoses[6] = buttonPos12_49; buttonPoses[7] = buttonPos13_49; buttonPoses[8] = buttonPos14_49; buttonPoses[9] = buttonPos15_49; buttonPoses[10] = buttonPos16_49; buttonPoses[11] = buttonPos17_49; buttonPoses[12] = buttonPos18_49; buttonPoses[13] = buttonPos19_49; buttonPoses[14] = buttonPos20_49; buttonPoses[15] = buttonPos21_49; buttonPoses[16] = buttonPos22_49; buttonPoses[17] = buttonPos23_49; buttonPoses[18] = buttonPos24_49; buttonPoses[19] = buttonPos25_49; buttonPoses[20] = buttonPos26_49; buttonPoses[21] = buttonPos27_49; buttonPoses[22] = buttonPos28_49; buttonPoses[23] = buttonPos29_49; buttonPoses[24] = buttonPos30_49; buttonPoses[25] = buttonPos31_49; buttonPoses[26] = buttonPos32_49; buttonPoses[27] = buttonPos33_49; buttonPoses[28] = buttonPos34_49; buttonPoses[29] = buttonPos35_49; buttonPoses[30] = buttonPos36_49; buttonPoses[31] = buttonPos37_49; buttonPoses[32] = buttonPos38_49; buttonPoses[33] = buttonPos39_49; buttonPoses[34] = buttonPos40_49; buttonPoses[35] = buttonPos41_49; buttonPoses[36] = buttonPos42_49; buttonPoses[37] = buttonPos45_49; buttonPoses[38] = buttonPos46_49; buttonPoses[39] = buttonPos47_49;
        } else if (buttonString.Count == 41) { buttonPoses[0] = buttonPos03_49; buttonPoses[1] = buttonPos04_49; buttonPoses[2] = buttonPos05_49; buttonPoses[3] = buttonPos08_49; buttonPoses[4] = buttonPos09_49; buttonPoses[5] = buttonPos10_49; buttonPoses[6] = buttonPos11_49; buttonPoses[7] = buttonPos12_49; buttonPoses[8] = buttonPos13_49; buttonPoses[9] = buttonPos14_49; buttonPoses[10] = buttonPos15_49; buttonPoses[11] = buttonPos16_49; buttonPoses[12] = buttonPos17_49; buttonPoses[13] = buttonPos18_49; buttonPoses[14] = buttonPos19_49; buttonPoses[15] = buttonPos20_49; buttonPoses[16] = buttonPos21_49; buttonPoses[17] = buttonPos22_49; buttonPoses[18] = buttonPos23_49; buttonPoses[19] = buttonPos24_49; buttonPoses[20] = buttonPos25_49; buttonPoses[21] = buttonPos26_49; buttonPoses[22] = buttonPos27_49; buttonPoses[23] = buttonPos28_49; buttonPoses[24] = buttonPos29_49; buttonPoses[25] = buttonPos30_49; buttonPoses[26] = buttonPos31_49; buttonPoses[27] = buttonPos32_49; buttonPoses[28] = buttonPos33_49; buttonPoses[29] = buttonPos34_49; buttonPoses[30] = buttonPos35_49; buttonPoses[31] = buttonPos36_49; buttonPoses[32] = buttonPos37_49; buttonPoses[33] = buttonPos38_49; buttonPoses[34] = buttonPos39_49; buttonPoses[35] = buttonPos40_49; buttonPoses[36] = buttonPos41_49; buttonPoses[37] = buttonPos42_49; buttonPoses[38] = buttonPos45_49; buttonPoses[39] = buttonPos46_49; buttonPoses[40] = buttonPos47_49;
        } else if (buttonString.Count == 42) { buttonPoses[0] = buttonPos02_49; buttonPoses[1] = buttonPos03_49; buttonPoses[2] = buttonPos05_49; buttonPoses[3] = buttonPos06_49; buttonPoses[4] = buttonPos08_49; buttonPoses[5] = buttonPos09_49; buttonPoses[6] = buttonPos10_49; buttonPoses[7] = buttonPos11_49; buttonPoses[8] = buttonPos12_49; buttonPoses[9] = buttonPos13_49; buttonPoses[10] = buttonPos14_49; buttonPoses[11] = buttonPos15_49; buttonPoses[12] = buttonPos16_49; buttonPoses[13] = buttonPos17_49; buttonPoses[14] = buttonPos18_49; buttonPoses[15] = buttonPos19_49; buttonPoses[16] = buttonPos20_49; buttonPoses[17] = buttonPos21_49; buttonPoses[18] = buttonPos22_49; buttonPoses[19] = buttonPos23_49; buttonPoses[20] = buttonPos24_49; buttonPoses[21] = buttonPos25_49; buttonPoses[22] = buttonPos26_49; buttonPoses[23] = buttonPos27_49; buttonPoses[24] = buttonPos28_49; buttonPoses[25] = buttonPos29_49; buttonPoses[26] = buttonPos30_49; buttonPoses[27] = buttonPos31_49; buttonPoses[28] = buttonPos32_49; buttonPoses[29] = buttonPos33_49; buttonPoses[30] = buttonPos34_49; buttonPoses[31] = buttonPos35_49; buttonPoses[32] = buttonPos36_49; buttonPoses[33] = buttonPos37_49; buttonPoses[34] = buttonPos38_49; buttonPoses[35] = buttonPos39_49; buttonPoses[36] = buttonPos40_49; buttonPoses[37] = buttonPos41_49; buttonPoses[38] = buttonPos42_49; buttonPoses[39] = buttonPos45_49; buttonPoses[40] = buttonPos46_49; buttonPoses[41] = buttonPos47_49;
        } else if (buttonString.Count == 43) { buttonPoses[0] = buttonPos02_49; buttonPoses[1] = buttonPos03_49; buttonPoses[2] = buttonPos05_49; buttonPoses[3] = buttonPos06_49; buttonPoses[4] = buttonPos08_49; buttonPoses[5] = buttonPos09_49; buttonPoses[6] = buttonPos10_49; buttonPoses[7] = buttonPos11_49; buttonPoses[8] = buttonPos12_49; buttonPoses[9] = buttonPos13_49; buttonPoses[10] = buttonPos14_49; buttonPoses[11] = buttonPos15_49; buttonPoses[12] = buttonPos16_49; buttonPoses[13] = buttonPos17_49; buttonPoses[14] = buttonPos18_49; buttonPoses[15] = buttonPos19_49; buttonPoses[16] = buttonPos20_49; buttonPoses[17] = buttonPos21_49; buttonPoses[18] = buttonPos22_49; buttonPoses[19] = buttonPos23_49; buttonPoses[20] = buttonPos24_49; buttonPoses[21] = buttonPos25_49; buttonPoses[22] = buttonPos26_49; buttonPoses[23] = buttonPos27_49; buttonPoses[24] = buttonPos28_49; buttonPoses[25] = buttonPos29_49; buttonPoses[26] = buttonPos30_49; buttonPoses[27] = buttonPos31_49; buttonPoses[28] = buttonPos32_49; buttonPoses[29] = buttonPos33_49; buttonPoses[30] = buttonPos34_49; buttonPoses[31] = buttonPos35_49; buttonPoses[32] = buttonPos36_49; buttonPoses[33] = buttonPos37_49; buttonPoses[34] = buttonPos38_49; buttonPoses[35] = buttonPos39_49; buttonPoses[36] = buttonPos40_49; buttonPoses[37] = buttonPos41_49; buttonPoses[38] = buttonPos42_49; buttonPoses[39] = buttonPos44_49; buttonPoses[40] = buttonPos45_49; buttonPoses[41] = buttonPos47_49; buttonPoses[42] = buttonPos48_49;
        } else if (buttonString.Count == 44) { buttonPoses[0] = buttonPos02_49; buttonPoses[1] = buttonPos03_49; buttonPoses[2] = buttonPos05_49; buttonPoses[3] = buttonPos06_49; buttonPoses[4] = buttonPos08_49; buttonPoses[5] = buttonPos09_49; buttonPoses[6] = buttonPos10_49; buttonPoses[7] = buttonPos11_49; buttonPoses[8] = buttonPos12_49; buttonPoses[9] = buttonPos13_49; buttonPoses[10] = buttonPos14_49; buttonPoses[11] = buttonPos15_49; buttonPoses[12] = buttonPos16_49; buttonPoses[13] = buttonPos17_49; buttonPoses[14] = buttonPos18_49; buttonPoses[15] = buttonPos19_49; buttonPoses[16] = buttonPos20_49; buttonPoses[17] = buttonPos21_49; buttonPoses[18] = buttonPos22_49; buttonPoses[19] = buttonPos23_49; buttonPoses[20] = buttonPos24_49; buttonPoses[21] = buttonPos25_49; buttonPoses[22] = buttonPos26_49; buttonPoses[23] = buttonPos27_49; buttonPoses[24] = buttonPos28_49; buttonPoses[25] = buttonPos29_49; buttonPoses[26] = buttonPos30_49; buttonPoses[27] = buttonPos31_49; buttonPoses[28] = buttonPos32_49; buttonPoses[29] = buttonPos33_49; buttonPoses[30] = buttonPos34_49; buttonPoses[31] = buttonPos35_49; buttonPoses[32] = buttonPos36_49; buttonPoses[33] = buttonPos37_49; buttonPoses[34] = buttonPos38_49; buttonPoses[35] = buttonPos39_49; buttonPoses[36] = buttonPos40_49; buttonPoses[37] = buttonPos41_49; buttonPoses[38] = buttonPos42_49; buttonPoses[39] = buttonPos44_49; buttonPoses[40] = buttonPos45_49; buttonPoses[41] = buttonPos46_49; buttonPoses[42] = buttonPos47_49; buttonPoses[43] = buttonPos48_49;
        } else if (buttonString.Count == 45) { buttonPoses[0] = buttonPos02_49; buttonPoses[1] = buttonPos03_49; buttonPoses[2] = buttonPos04_49; buttonPoses[3] = buttonPos05_49; buttonPoses[4] = buttonPos06_49; buttonPoses[5] = buttonPos08_49; buttonPoses[6] = buttonPos09_49; buttonPoses[7] = buttonPos10_49; buttonPoses[8] = buttonPos11_49; buttonPoses[9] = buttonPos12_49; buttonPoses[10] = buttonPos13_49; buttonPoses[11] = buttonPos14_49; buttonPoses[12] = buttonPos15_49; buttonPoses[13] = buttonPos16_49; buttonPoses[14] = buttonPos17_49; buttonPoses[15] = buttonPos18_49; buttonPoses[16] = buttonPos19_49; buttonPoses[17] = buttonPos20_49; buttonPoses[18] = buttonPos21_49; buttonPoses[19] = buttonPos22_49; buttonPoses[20] = buttonPos23_49; buttonPoses[21] = buttonPos24_49; buttonPoses[22] = buttonPos25_49; buttonPoses[23] = buttonPos26_49; buttonPoses[24] = buttonPos27_49; buttonPoses[25] = buttonPos28_49; buttonPoses[26] = buttonPos29_49; buttonPoses[27] = buttonPos30_49; buttonPoses[28] = buttonPos31_49; buttonPoses[29] = buttonPos32_49; buttonPoses[30] = buttonPos33_49; buttonPoses[31] = buttonPos34_49; buttonPoses[32] = buttonPos35_49; buttonPoses[33] = buttonPos36_49; buttonPoses[34] = buttonPos37_49; buttonPoses[35] = buttonPos38_49; buttonPoses[36] = buttonPos39_49; buttonPoses[37] = buttonPos40_49; buttonPoses[38] = buttonPos41_49; buttonPoses[39] = buttonPos42_49; buttonPoses[40] = buttonPos44_49; buttonPoses[41] = buttonPos45_49; buttonPoses[42] = buttonPos46_49; buttonPoses[43] = buttonPos47_49; buttonPoses[44] = buttonPos48_49;
        } else if (buttonString.Count == 46) { buttonPoses[0] = buttonPos01_49; buttonPoses[1] = buttonPos02_49; buttonPoses[2] = buttonPos03_49; buttonPoses[3] = buttonPos05_49; buttonPoses[4] = buttonPos06_49; buttonPoses[5] = buttonPos07_49; buttonPoses[6] = buttonPos08_49; buttonPoses[7] = buttonPos09_49; buttonPoses[8] = buttonPos10_49; buttonPoses[9] = buttonPos11_49; buttonPoses[10] = buttonPos12_49; buttonPoses[11] = buttonPos13_49; buttonPoses[12] = buttonPos14_49; buttonPoses[13] = buttonPos15_49; buttonPoses[14] = buttonPos16_49; buttonPoses[15] = buttonPos17_49; buttonPoses[16] = buttonPos18_49; buttonPoses[17] = buttonPos19_49; buttonPoses[18] = buttonPos20_49; buttonPoses[19] = buttonPos21_49; buttonPoses[20] = buttonPos22_49; buttonPoses[21] = buttonPos23_49; buttonPoses[22] = buttonPos24_49; buttonPoses[23] = buttonPos25_49; buttonPoses[24] = buttonPos26_49; buttonPoses[25] = buttonPos27_49; buttonPoses[26] = buttonPos28_49; buttonPoses[27] = buttonPos29_49; buttonPoses[28] = buttonPos30_49; buttonPoses[29] = buttonPos31_49; buttonPoses[30] = buttonPos32_49; buttonPoses[31] = buttonPos33_49; buttonPoses[32] = buttonPos34_49; buttonPoses[33] = buttonPos35_49; buttonPoses[34] = buttonPos36_49; buttonPoses[35] = buttonPos37_49; buttonPoses[36] = buttonPos38_49; buttonPoses[37] = buttonPos39_49; buttonPoses[38] = buttonPos40_49; buttonPoses[39] = buttonPos41_49; buttonPoses[40] = buttonPos42_49; buttonPoses[41] = buttonPos44_49; buttonPoses[42] = buttonPos45_49; buttonPoses[43] = buttonPos46_49; buttonPoses[44] = buttonPos47_49; buttonPoses[45] = buttonPos48_49;
        } else if (buttonString.Count == 47) { buttonPoses[0] = buttonPos01_49; buttonPoses[1] = buttonPos02_49; buttonPoses[2] = buttonPos03_49; buttonPoses[3] = buttonPos05_49; buttonPoses[4] = buttonPos06_49; buttonPoses[5] = buttonPos07_49; buttonPoses[6] = buttonPos08_49; buttonPoses[7] = buttonPos09_49; buttonPoses[8] = buttonPos10_49; buttonPoses[9] = buttonPos11_49; buttonPoses[10] = buttonPos12_49; buttonPoses[11] = buttonPos13_49; buttonPoses[12] = buttonPos14_49; buttonPoses[13] = buttonPos15_49; buttonPoses[14] = buttonPos16_49; buttonPoses[15] = buttonPos17_49; buttonPoses[16] = buttonPos18_49; buttonPoses[17] = buttonPos19_49; buttonPoses[18] = buttonPos20_49; buttonPoses[19] = buttonPos21_49; buttonPoses[20] = buttonPos22_49; buttonPoses[21] = buttonPos23_49; buttonPoses[22] = buttonPos24_49; buttonPoses[23] = buttonPos25_49; buttonPoses[24] = buttonPos26_49; buttonPoses[25] = buttonPos27_49; buttonPoses[26] = buttonPos28_49; buttonPoses[27] = buttonPos29_49; buttonPoses[28] = buttonPos30_49; buttonPoses[29] = buttonPos31_49; buttonPoses[30] = buttonPos32_49; buttonPoses[31] = buttonPos33_49; buttonPoses[32] = buttonPos34_49; buttonPoses[33] = buttonPos35_49; buttonPoses[34] = buttonPos36_49; buttonPoses[35] = buttonPos37_49; buttonPoses[36] = buttonPos38_49; buttonPoses[37] = buttonPos39_49; buttonPoses[38] = buttonPos40_49; buttonPoses[39] = buttonPos41_49; buttonPoses[40] = buttonPos42_49; buttonPoses[41] = buttonPos43_49; buttonPoses[42] = buttonPos44_49; buttonPoses[43] = buttonPos45_49; buttonPoses[44] = buttonPos47_49; buttonPoses[45] = buttonPos48_49; buttonPoses[46] = buttonPos49_49;
        } else if (buttonString.Count == 48) { buttonPoses[0] = buttonPos01_49; buttonPoses[1] = buttonPos02_49; buttonPoses[2] = buttonPos03_49; buttonPoses[3] = buttonPos05_49; buttonPoses[4] = buttonPos06_49; buttonPoses[5] = buttonPos07_49; buttonPoses[6] = buttonPos08_49; buttonPoses[7] = buttonPos09_49; buttonPoses[8] = buttonPos10_49; buttonPoses[9] = buttonPos11_49; buttonPoses[10] = buttonPos12_49; buttonPoses[11] = buttonPos13_49; buttonPoses[12] = buttonPos14_49; buttonPoses[13] = buttonPos15_49; buttonPoses[14] = buttonPos16_49; buttonPoses[15] = buttonPos17_49; buttonPoses[16] = buttonPos18_49; buttonPoses[17] = buttonPos19_49; buttonPoses[18] = buttonPos20_49; buttonPoses[19] = buttonPos21_49; buttonPoses[20] = buttonPos22_49; buttonPoses[21] = buttonPos23_49; buttonPoses[22] = buttonPos24_49; buttonPoses[23] = buttonPos25_49; buttonPoses[24] = buttonPos26_49; buttonPoses[25] = buttonPos27_49; buttonPoses[26] = buttonPos28_49; buttonPoses[27] = buttonPos29_49; buttonPoses[28] = buttonPos30_49; buttonPoses[29] = buttonPos31_49; buttonPoses[30] = buttonPos32_49; buttonPoses[31] = buttonPos33_49; buttonPoses[32] = buttonPos34_49; buttonPoses[33] = buttonPos35_49; buttonPoses[34] = buttonPos36_49; buttonPoses[35] = buttonPos37_49; buttonPoses[36] = buttonPos38_49; buttonPoses[37] = buttonPos39_49; buttonPoses[38] = buttonPos40_49; buttonPoses[39] = buttonPos41_49; buttonPoses[40] = buttonPos42_49; buttonPoses[41] = buttonPos43_49; buttonPoses[42] = buttonPos44_49; buttonPoses[43] = buttonPos45_49; buttonPoses[44] = buttonPos46_49; buttonPoses[45] = buttonPos47_49; buttonPoses[46] = buttonPos48_49; buttonPoses[47] = buttonPos49_49;
        } else if (buttonString.Count == 49) { buttonPoses[0] = buttonPos01_49; buttonPoses[1] = buttonPos02_49; buttonPoses[2] = buttonPos03_49; buttonPoses[3] = buttonPos04_49; buttonPoses[4] = buttonPos05_49; buttonPoses[5] = buttonPos06_49; buttonPoses[6] = buttonPos07_49; buttonPoses[7] = buttonPos08_49; buttonPoses[8] = buttonPos09_49; buttonPoses[9] = buttonPos10_49; buttonPoses[10] = buttonPos11_49; buttonPoses[11] = buttonPos12_49; buttonPoses[12] = buttonPos13_49; buttonPoses[13] = buttonPos14_49; buttonPoses[14] = buttonPos15_49; buttonPoses[15] = buttonPos16_49; buttonPoses[16] = buttonPos17_49; buttonPoses[17] = buttonPos18_49; buttonPoses[18] = buttonPos19_49; buttonPoses[19] = buttonPos20_49; buttonPoses[20] = buttonPos21_49; buttonPoses[21] = buttonPos22_49; buttonPoses[22] = buttonPos23_49; buttonPoses[23] = buttonPos24_49; buttonPoses[24] = buttonPos25_49; buttonPoses[25] = buttonPos26_49; buttonPoses[26] = buttonPos27_49; buttonPoses[27] = buttonPos28_49; buttonPoses[28] = buttonPos29_49; buttonPoses[29] = buttonPos30_49; buttonPoses[30] = buttonPos31_49; buttonPoses[31] = buttonPos32_49; buttonPoses[32] = buttonPos33_49; buttonPoses[33] = buttonPos34_49; buttonPoses[34] = buttonPos35_49; buttonPoses[35] = buttonPos36_49; buttonPoses[36] = buttonPos37_49; buttonPoses[37] = buttonPos38_49; buttonPoses[38] = buttonPos39_49; buttonPoses[39] = buttonPos40_49; buttonPoses[40] = buttonPos41_49; buttonPoses[41] = buttonPos42_49; buttonPoses[42] = buttonPos43_49; buttonPoses[43] = buttonPos44_49; buttonPoses[44] = buttonPos45_49; buttonPoses[45] = buttonPos46_49; buttonPoses[46] = buttonPos47_49; buttonPoses[47] = buttonPos48_49; buttonPoses[48] = buttonPos49_49;
        } else if (buttonString.Count == 50) { buttonPoses[0] = buttonPos03_64; buttonPoses[1] = buttonPos04_64; buttonPoses[2] = buttonPos05_64; buttonPoses[3] = buttonPos06_64; buttonPoses[4] = buttonPos10_64; buttonPoses[5] = buttonPos11_64; buttonPoses[6] = buttonPos12_64; buttonPoses[7] = buttonPos13_64; buttonPoses[8] = buttonPos14_64; buttonPoses[9] = buttonPos15_64; buttonPoses[10] = buttonPos17_64; buttonPoses[11] = buttonPos18_64; buttonPoses[12] = buttonPos19_64; buttonPoses[13] = buttonPos20_64; buttonPoses[14] = buttonPos21_64; buttonPoses[15] = buttonPos22_64; buttonPoses[16] = buttonPos23_64; buttonPoses[17] = buttonPos24_64; buttonPoses[18] = buttonPos25_64; buttonPoses[19] = buttonPos26_64; buttonPoses[20] = buttonPos27_64; buttonPoses[21] = buttonPos28_64; buttonPoses[22] = buttonPos29_64; buttonPoses[23] = buttonPos30_64; buttonPoses[24] = buttonPos31_64; buttonPoses[25] = buttonPos32_64; buttonPoses[26] = buttonPos33_64; buttonPoses[27] = buttonPos34_64; buttonPoses[28] = buttonPos35_64; buttonPoses[29] = buttonPos36_64; buttonPoses[30] = buttonPos37_64; buttonPoses[31] = buttonPos38_64; buttonPoses[32] = buttonPos39_64; buttonPoses[33] = buttonPos40_64; buttonPoses[34] = buttonPos41_64; buttonPoses[35] = buttonPos42_64; buttonPoses[36] = buttonPos43_64; buttonPoses[37] = buttonPos44_64; buttonPoses[38] = buttonPos45_64; buttonPoses[39] = buttonPos46_64; buttonPoses[40] = buttonPos47_64; buttonPoses[41] = buttonPos48_64; buttonPoses[42] = buttonPos50_64; buttonPoses[43] = buttonPos51_64; buttonPoses[44] = buttonPos52_64; buttonPoses[45] = buttonPos53_64; buttonPoses[46] = buttonPos54_64; buttonPoses[47] = buttonPos55_64; buttonPoses[48] = buttonPos60_64; buttonPoses[49] = buttonPos61_64;
        } else if (buttonString.Count >= 51) { buttonPoses[0] = buttonPos01_64; buttonPoses[1] = buttonPos02_64; buttonPoses[2] = buttonPos03_64; buttonPoses[3] = buttonPos04_64; buttonPoses[4] = buttonPos05_64; buttonPoses[5] = buttonPos06_64; buttonPoses[6] = buttonPos07_64; buttonPoses[7] = buttonPos08_64; buttonPoses[8] = buttonPos09_64; buttonPoses[9] = buttonPos10_64; buttonPoses[10] = buttonPos11_64; buttonPoses[11] = buttonPos12_64; buttonPoses[12] = buttonPos13_64; buttonPoses[13] = buttonPos14_64; buttonPoses[14] = buttonPos15_64; buttonPoses[15] = buttonPos16_64; buttonPoses[16] = buttonPos17_64; buttonPoses[17] = buttonPos18_64; buttonPoses[18] = buttonPos19_64; buttonPoses[19] = buttonPos20_64; buttonPoses[20] = buttonPos21_64; buttonPoses[21] = buttonPos22_64; buttonPoses[22] = buttonPos23_64; buttonPoses[23] = buttonPos24_64; buttonPoses[24] = buttonPos25_64; buttonPoses[25] = buttonPos26_64; buttonPoses[26] = buttonPos27_64; buttonPoses[27] = buttonPos28_64; buttonPoses[28] = buttonPos29_64; buttonPoses[29] = buttonPos30_64; buttonPoses[30] = buttonPos31_64; buttonPoses[31] = buttonPos32_64; buttonPoses[32] = buttonPos33_64; buttonPoses[33] = buttonPos34_64; buttonPoses[34] = buttonPos35_64; buttonPoses[35] = buttonPos36_64; buttonPoses[36] = buttonPos37_64; buttonPoses[37] = buttonPos38_64; buttonPoses[38] = buttonPos39_64; buttonPoses[39] = buttonPos40_64; buttonPoses[40] = buttonPos41_64; buttonPoses[41] = buttonPos42_64; buttonPoses[42] = buttonPos43_64; buttonPoses[43] = buttonPos44_64; buttonPoses[44] = buttonPos45_64; buttonPoses[45] = buttonPos46_64; buttonPoses[46] = buttonPos47_64; buttonPoses[47] = buttonPos48_64; buttonPoses[48] = buttonPos49_64; buttonPoses[49] = buttonPos50_64; buttonPoses[50] = buttonPos51_64; buttonPoses[51] = buttonPos52_64; buttonPoses[52] = buttonPos53_64; buttonPoses[53] = buttonPos54_64; buttonPoses[54] = buttonPos55_64; buttonPoses[55] = buttonPos56_64; buttonPoses[56] = buttonPos57_64; buttonPoses[57] = buttonPos58_64; buttonPoses[58] = buttonPos59_64; buttonPoses[59] = buttonPos60_64; buttonPoses[60] = buttonPos61_64; buttonPoses[61] = buttonPos62_64; buttonPoses[62] = buttonPos63_64; buttonPoses[63] = buttonPos64_64;
        }
        if (buttonString.Count <= 9) {
            for (j1 = 0; j1 < buttonString.Count; j1++) { buttons[j1].sizeDelta = buttonSize9; }
        } else if (buttonString.Count <= 16) {
            for (j1 = 0; j1 < buttonString.Count; j1++) { buttons[j1].sizeDelta = buttonSize16; }
        } else if (buttonString.Count <= 25) {
            for (j1 = 0; j1 < buttonString.Count; j1++) { buttons[j1].sizeDelta = buttonSize25; }
        } else if (buttonString.Count <= 36) {
            for (j1 = 0; j1 < buttonString.Count; j1++) { buttons[j1].sizeDelta = buttonSize36; }
        } else if (buttonString.Count <= 49) {
            for (j1 = 0; j1 < buttonString.Count; j1++) { buttons[j1].sizeDelta = buttonSize49; }
        } else {
            for (j1 = 0; j1 < buttonString.Count; j1++) {
                Debug.Log (j1.ToString () + " / " + buttonString.Count.ToString () + " / " + buttons.Count.ToString ());
                buttons[j1].sizeDelta = buttonSize64; }
        }
        for (j1 = 0; j1 < buttonString.Count; j1++) {
            buttonTexts[j1].text = buttonString[j1];
            buttons[j1].gameObject.SetActive(true);
        }
        for (j1 = buttonString.Count; j1 < buttons.Count; j1++) {
            buttons[j1].gameObject.SetActive(false);
        }
        buttonContainer.SetActive(true);
    }


    /// <summary>
    /// Sets the level and calls EntryQuery - Called by NewEntry button Unity, currently always with the parameter = 1
    /// </summary>
    /// <param name="levelID">value to assign to this.level</param>
    public void EntryQueryCommand(int levelID) {
        inputTimer = 0;
        level = levelID;
        PopulateButtonsWithReasonCodes ();
        cancelButton.gameObject.SetActive (true);
        backButton.gameObject.SetActive   (true);
        //StartCoroutine(EntryQuery());
    }


    /// <summary>
    /// Prepares to save downtime entries to the database
    /// </summary>
    public void NotesTextChanged() {
        inputTimer = 0;
        //Debug.Log("NotesTextChanged");
        saveDelay = 5;
    }


    /// <summary>
    /// Checks if the minutes accounted for match the total downtime minutes
    /// If they match, prepares to save the downtime entries to the database
    /// </summary>
    public void MinutesTextChanged () {
        inputTimer = 0;
        runningCoroutines++;
        //Debug.Log("MinutesTextChanged");
        AddMinutes();
        if (accountedDowntime.color != Color.red) {
            saveDelay = 2;
        } else {
            saveDelay = 5;
        }
        runningCoroutines--;
    }

    /// <summary>
    /// Enables the "Copy last hour" button if: 
    /// there are no entries for the hour
    /// the last hour has at least 1 entry
    /// there aren't any accounted minutes of downtime in the current hour
    /// there is downtime in the current hour
    /// there is at least as much downtime in the current hour as there was in the previous 
    /// </summary>
    public void CheckCopyButton () {
        if (entryButtons[1].transform.parent.gameObject.activeSelf || hours [hourID+1].dtMinutes <= 0 || accountedDtMinutes > 0) {
            copyLastHourButton.SetActive(false);
        } else {
            copyLastHourButton.SetActive(true);
        }
    }


    public void PopulateStatesNew () {
        string shiftHoursAndProdString = "";
        string shiftCasesString        = "";
        string shiftEqString           = "";
        string shiftScrapString        = "";
        string shiftEfString           = "";
        
        for (int i = hourID; i < hours.Count; i++) {
            shiftHoursAndProdString += hours [i].hourText + "  " + hours [i].product + "\n";
            shiftCasesString        += hours [i].cases.ToString () + "\n";
            shiftEqString           += hours [i].eqCases.ToString () + "\n";
            shiftScrapString        += hours [i].scrap.ToString () + "\n";
            shiftEfString           += hours [i].oem.ToString ("0") + "\n";
        }
        
        shiftHoursAndProd.text = shiftHoursAndProdString;
        shiftCases.text = shiftCasesString;
        shiftEq.text = shiftEqString;
        shiftScrap.text = shiftScrapString;
        shiftEf.text = shiftEfString;
        
        dtMinutes         = hours [hourID].dtMinutes;
        materialID        = hours [hourID].materialID;
        baseID            = hours [hourID].baseID;
        materialText.text = hours [hourID].product;
        
        AddMinutes ();
        
        if (firstStart) {
            StartCoroutine (DownloadDowntimeEntrys ());
        }
    }


    public IEnumerator DownloadHours () {
        if (hourID == 0) { hourDownloadTimer = 60; } else { hourDownloadTimer = 300; }
        while (SqlJobManager.Instance.sqlRunning) {
            if (hourID == 0) { hourDownloadTimer = 60; } else { hourDownloadTimer = 300; }
            yield return  null;
        }
        SqlJobManager.Instance.StartJob();
        yield return null;
        using (SqlConnection dbCon = new SqlConnection(connectionString)) {
            SqlCommand cmd = new SqlCommand("select top (72) [Hour].[HourLocal], [Hour].[HourUTC], [Hour].[BaseID], IsNull ([Hour].[FGID],[Hour].[BaseID]) [MaterialID], [IM].[ShortDescription], isNull(cast(Floor ([Hour].[Produced] / [Hour].[BPC]) as int),0) [Cases], cast(isNull(Floor ([Hour].[Produced] / [EQ].[BottlesPerEQ]),0) as int) [EQ], cast([Hour].[Scrapped] as int) [Scrap], cast([Hour].[DtMinutes] as int) [DtMinutes], cast (isNull([Hour].[AccMinutes],0) as int) [AccMinutes], [Eff] [OEMe] from [FairlifeOperations].[dbo].[OpInsightsHourlyLog] [Hour] left join [FairlifeMaster].[dbo].[ItemMaster] [IM] on [IM].[ItemNum] = IsNull ([Hour].[FGID],[Hour].[BaseID]) left join [FairlifeMaster].[dbo].[CaseEQbySize] [EQ] on [EQ].[Size] = [IM].[VolumePerUnit] where [Line] = "+line.ToString ()+" order by [HourUTC] desc", dbCon);
            //SqlCommand cmd = new SqlCommand("select top (72) format ([Hour].[HourLocal], 'MM/dd HH') [Hour], [IM].[ShortDescription], Floor ([Hour].[Produced] / [Hour].[BPC]) [Cases], [Hour].[Scrapped], [Hour].[DtMinutes], [Hour].[AccMinutes], [Eff] [OEMe] from [FairlifeOperations].[dbo].[OpInsightsHourlyLog] [Hour] inner join [FairlifeMaster].[dbo].[ItemMaster] [IM] on [IM].[ItemNum] = IsNull ([Hour].[FGID],[Hour].[BaseID]) where [Line] = "+line.ToString ()+" order by [HourUTC] desc", dbCon);
            try {
                dbCon.Open();
                reader = cmd.ExecuteReader();
                if (reader.HasRows) {
                    int i = 0;
                    while (reader.Read()) {
                        if (hours.Count <= i) { hours.Add (new Hour ()); }
                        
                        try {hours [i].hour       = (DateTime)reader.GetSqlDateTime( 0);             } catch (InvalidCastException exception) {hours [i].hour       = DateTime.Now;   if (debug) {Debug.Log (exception);}}   
                        try {hours [i].hourUTC    = (DateTime)reader.GetSqlDateTime( 1);             } catch (InvalidCastException exception) {hours [i].hourUTC    = DateTime.UtcNow;if (debug) {Debug.Log (exception);}}
                        try {hours [i].baseID     = (int)     reader.GetSqlInt32   ( 2);             } catch (InvalidCastException exception) {hours [i].baseID     = 0;              if (debug) {Debug.Log (exception);}}
                        try {hours [i].materialID = (int)     reader.GetSqlInt32   ( 3);             } catch (InvalidCastException exception) {hours [i].materialID = 0;              if (debug) {Debug.Log (exception);}}
                        try {hours [i].product    =           reader.GetSqlString  ( 4).ToString (); } catch (InvalidCastException exception) {hours [i].product    = "-----";        if (debug) {Debug.Log (exception);}}
                        try {hours [i].cases      = (int)     reader.GetSqlInt32   ( 5);             } catch (InvalidCastException exception) {hours [i].cases      = 0;              if (debug) {Debug.Log (exception);}}
                        try {hours [i].eqCases    = (int)     reader.GetSqlInt32   ( 6);             } catch (InvalidCastException exception) {hours [i].eqCases    = 0;              if (debug) {Debug.Log (exception);}}
                        try {hours [i].scrap      = (int)     reader.GetSqlInt32   ( 7);             } catch (InvalidCastException exception) {hours [i].scrap      = 0;              if (debug) {Debug.Log (exception);}}
                        try {hours [i].dtMinutes  = (int)     reader.GetSqlInt32   ( 8);             } catch (InvalidCastException exception) {hours [i].dtMinutes  = 0;              if (debug) {Debug.Log (exception);}}
                        try {hours [i].accMinutes = (int)     reader.GetSqlInt32   ( 9);             } catch (InvalidCastException exception) {hours [i].accMinutes = 0;              if (debug) {Debug.Log (exception);}}
                        try {hours [i].oem        = (float)   reader.GetSqlDouble  (10);             } catch (InvalidCastException exception) {hours [i].oem        = 0;              if (debug) {Debug.Log (exception);}}
                        hours [i].hourText = hours [i].hour.ToString ("MM/dd HH");
                        i++;
                    }
                }
                reader.Close();
            } catch (SqlException exception){
                if (debug) {
                    Debug.LogError(exception.ToString());
                    Debug.Log(query);
                }
            }
        }
        yield return null;
        SqlJobManager.Instance.EndJob();
        if (hourID == 0) { hourDownloadTimer = 60; } else { hourDownloadTimer = 300; }
        if (hours [lastUnaccountedForHourID].hour != lastUnaccountedForHour) {
            for (int i = 0; i < hours.Count; i++) {
                if (hours [i].hour == lastUnaccountedForHour) {
                    lastUnaccountedForHourID = i;
                    break;
                }
            }                
        }
        if (hours [hourID].hour != selectedHour) {
            for (int i = 0; i < hours.Count; i++) {
                if (hours [i].hour == selectedHour) {
                    hourID = i;
                    break;
                }
            }                
        }
        AddMinutes();
        CheckCopyButton();
        yield return null;
    }
    
    
    public IEnumerator DownloadReasonCodes () {
        while (SqlJobManager.Instance.sqlRunning) {
            yield return  null;
        }
        SqlJobManager.Instance.StartJob();
        yield return null;
        string tempQuery = @"select     [Codes].[ID]                                         [ID],
                                        isNull(isNull([Masks].[Level1],[Codes].[Level1]),'') [Level1],
                                        isNull(isNull([Masks].[Level2],[Codes].[Level2]),'') [Level2],
                                        isNull(isNull([Masks].[Level3],[Codes].[Level3]),'') [Level3],
                                        isNull(isNull([Masks].[Level4],[Codes].[Level4]),'') [Level4],
                                        isNull(isNull([Masks].[Level5],[Codes].[Level5]),'') [Level5],
                                        isNull(isNull([Masks].[Notes],[Codes].[Notes]),'')   [Notes]
                             from       [FairlifeMaster].[dbo].[OpInsightsCodes]       [Codes]
                             inner join [FairlifeMaster].[dbo].[SiteMaster]            [Sites] on [Codes].[Active] = 1 and ([Codes].[SiteCode] = 0 or [Codes].[SiteCode] & POWER(2,[Sites].[ID]-1) > 0)
                             left  join [FairlifeMaster].[dbo].[OpInsightsCodes_Masks] [Masks] on [Masks].[SiteCode] & POWER(2,[Sites].[ID]-1) > 0 and [Codes].[ID] = [Masks].[ReasonID]
                             where      [Sites].[Abbreviation] = '"+site+@"'
                             order by   isNull(isNull([Masks].[Level1],[Codes].[Level1]),''),
                                        isNull(isNull([Masks].[Level2],[Codes].[Level2]),''),
                                        isNull(isNull([Masks].[Level3],[Codes].[Level3]),''),
                                        isNull(isNull([Masks].[Level4],[Codes].[Level4]),''),
                                        isNull(isNull([Masks].[Level5],[Codes].[Level5]),'')";
        using (SqlConnection dbCon = new SqlConnection(connectionString)) {
            SqlCommand cmd = new SqlCommand(tempQuery, dbCon);
            try {
                dbCon.Open();
                reader = cmd.ExecuteReader();
                if (reader.HasRows) {
                    int i = 0;
                    reasonCodes.Clear ();
                    while (reader.Read()) {
                        if (!reader.IsDBNull(0) && !reader.IsDBNull(1) && !reader.IsDBNull(2) && !reader.IsDBNull(3) && !reader.IsDBNull(4) && !reader.IsDBNull(5)) {
                            reasonCodes.Add (new ReasonCode ());
                            try {reasonCodes [i].ID     = (int)     reader.GetSqlInt32   ( 0);             } catch (InvalidCastException exception) {reasonCodes [i].ID     = 0;  if (debug) {Debug.Log (exception);}}
                            try {reasonCodes [i].level1 =           reader.GetSqlString  ( 1).ToString (); } catch (InvalidCastException exception) {reasonCodes [i].level1 = ""; if (debug) {Debug.Log (exception);}}
                            try {reasonCodes [i].level2 =           reader.GetSqlString  ( 2).ToString (); } catch (InvalidCastException exception) {reasonCodes [i].level2 = ""; if (debug) {Debug.Log (exception);}}
                            try {reasonCodes [i].level3 =           reader.GetSqlString  ( 3).ToString (); } catch (InvalidCastException exception) {reasonCodes [i].level3 = ""; if (debug) {Debug.Log (exception);}}
                            try {reasonCodes [i].level4 =           reader.GetSqlString  ( 4).ToString (); } catch (InvalidCastException exception) {reasonCodes [i].level4 = ""; if (debug) {Debug.Log (exception);}}
                            try {reasonCodes [i].level5 =           reader.GetSqlString  ( 5).ToString (); } catch (InvalidCastException exception) {reasonCodes [i].level5 = ""; if (debug) {Debug.Log (exception);}}
                            try {reasonCodes [i].notes  =           reader.GetSqlString  ( 6).ToString (); } catch (InvalidCastException exception) {reasonCodes [i].notes  = ""; if (debug) {Debug.Log (exception);}}
                            i++;
                        }
                    }
                }
                reader.Close();
            } catch (SqlException exception){
                if (debug) {
                    Debug.LogError(exception.ToString());
                    Debug.Log(query);
                }
            }
        }
        yield return null;
        SqlJobManager.Instance.EndJob();
        reasonDowmloadTimer = 3600;
        yield return null;
    }
    
    
    public IEnumerator DownloadDowntimeEntrys () {
        downtimeEntriesDownloadTimer = 300;
        
        while (SqlJobManager.Instance.sqlRunning) {
            yield return  null;
        }
        SqlJobManager.Instance.StartJob();
        yield return null;
        using (SqlConnection dbCon = new SqlConnection(connectionString)) {
            
            SqlCommand cmd = new SqlCommand("select [DateHourUTC],[DownTime],[Operator],[Level1],[Level2],[Level3],[Level4],[Level5],[OpNotes],[EngNotes] from [FairlifeOperations].[dbo].[OpInsights] where [Line] = "+line.ToString ()+" and [DateHourUTC] > DateAdd (hour, -48, GetUTCDate ()) and [Site] = '"+site+"' order by [DateHourUTC] desc", dbCon);
            try {
                dbCon.Open();
                reader = cmd.ExecuteReader();
                if (reader.HasRows) {
                    int i = 0;
                    while (reader.Read()) {
                        if (downtimeEntries.Count <= i) { downtimeEntries.Add (new DowntimeEntry ()); }
                        
                        try {downtimeEntries [i].hourUTC   = (DateTime)reader.GetSqlDateTime( 0);             } catch (InvalidCastException exception) {downtimeEntries [i].hourUTC       = DateTime.Now;if (debug) {Debug.Log (exception);}}
                        try {downtimeEntries [i].dtMinutes = (int)     reader.GetSqlInt32   ( 1);             } catch (InvalidCastException exception) {downtimeEntries [i].dtMinutes     = 0;           if (debug) {Debug.Log (exception);}}
                        try {downtimeEntries [i].initials  =           reader.GetSqlString  ( 2).ToString (); } catch (InvalidCastException exception) {downtimeEntries [i].initials      = "";          if (debug) {Debug.Log (exception);}}
                        try {downtimeEntries [i].level1    =           reader.GetSqlString  ( 3).ToString (); } catch (InvalidCastException exception) {downtimeEntries [i].level1        = "";          if (debug) {Debug.Log (exception);}}
                        try {downtimeEntries [i].level2    =           reader.GetSqlString  ( 4).ToString (); } catch (InvalidCastException exception) {downtimeEntries [i].level2        = "";          if (debug) {Debug.Log (exception);}}
                        try {downtimeEntries [i].level3    =           reader.GetSqlString  ( 5).ToString (); } catch (InvalidCastException exception) {downtimeEntries [i].level3        = "";          if (debug) {Debug.Log (exception);}}
                        try {downtimeEntries [i].level4    =           reader.GetSqlString  ( 6).ToString (); } catch (InvalidCastException exception) {downtimeEntries [i].level4        = "";          if (debug) {Debug.Log (exception);}}
                        try {downtimeEntries [i].level5    =           reader.GetSqlString  ( 7).ToString (); } catch (InvalidCastException exception) {downtimeEntries [i].level5        = "";          if (debug) {Debug.Log (exception);}}
                        try {downtimeEntries [i].opNotes   =           reader.GetSqlString  ( 8).ToString (); } catch (InvalidCastException exception) {downtimeEntries [i].opNotes       = "";          if (debug) {Debug.Log (exception);}}
                        try {downtimeEntries [i].engNotes  =           reader.GetSqlString  ( 9).ToString (); } catch (InvalidCastException exception) {downtimeEntries [i].engNotes      = "";          if (debug) {Debug.Log (exception);}}
                        DateTime.SpecifyKind (downtimeEntries [i].hourUTC, DateTimeKind.Utc);
                        downtimeEntries [i].hourLocal = downtimeEntries [i].hourUTC.ToLocalTime ();
                        downtimeEntries [i].display = i.ToString ("000") + downtimeEntries [i].hourLocal.ToString ("MM-dd  HH");
                        i++;
                    }
                    if (downtimeEntries.Count > i) { downtimeEntries.RemoveAt (i); }
                }
                reader.Close();
            } catch (SqlException exception){
                if (debug) {
                    Debug.LogError(exception.ToString());
                    Debug.Log(query);
                }
            }
        }
        yield return null;
        
        UpdateDowntimeEntrysText ();
        
        yield return null;
        
        SqlJobManager.Instance.EndJob();
        if (inputTimer > 10 || firstStart) {
            UpdateCurrentDowntimeEntrysFromList ();
            firstStart = false;
        } else {
            //Debug.Log ("There was user input, so we're not going to change the main reasons");
        }
        
        yield return null;
        downtimeEntriesDownloadTimer = 300;
        yield return null;
    }
    
    
    void UpdateDowntimeEntrysText () {
        string pastEntriesHoursString = "";
        string pastEntriesDTString = "";
        string pastEntriesOpString = "";
        string pastEntriesReasonsString = "";
        
        for (int i = 0; i < downtimeEntries.Count && i < 50; i++) {
            if (downtimeEntries [i].hourLocal <= hours [hourID].hour) {
                pastEntriesHoursString   += downtimeEntries [i].hourLocal.ToString ("MM-dd HH") + "\n\n";
                pastEntriesDTString      += downtimeEntries [i].dtMinutes.ToString ("#0") + "\n\n";
                pastEntriesOpString      += downtimeEntries [i].initials + "\n\n";
                pastEntriesReasonsString += downtimeEntries [i].level1 + (downtimeEntries [i].level2.Length > 1 ? " > " + downtimeEntries [i].level2 : "") + (downtimeEntries [i].level3.Length > 1 ? " > " + downtimeEntries [i].level3 : "") + (downtimeEntries [i].level4.Length > 1 ? " > " + downtimeEntries [i].level4 : "") + (downtimeEntries [i].level5.Length > 1 ? " > " + downtimeEntries [i].level5 : "") + "\n";
                pastEntriesReasonsString += "  " + (downtimeEntries [i].opNotes.Length > 1 && downtimeEntries [i].engNotes.Length > 1 ? downtimeEntries [i].opNotes + " > " : downtimeEntries [i].opNotes) + downtimeEntries [i].engNotes + "\n";
            }
        }
        pastEntriesHours.text = pastEntriesHoursString;
        pastEntriesDT.text = pastEntriesDTString;
        pastEntriesOp.text = pastEntriesOpString;
        pastEntriesReasons.text = pastEntriesReasonsString;
    }
    
    
    void UpdateCurrentDowntimeEntrysFromList () {
        int reasonID = 0;
        
        for (int i = 0; i < downtimeEntries.Count; i++) {
            if (downtimeEntries [i].hourUTC < hours [hourID].hourUTC) {
                i = downtimeEntries.Count;
                break;
            } else if (downtimeEntries [i].hourUTC == hours [hourID].hourUTC) {
                entryMinutesText [reasonID].text = downtimeEntries [i].dtMinutes.ToString ("#0");
                entryTopLineTexts[reasonID].text = downtimeEntries [i].level1 + (downtimeEntries [i].level2.Length > 1 ? " > " + downtimeEntries [i].level2 : "") + (downtimeEntries [i].level3.Length > 1 ? " > " + downtimeEntries [i].level3 : "") + (downtimeEntries [i].level4.Length > 1 ? " > " + downtimeEntries [i].level4 : "") + (downtimeEntries [i].level5.Length > 1 ? " > " + downtimeEntries [i].level5 : "");
                entryOpNotes     [reasonID].text = downtimeEntries [i].opNotes;
                entryEngNotes    [reasonID].text = downtimeEntries [i].engNotes;
                
                entryButtons[reasonID].SetActive(false);
                entryButtons[reasonID].transform.parent.gameObject.SetActive(true);
                reasonID++;
            }
        }
        if (reasonID < entryButtons.Count) {
            entryButtons[reasonID].SetActive(true);
            entryButtons[reasonID].transform.parent.gameObject.SetActive(true);
            reasonID++;
            
            for (int i = reasonID; i < entryButtons.Count; i++) {
                entryButtons[i].SetActive (true);
                entryButtons[i].transform.parent.gameObject.SetActive (false);
            }
        }
        AddMinutes ();
    }


    /// <summary>
    /// Queries [FairlifeMaster].[dbo].[OpInsightsCodes] for valid options of the next level given the user's selection for the current and previous level(s)
    /// </summary>
/*    public IEnumerator EntryQuery()
    {
        runningCoroutines++;
        while (SqlJobManager.Instance.sqlRunning)
        {
            yield return new WaitForSeconds(.1f);
        }
        loadingBlocker.SetActive (true);
        yield return null;
        SqlJobManager.Instance.StartJob();
        if (level <= 1) { query = "select distinct [Level1] from [FairlifeMaster].[dbo].[OpInsightsCodes] where [Level1] is not null order by [Level1]"; }
        else if (level == 2) { query = "select distinct [Level2] from [FairlifeMaster].[dbo].[OpInsightsCodes] where [Level2] is not null and [Level1] = '" + level1 + "' order by [Level2]"; }
        else if (level == 3) { query = "select distinct [Level3] from [FairlifeMaster].[dbo].[OpInsightsCodes] where [Level3] is not null and [Level1] = '" + level1 + "' and [Level2] = '" + level2 + "' order by [Level3]"; }
        else if (level == 4) { query = "select distinct [Level4] from [FairlifeMaster].[dbo].[OpInsightsCodes] where [Level4] is not null and [Level1] = '" + level1 + "' and [Level2] = '" + level2 + "' and [Level3] = '" + level3 + "' order by [Level4]"; }
        else if (level == 5)    { query = $"select distinct [Level5] from [FairlifeMaster].[dbo].[OpInsightsCodes] where [Level5] is not null and [Level1] = '{level1}' and [Level2] = '{level2}' and [Level3] = '{level3}' and [Level4] = '{level4}' order by [Level5]";}
        if (level <= 5) { buttonString.Clear(); } 
        using (SqlConnection dbCon = new SqlConnection(connectionString))
        {
            SqlCommand cmd = new SqlCommand(query, dbCon);
            try
            {
                dbCon.Open();
                reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        buttonString.Add(reader[0].ToString());
                    }
                }
                reader.Close();
            }
            catch (SqlException exception) {
                if (debug) {
                    Debug.LogError(exception.ToString());
                    Debug.Log(query);
                }
            }
        }
        yield return new WaitForSeconds(0);
        if (buttonString.Count > 0)
        {
            ChangeButtonCount();
            saveButton.gameObject.SetActive(false);
        }
        else
        {
            DisplayNotes();
        }
        cancelButton.gameObject.SetActive(true);
        backButton.gameObject.SetActive(true);
        SqlJobManager.Instance.EndJob();
        yield return null;
        runningCoroutines--;
    }
 */   
    
    
    void PopulateButtonsWithReasonCodes () {
        buttonString.Clear ();
        reasonCodeSQLIDs.Clear ();
        string lastEntry = "";
        for (int i = 0; i < reasonCodes.Count; i++) {
            if (lastEntry != (level <= 1 ? reasonCodes [i].level1 : level <= 2 ? reasonCodes [i].level2 : level <= 3 ? reasonCodes [i].level3 : level <= 4 ? reasonCodes [i].level4 : reasonCodes [i].level5) ) {
                if (level <= 1 || (level <= 2 && reasonCodes [i].level1 == level1) || (level <= 3 && reasonCodes [i].level1 == level1 && reasonCodes [i].level2 == level2) || (level <= 4 && reasonCodes [i].level1 == level1 && reasonCodes [i].level2 == level2 && reasonCodes [i].level3 == level3) || (level <= 5 && reasonCodes [i].level1 == level1 && reasonCodes [i].level2 == level2 && reasonCodes [i].level3 == level3 && reasonCodes [i].level4 == level4) ) {
                    lastEntry  = (level <= 1 ? reasonCodes [i].level1 : level <= 2 ? reasonCodes [i].level2 : level <= 3 ? reasonCodes [i].level3 : level <= 4 ? reasonCodes [i].level4 : reasonCodes [i].level5);
                    Debug.Log (" -" + lastEntry + "- ");
                    if (lastEntry.Length > 0) {
                        buttonString.Add (lastEntry);
                        reasonCodeSQLIDs.Add (reasonCodes [i].ID);
                        reasonCodeSQLID = reasonCodes [i].ID;
                    }
                }
            }
        }
        if (buttonString.Count > 0) {
            ChangeButtonCount();
            saveButton.gameObject.SetActive (false);
        } else {
            DisplayNotes();
        }
    }


    public void GetLastAccountedForHourNew () {
        int i;
        for (i = 12; i >= 0; i--) {
            if (hours [i].dtMinutes != hours [i].accMinutes) {
                lastUnaccountedForHour = hours [i].hour;
                lastUnaccountedForHourID = i;
                i = -1;
            }
        }
        if (i == -1) {
            lastUnaccountedForHour = hours [0].hour;
            lastUnaccountedForHourID = 0;
        }
        if (firstStart) {
            hourID = lastUnaccountedForHourID;
            selectedHour = lastUnaccountedForHour;
            selectedHourText.text = selectedHour.ToString("MM-dd  HH:mm");
            UpdateCurrentDowntimeEntrysFromList ();
        }
        if (sectionID == 2) {
            buttonString.Clear();

            for (i = lastUnaccountedForHourID; i < lastUnaccountedForHourID+12; i++) {
                buttonString.Add (hours [i].hour.ToString("MM-dd  HH:mm"));
            }
            ChangeButtonCount();
            cancelButton.gameObject.SetActive(false);
            backButton.gameObject.SetActive(false);
            saveButton.gameObject.SetActive(false);
            CenterButtons(selectedHourRect.anchoredPosition);
            buttonContainer.SetActive(true);
        }
    }

    
    public void SaveButton () {
        StartCoroutine (Save ());
    }


    /// <summary>
    /// Drops any existing records in [FairlifeOperations].[dbo].[OpInsights] for the current line for the selected hour
    /// Inserts records for all downtime entries created by the user
    /// </summary>
    public IEnumerator Save () {
        inputTimer = 0;
        hourDownloadTimer = 60;
        saveDelay = 60;
        runningCoroutines++;
        bool noInvalidEntry = true;

        yield return null;

        loadingBlocker.SetActive (true);
        uploadingIcon.SetActive (true);
        
        
        //DateTime.SpecifyKind(selectedHour, DateTimeKind.Local);
        //DateTime SelectedHourUTC = TimeZoneInfo.ConvertTimeToUtc(selectedHour);
        while (SqlJobManager.Instance.sqlRunning) {
            yield return null;
        }
        SqlJobManager.Instance.StartJob();
        using (SqlConnection dbCon = new SqlConnection(connectionString)) {
            dbCon.Open();
            //Delete records in [FairlifeOperations].[dbo].[OpInsights] for current hour on the specified line
            query = $"declare @line int = '{line}'; declare @hour DateTime = '{hours [hourID].hourUTC:yyyy-MM-dd HH:mm}'; declare @site VarChar (5) = '{site}'; " +
                "DELETE FROM [FairlifeOperations].[dbo].[OpInsights] WHERE [Site] = @site AND [Line] = @line AND [DateHourUTC] = @hour; exec [FairlifeOperations].[dbo].[spUpdate_OpInsightsHourlyLog_AccMinutes];";
            //Debug.Log(query);
            
            SqlCommand cmd = new SqlCommand(query, dbCon);
            try {
                cmd.ExecuteNonQuery();
            } catch (Exception e) {
                Debug.LogError(e);
                Debug.Log(query);
            }
            
            //for each downtime entry
            for (saveInt = 0; saveInt < entryButtons.Count; saveInt++) {
                if (entryButtons[saveInt].transform.parent.gameObject.activeSelf && !entryButtons[saveInt].activeSelf) {
                    splitString = entryTopLineTexts[saveInt].text.Split('>');
                    if (splitString.Length > 0) { level1 = splitString[0].Trim(); } else { level1 = ""; }
                    if (splitString.Length > 1) { level2 = splitString[1].Trim(); } else { level2 = ""; }
                    if (splitString.Length > 2) { level3 = splitString[2].Trim(); } else { level3 = ""; }
                    if (splitString.Length > 3) { level4 = splitString[3].Trim(); } else { level4 = ""; }
                    if (splitString.Length > 4) { level5 = splitString[4].Trim(); } else { level5 = ""; }

                    //insert downtime entry record in [FairlifeOperations].[dbo].[OpInsights]
                    query = "INSERT INTO [FairlifeOperations].[dbo].[OpInsights] ([DateHourUTC],[Site],[Line],[DownTime],[Operator],[CodeID],[Level1],[Level2],[Level3],[Level4],[Level5],[OpNotes],[EngNotes],[TimeStampUTC]) " +
                        "VALUES (@datehourUTC, @site, @line, @entryMinText, @initials, @code, @lvl1, @lvl2, @lvl3, @lvl4,@lvl5, @opNotes, @engNotes, @timestampUTC); ";
                    //Debug.Log (query);

                    using (SqlTransaction dbTrans = dbCon.BeginTransaction()) {
                        using (cmd = new SqlCommand(query, dbCon, dbTrans)) {
                            cmd.Parameters.Add(new SqlParameter("@datehourUTC", SqlDbType.DateTime));
                            cmd.Parameters.Add(new SqlParameter("@site", SqlDbType.VarChar, 5));
                            cmd.Parameters.Add(new SqlParameter("@line", SqlDbType.Int));
                            cmd.Parameters.Add(new SqlParameter("@entryMinText", SqlDbType.Int));
                            cmd.Parameters.Add(new SqlParameter("@initials", SqlDbType.VarChar, 20));
                            cmd.Parameters.Add(new SqlParameter("@code", SqlDbType.Int));
                            cmd.Parameters.Add(new SqlParameter("@lvl1", SqlDbType.VarChar, 25));
                            cmd.Parameters.Add(new SqlParameter("@lvl2", SqlDbType.VarChar, 20));
                            cmd.Parameters.Add(new SqlParameter("@lvl3", SqlDbType.VarChar, 20));
                            cmd.Parameters.Add(new SqlParameter("@lvl4", SqlDbType.VarChar, 30));
                            cmd.Parameters.Add(new SqlParameter("@lvl5", SqlDbType.VarChar, 50));
                            cmd.Parameters.Add(new SqlParameter("@opNotes", SqlDbType.VarChar, 5000));
                            cmd.Parameters.Add(new SqlParameter("@engNotes", SqlDbType.VarChar, 5000));
                            cmd.Parameters.Add(new SqlParameter("@timestampUTC", SqlDbType.DateTime));
                            try {
                                cmd.Parameters[ 0].Value = hours [hourID].hourUTC.ToString("yyyy-MM-dd HH:mm");
                                cmd.Parameters[ 1].Value = site;
                                cmd.Parameters[ 2].Value = line;
                                cmd.Parameters[ 3].Value = entryMinutesText[saveInt].text;
                                cmd.Parameters[ 4].Value = userInitials;
                                cmd.Parameters[ 5].Value = reasonCodeSQLID;
                                cmd.Parameters[ 6].Value = level1;
                                cmd.Parameters[ 7].Value = level2;
                                cmd.Parameters[ 8].Value = level3;
                                cmd.Parameters[ 9].Value = level4;
                                cmd.Parameters[10].Value = level5;
                                cmd.Parameters[11].Value = entryOpNotes[saveInt].text;
                                cmd.Parameters[12].Value = entryEngNotes[saveInt].text;
                                cmd.Parameters[13].Value = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm");
                                if (cmd.ExecuteNonQuery() != 1) {
                                    throw new InvalidProgramException();
                                }
                                dbTrans.Commit();
                            } catch (Exception e) {
                                Debug.LogError(e);
                                Debug.Log(query);
                                dbTrans.Rollback();
                            } finally {
                                dbTrans.Dispose();
                            }
                        };
                    };
                    
                }
            }
            query = "/*EXEC [FairlifeOperations].[dbo].[spUpdate_OpInsightsHourlyLog]; */EXEC [FairlifeOperations].[dbo].[spUpdate_OpInsightsHourlyLog_FGOverride]; exec [FairlifeOperations].[dbo].[spUpdate_OpInsightsHourlyLog_AccMinutes];";
            cmd = new SqlCommand(query, dbCon);
            try
            {
                reader = cmd.ExecuteReader();
            }
            catch (SqlException exception)
            {
                if (debug)
                {
                    Debug.LogError(exception.ToString());
                    Debug.Log(query);
                }
            }
            reader.Close();
            dbCon.Close();
            dbCon.Dispose();
        }
        yield return null;
        
        
        SqlJobManager.Instance.EndJob();
        
        loadingBlocker.SetActive (false);
        uploadingIcon.SetActive (false);
        yield return null;
        StartCoroutine (DownloadHours ());
        yield return null;
        GetLastAccountedForHourNew ();
        yield return null;
        firstStart = true;
        StartCoroutine (DownloadDowntimeEntrys ());
        yield return null;   
        UpdateCurrentDowntimeEntrysFromList ();
        yield return null;
        CheckCopyButton();

        yield return null;
        AddMinutes ();
        runningCoroutines--;
        CheckCopyButton();
    }

    /// <summary>
    /// Queries [FairlifeMaster].[dbo].[ItemMaster] and [FairlifeMaster].[dbo].[BaseIDsAndFGIDs]
    /// to update the program's lists for materialIDs, Descriptions, BottleSizes, and PackSizes
    /// </summary>
    public IEnumerator CheckMaterialID () {
        runningCoroutines++;
        while (SqlJobManager.Instance.sqlRunning) {
            yield return new WaitForSeconds(.1f);
        }
        yield return new WaitForSeconds(0);
        if (baseID != 0)
        {
            //Gets list of possible material IDs
            query = "select [IM].[ItemNum],[IM].[Description],[IM].[VolumePerUnit],[IM].[UnitsPer] from [FairlifeMaster].[dbo].[ItemMaster] [IM] " +
                $"left join [FairlifeMaster].[dbo].[BaseIDsAndFGIDs] [IDs] on [IM].[ItemNum] = [IDs].[FGID] where [IDs].[BaseID] = '{baseID}' and [IM].[Inactive] = '0'";
            if (ml != null)
            {
                query += $"and [IM].[Milliliters] = '{ml}'";
            }

            using (SqlConnection dbCon = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(query, dbCon);
                try
                {
                    dbCon.Open();
                    reader = cmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        j1 = 0;
                        materialIDs.Clear();
                        materialDescriptions.Clear();
                        bottleSizes.Clear();
                        packSizes.Clear();
                        while (reader.Read())
                        {
                            if (!reader.IsDBNull(0) && !reader.IsDBNull(1) && !reader.IsDBNull(2) && !reader.IsDBNull(3))
                            {
                                materialIDs.Add(reader.GetInt32(0));
                                materialDescriptions.Add((string)reader.GetSqlString(1));
                                bottleSizes.Add((float)reader.GetDouble(2));
                                packSizes.Add(reader.GetInt32(3));
                            }
                            j1++;
                        }
                    }
                    reader.Close();
                }
                catch (SqlException exception)
                {
                    fiveMinuteRunning = false;
                    if (debug)
                    {
                        Debug.LogError(exception.ToString());
                        Debug.Log(query);
                    }
                }
            }
            //If we have a baseID and don't know the finished good, prompt the user for it
            if (sectionID == 0 && materialID == 0 && materialIDs.Count > 0)
            {
                sectionID = -3;
                buttonString.Clear();
                buttonString.AddRange(materialDescriptions);
                cancelButton.gameObject.SetActive(true);
                backButton.gameObject.SetActive(true);
                saveButton.gameObject.SetActive(false);
                //FGIDOverride.gameObject.SetActive(true);
                ChangeButtonCount();
                buttonContainer.gameObject.SetActive(true);
            }
        }
        yield return null;
        SqlJobManager.Instance.EndJob();
        runningCoroutines--;
        yield return null;
    }

    /// <summary>
    /// Updates the [FGID] and [ml] values of records in [FairlifeOperations].[dbo].[OpInsightsHourlyLog_FGOverride]
    /// for the selected hour at CPS on the specified line
    /// </summary>
    public IEnumerator UpdateMaterialID() {
        while (SqlJobManager.Instance.sqlRunning) {
            yield return new WaitForSeconds(.1f);
        }
        SqlJobManager.Instance.StartJob();
        uploadingIcon.SetActive (true);
        yield return null;
        //TODO TEST
        query = $"Delete from [FairlifeOperations].[dbo].[OpInsightsHourlyLog_FGOverride] where [Site] = '{site}' and [Line] = '{line}' and [HourLocal] = '{hours [hourID].hour:yyyy-MM-dd HH:mm}'; " +
            $"Insert into [FairlifeOperations].[dbo].[OpInsightsHourlyLog_FGOverride] select [Site],[Line],[Equipment],[HourUTC],[HourLocal],[BaseID],'{materialID}',[Produced]," +
            $"[Scrapped],[Rate],[UoM],[Eff],[DtMinutes],[AccMinutes],[ml],[BPC] from [FairlifeOperations].[dbo].[OpInsightsHourlyLog] where " +
            $"[Site] = '{site}' and [Line] = '{line}' and [Equipment] = 'Filler' and [HourLocal] = '{hours [hourID].hour:yyyy-MM-dd HH:mm}'";
        using (SqlConnection dbCon = new SqlConnection(connectionString))
        {
            SqlCommand cmd = new SqlCommand(query, dbCon);
            try
            {
                dbCon.Open();
                reader = cmd.ExecuteReader();
                reader.Close();
            }
            catch (SqlException exception)
            {
                if (debug)
                {
                    Debug.LogError(exception.ToString());
                    Debug.Log(query);
                }
            }
        }
        yield return null;
        SqlJobManager.Instance.EndJob();
        uploadingIcon.SetActive (false);
        yield return null;
    }

    /// <summary>
    /// Stores chat logs in [FairlifeOperations].[dbo].[OperatorCommunications]
    /// </summary>
    public IEnumerator SendOpsComs() {
        inputTimer = 0;
        while (SqlJobManager.Instance.sqlRunning) {
            yield return new WaitForSeconds(.1f);
        }
        opComsCountdown = 10;
        SqlJobManager.Instance.StartJob();
        uploadingIcon.SetActive (true);
        yield return null;
        using (SqlConnection dbCon = new SqlConnection(connectionString))
        {
            dbCon.Open();
            query = "INSERT INTO [FairlifeOperations].[dbo].[OperatorCommunications] ([UTC],[Channel],[Initials],[Message]) " +
                "VALUES (GetUTCDate(),@channel,@initials,@message)";
            using (SqlTransaction dbTrans = dbCon.BeginTransaction())
            {
                using (SqlCommand cmd = new SqlCommand(query, dbCon, dbTrans))
                {
                    cmd.Parameters.Add(new SqlParameter("@channel", SqlDbType.VarChar, 25));
                    cmd.Parameters.Add(new SqlParameter("@initials", SqlDbType.VarChar, 5));
                    cmd.Parameters.Add(new SqlParameter("@message", SqlDbType.VarChar, 256));
                    try
                    {
                        cmd.Parameters[0].Value = site + " " + equipment;
                        cmd.Parameters[1].Value = userInitials;
                        cmd.Parameters[2].Value = opComsSend.text;
                        if (cmd.ExecuteNonQuery() != 1)
                        {
                            throw new InvalidProgramException();
                        }
                        dbTrans.Commit();
                        query = "UPDATE [master].[dbo].[TableInfo] SET [LastUpdatedUTC] = GetUTCDate(), [UpdatedBy] = 'OpInsights App' " +
                            "WHERE [Database] = 'FairlifeOperations' and [Table] = 'OperatorCommunications';";
                        SqlCommand cmd2 = new SqlCommand(query, dbCon);
                        try
                        {
                            cmd2.ExecuteNonQuery();
                        }
                        catch (Exception)
                        {
                            Debug.LogError("Update to master.dbo.TableInfo failed.");
                            throw;
                        }
                    }
                    catch (Exception exception)
                    {
                        if (debug)
                        {
                            Debug.LogError(exception.ToString());
                            Debug.Log(query);
                        }
                    }
                };
            };
        }
        opComsSend.text = "";
        SqlJobManager.Instance.EndJob();
        StartCoroutine(RecieveOpsComs());
        uploadingIcon.SetActive (false);
        yield return null;
    }

    /// <summary>
    /// Retrieves chat logs from [FairlifeOperations].[dbo].[OperatorCommunications]
    /// </summary>
    public IEnumerator RecieveOpsComs () {
        while (SqlJobManager.Instance.sqlRunning) {
            yield return null;
        }
        SqlJobManager.Instance.StartJob();

        if (!firstStart) {
            query = "select max(case when DateDiff (second,[LastUpdatedUTC],GetUTCDate()) < 10 then 1 else 0 end) from [master].[dbo].[TableInfo] " +
                "where [Table] = 'OperatorCommunications' and [Database] = 'FairlifeOperations'";

            using (SqlConnection dbCon = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(query, dbCon);
                try
                {
                    dbCon.Open();
                    reader = cmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            if (!reader.IsDBNull(0))
                            {
                                k2 = (int)reader.GetSqlInt32(0);
                            }
                        }
                    }
                    reader.Close();
                }
                catch (SqlException exception)
                {
                    if (debug)
                    {
                        Debug.LogError(exception.ToString());
                        Debug.Log(query);
                    }
                    k2 = 0;
                }
            };
        }
        else
        {
            k2 = 1;
        }
        if (k2 < 1)
        {//This means there is no new message in the chat, so we don't need to download the whole message
            SqlJobManager.Instance.EndJob();
        }
        else
        {
            opComsRecieve.text = "";
            query = "select top(50) format ([UTC]at time zone'UTC'at time zone'Eastern Standard Time', 'MM/dd HH:mm ')  +[Initials] + ' '  +[Message]" +
                "FROM [FairlifeOperations].[dbo].[OperatorCommunications] ORDER BY [UTC] desc";
            yield return new WaitForSeconds(0);
            using (SqlConnection dbCon = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(query, dbCon);
                try
                {
                    dbCon.Open();
                    reader = cmd.ExecuteReader();
                    opsComsString = "";
                    opsComsListA.Clear();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            if (!reader.IsDBNull(0))
                            {
                                opsComsListA.Insert(0, reader.GetSqlString(0).ToString());
                                //opComsRecieve.text += "\n"+(reader.GetSqlString(0)).ToString ();
                            }/* else {
								opComsRecieve.text += "\n";
							}*/
                        }
                    }
                    reader.Close();
                }
                catch (SqlException exception)
                {
                    if (debug)
                    {
                        Debug.LogError(exception.ToString());
                        Debug.Log(query);
                    }
                }
            }
            yield return new WaitForSeconds(0);
            SqlJobManager.Instance.EndJob();
            opsComsListB.Clear();
            for (j1 = opsComsListA.Count - 1; j1 >= 0; j1--)
            {
                if (opsComsString.Length + opsComsListA[j1].Length < 2600)
                {
                    opsComsString += "\n" + opsComsListA[j1];
                    opsComsListB.Add(opsComsListA[j1]);
                }
            }
            yield return new WaitForSeconds(0);
            opsComsString = "";
            for (j1 = opsComsListB.Count - 1; j1 >= 0; j1--)
            {
                if (opsComsString.Length + opsComsListB[j1].Length < 2600)
                {   //Just double checking
                    opsComsString += "\n" + opsComsListB[j1];
                }
            }
            yield return new WaitForSeconds(0);
            opComsRecieve.text = opsComsString;
            yield return null;
        }
    }
    
    
    /// <summary>
    /// Creates downtime entries matching the previous hours' entries in the downtime entry panel
    /// </summary>
    public void CopyLastHourNew () {
        hourID++;
        UpdateCurrentDowntimeEntrysFromList ();
        hourID--;
        AddMinutes ();
        CheckCopyButton();
    }


    /// <summary>
    /// Used by refresh button to reload the page data
    /// </summary>
    public void Refresh() {
        //Debug.Log("Running Refresh method");
        runningCoroutines++;
        //turn button grey
        //if button grey return
        //update button color in Update
        if (refreshButton.image.color == Color.gray) { runningCoroutines--; return; }
        refreshButton.image.color = Color.gray;
        StartCoroutine(RunSPs());
        PopulateStatesNew ();
        //StartCoroutine(PopulateStates());
        StartCoroutine(CheckMaterialID());
        StartCoroutine(RecieveOpsComs());
        //StartCoroutine(UpdateHourlyReport());
        if (saveDelay > 0) { saveDelay = -1; StartCoroutine(Save()); }
        runningCoroutines--;
        CheckCopyButton ();
    }

    #region helper methods
    /// <summary>
    /// Executes the stored procedures to update [FairlifeDashboard].[dbo].[FTMetricsMini_OEEWorkCell_V2], [FairlifeOperations].[dbo].[OpInsightsHourlyLog], 
    /// and [FairlifeOperations].[dbo].[OpInsightsHourlyLog_FGOverride]
    /// </summary>
    private IEnumerator RunSPs() {
        runningCoroutines++;
        while (SqlJobManager.Instance.sqlRunning) {
            yield return new WaitForSeconds(.1f);
        }
        SqlJobManager.Instance.sqlRunning = true;
        loadingBlocker.SetActive (true);
        yield return null;
        query = "SET NOCOUNT ON; EXEC [FairlifeDashboard].[dbo].[spUpdate_FTMetricsMini_OEEWorkCell_V2]; EXEC [FairlifeOperations].[dbo].[spUpdate_OpInsightsHourlyLog]; EXEC [FairlifeOperations].[dbo].[spUpdate_OpInsightsHourlyLog_FGOverride];";
        SqlJobManager.Instance.StartJob();

        using (SqlConnection dbCon = new SqlConnection(connectionString)) {
            SqlCommand cmd = new SqlCommand(query, dbCon);
            try {
                dbCon.Open();
                cmd.ExecuteNonQuery();
            }
            catch (SqlException exception) {
                if (debug) {
                    Debug.LogError(exception.ToString());
                    Debug.Log(query);
                }
            }
            dbCon.Close();
            dbCon.Dispose();
            SqlJobManager.Instance.EndJob();
            runningCoroutines--;
            yield return null;
        }
        loadingBlocker.SetActive (false);
        SqlJobManager.Instance.sqlRunning = false;
        yield return null;
    }
        

    /// <summary>
    /// Checks if the bottleSizes, packSizes, and materialIDs have an index matching buttonID
    /// If they do, sets bottleSize, packSize, and materialID equal to their corrseponding entry. For any that do not, keeps old value and logs an error
    /// If materialDescriptions has an index matching the value of buttonID, calls @UpdateMaterialID
    /// </summary>
    /// <param name="buttonID">Identifying value of the button which initiated the method call</param>
    private void SelectMaterialID(int buttonID) {
        inputTimer = 0;
        sectionID = 0;
        buttonContainer.SetActive(false);
        if (bottleSizes.Count > buttonID && buttonID >= 0) {
            bottleSize = bottleSizes[buttonID];
        }
        else {
            //bottleSize = 0;
            Debug.LogError("bottleSizes out of range.  Asking for " + buttonID + " when only " + bottleSizes.Count + " is available");
        }
        if (packSizes.Count > buttonID && buttonID >= 0) {
            packSize = packSizes[buttonID];
        }
        else {
            // packSize = 0;
            Debug.LogError("packSizes out of range.  Asking for " + buttonID + " when only " + packSizes.Count + " is available");
        }
        if (materialIDs.Count > buttonID && buttonID >= 0) {
            materialID = materialIDs[buttonID];
        } else {
            //materialID = 0;
            Debug.LogError("materialIDs out of range.  Asking for " + buttonID + " when only " + materialIDs.Count + " is available");
        }
        if (materialDescriptions.Count > buttonID && buttonID >= 0) {
            if (materialText.text != materialDescriptions[buttonID]) {
                materialText.text = materialDescriptions[buttonID];
                StartCoroutine(UpdateMaterialID());
            }
        } else {
            materialText.text += " - Failed to update FGID";
            Debug.LogError($"materialDescriptions out of range.  Asking for {buttonID} when only {materialDescriptions.Count} is available");
        }
    }
    

    /// <summary>
    /// Saves the user input initials and takes user to the main screen. If no material ID is set, prompts user to specify material. 
    /// </summary>
    private void SaveInitials() {
        inputTimer = 0;
        initials.gameObject.SetActive(false);
        userInitials = initials.text.ToUpper();
        initialsButton.text = "User: " + userInitials;
        initials.text = "";
        selectedHourText.text = lastUnaccountedForHour.ToString("MM-dd  HH:mm");
        if (materialID == 0)
        {
            Debug.Log("Material ID is 0");
            buttonQuestion.text = $"Which product is/was running during the hour of {lastUnaccountedForHour}?";
            sectionID = -3;
            buttonString.Clear();
            if (materialDescriptions.Count > 0) {
                buttonString.AddRange(materialDescriptions);
                cancelButton.gameObject.SetActive(true);
                backButton.gameObject.SetActive(true);
                saveButton.gameObject.SetActive(false);
                //FGIDOverride.gameObject.SetActive(true);
                ChangeButtonCount();
                buttonContainer.gameObject.SetActive(true);
            } else {
                buttonQuestion.text = "No finished goods found for the active base.";
                Debug.LogError("No matching material IDs");
                //Add error handling for hours with no FGID?
                //GetFGIDOverride();
            }
        } else {
            cancelButton.gameObject.SetActive(false);
            backButton.gameObject.SetActive(false);
            saveButton.gameObject.SetActive(false);
            buttonContainer.gameObject.SetActive(false);
            sectionID = 0;
        }
        CheckCopyButton();
    }

    /// <summary>
    /// Sets the Line the user is on, sets selectedHour and lastUnaccountedForHour to the current hour
    /// Updates shift report and hourly report
    /// Takes user to initials entry screen
    /// </summary>
    /// <param name="buttonID">Identifying value of the button which initiated the method call</param>
    private IEnumerator SelectLine(int buttonID) {
        inputTimer = 0;
        if (buttonID == 0) { line = 1; }
        else if (buttonID == 1) { line = 2; }
        else if (buttonID == 2) { line = 3; }
        else if (buttonID == 3) { line = 6; }
        lineButtonText.text = "Line " + line.ToString();
        StartCoroutine (DownloadHours ());
        yield return null;
        StartCoroutine (DownloadReasonCodes ());
        yield return null;
        while (SqlJobManager.Instance.sqlRunning) {
            yield return null;
        }
        GetLastAccountedForHourNew ();
        hourID = lastUnaccountedForHourID;
        selectedHour = lastUnaccountedForHour;
        selectedHourText.text = selectedHour.ToString("MM-dd  HH:mm");
        PopulateStatesNew ();
        StartCoroutine(CheckMaterialID());
        saveButton.gameObject.SetActive(true);
        cancelButton.gameObject.SetActive(false);
        backButton.gameObject.SetActive(true);
        buttonContainer.SetActive(true);
        foreach (RectTransform b in buttons) {
            b.gameObject.SetActive(false);
        }
        sectionID = -1;
        buttonQuestion.text = "What are your initials?";
        initials.text = "";
        initials.gameObject.SetActive(true);
        initials.ActivateInputField();
        UpdateCurrentDowntimeEntrysFromList ();
        CheckCopyButton();
    }
    #endregion
}