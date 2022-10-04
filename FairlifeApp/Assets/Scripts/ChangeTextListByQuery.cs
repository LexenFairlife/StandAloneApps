using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using System.Data.SqlClient;

public class ChangeTextListByQuery : MonoBehaviour {

	public List<Text>   outputText;
	[TextArea(10,50)]public string query;
	public int  refreshDelay;
	public float  currentDelay;
	[TextArea(5,50)]public string reportBackQuery;
	[TextArea(5,50)]public string reportBackFailureQueryPart1,reportBackFailureQueryPart2;
	
	bool	updated = false;
	string	errorMessage;
	int	i1;

	SqlCommand cmd;
	SqlDataReader reader;
	private string connectionString = @"Data Source = 172.16.5.217; user id = cipapp;password = bestinclass&1;Connection Timeout=10;";
	
    public float completeResetTimer = -999; //Anything less than -100 disables this feature
	
	
	void Start () {
        if (refreshDelay == 0) {
            refreshDelay = 60;
        }
		if ((refreshDelay == 30 || refreshDelay == 60) && currentDelay > refreshDelay) {
			currentDelay = currentDelay % refreshDelay;
		} else if ((refreshDelay == 30 || refreshDelay == 60) && currentDelay < 0) {
			currentDelay = 15;
		}
	}
	
	void Update () {
        if (completeResetTimer > -100) {
            completeResetTimer -= Time.deltaTime;
            if (completeResetTimer <= 0) {
                UnityEngine.SceneManagement.SceneManager.LoadScene(0);
            }
        }
        
		if (refreshDelay == 30 || refreshDelay == 60) {
			if (System.DateTime.Now.Second % refreshDelay == currentDelay) {
				if (!updated) {
					updated = true;
					StartCoroutine (VisualUpdate ());
				}
			} else if (updated) {
				updated = false;
			}
		} else {
			currentDelay -= Time.deltaTime;
			if (currentDelay <= 0) {
				currentDelay += refreshDelay;
				StartCoroutine(VisualUpdate());
			}
		}
	}

	IEnumerator VisualUpdate () {
		errorMessage = "";
		using (SqlConnection dbCon = new SqlConnection(connectionString)) {
			cmd = new SqlCommand(query, dbCon);
			try {
				dbCon.Open();
				reader = cmd.ExecuteReader();
				if (reader.HasRows) {
					i1 = 0;
					while (reader.Read ()) {
						if (!reader.IsDBNull(0)) {
							outputText [i1].text = (string)reader.GetString(0);
						} else {
							outputText [i1].text = "-----";
						}
						i1++;
					}
				}
				reader.Close ();
			}
			catch (SqlException exception) {
				errorMessage += exception.ToString();
				Debug.LogWarning(errorMessage);
				//outputText [i1].text = "-----";
			}
		}
		if (reportBackQuery.Length > 10) {
			using (SqlConnection dbCon = new SqlConnection(connectionString)) {
				cmd = new SqlCommand(reportBackQuery, dbCon);
				try {
					dbCon.Open();
					reader = cmd.ExecuteReader();
					reader.Close ();
				}
				catch (SqlException exception) {
					Debug.LogWarning(exception.ToString());
				}
			}
		}
		if (reportBackFailureQueryPart1.Length > 10) {
			using (SqlConnection dbCon = new SqlConnection(connectionString)) {
				cmd = new SqlCommand(reportBackFailureQueryPart1 + errorMessage + reportBackFailureQueryPart2, dbCon);
				try {
					dbCon.Open();
					reader = cmd.ExecuteReader();
					reader.Close ();
				}
				catch (SqlException exception) {
					Debug.LogWarning(exception.ToString());
				}
			}
		}
		yield return null;	
	}	
}