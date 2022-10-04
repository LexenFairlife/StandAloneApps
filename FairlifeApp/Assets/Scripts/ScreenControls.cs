using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenControls : MonoBehaviour {
	
		//References
						public	Camera				mainCamera;
						public	List <GameObject>	layer0Screens,layer1Screens,layer2Screens,layer3Screens,layer4Screens;
						public	SwipeControls		swipe;
	//					public	Text				debugText;
	
						
	
		//Internal Values
						public	bool			zoomed;
						public	float			zoomSize;
						public	GameObject		tempGO;
						public	int				i1,i2,x,y;
						public	List <int>		layerMaxes;
								RaycastHit		hit;
								Ray				ray;
	
	
	void Start () {
		layerMaxes.Clear ();
		layerMaxes.Add (layer0Screens.Count - 1);
		layerMaxes.Add (layer1Screens.Count - 1);
		layerMaxes.Add (layer2Screens.Count - 1);
		layerMaxes.Add (layer3Screens.Count - 1);
		layerMaxes.Add (layer4Screens.Count - 1);
		
		if (layerMaxes.Count <= 4) {
			zoomSize = 2.25f;
		} else if (layerMaxes.Count <= 5) {
			zoomSize = 1.49f;
		} else if (layerMaxes.Count <= 8) {
			zoomSize = 1.128f;
		}
	}
	
	void Update () {
		/*if (Input.touches.Length != 0) {
			debugText.text = "Touch";
		}*/
		if (swipe.tap) {
			//debugText.text = "Tap";
			if (!zoomed) {
				ray = mainCamera.ScreenPointToRay (swipe.touchPoint);
				if (Physics.Raycast(ray,out hit, 100)){
					zoomed = true;
					tempGO = hit.transform.gameObject;
					SetXandYbasedOnObject ();
				}
				swipe.lastTap = 0;
			} else if (swipe.doubleTap) {
				zoomed = false;
			}
		} else if (zoomed && (swipe.swipeLeft || swipe.swipeRight || swipe.swipeUp || swipe.swipeDown)) {
			if (swipe.swipeRight) {
				if (x > 0) {
					x--;
					SetObjectBasedOnXandY ();
				} else if (y > 0) {
					y--;
					x = layerMaxes [y];
					SetObjectBasedOnXandY ();
				}
			} else if (swipe.swipeLeft) {
				if (x < layerMaxes [y]) {
					x++;
					SetObjectBasedOnXandY ();
				} else if (y < layerMaxes.Count - 1 && layerMaxes [y+1] >= 0) {
					y++;
					x = 0;
					SetObjectBasedOnXandY ();
				}
			} else if (swipe.swipeUp) {
				if (y < layerMaxes.Count - 1 && layerMaxes [y+1] >= 0) {
					y++;
					if (x > layerMaxes [y]) {
						x = layerMaxes [y];
					}
					SetObjectBasedOnXandY ();
				}
			} else if (swipe.swipeDown) {
				if (y > 0) {
					y--;
					if (x > layerMaxes [y]) {
						x = layerMaxes [y];
					}
					SetObjectBasedOnXandY ();
				}
			}
		}
		
		if (zoomed) {
			mainCamera.orthographicSize = Mathf.Lerp (mainCamera.orthographicSize, zoomSize, .25f);
			mainCamera.transform.position = Vector2.Lerp (mainCamera.transform.position, tempGO.transform.position, .25f);
		} else {
			mainCamera.orthographicSize = Mathf.Lerp (mainCamera.orthographicSize, 9, .25f);
			mainCamera.transform.position = Vector2.Lerp (mainCamera.transform.position, Vector2.zero, .25f);
		}
	}

	void SetXandYbasedOnObject () {
		for (y = 0; y < layerMaxes.Count; y++) {
					if (y == 0) {	for (x = 0; x < layer0Screens.Count; x++) {		if (layer0Screens [x] == tempGO) {return;}	}
			} else	if (y == 1) {	for (x = 0; x < layer1Screens.Count; x++) {		if (layer1Screens [x] == tempGO) {return;}	}
			} else	if (y == 2) {	for (x = 0; x < layer2Screens.Count; x++) {		if (layer2Screens [x] == tempGO) {return;}	}
			} else	if (y == 3) {	for (x = 0; x < layer3Screens.Count; x++) {		if (layer3Screens [x] == tempGO) {return;}	}
			} else	if (y == 4) {	for (x = 0; x < layer4Screens.Count; x++) {		if (layer4Screens [x] == tempGO) {return;}	}	}
		}
	}
	
	void SetObjectBasedOnXandY () {
				if (y == 0) {	tempGO = layer0Screens [x];
		} else	if (y == 1) {	tempGO = layer1Screens [x];
		} else	if (y == 2) {	tempGO = layer2Screens [x];
		} else	if (y == 3) {	tempGO = layer3Screens [x];
		} else	if (y == 4) {	tempGO = layer4Screens [x]; }
	}
}
