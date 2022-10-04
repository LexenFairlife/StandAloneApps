using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Data.SqlClient;

public class ChangeColorListByState : MonoBehaviour {

	public	List<Image>				backgroundPanels;
	[TextArea(10,50)]public	string					query;
	public	int  					refreshDelay;
	public	float					currentDelay;
	public	List   	<Color>			stateColors;
		  	int	  					stateColor;
			string 					state;
	public  List<Text>				text1,text2;
	//public  List   	<StateClass>	states;
			int 					i1, i2;
			bool					updated = false;

	/*[Serializable]
	public class  StateClass {
		[SerializeField]
		public string state;
		[SerializeField]
		public int   stateID;
	}*/

	SqlDataReader reader;
	private string connectionString = @"Data Source = 172.16.5.217; user id = cipapp;password = bestinclass&1;Connection Timeout=10;";
	SqlConnection dbCon;
	SqlCommand cmd;
	
	
	void Start () {
		if ((refreshDelay == 30 || refreshDelay == 60) && currentDelay > refreshDelay) {
			currentDelay = currentDelay % refreshDelay;
		} else if ((refreshDelay == 30 || refreshDelay == 60) && currentDelay < 0) {
			currentDelay = 15;
		}
		
		StartCoroutine(VisualUpdate());
	}
	
	
	void Update () {
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
		/*elapsedDelay += Time.deltaTime;
		if (elapsedDelay >= delay) {
			elapsedDelay -= delay;
			StartCoroutine(VisualUpdate());
		}*/
	}

	IEnumerator VisualUpdate () {
		using (dbCon = new SqlConnection(connectionString)) {
			try {
				dbCon.Open();
				cmd = new SqlCommand(query, dbCon);
				reader = cmd.ExecuteReader();
				if (reader.HasRows) {
					i1 = 0;
					while (reader.Read ()) {
						if (backgroundPanels[i1]) {
							if (!reader.IsDBNull(0)) {
								backgroundPanels[i1].color = stateColors[(int)reader.GetInt32(0)];
							} else {
								backgroundPanels[i1].color = stateColors[0];
							}
						}
						if (text1 [i1]) {
							if (!reader.IsDBNull(1)) {
								if ((string)reader.GetString(1) == "Unplanned Downtime" || (string)reader.GetString(1) == "Undefined" || (string)reader.GetString(1) == "Revolution Alarm" ) {
									text1 [i1].text = "";
								} else if ((string)reader.GetString(1) == "Blocked") {
									text1 [i1].text = "Downstream";
								} else { 
									text1 [i1].text = (string)reader.GetString(1);
								}

							} else {
								text1 [i1].text = "";
							}
						}
						if (text2 [i1]) {
							if (!reader.IsDBNull(1)) {
								text2 [i1].text = (string)reader.GetString(2);
							} else {
								text2 [i1].text = "";
							}
						}
						i1++;
					}
				}
				reader.Close ();
				dbCon.Close();
			}
			catch (SqlException exception) {
				Debug.LogWarning(exception.ToString());
				//outputText.text = "-----";
			}
		}
		yield return null;	
	}	
}