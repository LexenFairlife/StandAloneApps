using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Data.SqlClient;

public class PullRealTimeValueFromInput : MonoBehaviour	{
    
	public float currentDelay = 1, pullDelay = 5;
	public InputField inputField;
	public Text outputText;
	
	SqlDataReader reader;
	string query;
	private string connectionString = @"Data Source = 172.16.5.217; user id = cipapp;password = bestinclass&1;Connection Timeout=10;";
	
	void Update () {
		currentDelay -= Time.deltaTime;
		if (currentDelay <= 0) {
			currentDelay += pullDelay;
			StartCoroutine (PullUpdate ());
		}
	}
	
	IEnumerator PullUpdate () {
		if 	(inputField.text.Length > 1) 			{
			query = "exec [FairlifeDashboard].[dbo].[spPullCurrentHistorianValue] '"+ inputField.text +"'";
			using (SqlConnection dbCon = new SqlConnection(connectionString)) {
				SqlCommand cmd = new SqlCommand(query, dbCon);
				try {
					dbCon.Open();
					reader = cmd.ExecuteReader();
					if (reader.HasRows) {
						while (reader.Read ()) {
							if (!reader.IsDBNull(0)) {
								outputText.text = ((float)reader.GetDouble(0)).ToString("###,###,###,##0.##");
							}
						}
						/*
						if (!reader.IsDBNull(0)) {
							outputText.text = (string)reader.GetString(0);
						}*/
						/*while (reader.Read ()) {
							if (!reader.IsDBNull(0)) {
								values.Add ((float)reader.GetDouble(0));
							} else {
								if (values.Count > 0) {
									values.Add (values [values.Count-1]);
								} else {
									values.Add (0);
								}
							}
						}*/
					}
					reader.Close ();
				}
				catch (SqlException exception) {
					Debug.LogWarning(exception.ToString());
				}
			}
		
		} else {
			outputText.text = "-----";
		}
		yield return null;
	}
	
}
