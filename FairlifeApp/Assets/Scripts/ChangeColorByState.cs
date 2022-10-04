using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Data.SqlClient;

public class ChangeColorByState : MonoBehaviour {

	public	Image  					backgroundPanel;
	public	string 					query;
	public	float  					delay;
	public	float  					elapsedDelay;
	public	List   	<Color>			stateColors;
		  	int	  					stateColor;
			string 					state;
	public  Text  					stateText;
	public  List   	<StateClass>	states;
			int 					i1;

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
							//outputText = (string)reader.GetString(0);
							stateColor = 0;
							state = (string)reader.GetString(0);
							if (stateText) {								
								if (state == "Execute") {								//Change state text from Execute to Production
									stateText.text = "Production"; 
								} else if (state == "Blocked") {
									stateText.text = "Downstream";							
								} else if (state == "Unplanned Downtime") {
									stateText.text ="Down";
								} else if (state == "Undefined"){
									stateText.text = "";
								} else {
									stateText.text = state;
								}
							} 

							for (i1=0;i1<states.Count;i1++) {
								if (state == states[i1].state) {
									stateColor = i1;
									break;								
								}
							}
							backgroundPanel.color = stateColors[states[stateColor].stateID];
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