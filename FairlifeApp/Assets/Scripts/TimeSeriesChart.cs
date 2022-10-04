using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using UnityEngine;
using UnityEngine.UI;

public class TimeSeriesChart : MonoBehaviour {
    
	[System.Serializable]
	public class Point {
        public string           name;
		public DateTime         utc,local;
		public Series           series;
        public int              seriesID = -1;
		public float            value;
		public MeshRenderer     mesh;
		public RectTransform    line;
        public RectTransform    rt;
        public Vector2          pos;
	}
	
	[System.Serializable]
	public class Series {
		public string name, displayName, units;
        public bool chartValue = true, displayValue = false;
		public float pointSize, lineSize, mostRecentPoint;
        public int subValue = -1, maxListID = -1;
        public Color color;
	}
	
	public	float			delay = 1, updateDelay = 60, displayedSeconds = 80, minValue = 0, maxValue = 120, displayWidth = 1250, displayHeight = 800, minXDistance = 1;
	
	public  List <Series>   series;
	public  List <Point>    points = new List <Point> ();
    public  List <Text>     displayText;
    public  List <string>   maxTextPreface, maxTextPostface;
    public  List <Text>     maxText;
    
    public  int pointCount, unusedLineCount;
	
	//  Internal variabls
    float moveUnitsPerSecond = 1;
	List <RectTransform> usedPoints = new List <RectTransform> ();
	List <RectTransform> usedLines  = new List <RectTransform> ();
	DateTime timeA, timeB;
	
	//  SQL Variables
	[TextArea(5,50)] public	string			populateQuery, updateQuery, timescaleQuery;
	SqlDataReader reader;
	private string connectionString = @"Data Source = 172.16.5.217; user id = cipapp;password = bestinclass&1;Connection Timeout=10;";
	SqlCommand cmd;
	SqlConnection dbCon;

	// temp variables
    List <float>    fList           = new List <float> {0,0,0,0,0,0,0,0,0,0};
    List <int>      iList           = new List <int>   {0,0,0,0,0,0,0,0,0,0};
	int             i1, j1, j2, k1;
    GameObject      tempGO;
    RectTransform   tempRT;
	Vector2         v2, v2a, v2b;
	Vector3         v3;
    
    //New Values
	List <List<RectTransform>>	lines = new List <List<RectTransform>> {};
	
	
    void Start() {
        moveUnitsPerSecond = 1100 / displayedSeconds;
		StartCoroutine (PopulatePoints ());
    }

    void Update() {
               
        pointCount = points.Count;
        unusedLineCount = usedLines.Count;
        
		delay -= Time.deltaTime;
/*		if (updateQuery.Length > 1) {
			for (i1 = 0; i1 < points.Count; i1++) {
				if (points [i1].mesh) {
					v3 = points [i1].mesh.transform.localPosition;
					v3.x -= moveUnitsPerSecond * Time.deltaTime;
					if (v3.x < -560) {
                        usedPoints.Add (points [i1].mesh);
						points [i1].mesh.gameObject.SetActive (false);
						if (points [i1].line) {
							usedLines.Add (points [i1].line);
							points [i1].line.gameObject.SetActive (false);
						}
						points.RemoveAt (i1);
					} else {
						points [i1].mesh.transform.localPosition = v3;
						if (points [i1].line) {
							v3 = points [i1].line.transform.localPosition;
							v3.x -= moveUnitsPerSecond * Time.deltaTime;
							points [i1].line.transform.localPosition = v3;
						}
					}
				}
			}
		}*/
        if (delay <= 0) {
		    delay += updateDelay;
			if (updateQuery.Length > 0) {
				StartCoroutine (PullNewPoints ());
            } else if (populateQuery.Length > 0) {
                StartCoroutine (PopulatePoints ());
			} else {
				
			}
		}
    }
	
	IEnumerator PullNewPoints () {
		using (dbCon = new SqlConnection(connectionString)) {
			cmd = new SqlCommand(updateQuery, dbCon);
			try {
				dbCon.Open();
				reader = cmd.ExecuteReader();
				k1 = 0;
				if (reader.HasRows) {
					while (reader.Read ()) {
						if (!reader.IsDBNull(0) && !reader.IsDBNull(1) && !reader.IsDBNull(2)) {
							points.Add (new Point ());
							points [points.Count-1].utc = reader.GetDateTime (0);
							for (j1 = 0; j1 < series.Count; j1++) {
								if (series [j1].name == reader.GetString (1)) {
									points [points.Count-1].series = series [j1];
                                    points [points.Count-1].seriesID = j1;
									j1 = series.Count;
								}
							}
							if (points [points.Count-1].series != null) {
								points [points.Count-1].value = (float) reader.GetDouble (2);
                                if (points.Count >= 3 && points [points.Count-1].series.name == points [points.Count-2].series.name && points [points.Count-1].series.name == points [points.Count-3].series.name && points [points.Count-1].value == points [points.Count-2].value && points [points.Count-1].value == points [points.Count-3].value) {
                                    RemovePoint (points.Count-2);
                                }
                                AddPoint ();
								v3.x = (1250 * ( (displayedSeconds-(float)(DateTime.UtcNow - points [points.Count-1].utc).TotalSeconds) / (displayedSeconds) )) - 625;
								v3.y = ((Mathf.Clamp(points [points.Count-1].value,minValue,maxValue) / maxValue) * displayHeight) - (displayHeight/2);
								points [points.Count-1].rt.transform.localPosition = v3;
								j2 = -1;
								for (j1 = 0; j1 < points.Count-1; j1++) {
									if (points [j1].series == points [points.Count-1].series) {
										if (j2 == -1) {
											j2 = j1;
										} else {
											if (points [j1].utc > points [j2].utc) {
												j2 = j1;
											}
										}
									}
								}
								if (j2 > -1) {
//									NewLineMesh ();
								}
							} else {
								//points.RemoveAt (points.Count-1);
                                RemovePoint (points.Count-1);
							}
							k1++;
						}
					}
				}
			} catch (SqlException exception) {
				Debug.LogWarning(exception.ToString());
			}
		}
		yield return null;
	}
	
	IEnumerator PopulatePoints () {
        if (timescaleQuery.Length > 0) {
            using (dbCon = new SqlConnection(connectionString)) {
                cmd = new SqlCommand(timescaleQuery, dbCon);
                try {
                    dbCon.Open();
                    reader = cmd.ExecuteReader();
                    if (reader.HasRows) {
                        while (reader.Read ()) {
                            if (!reader.IsDBNull(0) && !reader.IsDBNull(1)) {
                                timeA = reader.GetDateTime (0);
                                timeB = reader.GetDateTime (1);
                                displayedSeconds = (float)(reader.GetDateTime (1) - reader.GetDateTime (0)).TotalSeconds;
                            }
                        }
                    }
                } catch (SqlException exception) {
                    Debug.LogWarning(exception.ToString());
                }
            }
            moveUnitsPerSecond = 1100 / displayedSeconds;
        }
        
        yield return new WaitForSeconds (0);
        
        for (j1 = 0; j1 < series.Count; j1++) {
            series [j1].mostRecentPoint = -999;
        }

        if (populateQuery.Length > 0) {
            using (dbCon = new SqlConnection(connectionString)) {
                cmd = new SqlCommand(populateQuery, dbCon);
                try {
                    dbCon.Open();
                    reader = cmd.ExecuteReader();
                    if (reader.HasRows) {
                        while (points.Count > 0) {
                            RemovePoint (0);
                            /*if (points [0].rt) {
                                usedPoints.Add (points [0].rt);
                                // points [0].rt.gameObject.SetActive (false); 
                            }
                            if (points [0].line) {
                                usedLines.Add (points [0].line);
                                // points [0].line.gameObject.SetActive (false);
                            }
                            points.RemoveAt (0);*/
                        }
                        
                        while (reader.Read ()) {
							points.Add (new Point ());
							for (j1 = 0; j1 < series.Count; j1++) {
								if (series [j1].name == reader.GetString (1)) {
									points [points.Count-1].series = series [j1];
									j1 = series.Count;
								}
							}
							if (points [points.Count-1].series != null) {
								points [points.Count-1].value = (float) reader.GetDouble (2);
                                if (points.Count >= 3 && points [points.Count-1].series.name == points [points.Count-2].series.name && points [points.Count-1].series.name == points [points.Count-3].series.name && points [points.Count-1].value == points [points.Count-2].value && points [points.Count-1].value == points [points.Count-3].value) {
                                    RemovePoint (points.Count-2);
                                }
                                points [points.Count-1].series.mostRecentPoint = points [points.Count-1].value;
                                if (points [points.Count-1].series.chartValue) {
                                    points [points.Count-1].utc = reader.GetDateTime (0);
                                    
                                    v2.x = LerpFloat (0, displayWidth,  (float)(points [points.Count-1].utc - timeA).TotalSeconds / displayedSeconds);
                                    v2.y = LerpFloat (0, displayHeight, 1-((maxValue - Mathf.Clamp(points [points.Count-1].value,minValue,maxValue)) / maxValue));
                                    points [points.Count-1].pos = v2;
                                    if (points.Count > 2 && points [points.Count-2].series.name == points [points.Count-1].series.name && Mathf.Abs(points [points.Count-1].pos.x - points [points.Count-2].pos.x) < minXDistance) {
                                        RemovePoint (points.Count-1);
                                    } else if (points [points.Count-1].series.pointSize > 0) {
                                        AddPoint ();
                                        points [points.Count-1].rt.anchoredPosition = v2;
                                    }
                                } else {
                                   RemovePoint (points.Count-1);
                                   //points.RemoveAt (points.Count-1); 
                                }
							} else {
                                RemovePoint (points.Count-1);
								//points.RemoveAt (points.Count-1);
							}
                        }
                    }
                } catch (SqlException exception) {
                    Debug.LogWarning(exception.ToString());
                }
            }
        }

        yield return new WaitForSeconds (0);
        
        v3 = Vector3.zero;
        
        for (j1 = 1; j1 < points.Count; j1++) {
            if (points [j1].series.lineSize > 0 && points [j1-1].series.name == points [j1].series.name) {
                v2.x = (points [j1-1].pos.x + points [j1].pos.x) / 2;
                v2.y = (points [j1-1].pos.y + points [j1].pos.y) / 2;
                AddLine ();
                points [j1].line.anchoredPosition = v2;
                v2 = points [j1].line.sizeDelta;
                v2.x = (Vector2.Distance (points [j1-1].pos, points [j1].pos)) + (points [j1].series.lineSize / 2);
                points [j1].line.sizeDelta = v2;
                v2 = (points [j1-1].pos - points [j1].pos).normalized;
                if (v2.x == 0) {
                    v3.z = 0;
                } else {
                    v3.z = Mathf.Atan (v2.y / v2.x) * 180 / Mathf.PI;
                }
                points [j1].line.localEulerAngles = v3;
            }
        }
        
        for (j1 = 0; j1 < fList.Count; j1++) {
            fList [j1] = 0;
            iList [j1] = -1;
        }
        
        j1 = 0;
        
        for (j2 = 0; j2 < series.Count; j2++) {
            if (series [j2].mostRecentPoint != -999) {
                if (series [j2].displayValue && displayText.Count > j1 && displayText [j1]) {
                    displayText [j1].text = series [j2].displayName + ": " + series [j2].mostRecentPoint.ToString ("#,##0.#") + series [j2].units;
                    displayText [j1].color = series [j2].color;
                    j1++;
                }
                if (series [j2].maxListID >= 0) {
                    if (series [j2].mostRecentPoint > fList [series [j2].maxListID]) {
                        fList [series [j2].maxListID] = series [j2].mostRecentPoint;
                        iList [series [j2].maxListID] = j2;
                    }
                }
            }
        }
        
        for (j2 = j1; j2 < displayText.Count; j2++) {
            displayText [j2].text = "";
        }
        
        for (j1 = 0; j1 < maxText.Count; j1++) {
            maxText [j1].text = "";
        }
        
        for (j2 = 0; j2 < iList.Count; j2++) {
            if (iList [j2] >= 0) {
                if (series [iList [j2]].displayName.Length == 0) {
                    maxText [j2].text = maxTextPreface [j2] + series [iList [j2]].mostRecentPoint.ToString ("#0.#") + maxTextPostface [j2];
                } else {
                    maxText [j2].text = maxTextPreface [j2] + series [iList [j2]].displayName + ": " + series [iList [j2]].mostRecentPoint.ToString ("#0.#") + maxTextPostface [j2];
                }
                maxText [j2].color = series [iList [j2]].color;
            }
        }
        
        yield return new WaitForSeconds (0);
        
        for (j1 = 0; j1 < usedPoints.Count; j1++) {
            usedPoints [j1].gameObject.SetActive (false);
        }
        for (j1 = 0; j1 < usedLines.Count; j1++) {
            Destroy (usedLines [j1].gameObject);
            //usedLines [j1].gameObject.SetActive (false);
        }
        usedLines.Clear ();
        
		yield return null;
	}

	void AddPoint () {
		if (usedPoints.Count > 0) {
			points [points.Count-1].rt = usedPoints [0];
			usedPoints.RemoveAt (0);
            points [points.Count-1].rt.gameObject.SetActive (true);
		} else {
			points [points.Count-1].rt = new GameObject (points [points.Count-1].series.name + " Point", typeof (Image)).GetComponent <RectTransform> ();
			points [points.Count-1].rt.transform.SetParent (transform);
		}
		points [points.Count-1].rt.sizeDelta = Vector2.one * points [points.Count-1].series.pointSize;		
		points [points.Count-1].rt.anchorMin = Vector2.zero;
		points [points.Count-1].rt.anchorMax = Vector2.zero;
		points [points.Count-1].rt.gameObject.GetComponent <Image> ().color = points [points.Count-1].series.color;
	}
    
    void RemovePoint (int i) {
        if (points [i].line) {
            //Destroy (points [i].line.gameObject);
            usedLines.Add (points [i].line);
            //points [i].line.gameObject.SetActive (false);
        }
        
        
        if (points [i].rt) {
           usedPoints.Add (points [i].rt); 
           //points [i].rt.gameObject.SetActive (false);
        }

        points.RemoveAt (i);
    }
    
	void AddLine () {
		if (usedLines.Count > 0) {
			points [j1].line = usedLines [0];
			usedLines.RemoveAt (0);
            points [j1].line.gameObject.SetActive (true);
		} else {
			points [j1].line = new GameObject (points [j1].series.name + " Line", typeof (Image)).GetComponent <RectTransform> ();
			points [j1].line.transform.SetParent (transform);
		}
		points [j1].line.sizeDelta = Vector2.one * points [j1].series.lineSize;		
		points [j1].line.anchorMin = Vector2.zero;
		points [j1].line.anchorMax = Vector2.zero;
		points [j1].line.gameObject.GetComponent <Image> ().color = points [j1].series.color;
	}
    
    float LerpFloat (float a, float b, float f) {
       return a + ( ( b - a) * f); 
    }
}
