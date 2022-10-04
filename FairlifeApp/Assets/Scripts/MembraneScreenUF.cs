using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using UnityEngine;
using UnityEngine.UI;

public class MembraneScreenUF : MonoBehaviour {
	
	public List <SmallGraph1> temperatureGraphs;
	public MembraneSchedule UfSchedule, DfSchedule, UnUfSchedule;

	Text currentTimeText;
	float frameDelay = 0;
	float	f1;
	int graphUpdateTimer = 2;
	int last6Timer = 3;
	//GameObject sqlIcon;
	
	
	SqlDataReader reader;
	private string connectionString = @"Data Source = 172.16.5.217; user id = cipapp;password = bestinclass&1;Connection Timeout=10;";
	string query;
	
	int j1;
	
	void Awake () {
		//currentTimeText = transform.Find ("Panel/Current Time").gameObject.GetComponent <Text> ();
		frameDelay = 1 - ((float)(DateTime.Now.Millisecond) / 1000);
		//sqlIcon	= transform.Find ("Panel/SQL Icon").gameObject;
		//sqlIcon.SetActive (true);
		//StartCoroutine (GraphPopulate ());
	}
	
	void FixedUpdate () {
		if (frameDelay > 0) {
			frameDelay -= Time.fixedDeltaTime;
			//return;
		}
		while (frameDelay < 0) {
			frameDelay += 1;
			//currentTimeText.text = DateTime.Now.ToString ("M/d HH:mm:ss");
			/*graphUpdateTimer--;
			if (graphUpdateTimer <= 0) {
				graphUpdateTimer = 10;
				//sqlIcon.SetActive (true);
				StartCoroutine (GraphUpdate ());
			}*/
			last6Timer--;
			if (last6Timer <= 0) {
				//sqlIcon.SetActive (true);
				last6Timer = 3600;
				StartCoroutine (Last6 ());
			}
		}
	}
	
	IEnumerator GraphPopulate () {
		yield return new WaitForSeconds (1);
		query = "exec [dbo].[spA-TestCode] @ID = 2";
		using (SqlConnection dbCon = new SqlConnection(connectionString)) {
			SqlCommand cmd = new SqlCommand(query, dbCon);
			try {
				dbCon.Open();
				reader = cmd.ExecuteReader();
				if (reader.HasRows) {
					while (reader.Read ()) {
						if (float.TryParse (reader [4].ToString (), out f1)) {	temperatureGraphs [0].values.Add (f1);	} else { Debug.Log ("Couldn't parse " + reader [4].ToString ()); }
						if (float.TryParse (reader [5].ToString (), out f1)) {	temperatureGraphs [1].values.Add (f1);	} else { Debug.Log ("Couldn't parse " + reader [5].ToString ()); }
						if (float.TryParse (reader [6].ToString (), out f1)) {	temperatureGraphs [2].values.Add (f1);	} else { Debug.Log ("Couldn't parse " + reader [6].ToString ()); }
						
						//temperatureGraphs 	[0].values.Add (float.Parse (reader [4].ToString ()));
						//temperatureGraphs 	[1].values.Add (float.Parse (reader [5].ToString ()));
						//temperatureGraphs 	[2].values.Add (float.Parse (reader [6].ToString ()));
					}
				}
				reader.Close ();
			}
			catch (SqlException exception) {
				Debug.LogWarning(exception.ToString());
			}
		}
		//sqlIcon.SetActive (false);
		yield return null;
	}
	
	IEnumerator GraphUpdate () {
		yield return new WaitForSeconds (0);
		query = "exec [dbo].[spA-TestCode] @ID = 1";
		using (SqlConnection dbCon = new SqlConnection(connectionString)) {
			SqlCommand cmd = new SqlCommand(query, dbCon);
			try {
				dbCon.Open();
				reader = cmd.ExecuteReader();
				if (reader.HasRows) {
					//j1 = 0;
					while (reader.Read ()) {
						temperatureGraphs 	[0].VisualUpdate (float.Parse (reader [4].ToString ()));
						temperatureGraphs 	[1].VisualUpdate (float.Parse (reader [5].ToString ()));
						temperatureGraphs 	[2].VisualUpdate (float.Parse (reader [6].ToString ()));
					}
				}
				reader.Close ();
			}
			catch (SqlException exception) {
				Debug.LogWarning(exception.ToString());
			}
		}
		//sqlIcon.SetActive (false);
		yield return null;
	}
	
	IEnumerator Last6 () {
		yield return new WaitForSeconds (0);
		UfSchedule.last6Gal.Clear ();
		query = "SELECT TOP (168) [Time],[Gal] FROM [FairlifeDashboard].[dbo].[Liquid_Inventory - Rev01] where [ID] = 11145 and [Time] > dateAdd (hour, -170, current_timestamp) order by [Time]";
		using (SqlConnection dbCon = new SqlConnection(connectionString)) {
			SqlCommand cmd = new SqlCommand(query, dbCon);
			try {
				dbCon.Open();
				reader = cmd.ExecuteReader();
				if (reader.HasRows) {
					j1 = 0;
					while (reader.Read ()) {
						UfSchedule.last6Gal.Add (float.Parse (reader [1].ToString ()));
						j1++;
					}
				}
				reader.Close ();
			}
			catch (SqlException exception) {
				Debug.LogWarning(exception.ToString());
			}
		}
		yield return new WaitForSeconds (0);
		UfSchedule.Last6Points ();
		DfSchedule.last6Gal.Clear ();
		query = "SELECT TOP (168) [Time],[Gal] FROM [FairlifeDashboard].[dbo].[Liquid_Inventory - Rev01] where [ID] = 11147 and [Time] > dateAdd (hour, -170, current_timestamp) order by [Time]";
		using (SqlConnection dbCon = new SqlConnection(connectionString)) {
			SqlCommand cmd = new SqlCommand(query, dbCon);
			try {
				dbCon.Open();
				reader = cmd.ExecuteReader();
				if (reader.HasRows) {
					j1 = 0;
					while (reader.Read ()) {
						DfSchedule.last6Gal.Add (float.Parse (reader [1].ToString ()));
						j1++;
					}
				}
				reader.Close ();
			}
			catch (SqlException exception) {
				Debug.LogWarning(exception.ToString());
			}
		}
		yield return new WaitForSeconds (0);
		DfSchedule.Last6Points ();
		UnUfSchedule.last6Gal.Clear ();
		query = "SELECT TOP (168) [Time],[Gal] FROM [FairlifeDashboard].[dbo].[Liquid_Inventory - Rev01] where [ID] = 10018 and [Time] > dateAdd (hour, -170, current_timestamp) order by [Time]";
		using (SqlConnection dbCon = new SqlConnection(connectionString)) {
			SqlCommand cmd = new SqlCommand(query, dbCon);
			try {
				dbCon.Open();
				reader = cmd.ExecuteReader();
				if (reader.HasRows) {
					j1 = 0;
					while (reader.Read ()) {
						UnUfSchedule.last6Gal.Add (float.Parse (reader [1].ToString ()));
						j1++;
					}
				}
				reader.Close ();
			}
			catch (SqlException exception) {
				Debug.LogWarning(exception.ToString());
			}
		}
		yield return new WaitForSeconds (0);
		UnUfSchedule.Last6Points ();
		//sqlIcon.SetActive (false);
		yield return null;
	}
}
