using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SqlJobManager : MonoBehaviour {
	
	public static SqlJobManager instance = null;
	public static SqlJobManager Instance { get { return instance; } }
	public SqlJobManager() : base() { instance = this; }


	public bool sqlRunning = false;
	public float jobTime;
	
	DateTime jobStart, jobEnd;
	//GameObject sqlIcon;
	
	/*void Awake () {
		sqlIcon	= transform.Find ("Panel/SQL Icon").gameObject;
	}*/
	
	public void StartJob () {
		sqlRunning = true;
		jobStart = DateTime.Now;
		//sqlIcon.SetActive (true);
	}
	
	public void EndJob () {
		sqlRunning = false;
		jobEnd = DateTime.Now;
		jobTime = (float)(jobEnd - jobStart).TotalSeconds;
		//Debug.Log (jobTime);
		//sqlIcon.SetActive (false);
	}
}
