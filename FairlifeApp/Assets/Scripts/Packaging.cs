using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Data.SqlClient;

public class Packaging : MonoBehaviour {

	public int line;
	public int sectionID = -2;
	public bool debug = true;
	
	GameObject buttonContainer;
	public Text title, equipmentText, currentRatesText, hourRatesText;
	
	List <RectTransform> buttons = new List <RectTransform> ();
	List <Text> buttonTexts = new List <Text> ();
	List <Vector2> buttonPoses = new List <Vector2> {};
	int activeButtons = 0;
	public	List <Text> hourReportStatusText;
	public	List <Text> hourReportGphText;
	public	List <Text> shiftReportHour, shiftReportProduct, shiftReportCases, shiftReportEff;
	public	List <Image> equipmentIcons;
	public	List <Color> statusColors;
			List <float>	equipmentRates = new List <float> {0,0,0,0,0,0,0,0,0,0,0};
	
	float	f;
	int i1,j1,j2,k1,k2;
	string		query;
	SqlDataReader reader;
	private string connectionString = @"Data Source = 172.16.5.217; user id = cipapp;password = bestinclass&1;Connection Timeout=10;";
	List <string> buttonString = new List <string> {"Line1", "Line2", "Line3", "Line6"};
	
	DateTime trackedTime;
	DateTime currentTime;
	
	Vector2 buttonPos1		= new Vector2 (-85f,	-30f);
	Vector2 buttonSize1		= new Vector2 ( 1740,	 1010);
	Vector2 buttonPos1_9	= new Vector2 (-520,	 225);
	Vector2 buttonPos2_9	= new Vector2 (-85,	 	 225);
	Vector2 buttonPos3_9	= new Vector2 ( 350, 	 225);
	Vector2 buttonPos4_9	= new Vector2 (-520,	-30);
	Vector2 buttonPos5_9	= new Vector2 (-85,	 	-30);
	Vector2 buttonPos6_9	= new Vector2 ( 350, 	-30);
	Vector2 buttonPos7_9	= new Vector2 (-520,	-280);
	Vector2 buttonPos8_9	= new Vector2 (-85,	 	-280);
	Vector2 buttonPos9_9	= new Vector2 ( 350, 	-280);
	Vector2 buttonSize9		= new Vector2 ( 425,	 225);
	
	Vector2 buttonPos01_16	= new Vector2 (-750,	 350);
	Vector2 buttonPos02_16	= new Vector2 (-305,	 350);
	Vector2 buttonPos03_16	= new Vector2 ( 135,	 350);
	Vector2 buttonPos04_16	= new Vector2 ( 575,	 350);
	Vector2 buttonPos05_16	= new Vector2 (-750,	 90);
	Vector2 buttonPos06_16	= new Vector2 (-305,	 90);
	Vector2 buttonPos07_16	= new Vector2 ( 135,	 90);
	Vector2 buttonPos08_16	= new Vector2 ( 575,	 90);
	Vector2 buttonPos09_16	= new Vector2 (-750,	-170);
	Vector2 buttonPos10_16	= new Vector2 (-305,	-170);
	Vector2 buttonPos11_16	= new Vector2 ( 135,	-170);
	Vector2 buttonPos12_16	= new Vector2 ( 575,	-170);
	Vector2 buttonPos13_16	= new Vector2 (-750,	-425);
	Vector2 buttonPos14_16	= new Vector2 (-305,	-425);
	Vector2 buttonPos15_16	= new Vector2 ( 135,	-425);
	Vector2 buttonPos16_16	= new Vector2 ( 575,	-425);
	Vector2 buttonSize16	= new Vector2 ( 400,	 200);
	
	Vector2 buttonPos01_25	= new Vector2 (-780,	 365);
	Vector2 buttonPos02_25	= new Vector2 (-430,	 365);
	Vector2 buttonPos03_25	= new Vector2 (-80,		 365);
	Vector2 buttonPos04_25	= new Vector2 ( 270,	 365);
	Vector2 buttonPos05_25	= new Vector2 ( 620,	 365);
	Vector2 buttonPos06_25	= new Vector2 (-780,	 165);
	Vector2 buttonPos07_25	= new Vector2 (-430,	 165);
	Vector2 buttonPos08_25	= new Vector2 (-80,		 165);
	Vector2 buttonPos09_25	= new Vector2 ( 270,	 165);
	Vector2 buttonPos10_25	= new Vector2 ( 620,	 165);
	Vector2 buttonPos11_25	= new Vector2 (-780,	-35);
	Vector2 buttonPos12_25	= new Vector2 (-430,	-35);
	Vector2 buttonPos13_25	= new Vector2 (-80,		-35);
	Vector2 buttonPos14_25	= new Vector2 ( 270,	-35);
	Vector2 buttonPos15_25	= new Vector2 ( 620,	-35);
	Vector2 buttonPos16_25	= new Vector2 (-780,	-235);
	Vector2 buttonPos17_25	= new Vector2 (-430,	-235);
	Vector2 buttonPos18_25	= new Vector2 (-80,		-235);
	Vector2 buttonPos19_25	= new Vector2 ( 270,	-235);
	Vector2 buttonPos20_25	= new Vector2 ( 620,	-235);
	Vector2 buttonPos21_25	= new Vector2 (-780,	-430);
	Vector2 buttonPos22_25	= new Vector2 (-430,	-430);
	Vector2 buttonPos23_25	= new Vector2 (-80,		-430);
	Vector2 buttonPos24_25	= new Vector2 ( 270,	-430);
	Vector2 buttonPos25_25	= new Vector2 ( 620,	-430);
	Vector2 buttonSize25	= new Vector2 ( 325	,	 190);
	
	Vector2 buttonPos01_36	= new Vector2 (-810,	 380);
	Vector2 buttonPos02_36	= new Vector2 (-519,	 380);
	Vector2 buttonPos03_36	= new Vector2 (-228,	 380);
	Vector2 buttonPos04_36	= new Vector2 ( 63,		 380);
	Vector2 buttonPos05_36	= new Vector2 ( 355,	 380);
	Vector2 buttonPos06_36	= new Vector2 ( 645,	 380);
	Vector2 buttonPos07_36	= new Vector2 (-810,	 212);
	Vector2 buttonPos08_36	= new Vector2 (-519,	 212);
	Vector2 buttonPos09_36	= new Vector2 (-228,	 212);
	Vector2 buttonPos10_36	= new Vector2 ( 63,		 212);
	Vector2 buttonPos11_36	= new Vector2 ( 355,	 212);
	Vector2 buttonPos12_36	= new Vector2 ( 645,	 212);
	Vector2 buttonPos13_36	= new Vector2 (-810,	 46);
	Vector2 buttonPos14_36	= new Vector2 (-519,	 46);
	Vector2 buttonPos15_36	= new Vector2 (-228,	 46);
	Vector2 buttonPos16_36	= new Vector2 ( 63,		 46);
	Vector2 buttonPos17_36	= new Vector2 ( 355,	 46);
	Vector2 buttonPos18_36	= new Vector2 ( 645,	 46);
	Vector2 buttonPos19_36	= new Vector2 (-810,	-119);
	Vector2 buttonPos20_36	= new Vector2 (-519,	-119);
	Vector2 buttonPos21_36	= new Vector2 (-228,	-119);
	Vector2 buttonPos22_36	= new Vector2 ( 63,		-119);
	Vector2 buttonPos23_36	= new Vector2 ( 355,	-119);
	Vector2 buttonPos24_36	= new Vector2 ( 645,	-119);
	Vector2 buttonPos25_36	= new Vector2 (-810,	-284);
	Vector2 buttonPos26_36	= new Vector2 (-519,	-284);
	Vector2 buttonPos27_36	= new Vector2 (-228,	-284);
	Vector2 buttonPos28_36	= new Vector2 ( 63,		-284);
	Vector2 buttonPos29_36	= new Vector2 ( 355,	-284);
	Vector2 buttonPos30_36	= new Vector2 ( 645,	-284);
	Vector2 buttonPos31_36	= new Vector2 (-810,	-450);
	Vector2 buttonPos32_36	= new Vector2 (-519,	-450);
	Vector2 buttonPos33_36	= new Vector2 (-228,	-450);
	Vector2 buttonPos34_36	= new Vector2 ( 63,		-450);
	Vector2 buttonPos35_36	= new Vector2 ( 355,	-450);
	Vector2 buttonPos36_36	= new Vector2 ( 645,	-450);
	Vector2 buttonSize36	= new Vector2 ( 275	,	 155);
	
	Vector2 buttonPos01_49	= new Vector2 (-830,	 395);
	Vector2 buttonPos02_49	= new Vector2 (-580,	 395);
	Vector2 buttonPos03_49	= new Vector2 (-330,	 395);
	Vector2 buttonPos04_49	= new Vector2 (-80,		 395);
	Vector2 buttonPos05_49	= new Vector2 ( 170,	 395);
	Vector2 buttonPos06_49	= new Vector2 ( 420,	 395);
	Vector2 buttonPos07_49	= new Vector2 ( 670,	 395);
	Vector2 buttonPos08_49	= new Vector2 (-830,	 255);
	Vector2 buttonPos09_49	= new Vector2 (-580,	 255);
	Vector2 buttonPos10_49	= new Vector2 (-330,	 255);
	Vector2 buttonPos11_49	= new Vector2 (-80,		 255);
	Vector2 buttonPos12_49	= new Vector2 ( 170,	 255);
	Vector2 buttonPos13_49	= new Vector2 ( 420,	 255);
	Vector2 buttonPos14_49	= new Vector2 ( 670,	 255);
	Vector2 buttonPos15_49	= new Vector2 (-830,	 111);
	Vector2 buttonPos16_49	= new Vector2 (-580,	 111);
	Vector2 buttonPos17_49	= new Vector2 (-330,	 111);
	Vector2 buttonPos18_49	= new Vector2 (-80,		 111);
	Vector2 buttonPos19_49	= new Vector2 ( 170,	 111);
	Vector2 buttonPos20_49	= new Vector2 ( 420,	 111);
	Vector2 buttonPos21_49	= new Vector2 ( 670,	 111);
	Vector2 buttonPos22_49	= new Vector2 (-830,	-33);
	Vector2 buttonPos23_49	= new Vector2 (-580,	-33);
	Vector2 buttonPos24_49	= new Vector2 (-330,	-33);
	Vector2 buttonPos25_49	= new Vector2 (-80,		-33);
	Vector2 buttonPos26_49	= new Vector2 ( 170,	-33);
	Vector2 buttonPos27_49	= new Vector2 ( 420,	-33);
	Vector2 buttonPos28_49	= new Vector2 ( 670,	-33);
	Vector2 buttonPos29_49	= new Vector2 (-830,	-176);
	Vector2 buttonPos30_49	= new Vector2 (-580,	-176);
	Vector2 buttonPos31_49	= new Vector2 (-330,	-176);
	Vector2 buttonPos32_49	= new Vector2 (-80,		-176);
	Vector2 buttonPos33_49	= new Vector2 ( 170,	-176);
	Vector2 buttonPos34_49	= new Vector2 ( 420,	-176);
	Vector2 buttonPos35_49	= new Vector2 ( 670,	-176);
	Vector2 buttonPos36_49	= new Vector2 (-830,	-320);
	Vector2 buttonPos37_49	= new Vector2 (-580,	-320);
	Vector2 buttonPos38_49	= new Vector2 (-330,	-320);
	Vector2 buttonPos39_49	= new Vector2 (-80,		-320);
	Vector2 buttonPos40_49	= new Vector2 ( 170,	-320);
	Vector2 buttonPos41_49	= new Vector2 ( 420,	-320);
	Vector2 buttonPos42_49	= new Vector2 ( 670,	-320);
	Vector2 buttonPos43_49	= new Vector2 (-830,	-465);
	Vector2 buttonPos44_49	= new Vector2 (-580,	-465);
	Vector2 buttonPos45_49	= new Vector2 (-330,	-465);
	Vector2 buttonPos46_49	= new Vector2 (-80,		-465);
	Vector2 buttonPos47_49	= new Vector2 ( 170,	-465);
	Vector2 buttonPos48_49	= new Vector2 ( 420,	-465);
	Vector2 buttonPos49_49	= new Vector2 ( 670,	-465);
	Vector2 buttonSize49	= new Vector2 ( 235,	 130);
	
	Vector2 buttonPos01_64	= new Vector2 (-842,	 395);
	Vector2 buttonPos02_64	= new Vector2 (-626,	 395);
	Vector2 buttonPos03_64	= new Vector2 (-408,	 395);
	Vector2 buttonPos04_64	= new Vector2 (-192,	 395);
	Vector2 buttonPos05_64	= new Vector2 ( 24,	 	 395);
	Vector2 buttonPos06_64	= new Vector2 ( 242,	 395);
	Vector2 buttonPos07_64	= new Vector2 ( 458,	 395);
	Vector2 buttonPos08_64	= new Vector2 ( 676,	 395);	
	Vector2 buttonPos09_64	= new Vector2 (-842,	 271);
	Vector2 buttonPos10_64	= new Vector2 (-626,	 271);
	Vector2 buttonPos11_64	= new Vector2 (-408,	 271);
	Vector2 buttonPos12_64	= new Vector2 (-192,	 271);
	Vector2 buttonPos13_64	= new Vector2 ( 24,	 	 271);
	Vector2 buttonPos14_64	= new Vector2 ( 242,	 271);
	Vector2 buttonPos15_64	= new Vector2 ( 458,	 271);
	Vector2 buttonPos16_64	= new Vector2 ( 676,	 271);	
	Vector2 buttonPos17_64	= new Vector2 (-842,	 147);
	Vector2 buttonPos18_64	= new Vector2 (-626,	 147);
	Vector2 buttonPos19_64	= new Vector2 (-408,	 147);
	Vector2 buttonPos20_64	= new Vector2 (-192,	 147);
	Vector2 buttonPos21_64	= new Vector2 ( 24,	 	 147);
	Vector2 buttonPos22_64	= new Vector2 ( 242,	 147);
	Vector2 buttonPos23_64	= new Vector2 ( 458,	 147);
	Vector2 buttonPos24_64	= new Vector2 ( 676,	 147);	
	Vector2 buttonPos25_64	= new Vector2 (-842,	 23);
	Vector2 buttonPos26_64	= new Vector2 (-626,	 23);
	Vector2 buttonPos27_64	= new Vector2 (-408,	 23);
	Vector2 buttonPos28_64	= new Vector2 (-192,	 23);
	Vector2 buttonPos29_64	= new Vector2 ( 24,	 	 23);
	Vector2 buttonPos30_64	= new Vector2 ( 242,	 23);
	Vector2 buttonPos31_64	= new Vector2 ( 458,	 23);
	Vector2 buttonPos32_64	= new Vector2 ( 676,	 23);	
	Vector2 buttonPos33_64	= new Vector2 (-842,	 -101);
	Vector2 buttonPos34_64	= new Vector2 (-626,	 -101);
	Vector2 buttonPos35_64	= new Vector2 (-408,	 -101);
	Vector2 buttonPos36_64	= new Vector2 (-192,	 -101);
	Vector2 buttonPos37_64	= new Vector2 ( 24,	 	 -101);
	Vector2 buttonPos38_64	= new Vector2 ( 242,	 -101);
	Vector2 buttonPos39_64	= new Vector2 ( 458,	 -101);
	Vector2 buttonPos40_64	= new Vector2 ( 676,	 -101);	
	Vector2 buttonPos41_64	= new Vector2 (-842,	 -225);
	Vector2 buttonPos42_64	= new Vector2 (-626,	 -225);
	Vector2 buttonPos43_64	= new Vector2 (-408,	 -225);
	Vector2 buttonPos44_64	= new Vector2 (-192,	 -225);
	Vector2 buttonPos45_64	= new Vector2 ( 24,	 	 -225);
	Vector2 buttonPos46_64	= new Vector2 ( 242,	 -225);
	Vector2 buttonPos47_64	= new Vector2 ( 458,	 -225);
	Vector2 buttonPos48_64	= new Vector2 ( 676,	 -225);	
	Vector2 buttonPos49_64	= new Vector2 (-842,	 -349);
	Vector2 buttonPos50_64	= new Vector2 (-626,	 -349);
	Vector2 buttonPos51_64	= new Vector2 (-408,	 -349);
	Vector2 buttonPos52_64	= new Vector2 (-192,	 -349);
	Vector2 buttonPos53_64	= new Vector2 ( 24,	 	 -349);
	Vector2 buttonPos54_64	= new Vector2 ( 242,	 -349);
	Vector2 buttonPos55_64	= new Vector2 ( 458,	 -349);
	Vector2 buttonPos56_64	= new Vector2 ( 676,	 -349);	
	Vector2 buttonPos57_64	= new Vector2 (-842,	 -473);
	Vector2 buttonPos58_64	= new Vector2 (-626,	 -473);
	Vector2 buttonPos59_64	= new Vector2 (-408,	 -473);
	Vector2 buttonPos60_64	= new Vector2 (-192,	 -473);
	Vector2 buttonPos61_64	= new Vector2 ( 24,	 	 -473);
	Vector2 buttonPos62_64	= new Vector2 ( 242,	 -473);
	Vector2 buttonPos63_64	= new Vector2 ( 458,	 -473);
	Vector2 buttonPos64_64	= new Vector2 ( 676,	 -473);
	
	Vector2 buttonSize64	= new Vector2 ( 210,	 115);
	
	void Awake () {
		buttonContainer = transform.Find ("Panel/Buttons").gameObject;
		for (i1 = 0; i1 < 65; i1++) {
			buttonPoses.Add (Vector2.zero);
		}
		sectionID = -2;
		for (i1 = 0; i1 <= 63; i1++) {
			buttons.Add (buttonContainer.transform.Find (i1.ToString ()).GetComponent <RectTransform> ());
			buttonTexts.Add (buttons[i1].gameObject.transform.GetChild (0).GetComponent <Text> ());
		}
		CenterButtons (Vector2.zero);
		buttonContainer.SetActive (true);
		buttonString = new List <string> {"Line 1", "Line 2", "Line 3", "Line 6"};
		ChangeButtonCount ();
	}
	
	void Update () {
		currentTime = System.DateTime.Now;
		if (buttonContainer.activeSelf) {
			for (i1 = 0; i1 < activeButtons; i1++) {
				buttons [i1].anchoredPosition	= Vector2.Lerp (buttons [i1].anchoredPosition, buttonPoses [i1], .25f);
			}
		}
		if (currentTime.Second == 5) {
			if (!SqlJobManager.Instance.sqlRunning) {
				StartCoroutine (MinuteUpdate ());
			}
		}
	}
	
	public void Button (int buttonID) {
		//We need to have a section to add minutes and we should make the notes manditory?
		if (sectionID == -2) { //Select Line
			if (buttonID <= -2) {	//Cancel
				UnityEngine.SceneManagement.SceneManager.LoadScene(0);
			} else {
				     if (buttonID == 0) {	line = 1; }
				else if (buttonID == 1) {	line = 2; }
				else if (buttonID == 2) {	line = 3; }
				else if (buttonID == 3) {	line = 6; }
				title.text = "Packaging Line " + line.ToString ();
				StartCoroutine (MinuteUpdate ());
				buttonContainer.SetActive (false);
			}
		}
	}
	
	public void CenterButtons (Vector2 pos) {
		for (j1 = 0; j1 < buttons.Count; j1++) {
			buttons [j1].anchoredPosition = pos;
		}
	}
	
	public void ChangeButtonCount () {
		activeButtons = buttonString.Count;
		if (buttonString.Count == 1) {		   buttonPoses[ 0]=buttonPos1;
		} else if (buttonString.Count ==  2) { buttonPoses[ 0]=buttonPos4_9;   buttonPoses[ 1]=buttonPos6_9;
		} else if (buttonString.Count ==  3) { buttonPoses[ 0]=buttonPos2_9;   buttonPoses[ 1]=buttonPos7_9;   buttonPoses[ 2]=buttonPos9_9;
		} else if (buttonString.Count ==  4) { buttonPoses[ 0]=buttonPos1_9;   buttonPoses[ 1]=buttonPos3_9;   buttonPoses[ 2]=buttonPos7_9;   buttonPoses[ 3]=buttonPos9_9;
		} else if (buttonString.Count ==  5) { buttonPoses[ 0]=buttonPos1_9;   buttonPoses[ 1]=buttonPos3_9;   buttonPoses[ 2]=buttonPos5_9;   buttonPoses[ 3]=buttonPos7_9;   buttonPoses[ 4]=buttonPos9_9;	
		} else if (buttonString.Count ==  6) { buttonPoses[ 0]=buttonPos1_9;   buttonPoses[ 1]=buttonPos2_9;   buttonPoses[ 2]=buttonPos3_9;   buttonPoses[ 3]=buttonPos7_9;   buttonPoses[ 4]=buttonPos8_9;   buttonPoses[ 5]=buttonPos9_9;
		} else if (buttonString.Count ==  7) { buttonPoses[ 0]=buttonPos1_9;   buttonPoses[ 1]=buttonPos2_9;   buttonPoses[ 2]=buttonPos3_9;   buttonPoses[ 3]=buttonPos5_9;   buttonPoses[ 4]=buttonPos7_9;   buttonPoses[ 5]=buttonPos8_9;   buttonPoses[ 6]=buttonPos9_9;
		} else if (buttonString.Count ==  8) { buttonPoses[ 0]=buttonPos1_9;   buttonPoses[ 1]=buttonPos2_9;   buttonPoses[ 2]=buttonPos3_9;   buttonPoses[ 3]=buttonPos4_9;   buttonPoses[ 4]=buttonPos6_9;   buttonPoses[ 5]=buttonPos7_9;   buttonPoses[ 6]=buttonPos8_9;   buttonPoses[ 7]=buttonPos9_9;
		} else if (buttonString.Count ==  9) { buttonPoses[ 0]=buttonPos1_9;   buttonPoses[ 1]=buttonPos2_9;   buttonPoses[ 2]=buttonPos3_9;   buttonPoses[ 3]=buttonPos4_9;   buttonPoses[ 4]=buttonPos5_9;   buttonPoses[ 5]=buttonPos6_9;   buttonPoses[ 6]=buttonPos7_9;   buttonPoses[ 7]=buttonPos8_9;   buttonPoses[ 8]=buttonPos9_9;
		} else if (buttonString.Count == 10) { buttonPoses[ 0]=buttonPos02_16; buttonPoses[ 1]=buttonPos03_16; buttonPoses[ 2]=buttonPos05_16; buttonPoses[ 3]=buttonPos06_16; buttonPoses[ 4]=buttonPos07_16; buttonPoses[ 5]=buttonPos08_16; buttonPoses[ 6]=buttonPos09_16; buttonPoses[ 7]=buttonPos10_16; buttonPoses[ 8]=buttonPos11_16; buttonPoses[ 9]=buttonPos12_16;
		} else if (buttonString.Count == 11) { buttonPoses[ 0]=buttonPos01_16; buttonPoses[ 1]=buttonPos02_16; buttonPoses[ 2]=buttonPos03_16; buttonPoses[ 3]=buttonPos05_16; buttonPoses[ 4]=buttonPos06_16; buttonPoses[ 5]=buttonPos07_16; buttonPoses[ 6]=buttonPos08_16; buttonPoses[ 7]=buttonPos09_16; buttonPoses[ 8]=buttonPos10_16; buttonPoses[ 9]=buttonPos11_16; buttonPoses[10]=buttonPos12_16;
		} else if (buttonString.Count == 12) { buttonPoses[ 0]=buttonPos01_16; buttonPoses[ 1]=buttonPos02_16; buttonPoses[ 2]=buttonPos03_16; buttonPoses[ 3]=buttonPos04_16; buttonPoses[ 4]=buttonPos05_16; buttonPoses[ 5]=buttonPos06_16; buttonPoses[ 6]=buttonPos07_16; buttonPoses[ 7]=buttonPos08_16; buttonPoses[ 8]=buttonPos09_16; buttonPoses[ 9]=buttonPos10_16; buttonPoses[10]=buttonPos11_16; buttonPoses[11]=buttonPos12_16;
		} else if (buttonString.Count == 13) { buttonPoses[ 0]=buttonPos02_16; buttonPoses[ 1]=buttonPos05_16; buttonPoses[ 2]=buttonPos06_16; buttonPoses[ 3]=buttonPos07_16; buttonPoses[ 4]=buttonPos08_16; buttonPoses[ 5]=buttonPos09_16; buttonPoses[ 6]=buttonPos10_16; buttonPoses[ 7]=buttonPos11_16; buttonPoses[ 8]=buttonPos12_16; buttonPoses[ 9]=buttonPos13_16; buttonPoses[10]=buttonPos14_16; buttonPoses[11]=buttonPos15_16; buttonPoses[12]=buttonPos15_16;
		} else if (buttonString.Count == 14) { buttonPoses[ 0]=buttonPos02_16; buttonPoses[ 1]=buttonPos03_16; buttonPoses[ 2]=buttonPos05_16; buttonPoses[ 3]=buttonPos06_16; buttonPoses[ 4]=buttonPos07_16; buttonPoses[ 5]=buttonPos08_16; buttonPoses[ 6]=buttonPos09_16; buttonPoses[ 7]=buttonPos10_16; buttonPoses[ 8]=buttonPos11_16; buttonPoses[ 9]=buttonPos12_16; buttonPoses[10]=buttonPos13_16; buttonPoses[11]=buttonPos14_16; buttonPoses[12]=buttonPos15_16; buttonPoses[13]=buttonPos16_16;
		} else if (buttonString.Count == 15) { buttonPoses[ 0]=buttonPos01_16; buttonPoses[ 1]=buttonPos02_16; buttonPoses[ 2]=buttonPos03_16; buttonPoses[ 3]=buttonPos05_16; buttonPoses[ 4]=buttonPos06_16; buttonPoses[ 5]=buttonPos07_16; buttonPoses[ 6]=buttonPos08_16; buttonPoses[ 7]=buttonPos09_16; buttonPoses[ 8]=buttonPos10_16; buttonPoses[ 9]=buttonPos11_16; buttonPoses[10]=buttonPos12_16; buttonPoses[11]=buttonPos13_16; buttonPoses[12]=buttonPos14_16; buttonPoses[13]=buttonPos15_16; buttonPoses[14]=buttonPos16_16;
		} else if (buttonString.Count == 16) { buttonPoses[ 0]=buttonPos01_16; buttonPoses[ 1]=buttonPos02_16; buttonPoses[ 2]=buttonPos03_16; buttonPoses[ 3]=buttonPos04_16; buttonPoses[ 4]=buttonPos05_16; buttonPoses[ 5]=buttonPos06_16; buttonPoses[ 6]=buttonPos07_16; buttonPoses[ 7]=buttonPos08_16; buttonPoses[ 8]=buttonPos09_16; buttonPoses[ 9]=buttonPos10_16; buttonPoses[10]=buttonPos11_16; buttonPoses[11]=buttonPos12_16; buttonPoses[12]=buttonPos13_16; buttonPoses[13]=buttonPos14_16; buttonPoses[14]=buttonPos15_16; buttonPoses[15]=buttonPos16_16;
		} else if (buttonString.Count == 17) { buttonPoses[ 0]=buttonPos03_25; buttonPoses[ 1]=buttonPos07_25; buttonPoses[ 2]=buttonPos08_25; buttonPoses[ 3]=buttonPos09_25; buttonPoses[ 4]=buttonPos11_25; buttonPoses[ 5]=buttonPos12_25; buttonPoses[ 6]=buttonPos13_25; buttonPoses[ 7]=buttonPos14_25; buttonPoses[ 8]=buttonPos15_25; buttonPoses[ 9]=buttonPos16_25; buttonPoses[10]=buttonPos17_25; buttonPoses[11]=buttonPos18_25; buttonPoses[12]=buttonPos19_25; buttonPoses[13]=buttonPos20_25; buttonPoses[14]=buttonPos22_25; buttonPoses[15]=buttonPos23_25; buttonPoses[16]=buttonPos24_25;
		} else if (buttonString.Count == 18) { buttonPoses[ 0]=buttonPos02_25; buttonPoses[ 1]=buttonPos03_25; buttonPoses[ 2]=buttonPos04_25; buttonPoses[ 3]=buttonPos06_25; buttonPoses[ 4]=buttonPos07_25; buttonPoses[ 5]=buttonPos08_25; buttonPoses[ 6]=buttonPos09_25; buttonPoses[ 7]=buttonPos10_25; buttonPoses[ 8]=buttonPos11_25; buttonPoses[ 9]=buttonPos12_25; buttonPoses[10]=buttonPos13_25; buttonPoses[11]=buttonPos14_25; buttonPoses[12]=buttonPos15_25; buttonPoses[13]=buttonPos16_25; buttonPoses[14]=buttonPos17_25; buttonPoses[15]=buttonPos18_25; buttonPoses[16]=buttonPos19_25; buttonPoses[17]=buttonPos20_25;
		} else if (buttonString.Count == 19) { buttonPoses[ 0]=buttonPos02_25; buttonPoses[ 1]=buttonPos04_25; buttonPoses[ 2]=buttonPos06_25; buttonPoses[ 3]=buttonPos07_25; buttonPoses[ 4]=buttonPos08_25; buttonPoses[ 5]=buttonPos09_25; buttonPoses[ 6]=buttonPos10_25; buttonPoses[ 7]=buttonPos11_25; buttonPoses[ 8]=buttonPos12_25; buttonPoses[ 9]=buttonPos13_25; buttonPoses[10]=buttonPos14_25; buttonPoses[11]=buttonPos15_25; buttonPoses[12]=buttonPos16_25; buttonPoses[13]=buttonPos17_25; buttonPoses[14]=buttonPos18_25; buttonPoses[15]=buttonPos19_25; buttonPoses[16]=buttonPos20_25; buttonPoses[17]=buttonPos22_25; buttonPoses[18]=buttonPos24_25;
		} else if (buttonString.Count == 20) { buttonPoses[ 0]=buttonPos02_25; buttonPoses[ 1]=buttonPos04_25; buttonPoses[ 2]=buttonPos06_25; buttonPoses[ 3]=buttonPos07_25; buttonPoses[ 4]=buttonPos08_25; buttonPoses[ 5]=buttonPos09_25; buttonPoses[ 6]=buttonPos10_25; buttonPoses[ 7]=buttonPos11_25; buttonPoses[ 8]=buttonPos12_25; buttonPoses[ 9]=buttonPos13_25; buttonPoses[10]=buttonPos14_25; buttonPoses[11]=buttonPos15_25; buttonPoses[12]=buttonPos16_25; buttonPoses[13]=buttonPos17_25; buttonPoses[14]=buttonPos18_25; buttonPoses[15]=buttonPos19_25; buttonPoses[16]=buttonPos20_25; buttonPoses[17]=buttonPos21_25; buttonPoses[18]=buttonPos23_25; buttonPoses[19]=buttonPos25_25;
		} else if (buttonString.Count == 21) { buttonPoses[ 0]=buttonPos01_25; buttonPoses[ 1]=buttonPos02_25; buttonPoses[ 2]=buttonPos03_25; buttonPoses[ 3]=buttonPos04_25; buttonPoses[ 4]=buttonPos05_25; buttonPoses[ 5]=buttonPos07_25; buttonPoses[ 6]=buttonPos08_25; buttonPoses[ 7]=buttonPos09_25; buttonPoses[ 8]=buttonPos12_25; buttonPoses[ 9]=buttonPos13_25; buttonPoses[10]=buttonPos14_25; buttonPoses[11]=buttonPos16_25; buttonPoses[12]=buttonPos17_25; buttonPoses[13]=buttonPos18_25; buttonPoses[14]=buttonPos19_25; buttonPoses[15]=buttonPos20_25; buttonPoses[16]=buttonPos21_25; buttonPoses[17]=buttonPos22_25; buttonPoses[18]=buttonPos23_25; buttonPoses[19]=buttonPos24_25; buttonPoses[20]=buttonPos25_25;
		} else if (buttonString.Count == 22) { buttonPoses[ 0]=buttonPos01_25; buttonPoses[ 1]=buttonPos02_25; buttonPoses[ 2]=buttonPos03_25; buttonPoses[ 3]=buttonPos04_25; buttonPoses[ 4]=buttonPos05_25; buttonPoses[ 5]=buttonPos06_25; buttonPoses[ 6]=buttonPos07_25; buttonPoses[ 7]=buttonPos08_25; buttonPoses[ 8]=buttonPos09_25; buttonPoses[ 9]=buttonPos10_25; buttonPoses[10]=buttonPos12_25; buttonPoses[11]=buttonPos14_25; buttonPoses[12]=buttonPos16_25; buttonPoses[13]=buttonPos17_25; buttonPoses[14]=buttonPos18_25; buttonPoses[15]=buttonPos19_25; buttonPoses[16]=buttonPos20_25; buttonPoses[17]=buttonPos21_25; buttonPoses[18]=buttonPos22_25; buttonPoses[19]=buttonPos23_25; buttonPoses[20]=buttonPos24_25; buttonPoses[21]=buttonPos25_25;
		} else if (buttonString.Count == 23) { buttonPoses[ 0]=buttonPos01_25; buttonPoses[ 1]=buttonPos02_25; buttonPoses[ 2]=buttonPos03_25; buttonPoses[ 3]=buttonPos04_25; buttonPoses[ 4]=buttonPos05_25; buttonPoses[ 5]=buttonPos06_25; buttonPoses[ 6]=buttonPos07_25; buttonPoses[ 7]=buttonPos08_25; buttonPoses[ 8]=buttonPos09_25; buttonPoses[ 9]=buttonPos10_25; buttonPoses[10]=buttonPos12_25; buttonPoses[11]=buttonPos13_25; buttonPoses[12]=buttonPos14_25; buttonPoses[13]=buttonPos16_25; buttonPoses[14]=buttonPos17_25; buttonPoses[15]=buttonPos18_25; buttonPoses[16]=buttonPos19_25; buttonPoses[17]=buttonPos20_25; buttonPoses[18]=buttonPos21_25; buttonPoses[19]=buttonPos22_25; buttonPoses[20]=buttonPos23_25; buttonPoses[21]=buttonPos24_25; buttonPoses[22]=buttonPos25_25;
		} else if (buttonString.Count == 24) { buttonPoses[ 0]=buttonPos01_25; buttonPoses[ 1]=buttonPos02_25; buttonPoses[ 2]=buttonPos03_25; buttonPoses[ 3]=buttonPos04_25; buttonPoses[ 4]=buttonPos05_25; buttonPoses[ 5]=buttonPos06_25; buttonPoses[ 6]=buttonPos07_25; buttonPoses[ 7]=buttonPos08_25; buttonPoses[ 8]=buttonPos09_25; buttonPoses[ 9]=buttonPos10_25; buttonPoses[10]=buttonPos11_25; buttonPoses[11]=buttonPos12_25; buttonPoses[12]=buttonPos13_25; buttonPoses[13]=buttonPos14_25; buttonPoses[14]=buttonPos15_25; buttonPoses[15]=buttonPos16_25; buttonPoses[16]=buttonPos17_25; buttonPoses[17]=buttonPos18_25; buttonPoses[18]=buttonPos19_25; buttonPoses[19]=buttonPos20_25; buttonPoses[20]=buttonPos21_25; buttonPoses[21]=buttonPos22_25; buttonPoses[22]=buttonPos24_25; buttonPoses[23]=buttonPos25_25;
		} else if (buttonString.Count == 25) { buttonPoses[ 0]=buttonPos01_25; buttonPoses[ 1]=buttonPos02_25; buttonPoses[ 2]=buttonPos03_25; buttonPoses[ 3]=buttonPos04_25; buttonPoses[ 4]=buttonPos05_25; buttonPoses[ 5]=buttonPos06_25; buttonPoses[ 6]=buttonPos07_25; buttonPoses[ 7]=buttonPos08_25; buttonPoses[ 8]=buttonPos09_25; buttonPoses[ 9]=buttonPos10_25; buttonPoses[10]=buttonPos11_25; buttonPoses[11]=buttonPos12_25; buttonPoses[12]=buttonPos13_25; buttonPoses[13]=buttonPos14_25; buttonPoses[14]=buttonPos15_25; buttonPoses[15]=buttonPos16_25; buttonPoses[16]=buttonPos17_25; buttonPoses[17]=buttonPos18_25; buttonPoses[18]=buttonPos19_25; buttonPoses[19]=buttonPos20_25; buttonPoses[20]=buttonPos21_25; buttonPoses[21]=buttonPos22_25; buttonPoses[22]=buttonPos23_25; buttonPoses[23]=buttonPos24_25; buttonPoses[24]=buttonPos25_25;	
		} else if (buttonString.Count == 26) { buttonPoses[ 0]=buttonPos03_36; buttonPoses[ 1]=buttonPos04_36; buttonPoses[ 2]=buttonPos08_36; buttonPoses[ 3]=buttonPos09_36; buttonPoses[ 4]=buttonPos10_36; buttonPoses[ 5]=buttonPos11_36; buttonPoses[ 6]=buttonPos13_36; buttonPoses[ 7]=buttonPos14_36; buttonPoses[ 8]=buttonPos15_36; buttonPoses[ 9]=buttonPos16_36; buttonPoses[10]=buttonPos17_36; buttonPoses[11]=buttonPos18_36; buttonPoses[12]=buttonPos19_36; buttonPoses[13]=buttonPos20_36; buttonPoses[14]=buttonPos21_36; buttonPoses[15]=buttonPos22_36; buttonPoses[16]=buttonPos23_36; buttonPoses[17]=buttonPos24_36; buttonPoses[18]=buttonPos26_36; buttonPoses[19]=buttonPos27_36; buttonPoses[20]=buttonPos28_36; buttonPoses[21]=buttonPos29_36; buttonPoses[22]=buttonPos32_36; buttonPoses[23]=buttonPos33_36; buttonPoses[24]=buttonPos34_36; buttonPoses[25]=buttonPos35_36;
		} else if (buttonString.Count == 27) { buttonPoses[ 0]=buttonPos08_36; buttonPoses[ 1]=buttonPos09_36; buttonPoses[ 2]=buttonPos10_36; buttonPoses[ 3]=buttonPos13_36; buttonPoses[ 4]=buttonPos14_36; buttonPoses[ 5]=buttonPos15_36; buttonPoses[ 6]=buttonPos16_36; buttonPoses[ 7]=buttonPos17_36; buttonPoses[ 8]=buttonPos18_36; buttonPoses[ 9]=buttonPos19_36; buttonPoses[10]=buttonPos20_36; buttonPoses[11]=buttonPos21_36; buttonPoses[12]=buttonPos22_36; buttonPoses[13]=buttonPos23_36; buttonPoses[14]=buttonPos24_36; buttonPoses[15]=buttonPos25_36; buttonPoses[16]=buttonPos26_36; buttonPoses[17]=buttonPos27_36; buttonPoses[18]=buttonPos28_36; buttonPoses[19]=buttonPos29_36; buttonPoses[20]=buttonPos30_36; buttonPoses[21]=buttonPos31_36; buttonPoses[22]=buttonPos32_36; buttonPoses[23]=buttonPos33_36; buttonPoses[24]=buttonPos34_36; buttonPoses[25]=buttonPos35_36; buttonPoses[26]=buttonPos36_36;	
		} else if (buttonString.Count == 28) { buttonPoses[ 0]=buttonPos08_36; buttonPoses[ 1]=buttonPos09_36; buttonPoses[ 2]=buttonPos10_36; buttonPoses[ 3]=buttonPos11_36; buttonPoses[ 4]=buttonPos13_36; buttonPoses[ 5]=buttonPos14_36; buttonPoses[ 6]=buttonPos15_36; buttonPoses[ 7]=buttonPos16_36; buttonPoses[ 8]=buttonPos17_36; buttonPoses[ 9]=buttonPos18_36; buttonPoses[10]=buttonPos19_36; buttonPoses[11]=buttonPos20_36; buttonPoses[12]=buttonPos21_36; buttonPoses[13]=buttonPos22_36; buttonPoses[14]=buttonPos23_36; buttonPoses[15]=buttonPos24_36; buttonPoses[16]=buttonPos25_36; buttonPoses[17]=buttonPos26_36; buttonPoses[18]=buttonPos27_36; buttonPoses[19]=buttonPos28_36; buttonPoses[20]=buttonPos29_36; buttonPoses[21]=buttonPos30_36; buttonPoses[22]=buttonPos31_36; buttonPoses[23]=buttonPos32_36; buttonPoses[24]=buttonPos33_36; buttonPoses[25]=buttonPos34_36; buttonPoses[26]=buttonPos35_36; buttonPoses[27]=buttonPos36_36;
		} else if (buttonString.Count == 29) { buttonPoses[ 0]=buttonPos07_36; buttonPoses[ 1]=buttonPos08_36; buttonPoses[ 2]=buttonPos09_36; buttonPoses[ 3]=buttonPos10_36; buttonPoses[ 4]=buttonPos11_36; buttonPoses[ 5]=buttonPos13_36; buttonPoses[ 6]=buttonPos14_36; buttonPoses[ 7]=buttonPos15_36; buttonPoses[ 8]=buttonPos16_36; buttonPoses[ 9]=buttonPos17_36; buttonPoses[10]=buttonPos18_36; buttonPoses[11]=buttonPos19_36; buttonPoses[12]=buttonPos20_36; buttonPoses[13]=buttonPos21_36; buttonPoses[14]=buttonPos22_36; buttonPoses[15]=buttonPos23_36; buttonPoses[16]=buttonPos24_36; buttonPoses[17]=buttonPos25_36; buttonPoses[18]=buttonPos26_36; buttonPoses[19]=buttonPos27_36; buttonPoses[20]=buttonPos28_36; buttonPoses[21]=buttonPos29_36; buttonPoses[22]=buttonPos30_36; buttonPoses[23]=buttonPos31_36; buttonPoses[24]=buttonPos32_36; buttonPoses[25]=buttonPos33_36; buttonPoses[26]=buttonPos34_36; buttonPoses[27]=buttonPos35_36; buttonPoses[28]=buttonPos36_36;
		} else if (buttonString.Count == 30) { buttonPoses[ 0]=buttonPos07_36; buttonPoses[ 1]=buttonPos08_36; buttonPoses[ 2]=buttonPos09_36; buttonPoses[ 3]=buttonPos10_36; buttonPoses[ 4]=buttonPos11_36; buttonPoses[ 5]=buttonPos12_36; buttonPoses[ 6]=buttonPos13_36; buttonPoses[ 7]=buttonPos14_36; buttonPoses[ 8]=buttonPos15_36; buttonPoses[ 9]=buttonPos16_36; buttonPoses[10]=buttonPos17_36; buttonPoses[11]=buttonPos18_36; buttonPoses[12]=buttonPos19_36; buttonPoses[13]=buttonPos20_36; buttonPoses[14]=buttonPos21_36; buttonPoses[15]=buttonPos22_36; buttonPoses[16]=buttonPos23_36; buttonPoses[17]=buttonPos24_36; buttonPoses[18]=buttonPos25_36; buttonPoses[19]=buttonPos26_36; buttonPoses[20]=buttonPos27_36; buttonPoses[21]=buttonPos28_36; buttonPoses[22]=buttonPos29_36; buttonPoses[23]=buttonPos30_36; buttonPoses[24]=buttonPos31_36; buttonPoses[25]=buttonPos32_36; buttonPoses[26]=buttonPos33_36; buttonPoses[27]=buttonPos34_36; buttonPoses[28]=buttonPos35_36; buttonPoses[29]=buttonPos36_36;
		} else if (buttonString.Count == 31) { buttonPoses[ 0]=buttonPos01_36; buttonPoses[ 1]=buttonPos07_36; buttonPoses[ 2]=buttonPos08_36; buttonPoses[ 3]=buttonPos09_36; buttonPoses[ 4]=buttonPos10_36; buttonPoses[ 5]=buttonPos11_36; buttonPoses[ 6]=buttonPos12_36; buttonPoses[ 7]=buttonPos13_36; buttonPoses[ 8]=buttonPos14_36; buttonPoses[ 9]=buttonPos15_36; buttonPoses[10]=buttonPos16_36; buttonPoses[11]=buttonPos17_36; buttonPoses[12]=buttonPos18_36; buttonPoses[13]=buttonPos19_36; buttonPoses[14]=buttonPos20_36; buttonPoses[15]=buttonPos21_36; buttonPoses[16]=buttonPos22_36; buttonPoses[17]=buttonPos23_36; buttonPoses[18]=buttonPos24_36; buttonPoses[19]=buttonPos25_36; buttonPoses[20]=buttonPos26_36; buttonPoses[21]=buttonPos27_36; buttonPoses[22]=buttonPos28_36; buttonPoses[23]=buttonPos29_36; buttonPoses[24]=buttonPos30_36; buttonPoses[25]=buttonPos31_36; buttonPoses[26]=buttonPos32_36; buttonPoses[27]=buttonPos33_36; buttonPoses[28]=buttonPos34_36; buttonPoses[29]=buttonPos35_36; buttonPoses[30]=buttonPos36_36;/*Fix this column*/
		} else if (buttonString.Count == 32) { buttonPoses[ 0]=buttonPos02_36; buttonPoses[ 1]=buttonPos03_36; buttonPoses[ 2]=buttonPos04_36; buttonPoses[ 3]=buttonPos05_36; buttonPoses[ 4]=buttonPos07_36; buttonPoses[ 5]=buttonPos08_36; buttonPoses[ 6]=buttonPos09_36; buttonPoses[ 7]=buttonPos10_36; buttonPoses[ 8]=buttonPos11_36; buttonPoses[ 9]=buttonPos12_36; buttonPoses[10]=buttonPos13_36; buttonPoses[11]=buttonPos14_36; buttonPoses[12]=buttonPos15_36; buttonPoses[13]=buttonPos16_36; buttonPoses[14]=buttonPos17_36; buttonPoses[15]=buttonPos18_36; buttonPoses[16]=buttonPos19_36; buttonPoses[17]=buttonPos20_36; buttonPoses[18]=buttonPos21_36; buttonPoses[19]=buttonPos22_36; buttonPoses[20]=buttonPos23_36; buttonPoses[21]=buttonPos24_36; buttonPoses[22]=buttonPos25_36; buttonPoses[23]=buttonPos26_36; buttonPoses[24]=buttonPos27_36; buttonPoses[25]=buttonPos28_36; buttonPoses[26]=buttonPos29_36; buttonPoses[27]=buttonPos30_36; buttonPoses[28]=buttonPos32_36; buttonPoses[29]=buttonPos33_36; buttonPoses[30]=buttonPos34_36; buttonPoses[31]=buttonPos35_36;
		} else if (buttonString.Count == 33) { buttonPoses[ 0]=buttonPos02_36; buttonPoses[ 1]=buttonPos03_36; buttonPoses[ 2]=buttonPos04_36; buttonPoses[ 3]=buttonPos05_36; buttonPoses[ 4]=buttonPos07_36; buttonPoses[ 5]=buttonPos08_36; buttonPoses[ 6]=buttonPos09_36; buttonPoses[ 7]=buttonPos10_36; buttonPoses[ 8]=buttonPos11_36; buttonPoses[ 9]=buttonPos12_36; buttonPoses[10]=buttonPos13_36; buttonPoses[11]=buttonPos14_36; buttonPoses[12]=buttonPos15_36; buttonPoses[13]=buttonPos16_36; buttonPoses[14]=buttonPos17_36; buttonPoses[15]=buttonPos18_36; buttonPoses[16]=buttonPos19_36; buttonPoses[17]=buttonPos20_36; buttonPoses[18]=buttonPos21_36; buttonPoses[19]=buttonPos22_36; buttonPoses[20]=buttonPos23_36; buttonPoses[21]=buttonPos24_36; buttonPoses[22]=buttonPos25_36; buttonPoses[23]=buttonPos26_36; buttonPoses[24]=buttonPos27_36; buttonPoses[25]=buttonPos28_36; buttonPoses[26]=buttonPos29_36; buttonPoses[27]=buttonPos30_36; buttonPoses[28]=buttonPos31_36; buttonPoses[29]=buttonPos32_36; buttonPoses[30]=buttonPos33_36; buttonPoses[31]=buttonPos34_36; buttonPoses[32]=buttonPos35_36;
		} else if (buttonString.Count == 34) { buttonPoses[ 0]=buttonPos02_36; buttonPoses[ 1]=buttonPos03_36; buttonPoses[ 2]=buttonPos04_36; buttonPoses[ 3]=buttonPos05_36; buttonPoses[ 4]=buttonPos07_36; buttonPoses[ 5]=buttonPos08_36; buttonPoses[ 6]=buttonPos09_36; buttonPoses[ 7]=buttonPos10_36; buttonPoses[ 8]=buttonPos11_36; buttonPoses[ 9]=buttonPos12_36; buttonPoses[10]=buttonPos13_36; buttonPoses[11]=buttonPos14_36; buttonPoses[12]=buttonPos15_36; buttonPoses[13]=buttonPos16_36; buttonPoses[14]=buttonPos17_36; buttonPoses[15]=buttonPos18_36; buttonPoses[16]=buttonPos19_36; buttonPoses[17]=buttonPos20_36; buttonPoses[18]=buttonPos21_36; buttonPoses[19]=buttonPos22_36; buttonPoses[20]=buttonPos23_36; buttonPoses[21]=buttonPos24_36; buttonPoses[22]=buttonPos25_36; buttonPoses[23]=buttonPos26_36; buttonPoses[24]=buttonPos27_36; buttonPoses[25]=buttonPos28_36; buttonPoses[26]=buttonPos29_36; buttonPoses[27]=buttonPos30_36; buttonPoses[28]=buttonPos31_36; buttonPoses[29]=buttonPos32_36; buttonPoses[30]=buttonPos33_36; buttonPoses[31]=buttonPos34_36; buttonPoses[32]=buttonPos35_36; buttonPoses[33]=buttonPos36_36;
		} else if (buttonString.Count == 35) { buttonPoses[ 0]=buttonPos01_36; buttonPoses[ 1]=buttonPos02_36; buttonPoses[ 2]=buttonPos03_36; buttonPoses[ 3]=buttonPos04_36; buttonPoses[ 4]=buttonPos05_36; buttonPoses[ 5]=buttonPos07_36; buttonPoses[ 6]=buttonPos08_36; buttonPoses[ 7]=buttonPos09_36; buttonPoses[ 8]=buttonPos10_36; buttonPoses[ 9]=buttonPos11_36; buttonPoses[10]=buttonPos12_36; buttonPoses[11]=buttonPos13_36; buttonPoses[12]=buttonPos14_36; buttonPoses[13]=buttonPos15_36; buttonPoses[14]=buttonPos16_36; buttonPoses[15]=buttonPos17_36; buttonPoses[16]=buttonPos18_36; buttonPoses[17]=buttonPos19_36; buttonPoses[18]=buttonPos20_36; buttonPoses[19]=buttonPos21_36; buttonPoses[20]=buttonPos22_36; buttonPoses[21]=buttonPos23_36; buttonPoses[22]=buttonPos24_36; buttonPoses[23]=buttonPos25_36; buttonPoses[24]=buttonPos26_36; buttonPoses[25]=buttonPos27_36; buttonPoses[26]=buttonPos28_36; buttonPoses[27]=buttonPos29_36; buttonPoses[28]=buttonPos30_36; buttonPoses[29]=buttonPos31_36; buttonPoses[30]=buttonPos32_36; buttonPoses[31]=buttonPos33_36; buttonPoses[32]=buttonPos34_36; buttonPoses[33]=buttonPos35_36; buttonPoses[34]=buttonPos36_36;
		} else if (buttonString.Count == 36) { buttonPoses[ 0]=buttonPos01_36; buttonPoses[ 1]=buttonPos02_36; buttonPoses[ 2]=buttonPos03_36; buttonPoses[ 3]=buttonPos04_36; buttonPoses[ 4]=buttonPos05_36; buttonPoses[ 5]=buttonPos06_36; buttonPoses[ 6]=buttonPos07_36; buttonPoses[ 7]=buttonPos08_36; buttonPoses[ 8]=buttonPos09_36; buttonPoses[ 9]=buttonPos10_36; buttonPoses[10]=buttonPos11_36; buttonPoses[11]=buttonPos12_36; buttonPoses[12]=buttonPos13_36; buttonPoses[13]=buttonPos14_36; buttonPoses[14]=buttonPos15_36; buttonPoses[15]=buttonPos16_36; buttonPoses[16]=buttonPos17_36; buttonPoses[17]=buttonPos18_36; buttonPoses[18]=buttonPos19_36; buttonPoses[19]=buttonPos20_36; buttonPoses[20]=buttonPos21_36; buttonPoses[21]=buttonPos22_36; buttonPoses[22]=buttonPos23_36; buttonPoses[23]=buttonPos24_36; buttonPoses[24]=buttonPos25_36; buttonPoses[25]=buttonPos26_36; buttonPoses[26]=buttonPos27_36; buttonPoses[27]=buttonPos28_36; buttonPoses[28]=buttonPos29_36; buttonPoses[29]=buttonPos30_36; buttonPoses[30]=buttonPos31_36; buttonPoses[31]=buttonPos32_36; buttonPoses[32]=buttonPos33_36; buttonPoses[33]=buttonPos34_36; buttonPoses[34]=buttonPos35_36; buttonPoses[35]=buttonPos36_36;
		} else if (buttonString.Count == 37) { buttonPoses[ 0]=buttonPos03_49; buttonPoses[ 1]=buttonPos04_49; buttonPoses[ 2]=buttonPos05_49; buttonPoses[ 3]=buttonPos09_49; buttonPoses[ 4]=buttonPos10_49; buttonPoses[ 5]=buttonPos11_49; buttonPoses[ 6]=buttonPos12_49; buttonPoses[ 7]=buttonPos13_49; buttonPoses[ 8]=buttonPos15_49; buttonPoses[ 9]=buttonPos16_49; buttonPoses[10]=buttonPos17_49; buttonPoses[11]=buttonPos18_49; buttonPoses[12]=buttonPos19_49; buttonPoses[13]=buttonPos20_49; buttonPoses[14]=buttonPos21_49; buttonPoses[15]=buttonPos22_49; buttonPoses[16]=buttonPos23_49; buttonPoses[17]=buttonPos24_49; buttonPoses[18]=buttonPos25_49; buttonPoses[19]=buttonPos26_49; buttonPoses[20]=buttonPos27_49; buttonPoses[21]=buttonPos28_49; buttonPoses[22]=buttonPos29_49; buttonPoses[23]=buttonPos30_49; buttonPoses[24]=buttonPos31_49; buttonPoses[25]=buttonPos32_49; buttonPoses[26]=buttonPos33_49; buttonPoses[27]=buttonPos34_49; buttonPoses[28]=buttonPos35_49; buttonPoses[29]=buttonPos37_49; buttonPoses[30]=buttonPos38_49; buttonPoses[31]=buttonPos39_49; buttonPoses[32]=buttonPos40_49; buttonPoses[33]=buttonPos41_49; buttonPoses[34]=buttonPos45_49; buttonPoses[35]=buttonPos46_49; buttonPoses[36]=buttonPos47_49;
		} else if (buttonString.Count == 38) { buttonPoses[ 0]=buttonPos03_49; buttonPoses[ 1]=buttonPos05_49; buttonPoses[ 2]=buttonPos08_49; buttonPoses[ 3]=buttonPos09_49; buttonPoses[ 4]=buttonPos10_49; buttonPoses[ 5]=buttonPos11_49; buttonPoses[ 6]=buttonPos12_49; buttonPoses[ 7]=buttonPos13_49; buttonPoses[ 8]=buttonPos14_49; buttonPoses[ 9]=buttonPos15_49; buttonPoses[10]=buttonPos16_49; buttonPoses[11]=buttonPos17_49; buttonPoses[12]=buttonPos18_49; buttonPoses[13]=buttonPos19_49; buttonPoses[14]=buttonPos20_49; buttonPoses[15]=buttonPos21_49; buttonPoses[16]=buttonPos22_49; buttonPoses[17]=buttonPos23_49; buttonPoses[18]=buttonPos24_49; buttonPoses[19]=buttonPos25_49; buttonPoses[20]=buttonPos26_49; buttonPoses[21]=buttonPos27_49; buttonPoses[22]=buttonPos28_49; buttonPoses[23]=buttonPos29_49; buttonPoses[24]=buttonPos30_49; buttonPoses[25]=buttonPos31_49; buttonPoses[26]=buttonPos32_49; buttonPoses[27]=buttonPos33_49; buttonPoses[28]=buttonPos34_49; buttonPoses[29]=buttonPos35_49; buttonPoses[30]=buttonPos37_49; buttonPoses[31]=buttonPos38_49; buttonPoses[32]=buttonPos39_49; buttonPoses[33]=buttonPos40_49; buttonPoses[34]=buttonPos41_49; buttonPoses[35]=buttonPos45_49; buttonPoses[36]=buttonPos46_49; buttonPoses[37]=buttonPos47_49;
		} else if (buttonString.Count == 39) { buttonPoses[ 0]=buttonPos03_49; buttonPoses[ 1]=buttonPos04_49; buttonPoses[ 2]=buttonPos05_49; buttonPoses[ 3]=buttonPos08_49; buttonPoses[ 4]=buttonPos09_49; buttonPoses[ 5]=buttonPos10_49; buttonPoses[ 6]=buttonPos11_49; buttonPoses[ 7]=buttonPos12_49; buttonPoses[ 8]=buttonPos13_49; buttonPoses[ 9]=buttonPos14_49; buttonPoses[10]=buttonPos15_49; buttonPoses[11]=buttonPos16_49; buttonPoses[12]=buttonPos17_49; buttonPoses[13]=buttonPos18_49; buttonPoses[14]=buttonPos19_49; buttonPoses[15]=buttonPos20_49; buttonPoses[16]=buttonPos21_49; buttonPoses[17]=buttonPos22_49; buttonPoses[18]=buttonPos23_49; buttonPoses[19]=buttonPos24_49; buttonPoses[20]=buttonPos25_49; buttonPoses[21]=buttonPos26_49; buttonPoses[22]=buttonPos27_49; buttonPoses[23]=buttonPos28_49; buttonPoses[24]=buttonPos29_49; buttonPoses[25]=buttonPos30_49; buttonPoses[26]=buttonPos31_49; buttonPoses[27]=buttonPos32_49; buttonPoses[28]=buttonPos33_49; buttonPoses[29]=buttonPos34_49; buttonPoses[30]=buttonPos35_49; buttonPoses[31]=buttonPos37_49; buttonPoses[32]=buttonPos38_49; buttonPoses[33]=buttonPos39_49; buttonPoses[34]=buttonPos40_49; buttonPoses[35]=buttonPos41_49; buttonPoses[36]=buttonPos45_49; buttonPoses[37]=buttonPos46_49; buttonPoses[38]=buttonPos47_49;
		} else if (buttonString.Count == 40) { buttonPoses[ 0]=buttonPos03_49; buttonPoses[ 1]=buttonPos05_49; buttonPoses[ 2]=buttonPos08_49; buttonPoses[ 3]=buttonPos09_49; buttonPoses[ 4]=buttonPos10_49; buttonPoses[ 5]=buttonPos11_49; buttonPoses[ 6]=buttonPos12_49; buttonPoses[ 7]=buttonPos13_49; buttonPoses[ 8]=buttonPos14_49; buttonPoses[ 9]=buttonPos15_49; buttonPoses[10]=buttonPos16_49; buttonPoses[11]=buttonPos17_49; buttonPoses[12]=buttonPos18_49; buttonPoses[13]=buttonPos19_49; buttonPoses[14]=buttonPos20_49; buttonPoses[15]=buttonPos21_49; buttonPoses[16]=buttonPos22_49; buttonPoses[17]=buttonPos23_49; buttonPoses[18]=buttonPos24_49; buttonPoses[19]=buttonPos25_49; buttonPoses[20]=buttonPos26_49; buttonPoses[21]=buttonPos27_49; buttonPoses[22]=buttonPos28_49; buttonPoses[23]=buttonPos29_49; buttonPoses[24]=buttonPos30_49; buttonPoses[25]=buttonPos31_49; buttonPoses[26]=buttonPos32_49; buttonPoses[27]=buttonPos33_49; buttonPoses[28]=buttonPos34_49; buttonPoses[29]=buttonPos35_49; buttonPoses[30]=buttonPos36_49; buttonPoses[31]=buttonPos37_49; buttonPoses[32]=buttonPos38_49; buttonPoses[33]=buttonPos39_49; buttonPoses[34]=buttonPos40_49; buttonPoses[35]=buttonPos41_49; buttonPoses[36]=buttonPos42_49; buttonPoses[37]=buttonPos45_49; buttonPoses[38]=buttonPos46_49; buttonPoses[39]=buttonPos47_49;
		} else if (buttonString.Count == 41) { buttonPoses[ 0]=buttonPos03_49; buttonPoses[ 1]=buttonPos04_49; buttonPoses[ 2]=buttonPos05_49; buttonPoses[ 3]=buttonPos08_49; buttonPoses[ 4]=buttonPos09_49; buttonPoses[ 5]=buttonPos10_49; buttonPoses[ 6]=buttonPos11_49; buttonPoses[ 7]=buttonPos12_49; buttonPoses[ 8]=buttonPos13_49; buttonPoses[ 9]=buttonPos14_49; buttonPoses[10]=buttonPos15_49; buttonPoses[11]=buttonPos16_49; buttonPoses[12]=buttonPos17_49; buttonPoses[13]=buttonPos18_49; buttonPoses[14]=buttonPos19_49; buttonPoses[15]=buttonPos20_49; buttonPoses[16]=buttonPos21_49; buttonPoses[17]=buttonPos22_49; buttonPoses[18]=buttonPos23_49; buttonPoses[19]=buttonPos24_49; buttonPoses[20]=buttonPos25_49; buttonPoses[21]=buttonPos26_49; buttonPoses[22]=buttonPos27_49; buttonPoses[23]=buttonPos28_49; buttonPoses[24]=buttonPos29_49; buttonPoses[25]=buttonPos30_49; buttonPoses[26]=buttonPos31_49; buttonPoses[27]=buttonPos32_49; buttonPoses[28]=buttonPos33_49; buttonPoses[29]=buttonPos34_49; buttonPoses[30]=buttonPos35_49; buttonPoses[31]=buttonPos36_49; buttonPoses[32]=buttonPos37_49; buttonPoses[33]=buttonPos38_49; buttonPoses[34]=buttonPos39_49; buttonPoses[35]=buttonPos40_49; buttonPoses[36]=buttonPos41_49; buttonPoses[37]=buttonPos42_49; buttonPoses[38]=buttonPos45_49; buttonPoses[39]=buttonPos46_49; buttonPoses[40]=buttonPos47_49;
		} else if (buttonString.Count == 42) { buttonPoses[ 0]=buttonPos02_49; buttonPoses[ 1]=buttonPos03_49; buttonPoses[ 2]=buttonPos05_49; buttonPoses[ 3]=buttonPos06_49; buttonPoses[ 4]=buttonPos08_49; buttonPoses[ 5]=buttonPos09_49; buttonPoses[ 6]=buttonPos10_49; buttonPoses[ 7]=buttonPos11_49; buttonPoses[ 8]=buttonPos12_49; buttonPoses[ 9]=buttonPos13_49; buttonPoses[10]=buttonPos14_49; buttonPoses[11]=buttonPos15_49; buttonPoses[12]=buttonPos16_49; buttonPoses[13]=buttonPos17_49; buttonPoses[14]=buttonPos18_49; buttonPoses[15]=buttonPos19_49; buttonPoses[16]=buttonPos20_49; buttonPoses[17]=buttonPos21_49; buttonPoses[18]=buttonPos22_49; buttonPoses[19]=buttonPos23_49; buttonPoses[20]=buttonPos24_49; buttonPoses[21]=buttonPos25_49; buttonPoses[22]=buttonPos26_49; buttonPoses[23]=buttonPos27_49; buttonPoses[24]=buttonPos28_49; buttonPoses[25]=buttonPos29_49; buttonPoses[26]=buttonPos30_49; buttonPoses[27]=buttonPos31_49; buttonPoses[28]=buttonPos32_49; buttonPoses[29]=buttonPos33_49; buttonPoses[30]=buttonPos34_49; buttonPoses[31]=buttonPos35_49; buttonPoses[32]=buttonPos36_49; buttonPoses[33]=buttonPos37_49; buttonPoses[34]=buttonPos38_49; buttonPoses[35]=buttonPos39_49; buttonPoses[36]=buttonPos40_49; buttonPoses[37]=buttonPos41_49; buttonPoses[38]=buttonPos42_49; buttonPoses[39]=buttonPos45_49; buttonPoses[40]=buttonPos46_49; buttonPoses[41]=buttonPos47_49;
		} else if (buttonString.Count == 43) { buttonPoses[ 0]=buttonPos02_49; buttonPoses[ 1]=buttonPos03_49; buttonPoses[ 2]=buttonPos05_49; buttonPoses[ 3]=buttonPos06_49; buttonPoses[ 4]=buttonPos08_49; buttonPoses[ 5]=buttonPos09_49; buttonPoses[ 6]=buttonPos10_49; buttonPoses[ 7]=buttonPos11_49; buttonPoses[ 8]=buttonPos12_49; buttonPoses[ 9]=buttonPos13_49; buttonPoses[10]=buttonPos14_49; buttonPoses[11]=buttonPos15_49; buttonPoses[12]=buttonPos16_49; buttonPoses[13]=buttonPos17_49; buttonPoses[14]=buttonPos18_49; buttonPoses[15]=buttonPos19_49; buttonPoses[16]=buttonPos20_49; buttonPoses[17]=buttonPos21_49; buttonPoses[18]=buttonPos22_49; buttonPoses[19]=buttonPos23_49; buttonPoses[20]=buttonPos24_49; buttonPoses[21]=buttonPos25_49; buttonPoses[22]=buttonPos26_49; buttonPoses[23]=buttonPos27_49; buttonPoses[24]=buttonPos28_49; buttonPoses[25]=buttonPos29_49; buttonPoses[26]=buttonPos30_49; buttonPoses[27]=buttonPos31_49; buttonPoses[28]=buttonPos32_49; buttonPoses[29]=buttonPos33_49; buttonPoses[30]=buttonPos34_49; buttonPoses[31]=buttonPos35_49; buttonPoses[32]=buttonPos36_49; buttonPoses[33]=buttonPos37_49; buttonPoses[34]=buttonPos38_49; buttonPoses[35]=buttonPos39_49; buttonPoses[36]=buttonPos40_49; buttonPoses[37]=buttonPos41_49; buttonPoses[38]=buttonPos42_49; buttonPoses[39]=buttonPos44_49; buttonPoses[40]=buttonPos45_49; buttonPoses[41]=buttonPos47_49; buttonPoses[42]=buttonPos48_49;
		} else if (buttonString.Count == 44) { buttonPoses[ 0]=buttonPos02_49; buttonPoses[ 1]=buttonPos03_49; buttonPoses[ 2]=buttonPos05_49; buttonPoses[ 3]=buttonPos06_49; buttonPoses[ 4]=buttonPos08_49; buttonPoses[ 5]=buttonPos09_49; buttonPoses[ 6]=buttonPos10_49; buttonPoses[ 7]=buttonPos11_49; buttonPoses[ 8]=buttonPos12_49; buttonPoses[ 9]=buttonPos13_49; buttonPoses[10]=buttonPos14_49; buttonPoses[11]=buttonPos15_49; buttonPoses[12]=buttonPos16_49; buttonPoses[13]=buttonPos17_49; buttonPoses[14]=buttonPos18_49; buttonPoses[15]=buttonPos19_49; buttonPoses[16]=buttonPos20_49; buttonPoses[17]=buttonPos21_49; buttonPoses[18]=buttonPos22_49; buttonPoses[19]=buttonPos23_49; buttonPoses[20]=buttonPos24_49; buttonPoses[21]=buttonPos25_49; buttonPoses[22]=buttonPos26_49; buttonPoses[23]=buttonPos27_49; buttonPoses[24]=buttonPos28_49; buttonPoses[25]=buttonPos29_49; buttonPoses[26]=buttonPos30_49; buttonPoses[27]=buttonPos31_49; buttonPoses[28]=buttonPos32_49; buttonPoses[29]=buttonPos33_49; buttonPoses[30]=buttonPos34_49; buttonPoses[31]=buttonPos35_49; buttonPoses[32]=buttonPos36_49; buttonPoses[33]=buttonPos37_49; buttonPoses[34]=buttonPos38_49; buttonPoses[35]=buttonPos39_49; buttonPoses[36]=buttonPos40_49; buttonPoses[37]=buttonPos41_49; buttonPoses[38]=buttonPos42_49; buttonPoses[39]=buttonPos44_49; buttonPoses[40]=buttonPos45_49; buttonPoses[41]=buttonPos46_49; buttonPoses[42]=buttonPos47_49; buttonPoses[43]=buttonPos48_49;
		} else if (buttonString.Count == 45) { buttonPoses[ 0]=buttonPos02_49; buttonPoses[ 1]=buttonPos03_49; buttonPoses[ 2]=buttonPos04_49; buttonPoses[ 3]=buttonPos05_49; buttonPoses[ 4]=buttonPos06_49; buttonPoses[ 5]=buttonPos08_49; buttonPoses[ 6]=buttonPos09_49; buttonPoses[ 7]=buttonPos10_49; buttonPoses[ 8]=buttonPos11_49; buttonPoses[ 9]=buttonPos12_49; buttonPoses[10]=buttonPos13_49; buttonPoses[11]=buttonPos14_49; buttonPoses[12]=buttonPos15_49; buttonPoses[13]=buttonPos16_49; buttonPoses[14]=buttonPos17_49; buttonPoses[15]=buttonPos18_49; buttonPoses[16]=buttonPos19_49; buttonPoses[17]=buttonPos20_49; buttonPoses[18]=buttonPos21_49; buttonPoses[19]=buttonPos22_49; buttonPoses[20]=buttonPos23_49; buttonPoses[21]=buttonPos24_49; buttonPoses[22]=buttonPos25_49; buttonPoses[23]=buttonPos26_49; buttonPoses[24]=buttonPos27_49; buttonPoses[25]=buttonPos28_49; buttonPoses[26]=buttonPos29_49; buttonPoses[27]=buttonPos30_49; buttonPoses[28]=buttonPos31_49; buttonPoses[29]=buttonPos32_49; buttonPoses[30]=buttonPos33_49; buttonPoses[31]=buttonPos34_49; buttonPoses[32]=buttonPos35_49; buttonPoses[33]=buttonPos36_49; buttonPoses[34]=buttonPos37_49; buttonPoses[35]=buttonPos38_49; buttonPoses[36]=buttonPos39_49; buttonPoses[37]=buttonPos40_49; buttonPoses[38]=buttonPos41_49; buttonPoses[39]=buttonPos42_49; buttonPoses[40]=buttonPos44_49; buttonPoses[41]=buttonPos45_49; buttonPoses[42]=buttonPos46_49; buttonPoses[43]=buttonPos47_49; buttonPoses[44]=buttonPos48_49;
		} else if (buttonString.Count == 46) { buttonPoses[ 0]=buttonPos01_49; buttonPoses[ 1]=buttonPos02_49; buttonPoses[ 2]=buttonPos03_49; buttonPoses[ 3]=buttonPos05_49; buttonPoses[ 4]=buttonPos06_49; buttonPoses[ 5]=buttonPos07_49; buttonPoses[ 6]=buttonPos08_49; buttonPoses[ 7]=buttonPos09_49; buttonPoses[ 8]=buttonPos10_49; buttonPoses[ 9]=buttonPos11_49; buttonPoses[10]=buttonPos12_49; buttonPoses[11]=buttonPos13_49; buttonPoses[12]=buttonPos14_49; buttonPoses[13]=buttonPos15_49; buttonPoses[14]=buttonPos16_49; buttonPoses[15]=buttonPos17_49; buttonPoses[16]=buttonPos18_49; buttonPoses[17]=buttonPos19_49; buttonPoses[18]=buttonPos20_49; buttonPoses[19]=buttonPos21_49; buttonPoses[20]=buttonPos22_49; buttonPoses[21]=buttonPos23_49; buttonPoses[22]=buttonPos24_49; buttonPoses[23]=buttonPos25_49; buttonPoses[24]=buttonPos26_49; buttonPoses[25]=buttonPos27_49; buttonPoses[26]=buttonPos28_49; buttonPoses[27]=buttonPos29_49; buttonPoses[28]=buttonPos30_49; buttonPoses[29]=buttonPos31_49; buttonPoses[30]=buttonPos32_49; buttonPoses[31]=buttonPos33_49; buttonPoses[32]=buttonPos34_49; buttonPoses[33]=buttonPos35_49; buttonPoses[34]=buttonPos36_49; buttonPoses[35]=buttonPos37_49; buttonPoses[36]=buttonPos38_49; buttonPoses[37]=buttonPos39_49; buttonPoses[38]=buttonPos40_49; buttonPoses[39]=buttonPos41_49; buttonPoses[40]=buttonPos42_49; buttonPoses[41]=buttonPos44_49; buttonPoses[42]=buttonPos45_49; buttonPoses[43]=buttonPos46_49; buttonPoses[44]=buttonPos47_49; buttonPoses[45]=buttonPos48_49;
		} else if (buttonString.Count == 47) { buttonPoses[ 0]=buttonPos01_49; buttonPoses[ 1]=buttonPos02_49; buttonPoses[ 2]=buttonPos03_49; buttonPoses[ 3]=buttonPos05_49; buttonPoses[ 4]=buttonPos06_49; buttonPoses[ 5]=buttonPos07_49; buttonPoses[ 6]=buttonPos08_49; buttonPoses[ 7]=buttonPos09_49; buttonPoses[ 8]=buttonPos10_49; buttonPoses[ 9]=buttonPos11_49; buttonPoses[10]=buttonPos12_49; buttonPoses[11]=buttonPos13_49; buttonPoses[12]=buttonPos14_49; buttonPoses[13]=buttonPos15_49; buttonPoses[14]=buttonPos16_49; buttonPoses[15]=buttonPos17_49; buttonPoses[16]=buttonPos18_49; buttonPoses[17]=buttonPos19_49; buttonPoses[18]=buttonPos20_49; buttonPoses[19]=buttonPos21_49; buttonPoses[20]=buttonPos22_49; buttonPoses[21]=buttonPos23_49; buttonPoses[22]=buttonPos24_49; buttonPoses[23]=buttonPos25_49; buttonPoses[24]=buttonPos26_49; buttonPoses[25]=buttonPos27_49; buttonPoses[26]=buttonPos28_49; buttonPoses[27]=buttonPos29_49; buttonPoses[28]=buttonPos30_49; buttonPoses[29]=buttonPos31_49; buttonPoses[30]=buttonPos32_49; buttonPoses[31]=buttonPos33_49; buttonPoses[32]=buttonPos34_49; buttonPoses[33]=buttonPos35_49; buttonPoses[34]=buttonPos36_49; buttonPoses[35]=buttonPos37_49; buttonPoses[36]=buttonPos38_49; buttonPoses[37]=buttonPos39_49; buttonPoses[38]=buttonPos40_49; buttonPoses[39]=buttonPos41_49; buttonPoses[40]=buttonPos42_49; buttonPoses[41]=buttonPos43_49; buttonPoses[42]=buttonPos44_49; buttonPoses[43]=buttonPos45_49; buttonPoses[44]=buttonPos47_49; buttonPoses[45]=buttonPos48_49; buttonPoses[46]=buttonPos49_49;
		} else if (buttonString.Count == 48) { buttonPoses[ 0]=buttonPos01_49; buttonPoses[ 1]=buttonPos02_49; buttonPoses[ 2]=buttonPos03_49; buttonPoses[ 3]=buttonPos05_49; buttonPoses[ 4]=buttonPos06_49; buttonPoses[ 5]=buttonPos07_49; buttonPoses[ 6]=buttonPos08_49; buttonPoses[ 7]=buttonPos09_49; buttonPoses[ 8]=buttonPos10_49; buttonPoses[ 9]=buttonPos11_49; buttonPoses[10]=buttonPos12_49; buttonPoses[11]=buttonPos13_49; buttonPoses[12]=buttonPos14_49; buttonPoses[13]=buttonPos15_49; buttonPoses[14]=buttonPos16_49; buttonPoses[15]=buttonPos17_49; buttonPoses[16]=buttonPos18_49; buttonPoses[17]=buttonPos19_49; buttonPoses[18]=buttonPos20_49; buttonPoses[19]=buttonPos21_49; buttonPoses[20]=buttonPos22_49; buttonPoses[21]=buttonPos23_49; buttonPoses[22]=buttonPos24_49; buttonPoses[23]=buttonPos25_49; buttonPoses[24]=buttonPos26_49; buttonPoses[25]=buttonPos27_49; buttonPoses[26]=buttonPos28_49; buttonPoses[27]=buttonPos29_49; buttonPoses[28]=buttonPos30_49; buttonPoses[29]=buttonPos31_49; buttonPoses[30]=buttonPos32_49; buttonPoses[31]=buttonPos33_49; buttonPoses[32]=buttonPos34_49; buttonPoses[33]=buttonPos35_49; buttonPoses[34]=buttonPos36_49; buttonPoses[35]=buttonPos37_49; buttonPoses[36]=buttonPos38_49; buttonPoses[37]=buttonPos39_49; buttonPoses[38]=buttonPos40_49; buttonPoses[39]=buttonPos41_49; buttonPoses[40]=buttonPos42_49; buttonPoses[41]=buttonPos43_49; buttonPoses[42]=buttonPos44_49; buttonPoses[43]=buttonPos45_49; buttonPoses[44]=buttonPos46_49; buttonPoses[45]=buttonPos47_49; buttonPoses[46]=buttonPos48_49; buttonPoses[47]=buttonPos49_49;
		} else if (buttonString.Count == 49) { buttonPoses[ 0]=buttonPos01_49; buttonPoses[ 1]=buttonPos02_49; buttonPoses[ 2]=buttonPos03_49; buttonPoses[ 3]=buttonPos04_49; buttonPoses[ 4]=buttonPos05_49; buttonPoses[ 5]=buttonPos06_49; buttonPoses[ 6]=buttonPos07_49; buttonPoses[ 7]=buttonPos08_49; buttonPoses[ 8]=buttonPos09_49; buttonPoses[ 9]=buttonPos10_49; buttonPoses[10]=buttonPos11_49; buttonPoses[11]=buttonPos12_49; buttonPoses[12]=buttonPos13_49; buttonPoses[13]=buttonPos14_49; buttonPoses[14]=buttonPos15_49; buttonPoses[15]=buttonPos16_49; buttonPoses[16]=buttonPos17_49; buttonPoses[17]=buttonPos18_49; buttonPoses[18]=buttonPos19_49; buttonPoses[19]=buttonPos20_49; buttonPoses[20]=buttonPos21_49; buttonPoses[21]=buttonPos22_49; buttonPoses[22]=buttonPos23_49; buttonPoses[23]=buttonPos24_49; buttonPoses[24]=buttonPos25_49; buttonPoses[25]=buttonPos26_49; buttonPoses[26]=buttonPos27_49; buttonPoses[27]=buttonPos28_49; buttonPoses[28]=buttonPos29_49; buttonPoses[29]=buttonPos30_49; buttonPoses[30]=buttonPos31_49; buttonPoses[31]=buttonPos32_49; buttonPoses[32]=buttonPos33_49; buttonPoses[33]=buttonPos34_49; buttonPoses[34]=buttonPos35_49; buttonPoses[35]=buttonPos36_49; buttonPoses[36]=buttonPos37_49; buttonPoses[37]=buttonPos38_49; buttonPoses[38]=buttonPos39_49; buttonPoses[39]=buttonPos40_49; buttonPoses[40]=buttonPos41_49; buttonPoses[41]=buttonPos42_49; buttonPoses[42]=buttonPos43_49; buttonPoses[43]=buttonPos44_49; buttonPoses[44]=buttonPos45_49; buttonPoses[45]=buttonPos46_49; buttonPoses[46]=buttonPos47_49; buttonPoses[47]=buttonPos48_49; buttonPoses[48]=buttonPos49_49;
		} else if (buttonString.Count == 50) { buttonPoses[ 0]=buttonPos03_64; buttonPoses[ 1]=buttonPos04_64; buttonPoses[ 2]=buttonPos05_64; buttonPoses[ 3]=buttonPos06_64; buttonPoses[ 4]=buttonPos10_64; buttonPoses[ 5]=buttonPos11_64; buttonPoses[ 6]=buttonPos12_64; buttonPoses[ 7]=buttonPos13_64; buttonPoses[ 8]=buttonPos14_64; buttonPoses[ 9]=buttonPos15_64; buttonPoses[10]=buttonPos17_64; buttonPoses[11]=buttonPos18_64; buttonPoses[12]=buttonPos19_64; buttonPoses[13]=buttonPos20_64; buttonPoses[14]=buttonPos21_64; buttonPoses[15]=buttonPos22_64; buttonPoses[16]=buttonPos23_64; buttonPoses[17]=buttonPos24_64; buttonPoses[18]=buttonPos25_64; buttonPoses[19]=buttonPos26_64; buttonPoses[20]=buttonPos27_64; buttonPoses[21]=buttonPos28_64; buttonPoses[22]=buttonPos29_64; buttonPoses[23]=buttonPos30_64; buttonPoses[24]=buttonPos31_64; buttonPoses[25]=buttonPos32_64; buttonPoses[26]=buttonPos33_64; buttonPoses[27]=buttonPos34_64; buttonPoses[28]=buttonPos35_64; buttonPoses[29]=buttonPos36_64; buttonPoses[30]=buttonPos37_64; buttonPoses[31]=buttonPos38_64; buttonPoses[32]=buttonPos39_64; buttonPoses[33]=buttonPos40_64; buttonPoses[34]=buttonPos41_64; buttonPoses[35]=buttonPos42_64; buttonPoses[36]=buttonPos43_64; buttonPoses[37]=buttonPos44_64; buttonPoses[38]=buttonPos45_64; buttonPoses[39]=buttonPos46_64; buttonPoses[40]=buttonPos47_64; buttonPoses[41]=buttonPos48_64; buttonPoses[42]=buttonPos50_64; buttonPoses[43]=buttonPos51_64; buttonPoses[44]=buttonPos52_64; buttonPoses[45]=buttonPos53_64; buttonPoses[46]=buttonPos54_64; buttonPoses[47]=buttonPos55_64; buttonPoses[48]=buttonPos60_64; buttonPoses[49]=buttonPos61_64; 
		} else if (buttonString.Count >= 51) { buttonPoses[ 0]=buttonPos01_64; buttonPoses[ 1]=buttonPos02_64; buttonPoses[ 2]=buttonPos03_64; buttonPoses[ 3]=buttonPos04_64; buttonPoses[ 4]=buttonPos05_64; buttonPoses[ 5]=buttonPos06_64; buttonPoses[ 6]=buttonPos07_64; buttonPoses[ 7]=buttonPos08_64; buttonPoses[ 8]=buttonPos09_64; buttonPoses[ 9]=buttonPos10_64; buttonPoses[10]=buttonPos11_64; buttonPoses[11]=buttonPos12_64; buttonPoses[12]=buttonPos13_64; buttonPoses[13]=buttonPos14_64; buttonPoses[14]=buttonPos15_64; buttonPoses[15]=buttonPos16_64; buttonPoses[16]=buttonPos17_64; buttonPoses[17]=buttonPos18_64; buttonPoses[18]=buttonPos19_64; buttonPoses[19]=buttonPos20_64; buttonPoses[20]=buttonPos21_64; buttonPoses[21]=buttonPos22_64; buttonPoses[22]=buttonPos23_64; buttonPoses[23]=buttonPos24_64; buttonPoses[24]=buttonPos25_64; buttonPoses[25]=buttonPos26_64; buttonPoses[26]=buttonPos27_64; buttonPoses[27]=buttonPos28_64; buttonPoses[28]=buttonPos29_64; buttonPoses[29]=buttonPos30_64; buttonPoses[30]=buttonPos31_64; buttonPoses[31]=buttonPos32_64; buttonPoses[32]=buttonPos33_64; buttonPoses[33]=buttonPos34_64; buttonPoses[34]=buttonPos35_64; buttonPoses[35]=buttonPos36_64; buttonPoses[36]=buttonPos37_64; buttonPoses[37]=buttonPos38_64; buttonPoses[38]=buttonPos39_64; buttonPoses[39]=buttonPos40_64; buttonPoses[40]=buttonPos41_64; buttonPoses[41]=buttonPos42_64; buttonPoses[42]=buttonPos43_64; buttonPoses[43]=buttonPos44_64; buttonPoses[44]=buttonPos45_64; buttonPoses[45]=buttonPos46_64; buttonPoses[46]=buttonPos47_64; buttonPoses[47]=buttonPos48_64; buttonPoses[48]=buttonPos49_64; buttonPoses[49]=buttonPos50_64; buttonPoses[50]=buttonPos51_64; buttonPoses[51]=buttonPos52_64; buttonPoses[52]=buttonPos53_64; buttonPoses[53]=buttonPos54_64; buttonPoses[54]=buttonPos55_64; buttonPoses[55]=buttonPos56_64; buttonPoses[56]=buttonPos57_64; buttonPoses[57]=buttonPos58_64; buttonPoses[58]=buttonPos59_64; buttonPoses[59]=buttonPos60_64; buttonPoses[60]=buttonPos61_64; buttonPoses[61]=buttonPos62_64; buttonPoses[62]=buttonPos63_64; buttonPoses[63]=buttonPos64_64;
		}
		if (buttonString.Count <= 1) {			buttons [0].sizeDelta			= buttonSize1;
		} else if (buttonString.Count <= 9) {	for (j1 = 0; j1 < buttonString.Count; j1++) {	buttons [j1].sizeDelta		= buttonSize9;	}
		} else if (buttonString.Count <= 16) {	for (j1 = 0; j1 < buttonString.Count; j1++) {	buttons [j1].sizeDelta		= buttonSize16;	}
		} else if (buttonString.Count <= 25) {	for (j1 = 0; j1 < buttonString.Count; j1++) {	buttons [j1].sizeDelta		= buttonSize25;	}
		} else if (buttonString.Count <= 36) {	for (j1 = 0; j1 < buttonString.Count; j1++) {	buttons [j1].sizeDelta		= buttonSize36;	}
		} else if (buttonString.Count <= 49) {	for (j1 = 0; j1 < buttonString.Count; j1++) {	buttons [j1].sizeDelta		= buttonSize49;	}
		} else {								for (j1 = 0; j1 < buttonString.Count; j1++) {	buttons [j1].sizeDelta		= buttonSize64;	}
		}
		for (j1 = 0; j1 < buttonString.Count; j1++) {
			buttonTexts [j1].text = buttonString [j1];
			buttons [j1].gameObject.SetActive (true);
		}
		for (j1 = buttonString.Count; j1 < buttons.Count; j1++) {
			buttons [j1].gameObject.SetActive (false);
		}
		buttonContainer.SetActive (true);
	}
	
	public IEnumerator MinuteUpdate () {
		//Debug.Log ("Started Entry Query");
		while (SqlJobManager.Instance.sqlRunning) {
			yield return new WaitForSeconds (.1f);
		}
		SqlJobManager.Instance.StartJob ();
		if (line <= 1) {query = "select cast(max(a.[Separator1_gpm])+max(a.[Separator2_gpm])+max(a.[Separator3_gpm])+max(a.[Separator4_gpm])as float),cast(max(a.[UF4200_gpm])+max(a.[UF4600_gpm])+max(a.[DF4100_gpm])+max(a.[RO4000_gpm])+max(a.[NF4300_gpm])+max(a.[NF4400_gpm])as float),sum(cast(b.[Gallons]as float)),case when max(a.[VTIS4_gpm])>0 then max(a.[VTIS4_gpm]) else 0 end,sum(cast(c.[Gallons]as float)),case when max(a.[BlowMolder1_bpm])>0 then max(a.[BlowMolder1_bpm]) end,case when max(a.[Filler1_bpm])>0 then max(a.[Filler1_bpm]) else 0 end,case when max(a.[Sleever1L_bpm])+max (a.[Sleever1R_bpm])>0 then max(a.[Sleever1L_bpm])+max (a.[Sleever1R_bpm]) else 0 end,case when max(a.[CasePacker1_cpm])>0 then max(a.[CasePacker1_cpm]) else 0 end,case when max(a.[Palletizer1_ppm])>0 then max(a.[Palletizer1_ppm]) else 0 end,max(wh.[Value]) from [FairlifeDashboard].[dbo].[Equipment_FiveMinuteRates] [a] left join [FairlifeDashboard].[dbo].[TankInventory] [b] on b.[Tank]='TKBA41' or b.[Tank]='TKBA42' or b.[Tank]='TKBA43' left join [FairlifeDashboard].[dbo].[TankInventory] [c] on c.[Tank]='Alsafe3' or c.[Tank]='Alsafe4' left join [Warehouse].[dbo].[Historian_Temp] wh on wh.[Tag] = 'FgSkids_CPS' where a.[Time]>DateAdd (minute, -5, (select max(x.[time]) from [FairlifeDashboard].[dbo].[Equipment_FiveMinuteRates] x where x.[Separator1_gpm] is not null)) and wh.[Time] = (select max (y.[Time]) from [Warehouse].[dbo].[Historian_Temp] y where y.[Tag] = 'FgSkids_CPS')";} else
		if (line <= 2) {query = "select cast(max(a.[Separator1_gpm])+max(a.[Separator2_gpm])+max(a.[Separator3_gpm])+max(a.[Separator4_gpm])as float),cast(max(a.[UF4200_gpm])+max(a.[UF4600_gpm])+max(a.[DF4100_gpm])+max(a.[RO4000_gpm])+max(a.[NF4300_gpm])+max(a.[NF4400_gpm])as float),sum(cast(b.[Gallons]as float)),case when max(a.[VTIS1_gpm])>0 then max(a.[VTIS1_gpm]) else 0 end,sum(cast(c.[Gallons]as float)),case when max(a.[BlowMolder2_bpm])>0 then max(a.[BlowMolder2_bpm]) end,case when max(a.[Filler2_bpm])>0 then max(a.[Filler2_bpm]) else 0 end,case when max(a.[Sleever2_bpm]) > 0                        then max(a.[Sleever2_bpm])                          else 0 end,case when max(a.[CasePacker2_cpm])>0 then max(a.[CasePacker2_cpm]) else 0 end,case when max(a.[Palletizer2_ppm])>0 then max(a.[Palletizer2_ppm]) else 0 end,max(wh.[Value]) from [FairlifeDashboard].[dbo].[Equipment_FiveMinuteRates] [a] left join [FairlifeDashboard].[dbo].[TankInventory] [b] on b.[Tank]='TKBA01' or b.[Tank]='TKBA02' or b.[Tank]='TKBA03' left join [FairlifeDashboard].[dbo].[TankInventory] [c] on c.[Tank]='Alsafe1'                       left join [Warehouse].[dbo].[Historian_Temp] wh on wh.[Tag] = 'FgSkids_CPS' where a.[Time]>DateAdd (minute, -5, (select max(x.[time]) from [FairlifeDashboard].[dbo].[Equipment_FiveMinuteRates] x where x.[Separator1_gpm] is not null)) and wh.[Time] = (select max (y.[Time]) from [Warehouse].[dbo].[Historian_Temp] y where y.[Tag] = 'FgSkids_CPS')";} else
		if (line <= 3) {query = "select cast(max(a.[Separator1_gpm])+max(a.[Separator2_gpm])+max(a.[Separator3_gpm])+max(a.[Separator4_gpm])as float),cast(max(a.[UF4200_gpm])+max(a.[UF4600_gpm])+max(a.[DF4100_gpm])+max(a.[RO4000_gpm])+max(a.[NF4300_gpm])+max(a.[NF4400_gpm])as float),sum(cast(b.[Gallons]as float)),case when max(a.[VTIS2_gpm])>0 then max(a.[VTIS2_gpm]) else 0 end,sum(cast(c.[Gallons]as float)),case when max(a.[BlowMolder3_bpm])>0 then max(a.[BlowMolder3_bpm]) end,case when max(a.[Filler3_bpm])>0 then max(a.[Filler3_bpm]) else 0 end,case when max(a.[Sleever3_bpm]) > 0                        then max(a.[Sleever3_bpm])                          else 0 end,case when max(a.[CasePacker3_cpm])>0 then max(a.[CasePacker3_cpm]) else 0 end,case when max(a.[Palletizer3_ppm])>0 then max(a.[Palletizer3_ppm]) else 0 end,max(wh.[Value]) from [FairlifeDashboard].[dbo].[Equipment_FiveMinuteRates] [a] left join [FairlifeDashboard].[dbo].[TankInventory] [b] on b.[Tank]='TKBA31' or b.[Tank]='TKBA32' or b.[Tank]='TKBA33' left join [FairlifeDashboard].[dbo].[TankInventory] [c] on c.[Tank]='Alsafe2'                       left join [Warehouse].[dbo].[Historian_Temp] wh on wh.[Tag] = 'FgSkids_CPS' where a.[Time]>DateAdd (minute, -5, (select max(x.[time]) from [FairlifeDashboard].[dbo].[Equipment_FiveMinuteRates] x where x.[Separator1_gpm] is not null)) and wh.[Time] = (select max (y.[Time]) from [Warehouse].[dbo].[Historian_Temp] y where y.[Tag] = 'FgSkids_CPS')";} else
		if (line <= 6) {query = "select cast(max(a.[Separator1_gpm])+max(a.[Separator2_gpm])+max(a.[Separator3_gpm])+max(a.[Separator4_gpm])as float),cast(max(a.[UF4200_gpm])+max(a.[UF4600_gpm])+max(a.[DF4100_gpm])+max(a.[RO4000_gpm])+max(a.[NF4300_gpm])+max(a.[NF4400_gpm])as float),sum(cast(b.[Gallons]as float)),case when max(a.[VTIS2_gpm])>0 then max(a.[VTIS2_gpm]) else 0 end,sum(cast(c.[Gallons]as float)),case when max(a.[BlowMolder3_bpm])>0 then max(a.[BlowMolder3_bpm]) end,case when max(a.[Filler3_bpm])>0 then max(a.[Filler3_bpm]) else 0 end,case when max(a.[Sleever3_bpm]) > 0                        then max(a.[Sleever3_bpm])                          else 0 end,case when max(a.[CasePacker3_cpm])>0 then max(a.[CasePacker3_cpm]) else 0 end,case when max(a.[Palletizer3_ppm])>0 then max(a.[Palletizer3_ppm]) else 0 end,max(wh.[Value]) from [FairlifeDashboard].[dbo].[Equipment_FiveMinuteRates] [a] left join [FairlifeDashboard].[dbo].[TankInventory] [b] on b.[Tank]='TKBA31' or b.[Tank]='TKBA32' or b.[Tank]='TKBA33' left join [FairlifeDashboard].[dbo].[TankInventory] [c] on c.[Tank]='Alsafe2'                       left join [Warehouse].[dbo].[Historian_Temp] wh on wh.[Tag] = 'FgSkids_CPS' where a.[Time]>DateAdd (minute, -5, (select max(x.[time]) from [FairlifeDashboard].[dbo].[Equipment_FiveMinuteRates] x where x.[Separator1_gpm] is not null)) and wh.[Time] = (select max (y.[Time]) from [Warehouse].[dbo].[Historian_Temp] y where y.[Tag] = 'FgSkids_CPS')";}
		
		using (SqlConnection dbCon = new SqlConnection(connectionString)) {
			SqlCommand cmd = new SqlCommand(query, dbCon);
			try {
				dbCon.Open();
				reader = cmd.ExecuteReader();
				if (reader.HasRows) {
					while (reader.Read ()) {
						currentRatesText.text = "\n";
						if (!reader.IsDBNull( 0)) {equipmentRates[ 0]=(float)reader.GetDouble( 0);if(reader.GetDouble( 0)>0){currentRatesText.text+=(reader.GetDouble( 0)).ToString("###,### gpm");   }else{currentRatesText.text+="0 gpm";}}else{equipmentRates[0]=-1;}currentRatesText.text += "\n\n\n";
						if (!reader.IsDBNull( 1)) {equipmentRates[ 1]=(float)reader.GetDouble( 1);if(reader.GetDouble( 1)>0){currentRatesText.text+=(reader.GetDouble( 1)).ToString("###,### gpm");   }else{currentRatesText.text+="0 gpm";}}else{equipmentRates[1]=-1;}currentRatesText.text += "\n\n\n";
						if (!reader.IsDBNull( 2)) {equipmentRates[ 2]=(float)reader.GetDouble( 2);if(reader.GetDouble( 2)>0){currentRatesText.text+=(reader.GetDouble( 2)).ToString("###,### gal");   }else{currentRatesText.text+="0 gal";}}else{equipmentRates[2]=-1;}currentRatesText.text += "\n\n\n";
						if (!reader.IsDBNull( 3)) {equipmentRates[ 3]=(float)reader.GetDouble( 3);if(reader.GetDouble( 3)>0){currentRatesText.text+=(reader.GetDouble( 3)).ToString("###,### gpm");   }else{currentRatesText.text+="0 gpm";}}else{equipmentRates[3]=-1;}currentRatesText.text += "\n\n\n";
						if (!reader.IsDBNull( 4)) {equipmentRates[ 4]=(float)reader.GetDouble( 4);if(reader.GetDouble( 4)>0){currentRatesText.text+=(reader.GetDouble( 4)).ToString("###,### gal");   }else{currentRatesText.text+="0 gal";}}else{equipmentRates[4]=-1;}currentRatesText.text += "\n\n\n";
						if (!reader.IsDBNull( 5)) {equipmentRates[ 5]=(float)reader.GetDouble( 5);if(reader.GetDouble( 5)>0){currentRatesText.text+=(reader.GetDouble( 5)).ToString("###,### bpm");   }else{currentRatesText.text+="0 bpm";}}else{equipmentRates[5]=-1;}currentRatesText.text += "\n\n\n";
						if (!reader.IsDBNull( 6)) {equipmentRates[ 6]=(float)reader.GetDouble( 6);if(reader.GetDouble( 6)>0){currentRatesText.text+=(reader.GetDouble( 6)).ToString("###,### bpm");   }else{currentRatesText.text+="0 bpm";}}else{equipmentRates[6]=-1;}currentRatesText.text += "\n\n\n";
						if (!reader.IsDBNull( 7)) {equipmentRates[ 7]=(float)reader.GetDouble( 7);if(reader.GetDouble( 7)>0){currentRatesText.text+=(reader.GetDouble( 7)).ToString("###,### bpm");   }else{currentRatesText.text+="0 bpm";}}else{equipmentRates[7]=-1;}currentRatesText.text += "\n\n\n";
						if (!reader.IsDBNull( 8)) {equipmentRates[ 8]=(float)reader.GetDouble( 8);if(reader.GetDouble( 8)>0){currentRatesText.text+=(reader.GetDouble( 8)).ToString("###,### cpm");   }else{currentRatesText.text+="0 cpm";}}else{equipmentRates[8]=-1;}currentRatesText.text += "\n\n\n";
						if (!reader.IsDBNull( 9)) {equipmentRates[ 9]=(float)reader.GetDouble( 9);if(reader.GetDouble( 9)>0){currentRatesText.text+=(reader.GetDouble( 9)*60).ToString( "###.## pph");}else{currentRatesText.text+="0 pph";}}else{equipmentRates[9]=-1;}currentRatesText.text += "\n\n\n";
						if (!reader.IsDBNull(10)) {equipmentRates[10]=(float)reader.GetDouble(10);if(reader.GetDouble(10)>0){currentRatesText.text+=(reader.GetDouble(10)).ToString()+"p / 300p"  ;   }else{currentRatesText.text+="";}     }else{equipmentRates[10]=-1;}currentRatesText.text += "\n\n\n";
						equipmentIcons [10].color = equipmentRates[10] < 0 ? statusColors[7] : equipmentRates[10] < 50 ? statusColors[1] : equipmentRates[10] < 250 ? statusColors[2] : equipmentRates[10] < 300 ? statusColors[3] : statusColors[6];
					}
				}
				reader.Close ();
			}
			catch (SqlException exception) {
				if (debug) {Debug.LogWarning(exception.ToString());}
			}
		}
		
		if (line <= 1) {query = "select top(1) case when 2 in([Separator1],[Separator2],[Separator3],[Separator4])then cast(2 as TinyInt) else cast (0 as tinyInt) end as 's',case when 2 in([NF4300],[NF4400],[RO4000],[UF4200],[UF4600])then cast(2 as TinyInt) else cast(0 as TinyInt) end as 'm',case when 2 in ([TKBA41],[TKBA42],[TKBA43]) then cast(2 as TinyInt) else cast(0 as TinyInt) end as 'b',[VTIS4] as 'v',case when 2 in([Alsafe3],[Alsafe4])then cast(2 as TinyInt) else cast(0 as TinyInt) end as 'a',[BlowMolder1] as 'b',[Filler1] as 'f',case when [Sleever1L]=2 and [Sleever1R]=2 then cast(2 as TinyInt) else case when 2 in ([Sleever1L],[Sleever1R]) then cast(1 as TinyInt) else cast(0 as TinyInt) end end as 's',[CasePacker1] as 'c',[Palletizer1] as 'p' from [FairlifeDashboard].[dbo].[EquipmentStates_MinuteData_Prototype1] order by [Time] desc";} else
		if (line <= 2) {query = "select top(1) case when 2 in([Separator1],[Separator2],[Separator3],[Separator4])then cast(2 as TinyInt) else cast (0 as tinyInt) end as 's',case when 2 in([NF4300],[NF4400],[RO4000],[UF4200],[UF4600])then cast(2 as TinyInt) else cast(0 as TinyInt) end as 'm',case when 2 in ([TKBA01],[TKBA02],[TKBA03]) then cast(2 as TinyInt) else cast(0 as TinyInt) end as 'b',[VTIS1] as 'v',[Alsafe1]                                                                              as 'a',[BlowMolder2] as 'b',[Filler2] as 'f',[Sleever2]                                                                                                                                                              as 's',[CasePacker2] as 'c',[Palletizer2] as 'p' from [FairlifeDashboard].[dbo].[EquipmentStates_MinuteData_Prototype1] order by [Time] desc";} else
		if (line <= 3) {query = "select top(1) case when 2 in([Separator1],[Separator2],[Separator3],[Separator4])then cast(2 as TinyInt) else cast (0 as tinyInt) end as 's',case when 2 in([NF4300],[NF4400],[RO4000],[UF4200],[UF4600])then cast(2 as TinyInt) else cast(0 as TinyInt) end as 'm',case when 2 in ([TKBA31],[TKBA32],[TKBA33]) then cast(2 as TinyInt) else cast(0 as TinyInt) end as 'b',[VTIS2] as 'v',[Alsafe2]                                                                              as 'a',[BlowMolder3] as 'b',[Filler3] as 'f',[Sleever3]                                                                                                                                                              as 's',[CasePacker3] as 'c',[Palletizer3] as 'p' from [FairlifeDashboard].[dbo].[EquipmentStates_MinuteData_Prototype1] order by [Time] desc";} else
		if (line <= 6) {query = "select top(1) case when 2 in([Separator1],[Separator2],[Separator3],[Separator4])then cast(2 as TinyInt) else cast (0 as tinyInt) end as 's',case when 2 in([NF4300],[NF4400],[RO4000],[UF4200],[UF4600])then cast(2 as TinyInt) else cast(0 as TinyInt) end as 'm',case when 2 in ([TKBA61],[TKBA62],[TKBA63]) then cast(2 as TinyInt) else cast(0 as TinyInt) end as 'b',[VTIS6] as 'v',[Alsafe6]                                                                              as 'a',[BlowMolder6] as 'b',[Filler6] as 'f',case when [Sleever6L]=2 and [Sleever6R]=2 then cast(2 as TinyInt) else case when 2 in ([Sleever6L],[Sleever6R]) then cast(1 as TinyInt) else cast(0 as TinyInt) end end as 's',[CasePacker6] as 'c',[Palletizer6] as 'p' from [FairlifeDashboard].[dbo].[EquipmentStates_MinuteData_Prototype1] order by [Time] desc";}
		
		using (SqlConnection dbCon = new SqlConnection(connectionString)) {
			SqlCommand cmd = new SqlCommand(query, dbCon);
			try {
				dbCon.Open();
				reader = cmd.ExecuteReader();
				if (reader.HasRows) {
					while (reader.Read ()) {
						for (j1 = 0; j1 < 10; j1++) {
							if (!reader.IsDBNull(j1)) {
								equipmentIcons[j1].color=statusColors [(int)reader.GetByte(j1)];
							}else{
								equipmentIcons[j1].color=statusColors[7];
							}
						}
					}
				}
				reader.Close ();
			}
			catch (SqlException exception) {
				if (debug) {Debug.LogWarning(exception.ToString());}
			}
		}
		
		yield return new WaitForSeconds (1);
		SqlJobManager.Instance.EndJob ();
		yield return null;
	}
}
