using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Data.SqlClient;

public class HistorianGraph : MonoBehaviour {

	public string shortTitle = "";
	public string tag = "";
	public string populateQuery, updateQuery;
	public HistorianIndicator	historianIndicator;
	public float startDelay = 0;
	public float updateDelay = 1;
	public int maxPoints = 60;
	/*[HideInInspector]*/public List <float> values;
	public float minValue, maxValue;
	public Color 	lineColor;
	public Color 	failColor;
	public float lineWidth = 2;
	public bool		barChart;
	public string units = "°F";
	public int decimalPlaces = 0;
	public enum FailState {none, AboveBaseline, BelowBaseline};
	public		bool		displayFailTimes	= true;
	public FailState failState;
	public int baseline = -1;
	public Color baselineColor;
	public enum TimeDisplay {none, Days, Hours, HalfHours, QuarterHours, Minutes};
	public 		TimeDisplay displayTime;
	public 		bool		displayValue	= true;
	//public GameObject newPoint;
	
	bool debug;
	RectTransform rt;
	List <RectTransform> points = new List <RectTransform> {};
	List <RectTransform> lines = new List <RectTransform> {};
	List <Image>			pointImages = new List <Image> ();
	List <Image>			lineImages	= new List <Image> ();
	float delay = 1;
	Text minLabel, midLabel, maxLabel, valueLabel, titleLabel;
	RectTransform valueRT;
	[HideInInspector]public		string valueToString = "0.0";
	Transform pointContainer;
	RectTransform baselineRT;
	Vector2 range;
	List <Text> 			times 	= new List <Text> ();
	List <RectTransform>	timeRTs	= new List <RectTransform> ();
	int 					totalSeconds = 1000;
	float 					spacePerSecond;
	TimeSpan 				intervalTimeSpan;
	float 					maxX;
	List <Text> 			failTimes 			= new List <Text> ();
	List <RectTransform>	failTimeRTs			= new List <RectTransform> ();
	bool					inFailState 		= false;
	public List <int>		failEvents 			= new List <int> ();
	public List <string>	failEventStrings	= new List <string> ();			//This is the string of text that will be displayed on a "failTimes" object
	
	SqlDataReader reader;
	private string connectionString = @"Data Source = 172.16.5.217; user id = cipapp;password = bestinclass&1;Connection Timeout=10;";
	
	//Temp Variables
	bool tempBool;
	DateTime tempDateTime1, tempDateTime2;
	GameObject tempGO;
	RectTransform tempRT;
	int i1, i2;
	float f1, f2, f3, x, y;
	Vector2 v2;
	Vector3 v3;
	string query;
	
	void Awake () {
		if (Application.isEditor) {debug = true;} else {debug = false;}
		rt 				= gameObject.GetComponent <RectTransform> ();
		midLabel		= transform.Find ("Mid Label").gameObject.GetComponent <Text> ();
		maxLabel 		= transform.Find ("Max Label").gameObject.GetComponent <Text> ();
		titleLabel 		= transform.Find ("Title Label").gameObject.GetComponent <Text> ();
		pointContainer 	= transform.Find ("Point Container");
		range = new Vector2 (minValue, maxValue);
		if (decimalPlaces <= 0) {
			valueToString = "0";
		} else {
			valueToString = "0.";
			for (i1 = 0; i1 < decimalPlaces; i1++) {
				valueToString += "0";
			}
		}
		if(minLabel){minLabel = transform.Find ("Min Label").gameObject.GetComponent <Text> ();minLabel.text = 	range.x.ToString (valueToString) + units;}
		midLabel.text = 	((range.y + range.x) / 2).ToString ("0") + units;
		maxLabel.text = 	range.y.ToString ("0") + units;
		titleLabel.text = 	shortTitle;
		if (displayValue) {
			valueLabel	= transform.Find ("Value Label").gameObject.GetComponent <Text> ();
			valueRT 		= valueLabel.gameObject.GetComponent <RectTransform> ();
			valueLabel.text = 	range.y.ToString (valueToString) + units;
		}
		if (baseline > -1) {
			AddLine ();
			lines.Clear ();
			lineImages.Clear ();
			baselineRT = tempRT;
			tempRT.gameObject.GetComponent <Image> ().color = baselineColor;
			tempRT.sizeDelta = new Vector2 (rt.sizeDelta.x, 2); //Hard coded the line width for baseline
			y = ((baseline - range.x) / (range.y - range.x)) * rt.sizeDelta.y;
			tempRT.anchoredPosition = new Vector2 (rt.sizeDelta.x / 2, y);
			tempGO.name = "Baseline";
		}
		times.Add 		(transform.Find ("Time Container/Time").gameObject.GetComponent <Text> ());
		timeRTs.Add 	(times [0].transform.GetComponent <RectTransform> ());
		if (displayTime == TimeDisplay.none) {
			times [0].text = "";
		}
		totalSeconds = Mathf.RoundToInt (updateDelay * maxPoints);
		spacePerSecond = (float)rt.sizeDelta.x / (float)totalSeconds;
		if (displayTime == TimeDisplay.Days) {
			intervalTimeSpan = new TimeSpan (1,0, 0,0,0);
		} else if (displayTime == TimeDisplay.Hours) {
			intervalTimeSpan = new TimeSpan (0,1, 0,0,0);
		} else if (displayTime == TimeDisplay.HalfHours) {
			intervalTimeSpan = new TimeSpan (0,0,30,0,0);
		} else if (displayTime == TimeDisplay.QuarterHours) {
			intervalTimeSpan = new TimeSpan (0,0,15,0,0);
		} else if (displayTime == TimeDisplay.Minutes) {
			intervalTimeSpan = new TimeSpan (0,0, 1,0,0);
		}
		maxX = rt.sizeDelta.x - 50;
		failTimes.Add 	(transform.Find ("Fail Container/Fail").gameObject.GetComponent <Text> ());
		if (failState != FailState.none) {
			failTimeRTs.Add (failTimes [0].transform.GetComponent <RectTransform> ());
		}
		failTimes [0].text = "";
		StartCoroutine (Populate ());
		delay = startDelay;
	}
	
	void Update () {
		delay -= Time.deltaTime;
		if (delay <= 0) {
			delay += updateDelay;
			StartCoroutine (VisualUpdate ());
		}
	}
	
	IEnumerator Populate () {
		while (SqlJobManager.Instance.sqlRunning) {
			yield return new WaitForSeconds (.1f);
		}
		SqlJobManager.Instance.StartJob ();
		if 			(tag != "") 			{ query = "select cast ([value] as float) from openquery([FLUSCVFTH01],'select value from [piarchive]..[piinterp2] where tag =''" + tag + "'' and time >= '' *-" + totalSeconds + "s '' AND time<='' * '' AND timestep=''" + updateDelay + "s''')";
		} else if 	(populateQuery != "") 	{ query = populateQuery;}
		using (SqlConnection dbCon = new SqlConnection(connectionString)) {
			SqlCommand cmd = new SqlCommand(query, dbCon);
			try {
				dbCon.Open();
				reader = cmd.ExecuteReader();
				if (reader.HasRows) {
					while (reader.Read ()) {
						if (!reader.IsDBNull(0)) {
							values.Add ((float)reader.GetDouble(0));
						} else {
							if (values.Count > 0) {
								values.Add (values [values.Count-1]);
							} else {
								values.Add (0);
							}
						}
					}
				}
				reader.Close ();
			}
			catch (SqlException exception) {
				if (debug) {Debug.LogWarning(exception.ToString()); Debug.LogWarning(query);}
			}
		}
		SqlJobManager.Instance.EndJob ();
		if (failState == FailState.AboveBaseline) {
			inFailState = false;
			failEvents.Clear ();
			tempDateTime1 = DateTime.Now - new TimeSpan (0,0,0, Mathf.RoundToInt (updateDelay * maxPoints),0);
			for (i1 = 0; i1 < values.Count; i1++) {
				if (values [i1] < baseline) {
					inFailState = false;
				} else if (!inFailState) { //This means they are out of range, and they weren't at the last point
					if (i1 != 0) { //If this is the first point, we still want to know it is in a fail state, but we don't want to mark the time, because it could have failed before this point
						failEvents.Add (i1);
						failEventStrings.Add ( (tempDateTime1 + new TimeSpan (0,0,0,Mathf.FloorToInt(i1 * updateDelay))).ToString ("HH:mm"));
					}
					inFailState = true;
				}
			}
		}
		yield return null;
	}

	IEnumerator VisualUpdate () {
		while (SqlJobManager.Instance.sqlRunning) {
			yield return new WaitForSeconds (.1f);
		}
		SqlJobManager.Instance.StartJob ();
		if 			(tag != "") 			{ query = "exec [dbo].[spPullCurrentHistorianValue] @tag = '" + tag + "'";
		} else if 	(updateQuery != "") 	{ query = updateQuery;}
		using (SqlConnection dbCon = new SqlConnection(connectionString)) {
			SqlCommand cmd = new SqlCommand(query, dbCon);
			try {
				dbCon.Open();
				reader = cmd.ExecuteReader();
				if (reader.HasRows) {
					while (reader.Read ()) {
						if (!reader.IsDBNull(0)) {
							f1 = (float)reader.GetDouble(0);
						} else {
							if (values.Count > 0) {
								f1 = values [values.Count-1];
							} else {
								f1 =0;
							}
						}
						
						
						/*if (!reader.IsDBNull(0) && float.TryParse (reader [0].ToString (), out f1)) {
						
						} else {
							if (values.Count > 0) {
								f1 = values [values.Count-1];
								//values.Add (values [values.Count-1]);
							} else {
								f1 =0;
							}
						}*/
						
						values.Add (f1);
						if (displayValue) {
							valueLabel.text = f1.ToString (valueToString) + units;
						}
						if (historianIndicator) {
							historianIndicator.valueLabel.text = f1.ToString (valueToString) + units;
							//We're going to check if we are in a fail state
							if (failState != FailState.none) {
								
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
		SqlJobManager.Instance.EndJob ();
		if (values.Count > 1) {
			range.x = minValue;
			range.y = maxValue;
			for (i1 = 0; i1 < values.Count; i1++) {
				if (values [i1] < range.x) {
					range.x = values [i1]; tempBool = true;
				} else if (values [i1] > range.y) {
					range.y = values [i1]; tempBool = true;
				}
			}

			if (tempBool) { //This means the line has moved outside of the given range
				range.x = Mathf.Floor 	(range.x);
				range.y = Mathf.Ceil 	(range.y);
				if (minLabel) {minLabel.text = range.x.ToString (valueToString) + units;}
				maxLabel.text = range.y.ToString (valueToString) + units;
				midLabel.text = ((range.y + range.x) / 2).ToString (valueToString) + units;
				if (baseline > -1) {
					y = ((baseline - range.x) / (range.y - range.x)) * rt.sizeDelta.y;
					baselineRT.anchoredPosition = new Vector2 (rt.sizeDelta.x / 2, y);
				}
			}

			while (values.Count > maxPoints) {
				values.RemoveAt (0);
			}
			f1 = rt.sizeDelta.x / (maxPoints - 1);
			f3 = (maxPoints - values.Count) * f1;
			for (i1 = 0; i1 < values.Count; i1++) {
				if (points.Count < values.Count) {
					AddPoint (new Vector3 (0,0));
				}
				if (!barChart && lines.Count < values.Count - 1) {
					AddLine ();
				}
				if (barChart) {
					v2.x = f3 + (i1 * f1);
					v2.y = ((values [i1] - range.x) / (range.y - range.x)) * rt.sizeDelta.y;
					if (float.IsNaN(v2.y)){
						v2.y = rt.sizeDelta.y / 2;
					}
					v2.y /= 2;
					points [i1].anchoredPosition = v2;
					points [i1].sizeDelta = new Vector2 (lineWidth, v2.y * 2);
				} else {
					v2.x = f3 + (i1 * f1);
					v2.y = ((values [i1] - range.x) / (range.y - range.x)) * rt.sizeDelta.y;
					if (float.IsNaN(v2.y)){
						v2.y = rt.sizeDelta.y / 2;
					}
					points [i1].anchoredPosition = v2;
				}
				if (i1 > 0) { //This means we are not at the last point
					if (barChart) {

					} else {
						f2 = Vector2.Distance (points [i1 - 1].anchoredPosition, points [i1].anchoredPosition);
						lines [i1 - 1].sizeDelta = new Vector2 (f2,lineWidth);
						lines [i1 - 1].anchoredPosition = Vector2.Lerp (points [i1 - 1].anchoredPosition, points [i1].anchoredPosition, .5f);
						v2 = (points [i1 - 1].anchoredPosition - points [i1].anchoredPosition).normalized;
						if (v2.x == 0) {
							lines [i1 - 1].localEulerAngles = Vector3.zero;
						} else {
							lines [i1 - 1].localEulerAngles = new Vector3 (0,0, Mathf.Atan (v2.y / v2.x) * 180 / Mathf.PI);
						}
					}
				}
			}
			if (displayValue) {
				y = points [points.Count - 1].anchoredPosition.y;
				if (y > rt.sizeDelta.y - 50) { //-40
					y = rt.sizeDelta.y - 50;
				} else if (y < 40) {
					y = 40;
				}
				valueRT.anchoredPosition = new Vector2 (valueRT.anchoredPosition.x, y);
			}
		}
		if (displayTime == TimeDisplay.none) {
			//Do nothing
		} else {
			tempDateTime1 = DateTime.Now.Subtract (new TimeSpan (0,0,0,totalSeconds,0)); //Start time of the chart
			 if (displayTime == TimeDisplay.Days) {
				tempDateTime2 = DateTime.Now.Subtract (new TimeSpan (0,DateTime.Now.Hour,	DateTime.Now.Minute,											DateTime.Now.Second,	DateTime.Now.Millisecond));
			} else if (displayTime == TimeDisplay.Hours) {
				tempDateTime2 = DateTime.Now.Subtract (new TimeSpan (0,0,					DateTime.Now.Minute,											DateTime.Now.Second,	DateTime.Now.Millisecond));
			} else if (displayTime == TimeDisplay.HalfHours) {
				tempDateTime2 = DateTime.Now.Subtract (new TimeSpan (0,0,					Mathf.FloorToInt (Mathf.Floor (DateTime.Now.Minute / 30) * 30),	DateTime.Now.Second,	DateTime.Now.Millisecond));
			} else if (displayTime == TimeDisplay.QuarterHours) {
				tempDateTime2 = DateTime.Now.Subtract (new TimeSpan (0,0,					DateTime.Now.Minute % 15,										DateTime.Now.Second,	DateTime.Now.Millisecond));
			} else if (displayTime == TimeDisplay.Minutes) {
				tempDateTime2 = DateTime.Now.Subtract (new TimeSpan (0,0,					0,																DateTime.Now.Second,	DateTime.Now.Millisecond));
			}
			v2.y = 0;
			for (i1 = 0; i1 < 100; i1++) {
				if (tempDateTime2 < tempDateTime1) {
					i1 = 100;
				} else {
					if (times.Count <= i1) {
						tempGO = Instantiate (timeRTs [0].gameObject, timeRTs [0].transform.position, new Quaternion (0,0,0,0), timeRTs [0].transform.parent);
						timeRTs.Add (tempGO.GetComponent <RectTransform> ());
						times.Add 	(tempGO.GetComponent <Text> ());
					}
					if (displayTime == TimeDisplay.Days) {
						
					} else if (displayTime == TimeDisplay.Hours) {
						times [i1].text = tempDateTime2.Hour.ToString ("00") + ":00";
					} else if (displayTime == TimeDisplay.HalfHours || displayTime == TimeDisplay.QuarterHours || displayTime == TimeDisplay.Minutes) {
						times [i1].text = tempDateTime2.Hour.ToString ("00") + ":" + tempDateTime2.Minute.ToString ("00");
					}
					v2.x = (spacePerSecond * (float)(tempDateTime2 - tempDateTime1).TotalSeconds) - 25; //Assumes time is 50 pixils wide
					if (v2.x > maxX) {
						v2.x = maxX;
					} else if (v2.x < 0) {
						v2.x = 0;
					}
					timeRTs[i1].anchoredPosition = v2;
					tempDateTime2 -= intervalTimeSpan;
				}
			}
		}
		if (failState != FailState.none) {
			for (i1 = 0; i1 < values.Count; i1++) {
				if ((failState == FailState.BelowBaseline && values [i1] < baseline) || (failState == FailState.AboveBaseline && values [i1] > baseline)) {
					pointImages [i1].color = failColor;
					if (!barChart && i1 < values.Count - 1 && i1 > 0) {
						if ((failState == FailState.BelowBaseline && values [i1 + 1] < baseline) || (failState == FailState.AboveBaseline && values [i1 + 1] > baseline)) {
							lineImages [i1].color = failColor;
						} else {
							lineImages [i1].color = lineColor;
						}
					}
				} else {
					pointImages [i1].color = lineColor;
					if (!barChart && i1 < values.Count - 1) {
						lineImages [i1].color = lineColor;
					}
				}
			}
			if (failState == FailState.AboveBaseline) {
				if (values [values.Count - 1] < baseline) {
					inFailState = false;
				} else if (!inFailState) {
					inFailState = true;
					tempDateTime1 = DateTime.Now - new TimeSpan (0,0,0, Mathf.RoundToInt (updateDelay * maxPoints),0);
					failEvents.Add (values.Count - 1);
					failEventStrings.Add ( (tempDateTime1 + new TimeSpan (0,0,0,Mathf.FloorToInt(i1 * updateDelay))).ToString ("HH:mm"));
					if (displayFailTimes) {
						tempGO = Instantiate (failTimeRTs [0].gameObject, failTimeRTs [0].transform.position, new Quaternion (0,0,0,0), failTimeRTs [0].transform.parent);
						failTimeRTs.Add (tempGO.GetComponent <RectTransform> ());
						failTimes.Add 	(tempGO.GetComponent <Text> ());
						failTimes [failTimes.Count - 1].text = failEventStrings [failTimes.Count - 1];
						failTimeRTs [failTimes.Count - 1].gameObject.name = failTimes [failTimes.Count - 1].text;
					}
					if (historianIndicator) {
						historianIndicator.lastFailed.text = "Failed " + (tempDateTime1 + new TimeSpan (0,0,0,Mathf.FloorToInt(i1 * updateDelay))).ToString ("MM-dd HH:mm");
					}
				}
			} else {	//Fails below line.  Much less common, but we still need this code to be written
				
			}
			if (failEvents.Count == 0) {
				if (failTimes.Count > 1) {
					if (failTimes [1].gameObject) {
						Destroy (failTimes [1].gameObject);
					}
					failTimes.RemoveAt (1);
				}
				if (failTimeRTs.Count > 1) {
					if (failTimeRTs [1].gameObject) {
						Destroy (failTimeRTs [1].gameObject);
					}
					failTimeRTs.RemoveAt (1);
				}
				if (failTimes [0].text != "") {
					failTimes [0].text  = "";
				}
			} else {
				for (i1 = 0; i1 < failEvents.Count; i1++) {
					failEvents [i1]--;
					if (failEvents [i1] <= 1) { //This means that this fail event happened before the scope of this chart, so it should be removed
						failEvents.RemoveAt (i1);
						failEventStrings.RemoveAt (i1);
						if (failTimes.Count > 1) {
							Destroy (failTimeRTs [i1].gameObject);
							Destroy (failTimes [i1].gameObject);
							failTimeRTs.RemoveAt (i1);
							failTimes.RemoveAt (i1);
						} else {
							failTimes [i1].text = "";
						}
					}

					if (displayFailTimes) {
						if (failEvents.Count > failTimes.Count) {
							tempGO = Instantiate (failTimeRTs [0].gameObject, failTimeRTs [0].transform.position, new Quaternion (0,0,0,0), failTimeRTs [0].transform.parent);
							failTimeRTs.Add (tempGO.GetComponent <RectTransform> ());
							failTimes.Add 	(tempGO.GetComponent <Text> ());
						}
						failTimes [i1].text = failEventStrings [i1];
						v2.x = points [failEvents[i1]].anchoredPosition.x;
						v2.y = baselineRT.anchoredPosition.y;
						if (v2.x > maxX) {
							v2.x = maxX;
						} else if (v2.x < 0) {
							v2.x = 0;
						}
						failTimeRTs [i1].anchoredPosition = v2;
					}
				}
			}
		}
		if (historianIndicator && failState != FailState.none) {
			if (inFailState) {
				if (historianIndicator.displayType == HistorianIndicator.DisplayType.SolidColor) {
					historianIndicator.backdrop.color = failColor;
				}
			} else {
				if (historianIndicator.displayType == HistorianIndicator.DisplayType.SolidColor) {
					historianIndicator.backdrop.color = historianIndicator.normalColor;
				}
			}
		}
		yield return null;	
	}
	
	void AddPoint (Vector2 point) {
		tempGO = new GameObject ("Point " + points.Count, typeof (Image));
		tempGO.transform.SetParent (pointContainer, false);
		tempRT = tempGO.GetComponent <RectTransform> ();
		tempRT.anchoredPosition = point;
		tempRT.sizeDelta = new Vector2 (lineWidth,lineWidth);
		tempRT.anchorMin = new Vector2 (0,0);
		tempRT.anchorMax = new Vector2 (0,0);
		tempGO.GetComponent <Image> ().color = lineColor;
		points.Add (tempRT);
		if (failState != FailState.none) {
			pointImages.Add (tempGO.GetComponent <Image> ());
		}
	}
	
	void AddLine () {
		tempGO = new GameObject ("Line " + lines.Count, typeof (Image));
		tempGO.transform.SetParent (pointContainer, false);
		tempGO.GetComponent <Image> ().color = lineColor;
		tempRT = tempGO.GetComponent <RectTransform> ();
		tempRT.sizeDelta = new Vector2 (0,0);
		tempRT.anchorMin = new Vector2 (0,0);
		tempRT.anchorMax = new Vector2 (0,0);
		tempGO.GetComponent <Image> ().color = lineColor;
		lines.Add (tempRT);
		if (failState != FailState.none) {
			lineImages.Add (tempGO.GetComponent <Image> ());
		}
	}
}
