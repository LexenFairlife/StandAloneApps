using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using UnityEngine;
using UnityEngine.UI;

public class TimeSeries3DChart : MonoBehaviour {
    
	[System.Serializable]
	public class Point {
		public DateTime     utc,local;
		public Series       series;
		public string       instrument;
		public float        value;
		public MeshRenderer mesh;
		public MeshRenderer lineMesh;
	}
	
	[System.Serializable]
	public class Series {
		public string instrument, units;
        public bool chartValue = true, displayValue = false;
		public float depth, pointSize, lineSize, mostRecentPoint;
        public int subValue = -1;
		public Material material;
	}
	
	
	//Data
	
	public	float			delay = 1, updateDelay = 60, displayedSeconds = 80, futureSeconds = 0, displayMaxY = 120, displayWidth = 1250, displayHeight = 800;
	
	public  List <Series>   series;
	public  List <Point>    points;
	public  MeshRenderer    lineMesh;
    public  List <Text>     displayText;
	
	//  Internal variabls
    float moveUnitsPerSecond = 1;
	List <MeshRenderer> usedPoints = new List <MeshRenderer> ();
	List <MeshRenderer> usedLines = new List <MeshRenderer> ();
	DateTime timeA, timeB;
	
	//  SQL Variables
	[TextArea(5,50)] public	string			populateQuery, updateQuery, timescaleQuery;
	SqlDataReader reader;
	private string connectionString = @"Data Source = 172.16.5.217; user id = cipapp;password = bestinclass&1;Connection Timeout=10;";
	SqlCommand cmd;
	SqlConnection dbCon;

	// temp variables
	int      i1, j1, j2, k1;
	Vector2  v2a, v2b;
	Vector3  v3;
	
	
    void Start() {
        moveUnitsPerSecond = 1100 / displayedSeconds;
		StartCoroutine (PopulatePoints ());
    }

    void Update() {
		delay -= Time.deltaTime;
		if (updateQuery.Length > 1) {
			for (i1 = 0; i1 < points.Count; i1++) {
				if (points [i1].mesh) {
					v3 = points [i1].mesh.transform.localPosition;
					v3.x -= moveUnitsPerSecond * Time.deltaTime;
					if (v3.x < -560) {
						usedPoints.Add (points [i1].mesh);
						points [i1].mesh.gameObject.SetActive (false);
						if (points [i1].lineMesh) {
							usedLines.Add (points [i1].lineMesh);
							points [i1].lineMesh.gameObject.SetActive (false);
						}
						points.RemoveAt (i1);
					} else {
						points [i1].mesh.transform.localPosition = v3;
						if (points [i1].lineMesh) {
							v3 = points [i1].lineMesh.transform.localPosition;
							v3.x -= moveUnitsPerSecond * Time.deltaTime;
							points [i1].lineMesh.transform.localPosition = v3;
						}
					}
				}
			}
		}
        if (delay <= 0) {
		    delay += updateDelay;
			if (updateQuery.Length > 0) {
				StartCoroutine (PullNewPoints ());
            } else if (populateQuery.Length > 0) {
                
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
								if (series [j1].instrument == reader.GetString (1)) {
									points [points.Count-1].series = series [j1];
									j1 = series.Count;
								}
							}
							if (points [points.Count-1].series != null) {
								points [points.Count-1].value = (float) reader.GetDouble (2);
								NewPointMesh ();
								v3.x = (1250 * ( (displayedSeconds-(float)(DateTime.UtcNow - points [points.Count-1].utc).TotalSeconds) / (displayedSeconds+futureSeconds) )) - 625;    //560 - (  ); //DateTime.UtcNow
								v3.y = ((points [points.Count-1].value / displayMaxY) * displayHeight) - (displayHeight/2);
								v3.z = points [points.Count-1].series.depth;
								points [points.Count-1].mesh.transform.localPosition = v3;
								points [points.Count-1].mesh.material = points [points.Count-1].series.material;
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
									NewLineMesh ();
								}
							} else {
								points.RemoveAt (points.Count-1);
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
            Debug.Log ("Timescale Query");
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
                                displayedSeconds = (float)(DateTime.UtcNow - reader.GetDateTime (0)).TotalSeconds;
                                futureSeconds = (float)(reader.GetDateTime (1) - DateTime.UtcNow).TotalSeconds;
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
            Debug.Log ("Populate Query");
            using (dbCon = new SqlConnection(connectionString)) {
                cmd = new SqlCommand(populateQuery, dbCon);
                try {
                    dbCon.Open();
                    reader = cmd.ExecuteReader();
                    if (reader.HasRows) {
                        while (points.Count > 0) {
                            usedPoints.Add (points [0].mesh);
                            //points [0].mesh.gameObject.SetActive (false);
                            if (points [0].lineMesh) {
                                usedLines.Add (points [0].lineMesh);
                                //points [0].lineMesh.gameObject.SetActive (false);
                            }
                            points.RemoveAt (0);
                        }
                        
                        while (reader.Read ()) {
							points.Add (new Point ());
							for (j1 = 0; j1 < series.Count; j1++) {
								if (series [j1].instrument == reader.GetString (1)) {
									points [points.Count-1].series = series [j1];
									j1 = series.Count;
								}
							}
							if (points [points.Count-1].series != null) {
								points [points.Count-1].value = (float) reader.GetDouble (2);
                                points [points.Count-1].series.mostRecentPoint = points [points.Count-1].value;
                                if (points [points.Count-1].series.chartValue) {
                                    points [points.Count-1].utc = reader.GetDateTime (0);
                                    NewPointMesh ();
                                    //v3.x = (1250 * ( (displayedSeconds-(float)(DateTime.UtcNow - points [points.Count-1].utc).TotalSeconds) / (displayedSeconds+futureSeconds) )) - 625;
                                    v3.x = (displayWidth * ( (displayedSeconds-(float)(DateTime.UtcNow - points [points.Count-1].utc).TotalSeconds) / (displayedSeconds+futureSeconds) )) + (displayWidth * 5);
                                    v3.y = ((points [points.Count-1].value / displayMaxY) * displayHeight) - (displayHeight/2);
                                    v3.z = points [points.Count-1].series.depth;
                                    points [points.Count-1].mesh.transform.localPosition = v3;
                                    points [points.Count-1].mesh.transform.localScale = Vector3.one * points [points.Count-1].series.pointSize;
                                    points [points.Count-1].mesh.material = points [points.Count-1].series.material;
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
                                    if (j2 > -1 && points [j2].series.lineSize > 0) {
                                        NewLineMesh ();
                                    }
                                } else {
                                   points.RemoveAt (points.Count-1); 
                                }
							} else {
								points.RemoveAt (points.Count-1);
							}
                        }
                        for (j1 = 0; j1 < usedPoints.Count; j1++) {
                            usedPoints [j1].gameObject.SetActive (false);
                        }
                        for (j1 = 0; j1 < usedLines.Count; j1++) {
                            usedLines [j1].gameObject.SetActive (false);
                        }
                    }
                } catch (SqlException exception) {
                    Debug.LogWarning(exception.ToString());
                }
            }
        }
        
        yield return new WaitForSeconds (0);
        
        j1 = 0;
        
        for (j2 = 0; j2 < series.Count; j2++) {
            if (series [j2].mostRecentPoint != -999 && series [j2].displayValue) {
                displayText [j1].text = series [j2].instrument + ": " + series [j2].mostRecentPoint.ToString ("#,##0.#") + series [j2].units;
                displayText [j1].color = series [j2].material.color;
                j1++;
            }
        }
        for (j2 = j1; j2 < displayText.Count; j2++) {
            displayText [j2].text = "";
        }
        
		yield return null;
	}

	
	void NewPointMesh () {
		if (usedPoints.Count > 0) {
			points [points.Count-1].mesh = usedPoints [0];
			usedPoints.RemoveAt (0);
		} else {
			points [points.Count-1].mesh = Instantiate (points [0].mesh);
			points [points.Count-1].mesh.transform.SetParent (points [0].mesh.transform.parent);
		}
		points [points.Count-1].mesh.gameObject.SetActive (true);
	}
	
	void NewLineMesh () {
		if (usedPoints.Count > 0) {
			points [j2].lineMesh = usedLines [0];
			usedLines.RemoveAt (0);
		} else {
			points [j2].lineMesh = Instantiate (lineMesh);
			points [j2].lineMesh.transform.SetParent (points [0].mesh.transform.parent);
		}
		points [j2].lineMesh.material = points [j2].series.material;
		
		v3.x = (points [points.Count-1].mesh.transform.localPosition.x + points [j2].mesh.transform.localPosition.x) / 2;
		v3.y = (points [points.Count-1].mesh.transform.localPosition.y + points [j2].mesh.transform.localPosition.y) / 2;
		v3.z = points [points.Count-1].series.depth;
		points [j2].lineMesh.transform.localPosition = v3;
		
		v3.x = points [j2].series.lineSize;
		v3.y = Vector3.Distance (points [points.Count-1].mesh.transform.localPosition,points [j2].mesh.transform.localPosition) / 2;
		v3.z = points [j2].series.lineSize;
		points [j2].lineMesh.transform.localScale = v3;
		
		
		v2a = points [j2].mesh.transform.localPosition;
		v2b = points [points.Count-1].mesh.transform.localPosition;
		
		v3 = Vector3.zero;
		v3.z = (Vector2.Angle(Vector2.right, v2a - v2b) * ((v2a.y < v2b.y)? -1.0f : 1.0f))+90; //Vector2.Angle (v2b,v2a);
		
		
		
		points [j2].lineMesh.transform.localEulerAngles = v3;
		
		points [j2].lineMesh.gameObject.SetActive (true);
	}
}
