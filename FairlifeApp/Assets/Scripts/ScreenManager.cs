using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenManager : MonoBehaviour {

	Camera mainCamera;
	public int frameCap = 2;
	int height;
	List <Text> times = new List <Text> ();
	
	string tempString;
	int i1;
	
	void Awake () {
		mainCamera = gameObject.GetComponent <Camera> ();
		if (!Application.isEditor) {
			QualitySettings.vSyncCount = 0;
			Application.targetFrameRate = frameCap;
		} else {
			Application.targetFrameRate = 60;
		}
		FindTimes ();
	}
	
	public void FindTimes() {
		times.Clear ();
		foreach (GameObject go in GameObject.FindObjectsOfType(typeof(GameObject))) {
			if(go.name == "Current Time") {
				times.Add (go.GetComponent <Text> ());
			}
		}
	}
	
	void Update () {
		tempString = DateTime.Now.ToString ("MM-dd HH:mm:ss");
		for (i1 = 0; i1 < times.Count; i1++) {
			times [i1].text = tempString;
		}
		if (Screen.fullScreen) {
			if (Input.GetKeyDown (KeyCode.F11)) {
				Screen.SetResolution (1280, 720, false);
			}
		} else {
			if (Input.GetKeyDown (KeyCode.F11)) {
				Screen.SetResolution (Screen.currentResolution.width, Screen.currentResolution.height, true);
			} else if (mainCamera.aspect != 1.77665f) {
				Screen.SetResolution (Mathf.RoundToInt (Screen.height * 1.77665f), Screen.height, Screen.fullScreen);
			}
		}
	}
	
	public void Reload () {
		UnityEngine.SceneManagement.SceneManager.LoadScene(0);
	}
}
