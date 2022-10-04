using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Data.SqlClient;

public class CountdownTimerByQuery : MonoBehaviour
{

    public  Text	 outputText; 
    	    float    seconds;
    public  string   query;
            float    updateTimer;

    	   	SqlDataReader reader;
	private string connectionString = @"Data Source = 172.16.5.217; user id = cipapp;password = bestinclass&1;Connection Timeout=10;";

	SqlCommand Countdown; // your date from the db.


    void Start()
    {

         StartCoroutine(VisualUpdate());
    }

    void Update()
    {
        if (updateTimer <= 0 ) { 
            updateTimer += 300; 
            StartCoroutine(VisualUpdate()); 
        }

        seconds -= Time.deltaTime;

        outputText.text = (seconds < 0 ? "-" : "") + Mathf.Abs(Mathf.Floor(seconds/3600)).ToString("00") + ":" + (Mathf.Abs(Mathf.Floor(seconds/60)%60)).ToString("00") + ":" + (Mathf.Abs(seconds%60)).ToString("00");
    }

	IEnumerator VisualUpdate () {
			using (SqlConnection dbCon = new SqlConnection(connectionString)) {
				SqlCommand cmd = new SqlCommand(query, dbCon);
				try {
					dbCon.Open();
					reader = cmd.ExecuteReader();
					if (reader.HasRows) {
						while (reader.Read ()) {
							seconds = (float)reader.GetDouble(0);
						}
					}
					reader.Close ();
				}
				catch (SqlException exception) {
					Debug.LogWarning(exception.ToString());
				}
			}
			yield return null;	
		}
    


}
