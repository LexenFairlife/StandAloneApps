using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenSelection : MonoBehaviour {

	public List <GameObject>	screens;
	public List <GameObject>	screenButtons;
	public List <int>			screenFPS;
	
	public ScreenManager screenManager;
	
	List <int> selectedScreens;
	List <Vector2> screenPositions;
	float screenScale = 1;
	float transitionTimer;
	GameObject focusedScreen;
	RaycastHit hit;
	Ray ray;
	Camera mainCamera;
	
	List <Vector2>	touchPos 	= new List <Vector2> 	{new Vector2 (0,0),new Vector2 (0,0),new Vector2 (0,0),new Vector2 (0,0),new Vector2 (0,0),new Vector2 (0,0),new Vector2 (0,0),new Vector2 (0,0),new Vector2 (0,0),new Vector2 (0,0)};
	
	List <GameObject> instantiatedButtons = new List <GameObject> ();
	
	int			i1;
	int			selection;
	GameObject	tempGO;

	void OnEnable () {
		if 			(screenButtons.Count <=  1){	screenScale = 1;		screenPositions = new List <Vector2> {new Vector2 ( 0,	     0)};
		} else if 	(screenButtons.Count <=  2){	screenScale = .4f;		screenPositions = new List <Vector2> {new Vector2 ( -480,    0), new Vector2 (  480,    0)};
		} else if 	(screenButtons.Count <=  3){	screenScale = .4f;		screenPositions = new List <Vector2> {new Vector2 ( -480, -270), new Vector2 (    0,  270), new Vector2 (  480, -270)};
		} else if 	(screenButtons.Count <=  4){	screenScale = .4f;		screenPositions = new List <Vector2> {new Vector2 ( -480,  270), new Vector2 (  480,  270), new Vector2 ( -480, -270), new Vector2 (  480, -270)};
		}
		for (i1 = 0; i1 < screenButtons.Count; i1++) {
			tempGO = Instantiate (screenButtons [i1]);
			instantiatedButtons.Add (tempGO);
			tempGO.transform.localScale = Vector3.one * screenScale;
			tempGO.transform.position = screenPositions [i1];
		}
		mainCamera = gameObject.GetComponent <Camera> ();
	}
	
	void Update () {
		if (transitionTimer > 0) {return;}
		if (Input.GetMouseButtonDown (0)) { //Left Click
			touchPos [0] = mainCamera.ScreenToViewportPoint(Input.mousePosition);
			Click ();
		}
	}
	
	void Click () {
		if (transitionTimer > 0) {return;}
		ray = mainCamera.ScreenPointToRay (mainCamera.ViewportToScreenPoint (touchPos [0]));
		if (Physics.Raycast (ray, out hit, 1000000)) {
			if (focusedScreen == null) {
				focusedScreen = hit.transform.gameObject;
				//Debug.Log (hit.transform.gameObject.name);
				selection = instantiatedButtons.IndexOf (focusedScreen);
				StartCoroutine (FocusTransition ());
			}
		}
	}
	
	IEnumerator FocusTransition () {
		transitionTimer = 3;
		tempGO = Instantiate (screens [selection], instantiatedButtons [selection].transform.position, instantiatedButtons [selection].transform.rotation);
		tempGO.transform.localScale = instantiatedButtons [selection].transform.localScale;
		instantiatedButtons [selection].SetActive (false);
		while (transitionTimer > 0) {
			transitionTimer -= Time.deltaTime;
			for (i1 = 0; i1 < instantiatedButtons.Count; i1++) {
				if (instantiatedButtons [i1] == focusedScreen) {
					tempGO.transform.localScale 		= Vector3.Lerp (Vector3.one,  tempGO.transform.localScale, transitionTimer / 3);
					tempGO.transform.localPosition 		= Vector3.Lerp (Vector3.zero, tempGO.transform.localPosition, transitionTimer / 3);
				} else {
					instantiatedButtons [i1].transform.localScale 		= Vector3.Lerp (Vector3.zero, instantiatedButtons [i1].transform.localScale, transitionTimer / 3);
				}
			}
			yield return new WaitForSeconds (0);
		}
		for (i1 = 0; i1 < instantiatedButtons.Count; i1++) {
			if (instantiatedButtons [i1] == focusedScreen) {
				tempGO.transform.localScale 		= new Vector3 (1.005f, 1.005f, 1);
				tempGO.transform.localPosition 	= Vector3.zero;
			}
			Destroy (instantiatedButtons [i1]);
		}
		yield return new WaitForSeconds (0);
		screenManager.FindTimes();
		yield return new WaitForSeconds (0);
		if (!Application.isEditor) {
			Application.targetFrameRate = screenFPS [selection];
			QualitySettings.vSyncCount = 0;
		}
		yield return new WaitForSeconds (0);
		Destroy (gameObject.GetComponent <ScreenSelection> ());
		yield return null;
	}
}
