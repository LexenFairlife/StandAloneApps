using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using UnityEngine;
using UnityEngine.UI;

public class Gant : MonoBehaviour {
    
	[System.Serializable]
	public class Event {
        public string       display;
		public DateTime     start,end;
        public int          state;
	}

	[System.Serializable]
	public class EventTypes {
		public string name;
        public bool   enabled;
        public Color  color;
	}
    
    [System.Serializable]
	public class Hour {
		public DateTime start, end;
	}
    
    [TextArea(5,50)] public	string query;
	SqlDataReader reader;
	private string connectionString = @"Data Source = 172.16.5.217; user id = cipapp;password = bestinclass&1;Connection Timeout=10;";
	SqlCommand cmd;
	SqlConnection dbCon;
    
    float delay = 1;
    float width = 1150;
    public List <RectTransform> eventRects, hourRects;
    
    DateTime minTime, maxTime;
    List <DateTime> hours = new List <DateTime> ();
    List <Hour> newHours = new List <Hour> ();
    public int totalMinutes;
    public List <Event> events;
    public List <EventTypes> eventTypes;
    
    Vector2 v2 = Vector2.zero;

    void Awake () {
        width = gameObject.GetComponent <RectTransform> ().sizeDelta.x;
    }

    void Update () {
        delay -= Time.deltaTime;
        if (delay <= 0) {
            delay = 120;
            StartCoroutine (PullData ());
        }
    }

	IEnumerator PullData () {
		using (dbCon = new SqlConnection(connectionString)) {
			cmd = new SqlCommand(query, dbCon);
			try {
				dbCon.Open();
				reader = cmd.ExecuteReader();
                events.Clear ();
                minTime = DateTime.UtcNow;
                maxTime = DateTime.UtcNow;
                int state = -1;
				if (reader.HasRows) {
					while (reader.Read ()) {
						if (!reader.IsDBNull(0) && !reader.IsDBNull(1)) {
                            if (eventTypes.Count > reader.GetInt32 (1) && eventTypes [reader.GetInt32 (1)].enabled) {
                                if (state == -1 || state != reader.GetInt32 (1)) {
                                    state = reader.GetInt32 (1);
                                    events.Add (new Event ());
                                    events [events.Count-1].state = state;
                                    events [events.Count-1].start = reader.GetDateTime (0);
                                    events [events.Count-1].display = reader.GetDateTime (0).ToLocalTime().ToString ("HH:mm");
                                }
                                events [events.Count-1].end = reader.GetDateTime (0).AddMinutes (1);
                            }
                            
                            if (minTime > reader.GetDateTime (0)) {
                                minTime = reader.GetDateTime (0);
                            }
                            if (maxTime < reader.GetDateTime (0).AddMinutes (1)) {
                                maxTime = reader.GetDateTime (0).AddMinutes (1);
                            }
						}
					}
				}
			} catch (SqlException exception) {
				Debug.LogWarning(exception.ToString());
			}
		}
        
        yield return new WaitForSeconds (0);
        
        totalMinutes = (int)(maxTime - minTime).TotalMinutes;
        
        int i1;
        
        hours.Clear ();
        newHours.Clear ();
        
        DateTime tempTime = minTime.ToLocalTime();
        
        for (i1 = 0; i1 <= totalMinutes; i1++) {
            if (tempTime.Hour % 2 == 0) {
                if (tempTime.Minute == 0 || tempTime == minTime.ToLocalTime()) {
                    hours.Add (tempTime);
                    
                    newHours.Add (new Hour ());
                    newHours [newHours.Count-1].start = tempTime;
                    newHours [newHours.Count-1].end   = tempTime.AddMinutes (60 - newHours [newHours.Count-1].start.Minute);
                }
                if (i1 >= totalMinutes) {
                    newHours [newHours.Count-1].end   = tempTime;
                }
            }
            tempTime = tempTime.AddMinutes(1);
        }
        
        yield return new WaitForSeconds (0);
        
        for (i1 = 0; i1 < eventRects.Count; i1++) {
            if (events.Count == 0) {
                eventRects [0].gameObject.SetActive (false);
            } else if (eventRects.Count > events.Count) {
                Destroy (eventRects [eventRects.Count-1].gameObject);
                eventRects.RemoveAt (eventRects.Count-1);
            }
        }
        
        for (i1 = 0; i1 < events.Count; i1++) {
            if (events.Count > eventRects.Count) {
                eventRects [eventRects.Count-1].gameObject.SetActive (true);
                eventRects.Add (Instantiate (eventRects [eventRects.Count-1].gameObject).GetComponent <RectTransform> ());
                eventRects [eventRects.Count-1].transform.SetParent (eventRects [eventRects.Count-2].transform.parent);
                eventRects [eventRects.Count-1].sizeDelta = eventRects [eventRects.Count-2].sizeDelta;
                eventRects [eventRects.Count-1].anchoredPosition = eventRects [eventRects.Count-2].anchoredPosition;
                eventRects [eventRects.Count-1].transform.localScale = eventRects [eventRects.Count-2].transform.localScale;
                eventRects [eventRects.Count-1].transform.localPosition = eventRects [eventRects.Count-2].transform.localPosition;
            }
            v2 = eventRects [i1].sizeDelta;
            v2.x = (float)(events [i1].end - events [i1].start).TotalMinutes / totalMinutes * width;
            eventRects [i1].sizeDelta = v2;
            if (v2.x < 40) {
                eventRects [i1].transform.Find ("Text").gameObject.GetComponent <Text> ().text = "";
            } else {
                eventRects [i1].transform.Find ("Text").gameObject.GetComponent <Text> ().text = events [i1].display;
            }
            v2 = eventRects [i1].anchoredPosition;
            v2.x = (float)((events [i1].start.AddSeconds ((float)(events [i1].end - events [i1].start).TotalMinutes * 30)) - minTime).TotalSeconds / (totalMinutes * 60) * width; //I multiplied by 30 as I wanted to add half a minute for each minute of runtime
            eventRects [i1].anchoredPosition = v2;
            eventRects [i1].gameObject.GetComponent <Image> ().color = eventTypes [events [i1].state].color;
            eventRects [i1].name = "Event " + i1.ToString ("00") + ": " + eventTypes [events [i1].state].name;
            
            yield return new WaitForSeconds (0);
        }
        
        float f1 = 60f / totalMinutes * width;
        
        for (i1 = 0; i1 < newHours.Count; i1++) {
            if (newHours.Count > hourRects.Count) {
                hourRects [hourRects.Count-1].gameObject.SetActive (true);
                hourRects.Add (Instantiate (hourRects [hourRects.Count-1].gameObject).GetComponent <RectTransform> ());
                hourRects [hourRects.Count-1].transform.SetParent (hourRects [hourRects.Count-2].transform.parent);
                hourRects [hourRects.Count-1].sizeDelta = hourRects [hourRects.Count-2].sizeDelta;
                hourRects [hourRects.Count-1].anchoredPosition = hourRects [hourRects.Count-2].anchoredPosition;
                hourRects [hourRects.Count-1].transform.localScale = hourRects [hourRects.Count-2].transform.localScale;
                hourRects [hourRects.Count-1].transform.localPosition = hourRects [hourRects.Count-2].transform.localPosition;
            }
            v2 = hourRects [i1].sizeDelta;
            v2.x = ((float)(newHours [i1].end - newHours [i1].start).TotalMinutes) / totalMinutes * width;
            hourRects [i1].sizeDelta = v2;
            if (v2.x < 40) {
                hourRects [i1].transform.Find ("Text").gameObject.GetComponent <Text> ().text = "";
            } else {
                hourRects [i1].transform.Find ("Text").gameObject.GetComponent <Text> ().text = newHours [i1].start.ToString ("HH");
            }
            v2 = hourRects [i1].anchoredPosition;
            v2.x = (float)(newHours [i1].start.AddMinutes ((newHours [i1].end - newHours [i1].start).TotalMinutes / 2) - minTime.ToLocalTime()).TotalSeconds / (totalMinutes * 60) * width; //I multiplied by 30 as I wanted to add half a minute for each minute of runtime
            hourRects [i1].anchoredPosition = v2;
            
            //hourRects [i1].name = "Hour " + i1.ToString ("00") + ": " + eventTypes [events [i1].state].name;
            yield return new WaitForSeconds (0);
        }
		yield return null;
	}
}
