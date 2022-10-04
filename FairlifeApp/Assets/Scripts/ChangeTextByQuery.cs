using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Data.SqlClient;

public class ChangeTextByQuery : MonoBehaviour {

	public Text   outputText;
	public string query;
	public float  delay;
	public float  elapsedDelay;

	SqlDataReader reader;
	private string connectionString = @"Data Source = 172.16.5.217; user id = cipapp;password = bestinclass&1;Connection Timeout=10;";
	
	void Update () {
		elapsedDelay += Time.deltaTime;
		if (elapsedDelay >= delay) {
			elapsedDelay -= delay;
			StartCoroutine(VisualUpdate());
		}
	}

	IEnumerator VisualUpdate () {
		yield return new WaitForSeconds(0);
		using (SqlConnection dbCon = new SqlConnection(connectionString)) {
			SqlCommand cmd = new SqlCommand(query, dbCon);
			try {
				dbCon.Open();
				reader = cmd.ExecuteReader();
				if (reader.HasRows) {
					while (reader.Read ()) {
						if (!reader.IsDBNull(0)) {
							outputText.text = (string)reader.GetString(0);
						} else {
							outputText.text = "-----";
						}
					}
				}
				reader.Close ();
			}
			catch (SqlException exception) {
				Debug.LogWarning(exception.ToString());
				outputText.text = "-----";
			}
		}
		yield return null;	
	}	
}