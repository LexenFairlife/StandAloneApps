using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Data.SqlClient;
using UnityEngine.Networking;

public class MultiLineGraph : MonoBehaviour {
    
	[TextArea(5,50)] public	string			populateQuery, updateQuery;
	public	float			delay = 1, updateDelay = 60;
	public	int				maxPoints;
	public	float			minValue, maxValue, semiMinValue, semiMaxValue, baseline = -1;
	public	enum			OutOfRangeHandling {Unbound, Scale, CutOff, Fit, SemiBound};
	public 					OutOfRangeHandling outOfRangeHandling;
	public	List <Color>	colors;
	public	Color			baselineColor;
	public	float			lineWidth = 2, pointWidth = -1;
	public	List<string>	units = new List <string> {"°F"};
	public	string			formatText = "0.0";
	public	enum			TimeDisplay {none, Days, Hours, HalfHours, QuarterHours, Minutes};
	public 					TimeDisplay displayTime;
	public	List <Text>		labelTexts;
	
	RectTransform rt;
	List <List <float>> values = new List <List <float>> {};
	List <List<RectTransform>>	points = new List <List<RectTransform>> {};
	List <List<RectTransform>>	lines = new List <List<RectTransform>> {};
	List <List<Image>>			pointImages = new List <List<Image>> ();
	List <List<Image>>			lineImages	= new List <List<Image>> ();
	Text						minLabel, midLabel, maxLabel, titleLabel;
	
	Transform pointContainer;
	RectTransform baselineRT;
	Vector2 range;
	List <Text> 			times 	= new List <Text> ();
	List <RectTransform>	timeRTs	= new List <RectTransform> ();
	int 					totalSeconds = 1000;
	float 					spacePerSecond;
	TimeSpan 				intervalTimeSpan;
	float 					maxX;
	
	SqlDataReader reader;
	private string connectionString = @"Data Source = 172.16.5.217; user id = cipapp;password = bestinclass&1;Connection Timeout=10;";
	SqlCommand cmd;
	SqlConnection dbCon;
	
	bool tempBool, updating;
	DateTime tempDateTime1, tempDateTime2;
	GameObject tempGO;
	RectTransform tempRT;
	int i1, i2;
	float f1, f2, f3, x, y;
	Vector2 v2, v2a;
	Vector3 v3;
	string query;
	
	
	void Awake () {
		if (pointWidth == -1) {pointWidth = lineWidth;}
		rt 				= gameObject.GetComponent <RectTransform> ();
		if (transform.Find ("Min Label")) {minLabel		= transform.Find ("Min Label").gameObject.GetComponent <Text> ();}
		if (transform.Find ("Mid Label")) {midLabel		= transform.Find ("Mid Label").gameObject.GetComponent <Text> ();}
		if (transform.Find ("Max Label")) {maxLabel		= transform.Find ("Max Label").gameObject.GetComponent <Text> ();}
		pointContainer 	= transform.Find ("Point Container");
		range.x = minValue;
		range.y = maxValue;
		if (minLabel){minLabel.text = 	range.x.ToString ("0");}
		if (midLabel){midLabel.text = 	((range.y + range.x) / 2).ToString ("0");}
		if (maxLabel){maxLabel.text = 	range.y.ToString ("0");}

		if (baseline > -1) {
			tempGO = new GameObject ("Base Line ", typeof (Image));
			tempGO.transform.SetParent (pointContainer, false);
			tempGO.GetComponent <Image> ().color = baselineColor;
			tempRT = tempGO.GetComponent <RectTransform> ();

			baselineRT = tempRT;
			tempRT.sizeDelta = new Vector2 (rt.sizeDelta.x, 2); //Hard coded the line width for baseline
			y = (((baseline - range.x) / (range.y - range.x)) * rt.sizeDelta.y) - (rt.sizeDelta.y / 2);
			tempRT.anchoredPosition = new Vector2 (0, y);
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
		for (i1 = 0; i1 < labelTexts.Count; i1++) {
			if (labelTexts [i1]) {
				labelTexts [i1].color = colors [i1];
			}
		}
		updating = true;
		if (maxPoints > 0) {
			StartCoroutine (Populate ());
		} else {
			VisualUpdate ();
		}
	}
	
	void Update () {
		
		if (updateDelay != 60) {
			delay -= Time.deltaTime;
		}
		if ((updateDelay == 60 && System.DateTime.Now.Second % 60 == delay) || (delay <= 0) ) {
			if (!updating) {
				updating = true;
				StartCoroutine (VisualUpdate ());
			}
		} else if (updating) {
			updating = false;
		}
	}
	
	IEnumerator Populate () {
		using (dbCon = new SqlConnection(connectionString)) {
			cmd = new SqlCommand(populateQuery, dbCon);
			try {
				dbCon.Open();
				reader = cmd.ExecuteReader();
				i1 = 0;
				i2 = reader.FieldCount;
				//values.Clear ();
				for (i1 = 0; i1 < i2; i1++) {
					if (values.Count <= i1) {							 values.Add (new List <float> {});	} else { values [i1].Clear ();}
					if (pointWidth > 0 && points.Count <= values.Count) {points.Add (new List<RectTransform> {});}
					if (lineWidth  > 0 && lines.Count <= values.Count)  {lines.Add  (new List<RectTransform> {});}
				}
				if (reader.HasRows) {
					while (reader.Read ()) {
						for (i1 = 0; i1 < i2; i1++) {
							if (!reader.IsDBNull(i1)) {
								values [i1].Add ((float)reader.GetDouble(i1));
							} else {
								if (values [i1].Count > 0) {
									values [i1].Add (values [i1][values[i1].Count-1]);
								} else {
									values [i1].Add (0);
								}
							}
						}
					}
				}
				reader.Close ();
				dbCon.Dispose ();
			}
			catch (SqlException exception) {
				Debug.LogWarning(exception.ToString());
			}
		}
		yield return null;
	}
	
	IEnumerator VisualUpdate () {
		if (maxPoints == 0 || updateQuery.Length < 1) {
			using (dbCon = new SqlConnection(connectionString)) {
				cmd = new SqlCommand(populateQuery, dbCon);
				try {
					dbCon.Open();
					reader = cmd.ExecuteReader();
					i1 = 0;
					i2 = reader.FieldCount;
					//values.Clear ();
					for (i1 = 0; i1 < i2; i1++) {
						if (values.Count <= i1) {							 values.Add (new List <float> {});	} else { values [i1].Clear ();}
						if (pointWidth > 0 && points.Count <= values.Count) {points.Add (new List<RectTransform> {});}
						if (lineWidth  > 0 && lines.Count <= values.Count)  {lines.Add  (new List<RectTransform> {});}
					}
					if (reader.HasRows) {
						while (reader.Read ()) {
							for (i1 = 0; i1 < i2; i1++) {
								if (!reader.IsDBNull(i1)) {
									values [i1].Add ((float)reader.GetDouble(i1));
								} else {
									if (values [i1].Count > 0) {
										values [i1].Add (values [i1][values[i1].Count-1]);
									} else {
										values [i1].Add (0);
									}
								}
							}
						}
					}
					reader.Close ();
					dbCon.Dispose ();
				}
				catch (SqlException exception) {
					Debug.LogWarning(exception.ToString());
				}
			}
			for (i1 = 0; i1 < i2; i1++) {
				if (labelTexts.Count > i1 && labelTexts [i1] && values.Count > i1 && values [i1].Count > values [i1].Count-1 && values [i1].Count > 0 && units.Count > i1) {
					labelTexts [i1].text = labelTexts [i1].gameObject.name + values [i1][values [i1].Count-1].ToString (formatText) + units [i1];
				}
			}
		} else {
			using (dbCon = new SqlConnection(connectionString)) {
				cmd = new SqlCommand(updateQuery, dbCon);
				try {
					dbCon.Open();
					reader = cmd.ExecuteReader();
					i2 = reader.FieldCount;
					if (reader.HasRows) {
						while (reader.Read ()) {
							for (i1 = 0; i1 < i2; i1++) {
								if (!reader.IsDBNull(i1)) {
									values [i1].Add ((float)reader.GetDouble(i1));
									if (labelTexts.Count > i1 && labelTexts [i1]) {
										labelTexts [i1].text = labelTexts [i1].gameObject.name + values [i1][values [i1].Count-1].ToString (formatText) + (units.Count > i1 ? units [i1] : "");
									}
								} else {
									if (values [i1].Count > 0) {
										values [i1].Add (values [i1][values[i1].Count-1]);
									} else {
										values [i1].Add (0);
									}
								}
							}
						}
					}
					reader.Close ();
					dbCon.Dispose ();
				}
				catch (SqlException exception) {
					Debug.LogWarning(exception.ToString());
				}
			}
		}
		
		if (outOfRangeHandling != OutOfRangeHandling.Unbound && outOfRangeHandling != OutOfRangeHandling.SemiBound) {
			range.x = minValue;
			range.y = maxValue;
			if (outOfRangeHandling == OutOfRangeHandling.Scale) {
				for (i1 = 0; i1 < values.Count; i1++) {
					for (i2 = 0; i2 < values [i1].Count; i2++) {
						if (values [i1][i2] < range.x) {
							range.x = values [i1][i2];
						} else if (values [i1][i2] > range.y) {
							range.y = values [i1][i2];
						}
					}
				}
			} else if (outOfRangeHandling == OutOfRangeHandling.Fit) {
				for (i1 = 0; i1 < values.Count; i1++) {
					for (i2 = 0; i2 < values [i1].Count; i2++) {
						if (values [i1][i2] < range.x) {
							values [i1][i2] = range.x;
						} else if (values [i1][i2] > range.y) {
							values [i1][i2] = range.y;
						}
					}
				}
			} else if (outOfRangeHandling == OutOfRangeHandling.CutOff) {
				for (i1 = 0; i1 < points.Count && i1 < values.Count; i1++) {
					for (i2 = 0; i2 < points [i1].Count && i2 < values [i1].Count; i2++) {
						if (values [i1][i2] < minValue || values [i1][i2] > maxValue) {
							if (pointWidth > 0 && points [i1][i2] && points [i1][i2].gameObject) {Destroy(points [i1][i2].gameObject);}
							if (lineWidth  > 0) {
								if (lines.Count > i1 && lines [i1].Count > i2) {
									if (i2 > 0 && lines.Count > i1 && lines [i1][i2-1].gameObject) {
										lines [i1][i2-1].gameObject.SetActive (false);
									}
									if (lines.Count > i1 && lines [i1].Count > i2 && lines [i1][i2].gameObject) {lines [i1][i2].gameObject.SetActive (false);}
								}
							}
						} else {
							if (pointWidth > 0 && !points [i1][i2]) {AddPoint (i1,Vector2.zero,false); points [i1][i2] = tempRT;}
						}
					}
				}
			}
		} else if (outOfRangeHandling == OutOfRangeHandling.SemiBound) {
			for (i1 = 0; i1 < values.Count; i1++) {
				for (i2 = 0; i2 < values [i1].Count; i2++) {
					if (values [i1][i2] < semiMinValue) {
						values [i1][i2] = semiMinValue;
					} else if (values [i1][i2] > semiMaxValue) {
						values [i1][i2] = semiMaxValue;
					}
				}
			}
		}
		
		if (values [0].Count > 1) {
			if (maxPoints > 0) {
				for (i1 = 0; i1 < values.Count; i1++) {
					while (values [i1].Count > maxPoints) {
						values[i1].RemoveAt (0);
					}
				}
				f1 = rt.sizeDelta.x / (maxPoints - 1);
				f3 = (maxPoints - values [0].Count) * f1;
			} else {
				f1 = rt.sizeDelta.x / (values [0].Count - 1);
			}

			for (i1 = 0; i1 < values.Count; i1++) {	//Number of lines
				if (points.Count > i1 && values [i1].Count > 0) {
					while (points [i1].Count > values [i1].Count && points [i1][0]) {
						if (points [i1][0]) {
							if (points [i1][0].gameObject) {
								Destroy(points [i1][0].gameObject);
							}
							points [i1].RemoveAt (0);
						}
					}
				}
				if (lines.Count >  i1) {
					while (lines [i1].Count > values [i1].Count-1) {
						Destroy(lines [i1][0].gameObject);
						lines [i1].RemoveAt (0);
					}
				}

				for (i2 = 0; i2 < values [i1].Count; i2++) {	//Points for each line
					if (pointWidth > 0 && points [i1].Count < values [i1].Count) {
						if (pointWidth > 0) { if (outOfRangeHandling == OutOfRangeHandling.CutOff && (values [i1][i2] < minValue || values [i1][i2] > maxValue) ) {points[i1].Add (null);} else {AddPoint (i1,new Vector3 (0,0),true);}}
						if (lineWidth  > 0) {AddLine (i1);}
					}
					if (maxPoints > 0) {
						v2.x = f3 + (i2 * f1);
					} else {
						v2.x = i2 * f1;
					}
					v2.y = ((values [i1][i2] - range.x) / (range.y - range.x)) * rt.sizeDelta.y;
					if (float.IsNaN(v2.y)){
						v2.y = rt.sizeDelta.y / 2;
					}
					if (pointWidth > 0 && points [i1][i2] && points [i1][i2].gameObject) {points [i1][i2].anchoredPosition = v2;}
					if (i2 > 0) {//This means we are not at the last point
						if (points [i1][i2 - 1] && points [i1][i2 - 1].gameObject && points [i1][i2] && points [i1][i2].gameObject) {	f2 = Vector2.Distance (points [i1][i2 - 1].anchoredPosition, points [i1][i2].anchoredPosition);}
						if (lineWidth  > 0) {lines [i1][i2 - 1].sizeDelta = new Vector2 (f2,lineWidth);}
						if (lineWidth  > 0 && points [i1][i2 - 1] && points [i1][i2 - 1].gameObject && points [i1][i2] && points [i1][i2].gameObject) {lines [i1][i2 - 1].anchoredPosition = Vector2.Lerp (points [i1][i2 - 1].anchoredPosition, points [i1][i2].anchoredPosition, .5f);}
						if (points [i1][i2 - 1] && points [i1][i2 - 1].gameObject && points [i1][i2] && points [i1][i2].gameObject) {	v2 = (points [i1][i2 - 1].anchoredPosition - points [i1][i2].anchoredPosition).normalized;}
						if (lineWidth  > 0) {
							if (v2.x == 0) {
								lines [i1][i2 - 1].localEulerAngles = Vector3.zero;
							} else {
								lines [i1][i2 - 1].localEulerAngles = new Vector3 (0,0, Mathf.Atan (v2.y / v2.x) * 180 / Mathf.PI);
							}
						}
					}
				}
				yield return new WaitForSeconds (0);
			}
		}
		
		if (displayTime == TimeDisplay.none) {
			//Do nothing
		} else {
			if (maxPoints == 0) {
				totalSeconds = Mathf.RoundToInt (updateDelay * values [0].Count);
				spacePerSecond = (float)rt.sizeDelta.x / (float)totalSeconds;
			}
			
			tempDateTime1 = DateTime.Now.Subtract (new TimeSpan (0,0,0,totalSeconds,0)); //Start time of the chart
			 if (displayTime == TimeDisplay.Days) {
				tempDateTime2 = DateTime.Now.Subtract (new TimeSpan (0,DateTime.Now.Hour,	DateTime.Now.Minute,											DateTime.Now.Second-1,	DateTime.Now.Millisecond));	//I did the offset of one second so it wouldn't round down below the current hour
			} else if (displayTime == TimeDisplay.Hours) {
				tempDateTime2 = DateTime.Now.Subtract (new TimeSpan (0,0,					DateTime.Now.Minute,											DateTime.Now.Second-1,	DateTime.Now.Millisecond));
			} else if (displayTime == TimeDisplay.HalfHours) {
				tempDateTime2 = DateTime.Now.Subtract (new TimeSpan (0,0,					Mathf.FloorToInt (Mathf.Floor (DateTime.Now.Minute / 30) * 30),	DateTime.Now.Second-1,	DateTime.Now.Millisecond));
			} else if (displayTime == TimeDisplay.QuarterHours) {
				tempDateTime2 = DateTime.Now.Subtract (new TimeSpan (0,0,					DateTime.Now.Minute % 15,										DateTime.Now.Second-1,	DateTime.Now.Millisecond));
			} else if (displayTime == TimeDisplay.Minutes) {
				tempDateTime2 = DateTime.Now.Subtract (new TimeSpan (0,0,					0,																DateTime.Now.Second-1,	DateTime.Now.Millisecond));
			}
			v2.y = 0;
			for (i1 = 0; i1 < 100; i1++) {
				if (tempDateTime2 < tempDateTime1) {
					if (times.Count > i1) {
						times [i1].text = "";
					} else {
						i1 = 100;
					}
				} else {
					if (times.Count <= i1) {
						tempGO = Instantiate (timeRTs [0].gameObject, timeRTs [0].transform.position, Quaternion.identity, timeRTs [0].transform.parent);
						timeRTs.Add (tempGO.GetComponent <RectTransform> ());
						times.Add 	(tempGO.GetComponent <Text> ());
					}
					if (displayTime == TimeDisplay.Days) {
						
					} else if (displayTime == TimeDisplay.Hours) {
						if ((DateTime.Now - tempDateTime1).TotalHours > 72) {
							if (tempDateTime2.Hour == 0) {
								times [i1].text = tempDateTime2.ToString ("MMM-dd");
							} else {
								times [i1].text = "";
							}
						} else if ((DateTime.Now - tempDateTime1).TotalHours > 24) {
							if (tempDateTime2.Hour % 6 == 0) {
								times [i1].text = tempDateTime2.Hour.ToString ("00") + ":00";
							} else {
								times [i1].text = "";
							}
						} else if ((DateTime.Now - tempDateTime1).TotalHours > 12) {
							if (tempDateTime2.Hour % 2 == 0) {
								times [i1].text = tempDateTime2.Hour.ToString ("00") + ":00";
							} else {
								times [i1].text = "";
							}
						} else {
							times [i1].text = tempDateTime2.Hour.ToString ("00") + ":00";
						}
						
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
		yield return null;	
	}

	void AddPoint (int list, Vector2 point, bool addToList) {
		tempGO = new GameObject ("Point " + list.ToString () +":" + points [list].Count, typeof (Image));
		tempGO.transform.SetParent (pointContainer, false);
		tempRT = tempGO.GetComponent <RectTransform> ();
		tempRT.anchoredPosition = point;
		tempRT.sizeDelta = Vector2.one * pointWidth;		
		tempRT.anchorMin = Vector2.zero;
		tempRT.anchorMax = Vector2.zero;
		tempGO.GetComponent <Image> ().color = colors [list];
		if (addToList) {points[list].Add (tempRT);}
	}
	
	void AddLine (int list) {
		tempGO = new GameObject ("Line " + list.ToString () +":" + lines[list].Count, typeof (Image));
		tempGO.transform.SetParent (pointContainer, false);
		tempGO.GetComponent <Image> ().color = colors [list];
		tempRT = tempGO.GetComponent <RectTransform> ();
		tempRT.sizeDelta = Vector2.zero;
		tempRT.anchorMin = Vector2.zero;
		tempRT.anchorMax = Vector2.zero;
		tempGO.GetComponent <Image> ().color = colors [list];
		lines [list].Add (tempRT);
	}
	
}
