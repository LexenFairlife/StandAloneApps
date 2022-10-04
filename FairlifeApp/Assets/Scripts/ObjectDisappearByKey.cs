using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Data.SqlClient;

public class ObjectDisappearByKey : MonoBehaviour
{
	public  GameObject toggleObject;
	public  GameObject toggleObject1;
	public  string     query;
	        Text       methodText;
	public	float  	   delay;
	public	float  	   elapsedDelay;

	SqlDataReader reader;
	private string connectionString = @"Data Source = 172.16.5.217; user id = cipapp;password = bestinclass&1;Connection Timeout=10;";
	SqlConnection dbCon;
	SqlCommand cmd;

	void Update() {
			elapsedDelay += Time.deltaTime;
				if (elapsedDelay >= delay) {
					elapsedDelay -= delay;
					StartCoroutine(VisualUpdate());
				}
	}

	IEnumerator VisualUpdate () {
		using (SqlConnection dbCon = new SqlConnection(connectionString)) {
			SqlCommand cmd = new SqlCommand(query, dbCon);
			try {
				dbCon.Open();
				reader = cmd.ExecuteReader();
				if (reader.HasRows) {
					while (reader.Read ()) {
						if (!reader.IsDBNull(0)) {
							if ((string)reader.GetString(0) == "True") {
								toggleObject.SetActive(true);
								toggleObject1.SetActive(false);
								// Debug.Log("True");
							}	else {
								toggleObject.SetActive(false);
								toggleObject1.SetActive(true);
								// Debug.Log("False");
							}
						}
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
