using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;


public class CIP_Circuits : MonoBehaviour {
	
	public Color					completedColor;
	public Color					scheduledColor;
	public Color					runningColor;
	public Color					conflictColor;
	public Color					warningColor;
	public Color					abortedColor;
	public Color					overdueColor;
	public GameObject				debugObject;
	
	bool							startingUp				= true;
	GameObject						defaultEntry;
	List <RectTransform> 			dayMarkers 				= new List <RectTransform> ();
	List <RectTransform> 			dayLabelRTs				= new List <RectTransform> ();
	List <Text>						dayLabels				= new List <Text> ();
	Transform						currentTimeMarker;
	float 							unitsPerSecond 			= 0.0208333333333333f;
	float							zoom					= 1;
	//Text							clockText;
	float 							refreshTime 			= 600;
	float 							refreshCounter 			= 598;
	float 							averageTimeCounter 		= 0;
	bool 							refreshing 				= false;
	List <Image>					activeObjects			= new List <Image>();
	List <Image>					inactiveObjects			= new List <Image>();
	Transform						activeContainer;
	Transform						inactiveContainer;
	DateTime 						currentDateTime;
	List <float>					skidYPos				= new List <float> 	{0,0,0,0,0,0,0,0,0,0,0, 450, 380, 310, 240, 170, 100,  30,  -40, 0,0,0,0,0,0,0,0,0,0,0,0, -110, -180, -250, -320, -390, -460};
	List <int> 						skidCircuitCount 		= new List <int> 	{0,0,0,0,0,0,0,0,0,0,0,  14,  17,  15,  15,  20,  19,  16,    8, 0,0,0,0,0,0,0,0,0,0,0,0,    8,    8,    1,    1,    1,    1};
	Scrollbar						horizontalScrollbar;
	GameObject						cipStatusBox;
	Text							cipStatusLeftText;
	Text							cipStatusRightText;
	List <string> 					CIPNames 				= new List <string> {"Unkown", "Caustic", "Acid Sanitization", "Caustic Acid", "Rinse", "Caustic Flush", "Caustic Acid Caustic", "Caustic Acid Sanitize", "Caustic Sanitize", "Hot Water", "Sanitization", "Hot Water Rinse"};
	Vector2							activeContainerStartingPoint;
	Camera							mainCamera;
	Image							schedulingBlock;
	Text							schedulingBlockCircuit;
	Text							schedulingBlockType;
	GameObject						refreshButton;
	GameObject						overlayButton;
	GameObject						loginButton;
	GameObject						backButton;
	GameObject						exitButton;
	Vector2							menuPosition			= new Vector2 (-885, 520);
	Vector2							refreshButtonPosition	= new Vector2 (-885, 470);
	Vector2							overlayButtonPosition	= new Vector2 (-885, 430);
	Vector2							loginButtonPosition		= new Vector2 (-885, 390);
	Vector2							backButtonPosition		= new Vector2 (-885, 350);
	Vector2							exitButtonPosition		= new Vector2 (-885, 310);
	Text							loggedInUser;
	bool							loggedIn;
	GameObject						cipSchedulerGO;
	Text							cipSchedulerText;
	InputField						scheduledReasonInput;
	GameObject						cancelCipButton;
	GameObject						dueOverlay;
	GameObject [] 					CIPDueMarkers 			= new GameObject [37];
	
	
	//Query Information
	SqlDataReader reader;
	string query;
	private string connectionString = @"Data Source = 172.16.5.217; user id = cipapp;password = bestinclass&1;Connection Timeout=10;";
	bool queryRunning = false;
	List <int> skids = new List <int> ();
	List <int> circuits = new List <int> ();
	List <int> IDs = new List <int> ();
	List <int> CIPTypes = new List <int> ();
	List <DateTime> startTimes = new List <DateTime> ();
	List <DateTime> endTimes = new List <DateTime> ();
	List <string> status = new List <string> ();
	List <string> reasons = new List <string> ();
	List <DateTime> entryTimes = new List <DateTime> ();
	List <string> schedulers = new List <string> ();
	int [,,] 						CIPAverageTimes		= new int [37,21,12];
	int []							CIPDueTimes			= new int [37];
	string [,] 						circuitDescriptions	= new string [37,21];
	int								scheduleID;
	
	//Scheduling CIP variables
	int								scheduledSkid;
	int								scheduledCircuit;
	int								scheduledCipType;
	DateTime						scheduledStart;
	DateTime						scheduledEnd;
	
	//Temp Variables
	DateTime 					tempTime;
	float 						f1;
	float						startFloat;
	float						endFloat;
	GameObject					tempGO;
	Image						tempIm;
	int 						i1, i2, j1, j2;
	Ray							ray;
	RaycastHit					hit;
	string						tempString, tempDescription;
	Text						tempText;
	Vector2 					v2 					= new Vector2 (0,0);
	Vector3						v3					= new Vector3 (0,0,0);
	int 						tempSkid, tempCircuit, tempID, tempSeconds;
	
	
	void OnEnable () {
		StartCoroutine (StartUp ());
	}
	
	IEnumerator StartUp () {
		yield return new WaitForSeconds (0);
		//Application.targetFrameRate = 60;	yield return new WaitForSeconds (0);
		defaultEntry 		= 	transform.Find ("Scroll View/Viewport/Content/Default Entry").gameObject;
		currentTimeMarker	=	transform.Find ("Scroll View/Viewport/Content/Current Time Marker");
		dayMarkers.Add 		(	transform.Find ("Scroll View/Viewport/Content/Day Markers/Day -2").gameObject.GetComponent <RectTransform> ());
		dayMarkers.Add 		(	transform.Find ("Scroll View/Viewport/Content/Day Markers/Day -1").gameObject.GetComponent <RectTransform> ());
		dayMarkers.Add 		(	transform.Find ("Scroll View/Viewport/Content/Day Markers/Day 0").gameObject.GetComponent <RectTransform> ());
		dayMarkers.Add 		(	transform.Find ("Scroll View/Viewport/Content/Day Markers/Day 1").gameObject.GetComponent <RectTransform> ());
		dayMarkers.Add 		(	transform.Find ("Scroll View/Viewport/Content/Day Markers/Day 2").gameObject.GetComponent <RectTransform> ());
		dayMarkers.Add 		(	transform.Find ("Scroll View/Viewport/Content/Day Markers/Day 3").gameObject.GetComponent <RectTransform> ());
		dayMarkers.Add 		(	transform.Find ("Scroll View/Viewport/Content/Day Markers/Day 4").gameObject.GetComponent <RectTransform> ());
		dayMarkers.Add 		(	transform.Find ("Scroll View/Viewport/Content/Day Markers/Day 5").gameObject.GetComponent <RectTransform> ());
		dayMarkers.Add 		(	transform.Find ("Scroll View/Viewport/Content/Day Markers/Day 6").gameObject.GetComponent <RectTransform> ());
		dayMarkers.Add 		(	transform.Find ("Scroll View/Viewport/Content/Day Markers/Day 7").gameObject.GetComponent <RectTransform> ());
		yield return new WaitForSeconds (0);
		dayLabelRTs.Add		(	transform.Find ("Day Labels/Day -2 Label").gameObject.GetComponent <RectTransform> ());
		dayLabelRTs.Add		(	transform.Find ("Day Labels/Day -1 Label").gameObject.GetComponent <RectTransform> ());
		dayLabelRTs.Add		(	transform.Find ("Day Labels/Day 0 Label").gameObject.GetComponent <RectTransform> ());
		dayLabelRTs.Add		(	transform.Find ("Day Labels/Day 1 Label").gameObject.GetComponent <RectTransform> ());
		dayLabelRTs.Add		(	transform.Find ("Day Labels/Day 2 Label").gameObject.GetComponent <RectTransform> ());
		dayLabelRTs.Add		(	transform.Find ("Day Labels/Day 3 Label").gameObject.GetComponent <RectTransform> ());
		dayLabelRTs.Add		(	transform.Find ("Day Labels/Day 4 Label").gameObject.GetComponent <RectTransform> ());
		dayLabelRTs.Add		(	transform.Find ("Day Labels/Day 5 Label").gameObject.GetComponent <RectTransform> ());
		dayLabelRTs.Add		(	transform.Find ("Day Labels/Day 6 Label").gameObject.GetComponent <RectTransform> ());
		dayLabelRTs.Add		(	transform.Find ("Day Labels/Day 7 Label").gameObject.GetComponent <RectTransform> ());
		yield return new WaitForSeconds (0);
		//clockText			=	transform.Find ("CurrentTime").gameObject.GetComponent <Text> ();
		activeContainer 	= 	transform.Find ("Scroll View/Viewport/Content/Active Container");
		inactiveContainer 	= 	transform.Find ("Scroll View/Viewport/Content/Inactive Container");
		dueOverlay			=	transform.Find ("Scroll View/Viewport/Content/Due Overlay").gameObject;
		horizontalScrollbar = 	transform.Find ("Scroll View/Scrollbar Horizontal").GetComponent <Scrollbar> ();
		cipStatusBox		=	transform.Find ("CIP Status").gameObject;
		cipStatusLeftText	=	transform.Find ("CIP Status/Left Text").gameObject.GetComponent <Text>();
		cipStatusRightText	=	transform.Find ("CIP Status/Right Text").gameObject.GetComponent <Text> ();
		mainCamera			=	GameObject.FindWithTag("MainCamera").GetComponent <Camera>();
		refreshButton		=	transform.Find ("Refresh Button").gameObject;
		overlayButton		=	transform.Find ("Overlay Button").gameObject;
		loginButton			=	transform.Find ("Login Button").gameObject;
		backButton			=	transform.Find ("Back Button").gameObject;
		exitButton			=	transform.Find ("Exit Button").gameObject;
		loggedInUser		=	transform.Find ("Logged In User").gameObject.GetComponent <Text> ();
		cipSchedulerGO		=	transform.Find ("CIP Scheduler").gameObject;
		cipSchedulerText	=	transform.Find ("CIP Scheduler/Left Text").gameObject.GetComponent <Text> ();
		scheduledReasonInput=	transform.Find ("CIP Scheduler/InputField").gameObject.GetComponent <InputField> ();
		cancelCipButton		=	transform.Find ("CIP Status/Cancel CIP").gameObject;
		yield return new WaitForSeconds (0);
		dayLabels.Add 		(	dayLabelRTs [0].gameObject.GetComponent <Text> ());
		dayLabels.Add 		(	dayLabelRTs [1].gameObject.GetComponent <Text> ());
		dayLabels.Add 		(	dayLabelRTs [2].gameObject.GetComponent <Text> ());
		dayLabels.Add 		(	dayLabelRTs [3].gameObject.GetComponent <Text> ());
		dayLabels.Add 		(	dayLabelRTs [4].gameObject.GetComponent <Text> ());
		dayLabels.Add 		(	dayLabelRTs [5].gameObject.GetComponent <Text> ());
		dayLabels.Add 		(	dayLabelRTs [6].gameObject.GetComponent <Text> ());
		dayLabels.Add 		(	dayLabelRTs [7].gameObject.GetComponent <Text> ());
		dayLabels.Add 		(	dayLabelRTs [8].gameObject.GetComponent <Text> ());
		dayLabels.Add 		(	dayLabelRTs [9].gameObject.GetComponent <Text> ());
		yield return new WaitForSeconds (0);
		currentDateTime = DateTime.Now;
		tempTime = DateTime.Now - new TimeSpan (4,0,0,0);
		tempTime = tempTime.Subtract (new TimeSpan (0,DateTime.Now.Hour,	DateTime.Now.Minute,											DateTime.Now.Second - 1,	DateTime.Now.Millisecond));
		v2.y = 0;
		for (i1 = 0; i1 < dayMarkers.Count; i1++) {
			tempTime = tempTime + new TimeSpan (1,0,0,0);
			f1 = (float)(tempTime - DateTime.Now).TotalSeconds * unitsPerSecond * zoom;
			f1 += -3600; //-3600 is the current time location
			v2.x = f1;
			dayMarkers [i1].localPosition = v2;
			dayLabels  [i1].text = tempTime.ToString ("MMM-dd");
		}
		yield return new WaitForSeconds (0);
		activeContainerStartingPoint = activeContainer.transform.localPosition;
		AddObject ();
		schedulingBlock = tempIm;
		activeObjects.Remove (tempIm);
		schedulingBlock.gameObject.name = "SchedulingBlock";
		schedulingBlockCircuit	=	schedulingBlock.transform.GetChild (0).GetComponent <Text> ();
		schedulingBlockType		=	schedulingBlock.transform.GetChild (1).GetComponent <Text> ();
		StartCoroutine ("FetchCircuitDescriptions");
		StartCoroutine (RefreshAverageTimes ());
		yield return new WaitForSeconds (0);
		if (debugObject) {
			Debug.Log (debugObject.transform.localPosition);
		}
		horizontalScrollbar.value = .3f;
		
		startingUp = false;
		yield return null;
	}
	
	void Update () {
		if (startingUp) {return;}
		currentDateTime = DateTime.Now;
		refreshCounter += Time.deltaTime;
		averageTimeCounter += Time.deltaTime;
		
		if (refreshCounter >= refreshTime && !refreshing) {
			refreshing = true;
			refreshCounter -= refreshTime;
			StartCoroutine ("Refresh");
			return;
		}
		
		v2.y = 0;
		for (i1 = 0; i1 < dayMarkers.Count; i1++) {
			v2.x = dayMarkers [i1].transform.localPosition.x - (Time.deltaTime * unitsPerSecond * zoom);
			dayMarkers [i1].transform.localPosition = v2;
		}
		
		//Moves day label to match with the actual time
		v2.y = dayLabelRTs [0].position.y;
		f1 = dayLabelRTs [0].localPosition.y;
		for (i1 = 0; i1 < dayLabelRTs.Count; i1++) {
			v2.x = dayMarkers [i1].position.x;
			dayLabelRTs [i1].position = v2;
			if (dayLabelRTs [i1].localPosition.x < -910 || dayLabelRTs [i1].localPosition.x > 900) {
				dayLabelRTs [i1].gameObject.SetActive (false);
			} else {
				dayLabelRTs [i1].gameObject.SetActive (true);
				v2.y = f1;
				v2.x = dayLabelRTs [i1].localPosition.x;
				dayLabelRTs [i1].localPosition = v2;
			}
		}
		activeContainer.transform.Translate (Vector2.left * -Time.deltaTime * unitsPerSecond * zoom);
		if (refreshButton.activeSelf) {
			refreshButton.transform.localPosition 	= Vector2.Lerp (refreshButton.transform.localPosition, 	refreshButtonPosition, .3f);
			overlayButton.transform.localPosition 	= Vector2.Lerp (overlayButton.transform.localPosition, 	overlayButtonPosition, .3f);
			loginButton.transform.localPosition 	= Vector2.Lerp (loginButton.transform.localPosition, 	loginButtonPosition, .3f);
			backButton.transform.localPosition 		= Vector2.Lerp (backButton.transform.localPosition, 	backButtonPosition, .3f);
			exitButton.transform.localPosition 		= Vector2.Lerp (exitButton.transform.localPosition, 	exitButtonPosition, .3f);
		}
		if (cipSchedulerGO.activeSelf) {
			if (scheduledStart < currentDateTime) {
				scheduledStart += new TimeSpan (0,0,1,0);
				UpdateSchedulingCIP ();
			}
		}
	}
	
	public void EventClick (GameObject go) {
		schedulingBlock.gameObject.SetActive (false);
		if (int.TryParse (go.name, out scheduleID)) {
			if 		(skids [scheduleID] <= 32) {
				cipStatusLeftText.text = 	(skids [scheduleID] <= 32 ? "CC" + skids [scheduleID].ToString () + "_" + circuits [scheduleID].ToString ("00") : (skids [scheduleID] == 33 ? "VTIS1" : skids [scheduleID] == 34 ? "VTIS2" : skids [scheduleID] == 35 ? "VTIS4" : skids [scheduleID] == 36 ? "VTIS6" : "")) + "\n" + 
											circuitDescriptions [skids [scheduleID],circuits [scheduleID]] + "\n" + 
											status [scheduleID] + "\n" + 
											(skids [scheduleID] <= 32 ? CIPNames [CIPTypes [scheduleID]] : CIPTypes [scheduleID] == 2 ? "AIC" : "CIP") + "\n" + 
											startTimes [scheduleID].ToString ("MMM dd HH:mm") + " - " + endTimes [scheduleID].ToString ("MMM dd HH:mm") + "\n" + 
											"User: " + schedulers [scheduleID] + "\n" + 
											"Scheduled " + entryTimes [scheduleID].ToString ("MMM dd HH:mm") + "\n" + 
											reasons [scheduleID];
			} else if (skids [scheduleID] <= 36) {
				cipStatusLeftText.text = 	(skids [scheduleID] == 33 ? "VTIS1" : skids [scheduleID] == 34 ? "VTIS2" : skids [scheduleID] == 35 ? "VTIS4" : skids [scheduleID] == 36 ? "VTIS6" : "") + "\n" + 
											status [scheduleID] + "\n" + 
											(CIPTypes [scheduleID] == 1 ? "CIP" : "AIC") + "\n" + 
											startTimes [scheduleID].ToString ("MMM dd HH:mm") + " - " + endTimes [scheduleID].ToString ("MMM dd HH:mm") + "\n" + 
											"User: " + schedulers [scheduleID] + "\n" + 
											"Scheduled " + entryTimes [scheduleID].ToString ("MMM dd HH:mm") + "\n" + 
											reasons [scheduleID];
			}

			cipStatusRightText.text =	IDs [scheduleID].ToString ();
			if (loggedIn && status [scheduleID] == "SCHEDULED") {
				cancelCipButton.SetActive (true);
			} else {
				cancelCipButton.SetActive (false);
			}
			cipStatusBox.SetActive (true);
			cipSchedulerGO.SetActive (false);
		} else {
			if (cipStatusBox.activeSelf) {	cipStatusBox.SetActive (false);	}
			if (cipSchedulerGO.activeSelf) {
				cipSchedulerGO.SetActive (false);
			}
		}
	}
		
	public void BackdropClick (int skid) {
		schedulingBlock.gameObject.SetActive (false);
		cipStatusBox.SetActive (false);
		if (!loggedIn) {
			return;
		}
		ray = mainCamera.ScreenPointToRay (Input.mousePosition);
		if (Physics.Raycast (ray, out hit)) {
			f1 = hit.transform.InverseTransformPoint (hit.point).x - hit.transform.InverseTransformPoint (currentTimeMarker.transform.position).x;
			if (f1 > 5) { //This means they are clicking a spot to the right of the current time marker, so we'll add something to schedule a new CIP
				scheduledSkid		=	skid;
				scheduledCircuit	=	1;
				scheduledCipType	=	scheduledSkid <= 32 ? 7 : 1;
				scheduledStart		= 	currentDateTime + new TimeSpan (0,0,0,Mathf.RoundToInt (f1 / (unitsPerSecond * zoom)));
				scheduledEnd		=	scheduledStart  + new TimeSpan (0,0,0,CIPAverageTimes [scheduledSkid,scheduledCircuit,scheduledCipType]);
				UpdateSchedulingCIP ();
				schedulingBlock.gameObject.SetActive (true);
				scheduledReasonInput.Select ();
				scheduledReasonInput.text = "";
				cipSchedulerGO.SetActive (true);
			} else if (cipSchedulerGO.activeSelf) {
				cipSchedulerGO.SetActive (false);
			}
		} else if (cipSchedulerGO.activeSelf) {
			cipSchedulerGO.SetActive (false);
		}
	}
	
	public void ButtonClick (int button) {
		if (button <= 0) {
			//Nothing assigned
		} else if (button == 1) {
			if (refreshButton.activeSelf) {
				CloseMenu ();
			} else {
				refreshButton.transform.localPosition 	= menuPosition;
				overlayButton.transform.localPosition 	= menuPosition;
				loginButton.transform.localPosition		= menuPosition;
				backButton.transform.localPosition 		= menuPosition;
				exitButton.transform.localPosition 		= menuPosition;
				refreshButton.SetActive (true);
				overlayButton.SetActive (true);
				loginButton.SetActive (true);
				backButton.SetActive (true);
				exitButton.SetActive (true);
			}
		} else if (button <= 6) {
			CloseMenu ();
			if (button == 2) { //Refresh
				//StartCoroutine (Refresh ());
				refreshCounter = refreshTime;
			} else if (button == 3) { //Overlay
				//Should open a menu asking what overlay (s) they would like
				if (!dueOverlay.activeSelf) {
					StartCoroutine(PullOverdueStatus ());
					dueOverlay.SetActive (true);
				} else {
					dueOverlay.SetActive (false);
				}
			} else if (button == 4) { //Login
				//Should send them to the login screen
				if (loggedIn) {
					loggedInUser.text = "";
					loginButton.transform.GetChild (0).GetComponent <Text> ().text = "Log In";
				} else {
					loggedInUser.text = "(Administrator)";
					loginButton.transform.GetChild (0).GetComponent <Text> ().text = "Log Out";
				}
				loggedIn = !loggedIn;
			} else if (button == 5) { //Back
				SceneManager.LoadScene(0);
				//Application.LoadLevel(Application.loadedLevel);
			} else { //Exit
				Application.Quit ();
				SceneManager.LoadScene(0); //Should only happen if you are in the editor
			}
		} else if (button <= 9) { //CancelCIP
			StartCoroutine (CancelCIP ());
		} else if (button <= 22) { //Schedule CIP
			if 			(button == 11) {
				if (scheduledCircuit > 1) 								{	scheduledCircuit--;	} else {	scheduledCircuit = skidCircuitCount [scheduledSkid];}
			} else if 	(button == 12) {
				if (scheduledCircuit < skidCircuitCount [scheduledSkid]){	scheduledCircuit++;	} else {	scheduledCircuit = 1;								}
			} else if 	(button == 13) {
				if (scheduledCipType > 1)								{	scheduledCipType--;	} else {	scheduledCipType = (scheduledSkid <= 32?11:2);		}
			} else if 	(button == 14) {
				if (scheduledCipType < (scheduledSkid <= 32?11:2))		{	scheduledCipType++;	} else {	scheduledCipType = 1;								}
			} else if 	(button == 15) {
				scheduledStart -= new TimeSpan ( 1,  0,  0,  0); if (scheduledStart < currentDateTime) {scheduledStart = currentDateTime;}
			} else if 	(button == 16) {
				scheduledStart += new TimeSpan ( 1,  0,  0,  0);
			} else if 	(button == 17) {
				scheduledStart -= new TimeSpan ( 0,  1,  0,  0); if (scheduledStart < currentDateTime) {scheduledStart = currentDateTime;}
			} else if 	(button == 18) {
				scheduledStart += new TimeSpan ( 0,  1,  0,  0);
			} else if 	(button == 19) {
				scheduledStart -= new TimeSpan ( 0,  0,  1,  0); if (scheduledStart < currentDateTime) {scheduledStart = currentDateTime;}
			} else if 	(button == 20) {
				scheduledStart += new TimeSpan ( 0,  0,  1,  0);
			} else if 	(button == 21) { //Cancel
				schedulingBlock.gameObject.SetActive (false);
				cipSchedulerGO.SetActive (false); 
				scheduledReasonInput.text = "THIS SHOULD BE HERE";return;
			} else {
				StartCoroutine (UploadSchedule ());
				schedulingBlock.gameObject.SetActive (false);
				cipSchedulerGO.SetActive (false); return;
			}
			UpdateSchedulingCIP ();
		}
	}
	
	public void UpdateSchedulingCIP () {
		scheduledEnd		=	scheduledStart  + new TimeSpan (0,0,0,CIPAverageTimes [scheduledSkid,scheduledCircuit,scheduledCipType]);
		if (scheduledSkid <= 32) {
			cipSchedulerText.text = "CC" + scheduledSkid.ToString ("00") + " <- " + scheduledCircuit + " ->\n" + 
									circuitDescriptions [scheduledSkid,scheduledCircuit] + "\n<- " +
									CIPNames [scheduledCipType] + " ->\nStarts: " + 
									scheduledStart.ToString ("MMM <dd> <HH>:<mm>") +
									"\nEnds: " + scheduledEnd.ToString ("MMM dd HH:mm") + "  (" + ReadableDateDifference (scheduledStart, scheduledEnd) + ")";
		} else if (scheduledSkid <= 36) { //VTIS
			cipSchedulerText.text = (scheduledSkid <= 33 ? "VTIS1\n\n" : scheduledSkid <= 34 ? "VTIS2\n\n" : scheduledSkid <= 35 ? "VTIS4\n\n" : "VTIS6\n\n") + 
									(scheduledCipType <= 1 ? "<- CIP ->\nStarts: " : "<- AIC ->\nStarts: ") + 
									scheduledStart.ToString ("MMM <dd> <HH>:<mm>") +
									"\nEnds: " + scheduledEnd.ToString ("MMM dd HH:mm") + "  (" + ReadableDateDifference (scheduledStart, scheduledEnd) + ")";
		}
		startFloat = (float)(scheduledStart - currentDateTime).TotalSeconds * unitsPerSecond * zoom;
		endFloat = (float)(scheduledEnd - currentDateTime).TotalSeconds * unitsPerSecond * zoom;
		v2.x = ((startFloat + endFloat) / 2) - 3600;
		v2.y = skidYPos [scheduledSkid];
		schedulingBlock.transform.localPosition = v2;
		v2.x = (endFloat - startFloat);
		v2.y = 50;
		schedulingBlock.transform.localScale = v2;
		schedulingBlockType.text = GetCIPTypeText (scheduledSkid, scheduledCipType);
		v2.x = 1 / schedulingBlock.transform.localScale.x;
		v2.y = 0.02f;
		schedulingBlockType.transform.localScale = v2;
		v2.x = schedulingBlock.transform.localScale.x;
		v2.y = 50;
		schedulingBlockType.GetComponent <RectTransform> ().sizeDelta = v2;
		if (schedulingBlock.transform.localScale.x > 20) {
			if (scheduledSkid <= 32) {
				if (scheduledCircuit < 10) {
					tempString = " " + scheduledCircuit.ToString ();
				} else {
					tempString = " " + scheduledCircuit.ToString().Substring (0,1) + "\n " + scheduledCircuit.ToString().Substring (1,1);
				}
				v2.x = 1 / schedulingBlock.transform.localScale.x;
				v2.y = 0.02f;
				schedulingBlockCircuit.transform.localScale = v2;
				v2.x = schedulingBlock.transform.localScale.x;
				v2.y = 50;
				schedulingBlockCircuit.GetComponent <RectTransform> ().sizeDelta = v2;
				schedulingBlockCircuit.text = tempString;
			} else if (scheduledSkid <= 36) {
				schedulingBlockCircuit.text = "";
			}
		}
		
		/*
		IDText.text 			= IDs [ID].ToString ();
		statusText.text			= status [ID];
		dateText.text			= startTimes [ID].ToString ("MMM dd HH:mm") + " - " + endTimes [ID].ToString ("MMM dd HH:mm");
		schedulerText.text		= schedulers [ID];
		scheduledDateText.text	= "Scheduled " + entryTimes [ID].ToString ("MMM dd HH:mm");
		reasonText.text			= reasons [ID];
		*/
	}
	
	void CloseMenu () {
		refreshButton.SetActive (false);
		overlayButton.SetActive (false);
		loginButton.SetActive (false);
		backButton.SetActive (false);
		exitButton.SetActive (false);
	}
	
	IEnumerator PullOverdueStatus () {
		while (queryRunning) {
			yield return new WaitForSeconds (.1f);
		}
		QueryOn ();
		query = "exec dbo.spPullCIPSkidOverdueStatus";
		using (SqlConnection dbCon = new SqlConnection(connectionString)) {
			SqlCommand cmd = new SqlCommand(query, dbCon);
			try {
				dbCon.Open();
				reader = cmd.ExecuteReader();
				if (reader.HasRows) {
					while (reader.Read ()) {
						tempSkid = 		(int)reader.GetSqlInt32 (0);
						if (!reader.IsDBNull (2)) {
							CIPDueTimes [tempSkid] = 	(int)reader.GetSqlInt32 (2) * 60;
						} else {
							CIPDueTimes [tempSkid] = 	21600;
						}
						if (CIPDueMarkers [tempSkid] == null) {
							AddObject ();
							CIPDueMarkers [tempSkid] = tempIm.gameObject;
							CIPDueMarkers [tempSkid].transform.SetParent (dueOverlay.transform);
							tempIm.color = overdueColor;
							CIPDueMarkers [tempSkid].name = "CIPDueMarker " + tempSkid;
							/*tempObject = Instantiate (defaultCube, dueOverlay);
							//tempObject.transform.SetParent (dueOverlay);
							CIPDueMarkers [tempSkid] = tempObject;
							CIPDueMarkers [tempSkid].GetComponent <MeshRenderer>().material = dueMaterial;
							CIPDueMarkers [tempSkid].GetComponent <BoxCollider>().enabled = false;
							CIPDueMarkers [tempSkid].transform.GetChild (0).gameObject.SetActive (false);
							//CIPDueMarkers [tempSkid].transform.GetChild (1).gameObject.SetActive (false);
							CIPDueMarkers [tempSkid].name = "CIPDueMarker " + tempSkid;*/
						}
						startFloat = ((float)CIPDueTimes [tempSkid] * unitsPerSecond * zoom) - 3600;
						//if (startFloat < 0) {startFloat = 0;} 
						endFloat = /*startFloat + 10;//*/649947 * unitsPerSecond * zoom;
						v2.x = ((startFloat + endFloat) / 2)/* - 3600*/;
						v2.y = skidYPos [tempSkid];
						tempIm.transform.localPosition = v2;
						v3.x = (endFloat - startFloat);
						v3.y = 70;
						v3.z = 0.1f;
						tempIm.transform.localScale = v3;
						
						/*CIPDueMarkers [tempSkid].transform.position = new Vector3 ((startFloat + endFloat) / 2, skidYPos [tempSkid], 50000);
						CIPDueMarkers [tempSkid].transform.localScale = new Vector3 ((endFloat - startFloat), 2500, 50);
						CIPDueMarkers [tempSkid].SetActive (true);*/
					}
				}
				reader.Close ();
			}
			catch (SqlException exception) {
				Debug.LogWarning(exception.ToString());
			}
		}
		QueryOff ();
		yield return null;	
	}
	
	IEnumerator Refresh () {
		skids.Clear ();
		circuits.Clear ();
		IDs.Clear ();
		startTimes.Clear ();
		endTimes.Clear ();
		status.Clear ();
		reasons.Clear ();
		entryTimes.Clear ();
		schedulers.Clear ();
		while (queryRunning) {
			yield return new WaitForSeconds (.1f);
		}
		QueryOn ();
		activeContainer.transform.localPosition = activeContainerStartingPoint;
		query = "exec dbo.spPullCIPSchedule";
		using (SqlConnection dbCon = new SqlConnection(connectionString)) {
			SqlCommand cmd = new SqlCommand(query, dbCon);
			try {
				dbCon.Open();
				reader = cmd.ExecuteReader();
				if (reader.HasRows) {
					while (reader.Read ()) {
						skids.Add		((int)reader.GetSqlInt32 (0));
						circuits.Add	((int)reader.GetSqlInt32 (1));
						IDs.Add 		((int)reader.GetSqlInt32 (2));
						CIPTypes.Add	((int)reader.GetSqlInt32 (3));
						startTimes.Add	((DateTime)reader.GetSqlDateTime (4));
						endTimes.Add	((DateTime)reader.GetSqlDateTime (5));
						status.Add		(reader.GetSqlString (6).ToString ());
						reasons.Add		(reader.GetSqlString (7).ToString ());
						entryTimes.Add	((DateTime)reader.GetSqlDateTime (8));
						schedulers.Add	(reader.GetSqlString (9).ToString ());
					}
				}
				reader.Close ();
			}
			catch (SqlException exception) {
				Debug.LogWarning(exception.ToString());
			}
		}
		QueryOff ();
		for (j1 = 0; j1 < 9999; j1++) {
			if (activeObjects.Count > 0) {RemoveObject (activeObjects [0]);
			} else { j1 = 9999; }
		}
		yield return new WaitForSeconds (.1f);
		for (j1 = 0; j1 < startTimes.Count; j1++) {
			AddObject ();
			tempIm.gameObject.name = j1.ToString ();

			if 			(status [j1] == "COMPLETE") {	tempIm.color = completedColor;
			} else if 	(status [j1] == "ABORTED") 	{	tempIm.color = abortedColor;
			} else if 	(status [j1] == "RUNNING") 	{	tempIm.color = runningColor;	if (endTimes [j1] < currentDateTime) {endTimes [j1] = currentDateTime; tempIm.color = warningColor; tempIm.transform.GetChild (2).gameObject.SetActive (true);}
			} else 									{	tempIm.color = scheduledColor;
			}
			startFloat = (float)(startTimes [j1] - currentDateTime).TotalSeconds * unitsPerSecond * zoom;
			endFloat = (float)(endTimes [j1] - currentDateTime).TotalSeconds * unitsPerSecond * zoom;
			v2.x = ((startFloat + endFloat) / 2) - 3600;
			v2.y = skidYPos [skids [j1]];
			tempIm.transform.localPosition = v2;
			v2.x = (endFloat - startFloat);
			v2.y = 50;
			tempIm.transform.localScale = v2;
			if (skids [j1] <= 32 && v2.x > 20) {
				//Circuit Name
				if (circuits [j1] < 10) {
					tempString = " " + circuits [j1].ToString ();
				} else {
					tempString = " " + circuits [j1].ToString().Substring (0,1) + "\n " + circuits [j1].ToString().Substring (1,1);
				}
				tempText = tempIm.transform.GetChild (0).GetComponent <Text> ();
				tempText.text = tempString;
				v2.x = 1 / tempIm.transform.localScale.x;
				v2.y = 0.02f;
				tempText.transform.localScale = v2;
				v2.x = tempIm.transform.localScale.x;
				v2.y = 50;
				tempText.GetComponent <RectTransform> ().sizeDelta = v2;
				tempText.gameObject.SetActive (true);
				//CIP Type
				//"Unkown", "Caustic", "Acid Sanitization", "Caustic Acid", "Rinse", "Caustic Flush", "Caustic Acid Caustic", "Caustic Acid Sanitize", "Caustic Sanitize", "Hot Water", "Sanitization", "Hot Water Rinse"
				if (tempIm.transform.localScale.x > 40) {
					if 			(CIPTypes [j1] <=  0) {
						tempString = "? ";
					} else if 	(CIPTypes [j1] <=  1) {
						tempString = "C ";
					} else if 	(CIPTypes [j1] <=  2) {
						tempString = "A \nS ";
					} else if 	(CIPTypes [j1] <=  3) {
						tempString = "C \nA ";
					} else if 	(CIPTypes [j1] <=  4) {
						tempString = "R ";
					} else if 	(CIPTypes [j1] <=  5) {
						tempString = "C \nF ";
					} else if 	(CIPTypes [j1] <=  6) {
						tempString = "C \nA \nC ";
					} else if 	(CIPTypes [j1] <=  7) {
						tempString = "C \nA \nS ";
					} else if 	(CIPTypes [j1] <=  8) {
						tempString = "C \nS ";
					} else if 	(CIPTypes [j1] <=  9) {
						tempString = "H \nW ";
					} else if 	(CIPTypes [j1] <= 10) {
						tempString = "S ";
					} else if 	(CIPTypes [j1] <= 11) {
						tempString = "H \nW \nR ";
					}
					tempText = tempIm.transform.GetChild (1).GetComponent <Text> ();
					tempText.text = tempString;
					v2.x = 1 / tempIm.transform.localScale.x;
					v2.y = 0.02f;
					tempText.transform.localScale = v2;
					v2.x = tempIm.transform.localScale.x;
					v2.y = 50;
					tempText.GetComponent <RectTransform> ().sizeDelta = v2;
					tempText.gameObject.SetActive (true);
				} else {
					tempIm.transform.GetChild (1).gameObject.SetActive (false);
				}
			} else { //VTIS is the only option right now
				tempIm.transform.GetChild (0).gameObject.SetActive (false);
				if (tempIm.transform.localScale.x > 40) {
					if 			(CIPTypes [j1] <=  1) {
						tempString = "C \nI  \nP ";
					} else if 	(CIPTypes [j1] <=  2) {
						tempString = "A \nI  \nC ";
					}
					tempText = tempIm.transform.GetChild (1).GetComponent <Text> ();
					tempText.text = tempString;
					v2.x = 1 / tempIm.transform.localScale.x;
					v2.y = 0.02f;
					tempText.transform.localScale = v2;
					v2.x = tempIm.transform.localScale.x;
					v2.y = 50;
					tempText.GetComponent <RectTransform> ().sizeDelta = v2;
					tempText.gameObject.SetActive (true);
				} else {
					tempIm.transform.GetChild (1).gameObject.SetActive (false);
				}
			}
			tempIm.gameObject.SetActive (true);
		}
		horizontalScrollbar.value = .3f;
		refreshing = false;
		refreshCounter = 0;
		/*StartCoroutine (PullOverdueStatus ());*/
		yield return null;
	}
	
	IEnumerator FetchCircuitDescriptions () {
		yield return new WaitForSeconds (1);
		/*while (queryRunning) {
			yield return new WaitForSeconds (.1f);
		}
		QueryOn ();*/
		query = "exec dbo.spPullCIPCircuitDescriptions";
		using (SqlConnection dbCon = new SqlConnection(connectionString)) {
			SqlCommand cmd = new SqlCommand(query, dbCon);
			try {
				dbCon.Open();
				reader = cmd.ExecuteReader();
				if (reader.HasRows) {
					while (reader.Read ()) {
						circuitDescriptions [(int)reader.GetSqlInt32 (0),(int)reader.GetSqlInt32 (1)] = reader.GetSqlString (2).ToString ();
					}
				}
				reader.Close ();
			}
			catch (SqlException exception) {
				Debug.LogWarning(exception.ToString());
			}
		}
		//QueryOff ();
		yield return null;	
	}
	
	IEnumerator RefreshAverageTimes () {
		for (j1 = 0; j1 < 33; j1++) { //This populates the table with default values in case we haven't run a given combination before
			for (j2 = 0; j2 < 21; j2++) {
				CIPAverageTimes					[j1, j2,  0] = 5749; //Unknown 				- This is the average for every CIP
				CIPAverageTimes					[j1, j2,  1] = 5151; //Caustic
				CIPAverageTimes					[j1, j2,  2] = 3770; //Acid Sanitization
				CIPAverageTimes					[j1, j2,  3] = 8127; //Caustic Acid
				CIPAverageTimes					[j1, j2,  4] =  820; //Rinse
				CIPAverageTimes					[j1, j2,  5] = 5749; //Caustic Flush 		- This is the average for every CIP
				CIPAverageTimes					[j1, j2,  6] = 5749; //Caustic Acid Caustic	- This is the average for every CIP
				CIPAverageTimes					[j1, j2,  7] = 6455; //Caustic Acid Sanitization
				CIPAverageTimes					[j1, j2,  8] = 4610; //Caustic Sanitization
				CIPAverageTimes					[j1, j2,  9] = 2890; //Hot Water
				CIPAverageTimes					[j1, j2, 10] = 1266; //Sanitization
				CIPAverageTimes					[j1, j2, 11] = 2372; //Hot Water Rinse
			}
		}
		for (j1 = 33; j1 < 37; j1++) {
				CIPAverageTimes					[j1,  1,  1] = 10643; //CIP
				CIPAverageTimes					[j1,  1,  2] = 3600;  //AIC
		}
		while (queryRunning) {
			yield return new WaitForSeconds (.1f);
		}
		QueryOn ();
		query = "exec dbo.spPullCIPAverageTimes";
		using (SqlConnection dbCon = new SqlConnection(connectionString)) {
			SqlCommand cmd = new SqlCommand(query, dbCon);
			try {
				dbCon.Open();
				reader = cmd.ExecuteReader();
				if (reader.HasRows) {
					while (reader.Read ()) {
						tempSkid = 		(int)reader.GetSqlInt32 (0);
						tempSeconds = (int)reader.GetSqlInt32 (3);
						if (tempSkid <= 32) {
							tempCircuit = 	(int)reader.GetSqlInt32 (1);
							tempDescription = reader.GetSqlString (2).ToString ();
							if 		(tempDescription == "Caustic") 					{CIPAverageTimes [tempSkid, tempCircuit,  1] = tempSeconds;}
							else if (tempDescription == "Caustic Sanitization") 	{CIPAverageTimes [tempSkid, tempCircuit,  2] = tempSeconds;}
							else if (tempDescription == "Caustic Acid") 			{CIPAverageTimes [tempSkid, tempCircuit,  3] = tempSeconds;}
							else if (tempDescription == "Rinse") 					{CIPAverageTimes [tempSkid, tempCircuit,  4] = tempSeconds;}
							else if (tempDescription == "Caustic Flush") 			{CIPAverageTimes [tempSkid, tempCircuit,  5] = tempSeconds;}
							else if (tempDescription == "Caustic Acid Caustic") 	{CIPAverageTimes [tempSkid, tempCircuit,  6] = tempSeconds;}
							else if (tempDescription == "Caustic Acid Sanitization"){CIPAverageTimes [tempSkid, tempCircuit,  7] = tempSeconds;}
							else if (tempDescription == "Caustic Sanitization") 	{CIPAverageTimes [tempSkid, tempCircuit,  8] = tempSeconds;}
							else if (tempDescription == "Hot Water") 				{CIPAverageTimes [tempSkid, tempCircuit,  9] = tempSeconds;}
							else if (tempDescription == "Sanitization") 			{CIPAverageTimes [tempSkid, tempCircuit, 10] = tempSeconds;}
							else if (tempDescription == "Hot Water Rinse") 			{CIPAverageTimes [tempSkid, tempCircuit, 11] = tempSeconds;}
							else 										 			{CIPAverageTimes [tempSkid, tempCircuit,  0] = tempSeconds;}
						} else if (tempSkid <= 36) {
							CIPAverageTimes [tempSkid, 1,  1] = tempSeconds;
						}
					}
				}
				reader.Close ();
			}
			catch (SqlException exception) {
				Debug.LogWarning(exception.ToString());
			}
		}
		QueryOff ();
		refreshing = false;
		yield return null;	
	}
	
	IEnumerator UploadSchedule () {
		while (queryRunning) {
			yield return new WaitForSeconds (.1f);
		}
		QueryOn ();
		query = "insert into [FairlifeDashboard].[dbo].[CIP_Schedule - Rev 02] values ("+scheduledSkid.ToString()+","+scheduledCircuit.ToString()+",0,"+scheduledCipType.ToString()+",'"+scheduledStart.ToString ("yyyy-MM-dd HH:mm")+"','"+scheduledEnd.ToString ("yyyy-MM-dd HH:mm")+"','SCHEDULED','"+AllowableSQLString(scheduledReasonInput.text)+"',current_timestamp,'CIP App')";
		using (SqlConnection dbCon = new SqlConnection(connectionString)) {
			SqlCommand cmd = new SqlCommand(query, dbCon);
			try {
				dbCon.Open();
				reader = cmd.ExecuteReader();
				reader.Close ();
			}
			catch (SqlException exception) {
				Debug.LogWarning(exception.ToString());
			}
		}
		QueryOff ();
		StartCoroutine (Refresh ());
		scheduledReasonInput.text = "THIS SHOULD BE HERE";
		yield return null;	
	}
	
	IEnumerator CancelCIP () {
		if (cipStatusBox.activeSelf) {	cipStatusBox.SetActive (false);	}
		while (queryRunning) {
			yield return new WaitForSeconds (.1f);
		}
		QueryOn ();
		query = "delete top (100) from [FairlifeDashboard].[dbo].[CIP_Schedule - Rev 02] where [Skid] = " + skids [scheduleID].ToString ("00") + " and [Circuit] = " + circuits [scheduleID].ToString () + " and ABS(dateDiff (second, [EntryTime], '" + entryTimes [scheduleID].ToString ("yyyy-MM-dd HH:mm:ss") + "')) < 1";
		using (SqlConnection dbCon = new SqlConnection(connectionString)) {
			SqlCommand cmd = new SqlCommand(query, dbCon);
			try {
				dbCon.Open();
				reader = cmd.ExecuteReader();
				reader.Close ();
			}
			catch (SqlException exception) {
				Debug.LogWarning(exception.ToString());
			}
		}
		QueryOff ();
		StartCoroutine ("Refresh");
		yield return null;	
	}
	
	void QueryOn () {
		queryRunning = true;
		//sqlIcon.SetActive (true);
	}
	
	void QueryOff () {
		queryRunning = false;
		//sqlIcon.SetActive (false);
	}
	
	string AllowableSQLString (string inputString) {
		tempString = inputString.Replace ("'","''");
		return tempString;
	}
	
	string ReadableDateDifference (DateTime start, DateTime end) {
		startFloat = Mathf.Ceil ((float)(end - start).TotalSeconds);
		tempString = "";
		if (startFloat > 3600) {
			tempString = tempString + (Mathf.Floor (startFloat / 3600)).ToString () + "h";
			startFloat = startFloat % 3600;
		}
		if (startFloat >= 60) {
			if (tempString.Length > 1) {
				tempString = tempString + " ";
			}
			tempString = tempString + (Mathf.Ceil (startFloat / 60)).ToString () + "m";
		}
		return tempString;
	}
	
	void AddObject () {
		if (inactiveObjects.Count > 0) {
			tempIm = inactiveObjects [0];
			inactiveObjects.Remove (tempIm);
		} else {
			tempIm = Instantiate (defaultEntry).GetComponent <Image> ();
			tempIm.gameObject.SetActive (true);
		}
		activeObjects.Add (tempIm);
		tempIm.transform.SetParent (activeContainer);
	}
	
	void RemoveObject (Image im) {
		im.gameObject.SetActive (false);
		activeObjects.Remove (im);
		inactiveObjects.Add (im);
		im.transform.SetParent (inactiveContainer);
		im.transform.GetChild (2).gameObject.SetActive (false);
	}
	
	string GetCIPTypeText (int skid, int type) {
		if (skid <= 32) {
			if 			(type <=  0) {
				tempString = "? ";
			} else if 	(type <=  1) {
				tempString = "C ";
			} else if 	(type <=  2) {
				tempString = "A \nS ";
			} else if 	(type <=  3) {
				tempString = "C \nA ";
			} else if 	(type <=  4) {
				tempString = "R ";
			} else if 	(type <=  5) {
				tempString = "C \nF ";
			} else if 	(type <=  6) {
				tempString = "C \nA \nC ";
			} else if 	(type <=  7) {
				tempString = "C \nA \nS ";
			} else if 	(type <=  8) {
				tempString = "C \nS ";
			} else if 	(type <=  9) {
				tempString = "H \nW ";
			} else if 	(type <= 10) {
				tempString = "S ";
			} else if 	(type <= 11) {
				tempString = "H \nW \nR ";
			}
		} else {
			if 			(type <=  1) {
				tempString = "C \nI  \nP ";
			} else if 	(type <=  2) {
				tempString = "A \nI  \nC ";
			}
		}
		return tempString;
	}
}
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	/*
	float refreshTime = 600;
	float refreshCounter = 598;
	float averageTimeCounter = 0;
	bool refreshing = false;
	public Camera mainCamera;
	public GameObject defaultCube;
	DateTime currentDateTime;
	public List <float> skidYPos;
	public Material completeMaterial, runningMaterial, abortedMaterial, scheduledMaterial, currentTimeMaterial, horizontalStripes, invisibleMaterial, conflictMaterial, conflictEvenMaterial, conflictOddMaterial, abortedEvenMaterial, abortedOddMaterial, dueMaterial;
	bool visualUpdateNeeded = true;
	string username = "cipApp";
	int privilages = 10; //0 = View only, 1 = Can Schedule CIPs, 2 = Can Cancel and Reschedule CIPs.  10 is the highest level of privilages
	
	//Graphics
	public GameObject stats, schedulingCIP, menuGO;
	public Text circuitText, IDText, descriptionText, CIPTypeText, statusText, dateText, schedulerText, scheduledDateText, reasonText;
	public List <Text> verticalLables, horizontalLabels;
	List <GameObject> horizontalMarkers = new List <GameObject> ();
	List <string> CIPNames = new List <string> {"Unkown", "Caustic", "Acid Sanitization", "Caustic Acid", "Rinse", "Caustic Flush", "Caustic Acid Caustic", "Caustic Acid Sanitize", "Caustic Sanitize", "Hot Water", "Sanitization", "How Water Rinse"};
	public List <Material> CipIconMaterials;
	public List <GameObject> touchDebugObjects;
	public Text currentTimeText;
	float displayTimer = 0;
	public List <GameObject> 	menuEntries;
	public List <Vector3>		menuEnriesPos;
	GameObject sqlIcon;
	GameObject [] CIPDueMarkers = new GameObject [37];
	Transform dueOverlay;
	
	//Skid Information
	//List <int> activeCircuit; //Will be -1 if the skid does not have an active CIP
	//                                            0  1  2  3  4  5  6  7  8  9 10 11 12 13 14 15 16 17 18 19 20 21 22 23 24 25 26 27 28 29 30 31 32 33 34 35 36
	List <int> skidCircuitCount = new List <int> {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,14,17,15,15,20,19,16, 8, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 8, 8, 1, 1, 1, 1};
	
	//Query Information
	SqlDataReader reader;
	string query;
	private string connectionString = @"Data Source = 172.16.5.217; user id = cipapp;password = bestinclass&1;Connection Timeout=10;";
	bool queryRunning = false;
	List <int> skids = new List <int> ();
	List <int> circuits = new List <int> ();
	List <int> IDs = new List <int> ();
	List <int> CIPTypes = new List <int> ();
	List <DateTime> startTimes = new List <DateTime> ();
	List <DateTime> endTimes = new List <DateTime> ();
	List <string> status = new List <string> ();
	List <string> reasons = new List <string> ();
	List <DateTime> entryTimes = new List <DateTime> ();
	List <string> schedulers = new List <string> ();
	int [,,] CIPAverageTimes	= new int [37,21,12];
	int []	CIPDueTimes			= new int [37];
	string [,] circuitDescriptions			= new string [37,21];
	
	//Pooled objects
	List <GameObject> activeObjects = new List <GameObject> ();
	List <GameObject> inactiveObjects = new List <GameObject> ();
	
	//Temp Variables
	DateTime tempStart, tempEnd;
	int i1, i2, j1, j2, tempSkid, tempCircuit, tempID, tempSeconds;
	float startFloat, endFloat, f1;
	GameObject tempObject;
	string tempString, tempDescription;
	float camX, camY, camZ;
	 
	//Input
	List <Vector2>	touchPos 	= new List <Vector2> 	{new Vector2 (0,0),new Vector2 (0,0),new Vector2 (0,0),new Vector2 (0,0),new Vector2 (0,0),new Vector2 (0,0),new Vector2 (0,0),new Vector2 (0,0),new Vector2 (0,0),new Vector2 (0,0)};
	List <float>	touchTime	= new List <float> 		{0,0,0,0,0,0,0,0,0,0,0};
	int touchCount = 0, lastTouchCount = 0;
	Vector3 cameraStartingPosition;
	Vector2 startingTouchPosition;
	float zoom = 1;
	float cameraStartingZoom = -10f;
	float cameraMaxZoom = 25000;
	RaycastHit hit;
	Ray ray;
	bool grabbingScheduledCIP = false;
	float pinchDistance = 0;
	float startingZoom = 1;
	
	
	//Shedule variables
	DateTime scheduledStart, scheduledEnd;
	float scheduleX, scheduleStartingX;
	int scheduleCIPType, scheduleID, scheduleSkid, scheduleCircuit;
	public Text schedulingSkidText, schedulingCircuitText, schedulingCircuitNameText, schedulingCIPTypeText, schedulingStartDateText, schedulingStartTimeText, schedulingEndTimeText;
	public InputField scheduleReasonInput;
	GameObject scheduleBlock;
	public GameObject cancelCIP;
	
	//Need to be able to check the user name for each scheduled CIP.  If a user has the same username as the user that scheduled the CIP, they should be able to cancel / reschedule it without admin privilages
	//Need to have 'ASAP' button, or 'Start When last CIP finishes' button, or something like that.  It would also be nice to have a 'Snap' feature to lock to last CIP end time
	//Move menu button to the left side, move the time to the upper right, swap the scheduled stats and stats
	//add bullet numbers to screen instructions
	//add to screen instructions to minimize side bar of power bi
	//Concider adding VTISs and Membranes to screen
	
	void Awake () {
		refreshCounter = refreshTime;
		tempObject = Instantiate (defaultCube);
		tempObject.transform.position = new Vector3 (0, 250, 50000);
		tempObject.transform.localScale = new Vector3 (250, 55500, 150);
		tempObject.GetComponent <MeshRenderer>().material = currentTimeMaterial;
		tempObject.name = "Current Time";
		tempObject.transform.GetChild (0).gameObject.SetActive (false);
		//tempObject.transform.GetChild (1).gameObject.SetActive (false);
		tempObject.SetActive (true);
		
		for (i1 = 0; i1 < skidYPos.Count; i1++) {
			if (skidYPos [i1] == 0) { //This means there is no skid under this number
				continue;
			}
			tempObject = Instantiate (defaultCube);
			tempObject.transform.position = new Vector3 (150000, skidYPos [i1], 50000);
			tempObject.transform.localScale = new Vector3 (1000000, 2500, 10);
			if (i1 %2 == 0) {
				tempObject.GetComponent <MeshRenderer>().material = invisibleMaterial;
			} else {
				tempObject.GetComponent <MeshRenderer>().material = horizontalStripes;
			}
			tempObject.name = "Skid" + i1.ToString ("00");
			tempObject.transform.GetChild (0).gameObject.SetActive (false);
			//tempObject.transform.GetChild (1).gameObject.SetActive (false);
			tempObject.SetActive (true);
		}
		tempObject = Instantiate (defaultCube);
		scheduleBlock = tempObject;
		scheduleBlock.GetComponent <MeshRenderer>().material = currentTimeMaterial;
		scheduleBlock.name = "scheduleBlock";
		
		sqlIcon			= transform.Find ("Canvas/SQL Icon").gameObject;
		dueOverlay		= transform.parent.Find ("Due Overlay");
		refreshing = true;
		StartCoroutine ("RefreshAverageTimes");
		StartCoroutine ("FetchCircuitDescriptions");
		StartCoroutine (PullOverdueStatus ());
	}*/
	
	/*void Update () {
		currentDateTime = DateTime.Now;
		currentDateTime.Add (new TimeSpan (0,50,0,0));
		displayTimer -= Time.deltaTime;
		if (displayTimer <= 0) {
			currentTimeText.text = currentDateTime.ToString ("MM-dd HH:mm:ss"); //Displays the current time in the uppper left hand corner
			displayTimer += .5f;
		}
		refreshCounter += Time.deltaTime;
		averageTimeCounter += Time.deltaTime;
		
		if (menuGO.activeSelf) {
			for (i1 = 0; i1 < menuEntries.Count; i1++) {
				//Only for debugging
				//menuEnriesPos [i1] = menuEntries [i1].transform.localPosition;
				menuEntries [i1].transform.localPosition = Vector3.Lerp (menuEntries [i1].transform.localPosition, menuEnriesPos [i1], .5f);
			}
		}
		
		touchCount = Input.touchCount;
		if (touchCount == 0) { //If there is no touch input, it looks for mouse input
			if (Input.GetMouseButton (0)) {
				touchCount = 1;
				touchPos [0] = mainCamera.ScreenToViewportPoint(Input.mousePosition);
			}
			if (Input.mouseScrollDelta.y != 0) {
				zoom += Input.mouseScrollDelta.y * .1f;
				visualUpdateNeeded = true;
			}
		} else {
			for (i1 = 0; i1 < touchCount; i1++) {
				touchPos [i1] = mainCamera.ScreenToViewportPoint(Input.GetTouch (i1).position);
			}
		}
		
		if (touchCount >= 1 && touchTime [0] > .2f) {
			visualUpdateNeeded = true;
			if (privilages >= 2 && grabbingScheduledCIP) {
				scheduleX = scheduleStartingX - ((startingTouchPosition.x - touchPos [0].x) * (100000 / (zoom)));
				if (schedulingCIP.activeSelf) {
					scheduleBlock.transform.position = new Vector3 (scheduleX, scheduleBlock.transform.position.y, scheduleBlock.transform.position.z);
					scheduledStart = currentDateTime + new TimeSpan (0, 0, 0, Mathf.CeilToInt (scheduleX - (scheduleBlock.transform.localScale.x / 2)));
					if (scheduledStart < (currentDateTime + new TimeSpan (0,0,0,1))) {scheduledStart = currentDateTime + new TimeSpan (0,0,0,1);}
					scheduledEnd = scheduledStart + new TimeSpan (0, 0, 0, Mathf.CeilToInt (scheduleBlock.transform.localScale.x));
					UpdateSchedulingCIP (); //Should check for overlap as well
				} else {
					activeObjects [scheduleID].transform.position = new Vector3 (scheduleX, activeObjects [scheduleID].transform.position.y, activeObjects [scheduleID].transform.position.z);
					startTimes [scheduleID] = currentDateTime + new TimeSpan (0, 0, 0, Mathf.CeilToInt (scheduleX - (activeObjects [scheduleID].transform.localScale.x / 2)));
					if (startTimes [scheduleID] < (currentDateTime + new TimeSpan (0,0,0,1))) {startTimes [scheduleID] = currentDateTime + new TimeSpan (0,0,0,1);}
					endTimes [scheduleID] = startTimes [scheduleID] + new TimeSpan (0, 0, 0, Mathf.CeilToInt (activeObjects [scheduleID].transform.localScale.x));
					dateText.text = startTimes [scheduleID].ToString ("MMM dd HH:mm") + " - " + endTimes [scheduleID].ToString ("MMM dd HH:mm");
					if (Overlap (scheduleID)) {
						activeObjects [scheduleID].GetComponent <MeshRenderer>().material = conflictMaterial;
					} else {
						activeObjects [scheduleID].GetComponent <MeshRenderer>().material = scheduledMaterial;
					}
				}
			} else {
				camX = cameraStartingPosition.x + ((startingTouchPosition.x - touchPos [0].x) * (100000 / (zoom)));
				camY = cameraStartingPosition.y + ((startingTouchPosition.y - touchPos [0].y) * (57000  / (zoom)));
				if (stats.activeSelf) {
					stats.SetActive (false);
				}
			}
		} else {
			camX = mainCamera.transform.position.x;
			camY = mainCamera.transform.position.y;
		}
		
		if (touchCount == 2) {
			if (pinchDistance == 0) {
				pinchDistance = Vector2.Distance (touchPos [0], touchPos [1]);
				startingZoom = zoom;
			} else {
				zoom = startingZoom / (pinchDistance / Vector2.Distance (touchPos [0], touchPos [1]));
			}
			visualUpdateNeeded = true;
		} else {
			pinchDistance = 0;
		}
		
		
		if (zoom > 2) {
			zoom = 2;
		} else if (zoom < 1) {
			zoom = 1;
		}
		camZ = Mathf.Lerp (cameraStartingZoom, cameraMaxZoom, zoom - 1);
		mainCamera.transform.position = new Vector3 (camX, camY, camZ);

		if (refreshCounter >= refreshTime && !refreshing) {
			refreshing = true;
			refreshCounter -= refreshTime;
			StartCoroutine ("Refresh");
		}
		if (!refreshing) {
			for (i1 = 0; i1 < activeObjects.Count; i1++) {
				if (status [i1] == "RUNNING" && endTimes [i1] < currentDateTime) {
					endTimes [i1] = currentDateTime + new TimeSpan (0,0,1,0); //Sets the start time to one minute in the future, so we only update this once a minute
					startFloat = (float)(startTimes [i1] - currentDateTime).TotalSeconds;
					endFloat = (float)(endTimes [i1] - currentDateTime).TotalSeconds;
					activeObjects [i1].transform.localScale = new Vector3 ((endFloat - startFloat), 2000, 101);
				} else if (status [i1] == "SCHEDULED" && startTimes [i1] < currentDateTime) {
					startTimes [i1] = currentDateTime + new TimeSpan (0,0,1,0);
					endTimes [i1] = startTimes [i1] + new TimeSpan (0, 0, 0, CIPAverageTimes [tempSkid, tempCircuit, CIPTypes [i1]]);
					startFloat = (float)(startTimes [i1] - currentDateTime).TotalSeconds;
					endFloat = (float)(endTimes [i1] - currentDateTime).TotalSeconds;
					activeObjects [i1].transform.localScale = new Vector3 ((endFloat - startFloat), 2000, 100);
					StartCoroutine (UpdateScheduledTime (i1));
				}
				activeObjects [i1].transform.localPosition = new Vector3 (((float)(startTimes [i1] - currentDateTime).TotalSeconds + (float)(endTimes [i1] - currentDateTime).TotalSeconds) / 2, skidYPos [skids [i1]], 50000);
			}
			for (i1 = 0; i1 < horizontalMarkers.Count; i1++) {
				horizontalMarkers [i1].transform.localPosition = new Vector3 (horizontalMarkers [i1].transform.localPosition.x - Time.deltaTime, 250, 50000);
			}
		}
		if (touchCount == 0) {
			if (lastTouchCount > 0) {
				TouchRelease ();
			}
		} else {
			if (lastTouchCount == 0) {
				TouchContact ();
			}
		}
		lastTouchCount = touchCount;
		
		for (i1 = 0; i1 < touchCount; i1++) {
			touchTime [i1] += Time.deltaTime;
			//Debug Code
			//touchDebugObjects [i1].transform.position = mainCamera.ScreenToWorldPoint (new Vector3(mainCamera.ViewportToScreenPoint (touchPos [i1]).x, mainCamera.ViewportToScreenPoint (touchPos [i1]).y, 40000));
			//touchDebugObjects [i1].SetActive (true);
		}
		for (i2 = i1; i2 <= 4; i2++) {
			touchTime [i2] = 0;
			touchDebugObjects [i2].SetActive (false);
		}
	}*/
	
	/*void OnGUI () {
		if (visualUpdateNeeded) {
			visualUpdateNeeded = false;
			for (i1 = 0; i1 <= 36; i1++) {
				if (skidYPos [i1] != 0) {
					verticalLables [i1].transform.position = new Vector3 (verticalLables [i1].transform.position.x, mainCamera.WorldToScreenPoint (new Vector3 (mainCamera.transform.position.x, skidYPos [i1], 50000)).y, verticalLables [i1].transform.position.z);
				}
			}
			for (i1 = 0; i1 < horizontalMarkers.Count; i1++) {
				if (i1 < 0 || i1 >= horizontalLabels.Count) {
					Debug.Log (i1);
				} else {
					horizontalLabels [i1].transform.position = new Vector3 (mainCamera.WorldToScreenPoint (new Vector3 (horizontalMarkers [i1].transform.position.x, 0, 50000)).x, horizontalLabels [i1].transform.position.y, horizontalLabels [i1].transform.position.z);
				}
			}
		}
	}*/
	
	/*IEnumerator Refresh () {
		skids.Clear ();
		circuits.Clear ();
		IDs.Clear ();
		startTimes.Clear ();
		endTimes.Clear ();
		status.Clear ();
		reasons.Clear ();
		entryTimes.Clear ();
		schedulers.Clear ();
		while (queryRunning) {
			yield return new WaitForSeconds (.1f);
		}
		QueryOn ();
		j2 = 0;
		query = "exec dbo.spPullCIPSchedule";
		using (SqlConnection dbCon = new SqlConnection(connectionString)) {
			SqlCommand cmd = new SqlCommand(query, dbCon);
			try {
				dbCon.Open();
				reader = cmd.ExecuteReader();
				if (reader.HasRows) {
					while (reader.Read ()) {
						skids.Add		((int)reader.GetSqlInt32 (0));
						circuits.Add	((int)reader.GetSqlInt32 (1));
						IDs.Add 		((int)reader.GetSqlInt32 (2));
						CIPTypes.Add	((int)reader.GetSqlInt32 (3));
						startTimes.Add	((DateTime)reader.GetSqlDateTime (4));
						endTimes.Add	((DateTime)reader.GetSqlDateTime (5));
						status.Add		(reader.GetSqlString (6).ToString ());
						reasons.Add		(reader.GetSqlString (7).ToString ());
						entryTimes.Add	((DateTime)reader.GetSqlDateTime (8));
						schedulers.Add	(reader.GetSqlString (9).ToString ());
						j2++;
					}
				}
				reader.Close ();
			}
			catch (SqlException exception) {
				Debug.LogWarning(exception.ToString());
			}
		}
		QueryOff ();
		
		for (j1 = 0; j1 < 9999; j1++) {
			if (activeObjects.Count > 0) {RemoveObject (activeObjects [0]);
			} else { j1 = 9999; }
		}
		for (j1 = 0; j1 < startTimes.Count; j1++) {
			AddObject ();
			startFloat = (float)(startTimes [j1] - currentDateTime).TotalSeconds;
			endFloat = (float)(endTimes [j1] - currentDateTime).TotalSeconds;
			tempObject.transform.position = new Vector3 ((startFloat + endFloat) / 2, skidYPos [skids [j1]], 50000);
			if 			(status [j1] == "COMPLETE") {								tempObject.GetComponent <MeshRenderer>().material = completeMaterial;	tempObject.transform.localScale = new Vector3 ((endFloat - startFloat), 2000, 125);
			} else if 	(status [j1] == "ABORTED") {								tempObject.GetComponent <MeshRenderer>().material = abortedMaterial;	tempObject.transform.localScale = new Vector3 ((endFloat - startFloat), 2000, 100);
			} else if 	(status [j1] == "RUNNING") {								tempObject.GetComponent <MeshRenderer>().material = runningMaterial; 	tempObject.transform.localScale = new Vector3 ((endFloat - startFloat), 2000, 125);	if (endTimes [j1] < currentDateTime) { endTimes [j1] = currentDateTime;}
			} else if 	(status [j1] == "SCHEDULED" || status [j1] == "UNKNOWN") {	tempObject.GetComponent <MeshRenderer>().material = scheduledMaterial;	tempObject.transform.localScale = new Vector3 ((endFloat - startFloat), 2000, 150);	Debug.Log ("B: " + startTimes [j1].ToString ("yyyy-MM-dd HH:mm:ss"));
			}

			tempObject.name = j1.ToString ();
			tempObject.GetComponent <BoxCollider>().enabled = true;
			if (tempObject.transform.localScale.x < 1) { //This is nessisary for CIPs that have the same start and end time
				tempObject.transform.localScale += new Vector3 (1,0,0);
			}
			if (skids [j1] <= 32) {
				if (circuits [j1] < 10) {
					tempString = circuits [j1].ToString ();
				} else {
					tempString = circuits [j1].ToString().Substring (0,1) + "\n" + circuits [j1].ToString().Substring (1,1);
				}
				tempObject.transform.GetChild (0).GetComponent <TextMesh> ().text = tempString;

				tempObject.transform.GetChild (0).gameObject.SetActive (true);
			} else {
				tempObject.transform.GetChild (0).gameObject.SetActive (false);
			}
			if (tempObject.transform.localScale.x < 1000) {
				tempObject.transform.GetChild (0).localPosition = new Vector3 (0,.5f,0);
			} else {
				tempObject.transform.GetChild (0).localPosition = new Vector3 (0.5f-((tempObject.transform.localScale.x-500)/tempObject.transform.localScale.x),.5f,0);
			}
			tempObject.transform.GetChild (0).localScale = new Vector3 (4 / tempObject.transform.localScale.x, 4 / tempObject.transform.localScale.y, 1);
			//tempObject.transform.GetChild (1).localScale = tempObject.transform.GetChild (0).localScale * 200;
			//tempObject.transform.GetChild (1).localPosition = new Vector3 (-tempObject.transform.GetChild (0).localPosition.x, -0.3f, -1);
			//tempObject.transform.GetChild (1).gameObject.GetComponent <MeshRenderer>().material = CipIconMaterials [CIPTypes [j1]];
			//tempObject.transform.GetChild (1).gameObject.SetActive (true);
			tempObject.SetActive (true);
		}
		for (j1 = 0; j1 < horizontalMarkers.Count; j1++) {
			Destroy (horizontalMarkers [j1]);
		}
		horizontalMarkers.Clear ();
		DateTime runningDate = DateTime.Now.Date;
		runningDate = runningDate + new TimeSpan (-4,0,0,1);
		for (j1 = 0; j1 < horizontalLabels.Count; j1++) {
			runningDate = runningDate + new TimeSpan (1,0,0,0);
			horizontalLabels [j1].text = runningDate.ToString ("MMM-dd");
			tempObject = Instantiate (defaultCube);
			tempObject.transform.position = new Vector3 ((float)(runningDate - currentDateTime).TotalSeconds, 250, 50000);

			tempObject.transform.localScale = new Vector3 (250, 55500, 150);
			tempObject.GetComponent <MeshRenderer>().material = scheduledMaterial;
			tempObject.GetComponent <BoxCollider>().enabled = false;
			tempObject.transform.GetChild (0).gameObject.SetActive (false);
			//tempObject.transform.GetChild (1).gameObject.SetActive (false);
			tempObject.SetActive (true);
			
			tempObject.name = runningDate.ToString ("MMM-dd");
			horizontalMarkers.Add (tempObject);
		}
		
		for (j1 = 0; j1 < startTimes.Count; j1++) {
			if (status [j1] == "SCHEDULED" && Overlap (j1)) {
				activeObjects [j1].GetComponent <MeshRenderer>().material = conflictMaterial;
			}
		}
		refreshing = false;
		refreshCounter = 0;
		StartCoroutine (PullOverdueStatus ());
		yield return null;
	}
	
	IEnumerator RefreshAverageTimes () {
		for (j1 = 0; j1 < 33; j1++) { //This populates the table with default values in case we haven't run a given combination before
			for (j2 = 0; j2 < 21; j2++) {
				CIPAverageTimes					[j1, j2,  0] = 5749; //Unknown 				- This is the average for every CIP
				CIPAverageTimes					[j1, j2,  1] = 5151; //Caustic
				CIPAverageTimes					[j1, j2,  2] = 3770; //Acid Sanitization
				CIPAverageTimes					[j1, j2,  3] = 8127; //Caustic Acid
				CIPAverageTimes					[j1, j2,  4] =  820; //Rinse
				CIPAverageTimes					[j1, j2,  5] = 5749; //Caustic Flush 		- This is the average for every CIP
				CIPAverageTimes					[j1, j2,  6] = 5749; //Caustic Acid Caustic	- This is the average for every CIP
				CIPAverageTimes					[j1, j2,  7] = 6455; //Caustic Acid Sanitization
				CIPAverageTimes					[j1, j2,  8] = 4610; //Caustic Sanitization
				CIPAverageTimes					[j1, j2,  9] = 2890; //Hot Water
				CIPAverageTimes					[j1, j2, 10] = 1266; //Sanitization
				CIPAverageTimes					[j1, j2, 11] = 2372; //Hot Water Rinse
			}
		}
		for (j1 = 33; j1 < 37; j1++) {
				CIPAverageTimes					[j1,  1,  1] = 5000;
				CIPAverageTimes					[j1,  1,  2] = 2000;
		}
		while (queryRunning) {
			yield return new WaitForSeconds (.1f);
		}
		QueryOn ();
		query = "exec dbo.spPullCIPAverageTimes";
		using (SqlConnection dbCon = new SqlConnection(connectionString)) {
			SqlCommand cmd = new SqlCommand(query, dbCon);
			try {
				dbCon.Open();
				reader = cmd.ExecuteReader();
				j2 = 0;
				if (reader.HasRows) {
					while (reader.Read ()) {
						tempSkid = 		(int)reader.GetSqlInt32 (0);
						tempCircuit = 	(int)reader.GetSqlInt32 (1);
						tempDescription = reader.GetSqlString (2).ToString ();
						tempSeconds = (int)reader.GetSqlInt32 (3);
						
						if 		(tempDescription == "Caustic") 					{CIPAverageTimes [tempSkid, tempCircuit,  1] = tempSeconds;}
						else if (tempDescription == "Caustic Sanitization") 	{CIPAverageTimes [tempSkid, tempCircuit,  2] = tempSeconds;}
						else if (tempDescription == "Caustic Acid") 			{CIPAverageTimes [tempSkid, tempCircuit,  3] = tempSeconds;}
						else if (tempDescription == "Rinse") 					{CIPAverageTimes [tempSkid, tempCircuit,  4] = tempSeconds;}
						else if (tempDescription == "Caustic Flush") 			{CIPAverageTimes [tempSkid, tempCircuit,  5] = tempSeconds;}
						else if (tempDescription == "Caustic Acid Caustic") 	{CIPAverageTimes [tempSkid, tempCircuit,  6] = tempSeconds;}
						else if (tempDescription == "Caustic Acid Sanitization"){CIPAverageTimes [tempSkid, tempCircuit,  7] = tempSeconds;}
						else if (tempDescription == "Caustic Sanitization") 	{CIPAverageTimes [tempSkid, tempCircuit,  8] = tempSeconds;}
						else if (tempDescription == "Hot Water") 				{CIPAverageTimes [tempSkid, tempCircuit,  9] = tempSeconds;}
						else if (tempDescription == "Sanitization") 			{CIPAverageTimes [tempSkid, tempCircuit, 10] = tempSeconds;}
						else if (tempDescription == "Hot Water Rinse") 			{CIPAverageTimes [tempSkid, tempCircuit, 11] = tempSeconds;}
						else 										 			{CIPAverageTimes [tempSkid, tempCircuit,  0] = tempSeconds;}
						j2++;
					}
				}
				reader.Close ();
			}
			catch (SqlException exception) {
				Debug.LogWarning(exception.ToString());
			}
		}
		QueryOff ();
		refreshing = false;
		yield return null;	
	}
	
	IEnumerator FetchCircuitDescriptions () {
		yield return new WaitForSeconds (1);
		while (queryRunning) {
			yield return new WaitForSeconds (.1f);
		}
		QueryOn ();
		query = "exec dbo.spPullCIPCircuitDescriptions";
		using (SqlConnection dbCon = new SqlConnection(connectionString)) {
			SqlCommand cmd = new SqlCommand(query, dbCon);
			try {
				dbCon.Open();
				reader = cmd.ExecuteReader();
				j2 = 0;
				if (reader.HasRows) {
					while (reader.Read ()) {
						circuitDescriptions [(int)reader.GetSqlInt32 (0),(int)reader.GetSqlInt32 (1)] = reader.GetSqlString (2).ToString ();
						j2++;
					}
				}
				reader.Close ();
			}
			catch (SqlException exception) {
				Debug.LogWarning(exception.ToString());
			}
		}
		QueryOff ();
		yield return null;	
	}

	void TouchContact () {
		startingTouchPosition = touchPos [0];//mainCamera.ViewportToScreenPoint (touchPos [0]);//mainCamera.ScreenToViewportPoint(Input.mousePosition);
		cameraStartingPosition = mainCamera.transform.position;
		if (privilages >= 2 && cancelCIP.activeSelf) {
			ray = mainCamera.ScreenPointToRay (mainCamera.ViewportToScreenPoint (touchPos [0]));
			if (Physics.Raycast (ray, out hit, 1000000)) {				
				if (int.TryParse (hit.transform.name, out scheduleID)) {
					if (status [scheduleID] == "SCHEDULED") {
						grabbingScheduledCIP = true;
						cameraStartingPosition = activeObjects [scheduleID].transform.position;
						scheduleStartingX = activeObjects [scheduleID].transform.position.x;
						UpdateStats (scheduleID);
						schedulingCIP.SetActive (false);
						scheduleBlock.SetActive (false);
						stats.SetActive (true);
					}
				}
			}
		} else if (privilages >= 1 && schedulingCIP.activeSelf) {
			ray = mainCamera.ScreenPointToRay (mainCamera.ViewportToScreenPoint (touchPos [0]));
			if (Physics.Raycast (ray, out hit, 1000000)) {
				if (hit.transform.name == "scheduleBlock") {
					grabbingScheduledCIP = true;
					cameraStartingPosition = scheduleBlock.transform.position;
					scheduleStartingX = scheduleBlock.transform.position.x;
					UpdateSchedulingCIP ();
				}
			}
		}
	}
	
	void TouchRelease () {
		if (grabbingScheduledCIP) {
			grabbingScheduledCIP = false;
			StartCoroutine (UpdateScheduledTime (scheduleID));
		} else if (touchTime [0] <= .2f) {
			Click (mainCamera.ViewportToScreenPoint (touchPos [0]));
		}
	}
	
	void Click (Vector3 pos) {
		//Debug.Log (touchPos [0].x + ", " + touchPos [0].y);
		if (queryRunning) {
			return;
		}
		if (menuGO.activeSelf) {
			if 			(touchPos [0].y < .75f || touchPos [0].y > .95f || touchPos [0].x >.1f) {
				//This is none of the buttons, so we'll close the menu at the end of the function
			} else if 	(touchPos [0].y < .79f) { 	//Exit
				Application.Quit ();
			} else if 	(touchPos [0].y < .827f) { 	//Back
			
			} else if 	(touchPos [0].y < .9f) { 	//Refresh
				StartCoroutine (Refresh ());
			} else { 								//Overlay
				dueOverlay.gameObject.SetActive (!dueOverlay.gameObject.activeSelf);
			}
			menuGO.SetActive (false);
			return;
		}
		
		if (touchPos [0].x < .082f && touchPos [0].y > .95f) { //Menu Open
			menuGO.SetActive (true);
			if (stats.activeSelf) { 		stats.SetActive (false);			}
			if (scheduleBlock.activeSelf) {	scheduleBlock.SetActive (false);	}
			if (schedulingCIP.activeSelf) {	schedulingCIP.SetActive (false);	}
			for (j1 = 0; j1 < menuEntries.Count; j1++) {
				menuEntries [j1].transform.localPosition = new Vector3 (-850, 500, 0);
			}
			return;
		} else if (privilages >= 1 && schedulingCIP.activeSelf) {
			if (stats.activeSelf) { 	stats.SetActive (false);	}
			if (touchPos [0].x > .35f || touchPos [0].y > .947f || touchPos [0].y < .578f) {
				//This means our click was out of bounds, so we'll close schedulingCIP
				schedulingCIP.SetActive (false);
				scheduleBlock.SetActive (false);
			} else {
				if (touchPos [0].y > .865f) { //Circuit Number
					if (touchPos [0].x > .15f || scheduleSkid > 32) { //do nothing
					} else if (touchPos [0].y > .9f && touchPos [0].x > .074f && touchPos [0].x < .098f) {//Descrease Circuit Number
						tempCircuit--;
						if (tempCircuit < 1) 							{ tempCircuit = skidCircuitCount [scheduleSkid]; }
					} else { //Increase Circuit Number
						tempCircuit++;
						if (tempCircuit > skidCircuitCount [scheduleSkid]) 	{ tempCircuit = 1; }
					}
					UpdateSchedulingCIP ();
				} else if (touchPos [0].y > .826f) { //CIP Type
					if (touchPos [0].x < .036f) { //Decrease CIP Type
						if 			(scheduleSkid <= 32) {
							if (scheduleCIPType <= 1) { scheduleCIPType = 12;}
						} else if 	(scheduleSkid <= 36) {
							if (scheduleCIPType <= 1) { scheduleCIPType = 3;}
						}
						scheduleCIPType--;
					} else {
						if 			(scheduleSkid <= 32) {
							if (scheduleCIPType >= 11){	scheduleCIPType = 0;}
						} else if 	(scheduleSkid <= 36) {
							if (scheduleCIPType >=  2){	scheduleCIPType = 0;}
						}
						scheduleCIPType++;
					}
					Debug.Log (scheduleCIPType);
					UpdateSchedulingCIP ();
				} else if (touchPos [0].y > .789f) { //CIP Start Time
					if (touchPos [0].x < .118f || touchPos [0].x > .293f) { //Out of bounds
					} else if (touchPos [0].x < .137f) { //Subtract Day
						scheduledStart = scheduledStart - new TimeSpan (1,0,0,0);
					} else if (touchPos [0].x < .18f)  { //Add Day
						scheduledStart = scheduledStart + new TimeSpan (1,0,0,0);
					} else if (touchPos [0].x < .203f) { //Subtract Hour
						scheduledStart = scheduledStart - new TimeSpan (0,1,0,0);
					} else if (touchPos [0].x < .24f)  { //Add Hour
						scheduledStart = scheduledStart + new TimeSpan (0,1,0,0);
					} else if (touchPos [0].x < .256f) { //Subtract Minute
						scheduledStart = scheduledStart - new TimeSpan (0,0,1,0);
					} else if (touchPos [0].x < .293f) { //Add Minute
						scheduledStart = scheduledStart + new TimeSpan (0,0,1,0);
					}
					if (scheduledStart < currentDateTime) { scheduledStart = currentDateTime;}
					UpdateSchedulingCIP ();
				} else if (touchPos [0].y < .617f) { //Confirm or Cancel
					if (touchPos [0].x > .26f) { //Confirm
						StartCoroutine ("UploadSchedule");
					}
					schedulingCIP.SetActive (false);
					scheduleBlock.SetActive (false);
				}
			}
			return;
		} else if (cancelCIP.activeSelf) {
			if (privilages < 2) { //This will eventually need to check if the user is the one who scheduled the CIP
				cancelCIP.SetActive (false);
				return;
			}
			if (status [scheduleID] != "SCHEDULED") {
				cancelCIP.SetActive (false);
				return;
			}
			if (touchPos [0].x > .635f && touchPos [0].x <.712f && touchPos [0].y > .867f && touchPos [0].y < .95f) {
				StartCoroutine ("CancelCIP");
				return;
			}
		}
		ray = mainCamera.ScreenPointToRay (pos);
		if (Physics.Raycast (ray, out hit, 10000000)) {
			if (int.TryParse (hit.transform.name, out j1)) {
				UpdateStats (j1);
				
				schedulingCIP.SetActive (false);
				scheduleBlock.SetActive (false);
				stats.SetActive (true);
				if (privilages >= 2 && status [j1] == "SCHEDULED") {
					cancelCIP.SetActive (true);
				}
			} else if (privilages >= 1 && hit.point.x > 0) {
				if (stats.activeSelf) {
					stats.SetActive (false);
				}
				//This will open the schedule dialog
				scheduledStart = currentDateTime + new TimeSpan (0,0,0,Mathf.RoundToInt(hit.point.x));
				 
				scheduleCIPType = 1;
				tempCircuit = 1;
				tempString = hit.transform.name.Substring (4, hit.transform.name.Length - 4);
				if (!int.TryParse (tempString, out scheduleSkid)) {
					scheduleSkid = 11;
				}

				//There should be some logic in place that checks if this is not a skid
				if 			(scheduleSkid <= 32) { //Skids
					schedulingSkidText.text 	= "CC" + (scheduleSkid).ToString ("00") + "  <- ";
				} else if 	(scheduleSkid <= 36) { //VTISs
					if 		(scheduleSkid == 33) { schedulingSkidText.text = "VTIS1";}
					else if (scheduleSkid == 34) { schedulingSkidText.text = "VTIS2";}
					else if (scheduleSkid == 35) { schedulingSkidText.text = "VTIS4";}
					else if (scheduleSkid == 36) { schedulingSkidText.text = "VTIS6";}
				}
				UpdateSchedulingCIP ();
				schedulingCIP.SetActive (true);
				scheduleBlock.SetActive (true);
			}
		} else {
			stats.SetActive (false);
			schedulingCIP.SetActive  (false);
			scheduleBlock.SetActive  (false);
		}
	}
	
	void UpdateStats (int ID) {
		if 			(skids [ID] <= 32) { //Skids
			circuitText.text 		= "CC" + skids [ID].ToString ("00") + "_" + circuits [ID].ToString ("00");
			descriptionText.text	= circuitDescriptions [skids [ID],circuits [ID]];
			if (CIPTypes [ID] >= CIPNames.Count) {
				Debug.Log (CIPTypes [ID] + " / " + CIPNames.Count);
			} else {
				CIPTypeText.text 		= CIPNames [CIPTypes [ID]];
			}
		} else if 	(skids [ID] <= 36) { //VTISs
			if (skids [ID] == 33) 	{circuitText.text = "VTIS1";} else
			if (skids [ID] == 34) 	{circuitText.text = "VTIS2";} else
			if (skids [ID] == 35) 	{circuitText.text = "VTIS4";} else
			if (skids [ID] == 36) 	{circuitText.text = "VTIS6";} else
			descriptionText.text	= "";
			Debug.Log (CIPTypes [ID]);
			if (CIPTypes [ID] == 1) {CIPTypeText.text = "CIP";} else
									{CIPTypeText.text = "AIC";}
		}
		
		IDText.text 			= IDs [ID].ToString ();
		statusText.text			= status [ID];
		dateText.text			= startTimes [ID].ToString ("MMM dd HH:mm") + " - " + endTimes [ID].ToString ("MMM dd HH:mm");
		schedulerText.text		= schedulers [ID];
		scheduledDateText.text	= "Scheduled " + entryTimes [ID].ToString ("MMM dd HH:mm");
		reasonText.text			= reasons [ID];
	}
	
	IEnumerator UploadSchedule () {
		while (queryRunning) {
			yield return new WaitForSeconds (.1f);
		}
		QueryOn ();
		query = "insert into [FairlifeDashboard].[dbo].[CIP_Schedule - Rev 02] values ("+scheduleSkid.ToString()+","+tempCircuit.ToString()+",0,"+scheduleCIPType.ToString()+",'"+scheduledStart.ToString ("yyyy-MM-dd HH:mm")+"','"+scheduledEnd.ToString ("yyyy-MM-dd HH:mm")+"','SCHEDULED','"+AllowableSQLString(scheduleReasonInput.text)+"',current_timestamp,'cipApp')";
		using (SqlConnection dbCon = new SqlConnection(connectionString)) {
			SqlCommand cmd = new SqlCommand(query, dbCon);
			try {
				dbCon.Open();
				reader = cmd.ExecuteReader();
				reader.Close ();
			}
			catch (SqlException exception) {
				Debug.LogWarning(exception.ToString());
			}
		}
		QueryOff ();
		StartCoroutine ("Refresh");
		scheduleReasonInput.text = "";
		yield return null;	
	}
	
	IEnumerator UpdateScheduledTime (int ID) {
		cancelCIP.SetActive (false);
		while (queryRunning) {
			yield return new WaitForSeconds (.01f);
			Debug.Log ("823");
		}
		Debug.Log ("A: " + startTimes [ID].ToString ("yyyy-MM-dd HH:mm:ss"));
		QueryOn ();
		query = "update [FairlifeDashboard].[dbo].[CIP_Schedule - Rev 02] set [StartTime] = '" + startTimes [ID].ToString ("yyyy-MM-dd HH:mm:ss") + "', [EndTime] = '" + endTimes [ID].ToString ("yyyy-MM-dd HH:mm:ss") + "', [Reason] = 'Moved' where [Status] = 'SCHEDULED' and [Skid] = " + skids [ID].ToString ("00") + " and [CIPType] = " + CIPTypes [ID].ToString ("00") + " and ABS(dateDiff (second, [EntryTime], '" + entryTimes [ID].ToString ("yyyy-MM-dd HH:mm:ss") + "')) < 1";
		using (SqlConnection dbCon = new SqlConnection(connectionString)) {
			SqlCommand cmd = new SqlCommand(query, dbCon);
			try {
				dbCon.Open();
				reader = cmd.ExecuteReader();
				reader.Close ();
			} catch (SqlException exception) {
				Debug.LogWarning(exception.ToString());
			}
		}
		QueryOff ();
		UpdateStats (ID);
		StartCoroutine ("Refresh");
		yield return null;
	}
	
	IEnumerator CancelCIP () {
		cancelCIP.SetActive (false);
		stats.SetActive (false);
		while (queryRunning) {
			yield return new WaitForSeconds (.1f);
		}
		QueryOn ();
		query = "delete top (100) from [FairlifeDashboard].[dbo].[CIP_Schedule - Rev 02] where [Skid] = " + skids [j1].ToString ("00") + " and [Circuit] = " + circuits [j1].ToString ("00") + " and ABS(dateDiff (second, [EntryTime], '" + entryTimes [j1].ToString ("yyyy-MM-dd HH:mm:ss") + "')) < 1";
		using (SqlConnection dbCon = new SqlConnection(connectionString)) {
			SqlCommand cmd = new SqlCommand(query, dbCon);
			try {
				dbCon.Open();
				reader = cmd.ExecuteReader();
				reader.Close ();
			}
			catch (SqlException exception) {
				Debug.LogWarning(exception.ToString());
			}
		}
		QueryOff ();
		StartCoroutine ("Refresh");
		yield return null;	
	}
	
	IEnumerator PullOverdueStatus () {
		cancelCIP.SetActive (false);
		stats.SetActive (false);
		while (queryRunning) {
			yield return new WaitForSeconds (.1f);
		}
		QueryOn ();
		query = "exec dbo.spPullCIPSkidOverdueStatus";
		using (SqlConnection dbCon = new SqlConnection(connectionString)) {
			SqlCommand cmd = new SqlCommand(query, dbCon);
			try {
				dbCon.Open();
				reader = cmd.ExecuteReader();
				j2 = 0;
				if (reader.HasRows) {
					while (reader.Read ()) {
						tempSkid = 		(int)reader.GetSqlInt32 (0);
						if (!reader.IsDBNull (2)) {
							CIPDueTimes [tempSkid] = 	(int)reader.GetSqlInt32 (2) * 60;
						} else {
							CIPDueTimes [tempSkid] = 	21600;
						}
						//Debug.Log (CIPDueTimes [j2]);
						if (CIPDueMarkers [tempSkid] == null) {
							tempObject = Instantiate (defaultCube, dueOverlay);
							//tempObject.transform.SetParent (dueOverlay);
							CIPDueMarkers [tempSkid] = tempObject;
							CIPDueMarkers [tempSkid].GetComponent <MeshRenderer>().material = dueMaterial;
							CIPDueMarkers [tempSkid].GetComponent <BoxCollider>().enabled = false;
							CIPDueMarkers [tempSkid].transform.GetChild (0).gameObject.SetActive (false);
							//CIPDueMarkers [tempSkid].transform.GetChild (1).gameObject.SetActive (false);
							CIPDueMarkers [tempSkid].name = "CIPDueMarker " + tempSkid;
						}
						startFloat = (float)CIPDueTimes [tempSkid];
						if (startFloat < -349990f) {startFloat = -349990f;} //346338.8f
						endFloat = 649947;//startFloat + 604800; //1 weeks in seconds
						CIPDueMarkers [tempSkid].transform.position = new Vector3 ((startFloat + endFloat) / 2, skidYPos [tempSkid], 50000);
						CIPDueMarkers [tempSkid].transform.localScale = new Vector3 ((endFloat - startFloat), 2500, 50);
						CIPDueMarkers [tempSkid].SetActive (true);
						j2++;
					}
				}
				reader.Close ();
			}
			catch (SqlException exception) {
				Debug.LogWarning(exception.ToString());
			}
		}
		QueryOff ();
		yield return null;	
	}
	
	void QueryOn () {
		queryRunning = true;
		sqlIcon.SetActive (true);
	}
	
	void QueryOff () {
		queryRunning = false;
		sqlIcon.SetActive (false);
	}
	
	void UpdateSchedulingCIP () {
		if 			(scheduleSkid <= 32) { //Skids
			schedulingCircuitText.text	= (tempCircuit).ToString ("00") + " ->";
			schedulingCircuitNameText.text	= circuitDescriptions [scheduleSkid, tempCircuit];
			schedulingCIPTypeText.text	 	= "<- " + CIPNames [scheduleCIPType] + " ->";
		} else if 	(scheduleSkid <= 36) { //VTISs
			schedulingCircuitText.text	= "";
			schedulingCircuitNameText.text	= "";
			if 		(scheduleCIPType <= 1) 	{schedulingCIPTypeText.text = "<- CIP ->";}
			else							{schedulingCIPTypeText.text = "<- AIC ->";}
		}
	
		scheduledEnd = scheduledStart + new TimeSpan (0, 0, 0, CIPAverageTimes [scheduleSkid, tempCircuit, scheduleCIPType]);
		schedulingStartDateText.text 	= scheduledStart.ToString ("MMM  <dd>");
		schedulingStartTimeText.text 	= scheduledStart.ToString ("<HH>:<mm>");
		schedulingEndTimeText.text 	 	= "Ends: " + scheduledEnd.ToString ("MMM dd HH:mm") + "  (" + ReadableDateDifference (scheduledStart, scheduledEnd) + ")";
		
		startFloat = (float)(scheduledStart - currentDateTime).TotalSeconds;
		endFloat = (float)(scheduledEnd - currentDateTime).TotalSeconds;
		scheduleBlock.transform.position = new Vector3 ((startFloat + endFloat) / 2, skidYPos [scheduleSkid], 50000);
		scheduleBlock.transform.localScale = new Vector3 ((endFloat - startFloat), 2000, 150);
		UpdateBlockInfo (scheduleBlock, tempCircuit, scheduleCIPType);
	}
	
	void UpdateBlockInfo (GameObject go, int circuit, int CIPType) {
		if (circuit < 10) {
			tempString = circuit.ToString ();
		} else {
			tempString = circuit.ToString().Substring (0,1) + "\n" + circuit.ToString().Substring (1,1);
		}
		go.transform.GetChild (0).GetComponent <TextMesh> ().text = tempString;
		go.transform.GetChild (0).localScale = new Vector3 (5 / go.transform.localScale.x, 5 / go.transform.localScale.y, 1);
		if (go.transform.localScale.x < 1000) {
			go.transform.GetChild (0).localPosition = new Vector3 (0,.5f,0);
		} else {
			go.transform.GetChild (0).localPosition = new Vector3 (0.5f-((go.transform.localScale.x-500)/go.transform.localScale.x),.5f,0);
		}
		//go.transform.GetChild (1).localScale = go.transform.GetChild (0).localScale * 200;
		//go.transform.GetChild (1).localPosition = new Vector3 (-go.transform.GetChild (0).localPosition.x, -0.3f, -1);
		//go.transform.GetChild (1).gameObject.GetComponent <MeshRenderer>().material = CipIconMaterials [CIPType];
		
		go.transform.GetChild (0).gameObject.SetActive (true);
		//go.transform.GetChild (1).gameObject.SetActive (true);
	}

	bool Overlap (int a) {
		for (j2 = 0; j2 < startTimes.Count; j2++) {
			if (a == j2 || (status [j2] != "SCHEDULED" && status [j2] != "RUNNING") || skids [a] != skids [j2]) { 
				//Disqualifiers
			} else if (startTimes [a] == startTimes [j2]) {
				return true;
			} else if (startTimes [a] < startTimes [j2]) {
				if (endTimes [a] > startTimes [j2]) { //This means a starts before j2, but a continues past the start time of j2
					return true;
				}
			} else if (startTimes [j2] < startTimes [a]) {
				if (endTimes [j2] > startTimes [a]) { //This means j2 starts before j1, but j2 continues past the start time of j1
					return true;
				}
			}
		}
		return false;
	}
	
	string ReadableDateDifference (DateTime start, DateTime end) {
		startFloat = Mathf.Ceil ((float)(end - start).TotalSeconds);
		tempString = "";
		if (startFloat > 3600) {
			tempString = tempString + (Mathf.Floor (startFloat / 3600)).ToString () + "h";
			startFloat = startFloat % 3600;
		}
		if (startFloat >= 60) {
			if (tempString.Length > 1) {
				tempString = tempString + " ";
			}
			tempString = tempString + (Mathf.Ceil (startFloat / 60)).ToString () + "m";
		}
		return tempString;
	}
	
	string AllowableSQLString (string inputString) {
		tempString = inputString.Replace ("'",",");
		return tempString;
	}
	
	void AddObject () {
		if (inactiveObjects.Count > 0) {
			tempObject = inactiveObjects [0];
			inactiveObjects.Remove (tempObject);
		} else {
			tempObject = Instantiate (defaultCube);
		}
		activeObjects.Add (tempObject);
	}
	
	void RemoveObject (GameObject removal) {
		removal.SetActive (false);
		activeObjects.Remove (removal);
		inactiveObjects.Add (removal);
	}
}*/