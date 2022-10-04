using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Data.SqlClient;

public class ChangeColorByQuery : MonoBehaviour {

	public Image  backgroundPanel;
	public string query;
	public float  delay;
	public float  elapsedDelay;
	public List   <Color>	stateColors;
	public int	  stateColor;
	public string state;
	public List   <StateClass>  stateClasses;
	int i1;
	[Serializable]
	public class  StateClass {
		[SerializeField]
		public string state;
		[SerializeField]
		public int   stateID;
	}

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
		using (SqlConnection dbCon = new SqlConnection(connectionString)) {
			SqlCommand cmd = new SqlCommand(query, dbCon);
			try {
				dbCon.Open();
				reader = cmd.ExecuteReader();
				if (reader.HasRows) {
					while (reader.Read ()) {
						if (!reader.IsDBNull(0)) {
							//outputText.text = (string)reader.GetString(0);
							stateColor = 0;
							state = (string)reader.GetString(0);
							for (i1=0;i1<stateClasses.Count;i1++) {
								if (state == stateClasses[i1].state) {
									stateColor = i1;
									break;
								}
							}
							backgroundPanel.color = stateColors[stateClasses[stateColor].stateID];
						} else {
							//outputText.text = "-----";
						}
					}
				}
				reader.Close ();
			}
			catch (SqlException exception) {
				Debug.LogWarning(exception.ToString());
				//outputText.text = "-----";
			}
		}
		yield return null;	
	}	
}