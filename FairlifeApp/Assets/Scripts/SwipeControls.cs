using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SwipeControls : MonoBehaviour {
	
	public	float deadzone			= 100;
	public	float doubleTapDelta		= .5f;
	public	bool tap, doubleTap, swipeLeft, swipeRight, swipeUp, swipeDown;
	public	Vector2		touchPoint = Vector2.zero;
	
	//Should be private
	public	Vector2	swipeDelta, startTouch;
	public	float	lastTap;
	public	float	sqrDeadZone;
	public	float	x,y;
	
	
	void Start () {
		sqrDeadZone = deadzone * deadzone;
		Application.targetFrameRate = 120;
		QualitySettings.vSyncCount = 1;
		Screen.sleepTimeout = SleepTimeout.NeverSleep;
	}
	
	void Update () {
		tap = doubleTap = swipeLeft = swipeRight = swipeUp = swipeDown = false;
		CheckInput ();
		/*if (Application.isEditor) {
			CheckStandalone ();
		} else {
			CheckMobile ();
		}*/
	}
	
	void CheckInput () {
		touchPoint = Input.mousePosition;
		if (Input.GetMouseButtonDown (0) || (Input.touches.Length != 0 && Input.touches [0].phase == TouchPhase.Began)) {
			tap = true;
			startTouch = Input.mousePosition;
			doubleTap = Time.time - lastTap < doubleTapDelta;
			if (!doubleTap) {
				lastTap = Time.time;
			}
		} else if (Input.GetMouseButtonUp (0) || (Input.touches.Length != 0 && (Input.touches [0].phase == TouchPhase.Ended || Input.touches [0].phase == TouchPhase.Canceled))) {
			startTouch = Vector2.zero;
		}
		swipeDelta = Vector2.zero;
		if (startTouch != Vector2.zero && (Input.GetMouseButton (0) || Input.touches.Length != 0)) {
			swipeDelta = (Vector2)Input.mousePosition - startTouch;
		}
		if (swipeDelta.sqrMagnitude > sqrDeadZone) {
			x = swipeDelta.x;
			y = swipeDelta.y;
			if (Mathf.Abs (x) > Mathf.Abs (y)) {
				if (x < 0) {
					swipeLeft = true;
				} else {
					swipeRight = true;
				}
			} else {
				if (y < 0) {
					swipeDown = true;
				} else {
					swipeUp = true;
				}
			}
			startTouch = swipeDelta = Vector2.zero;
		}
	}
	
	void CheckStandalone () {
		touchPoint = Input.mousePosition;
		if (Input.GetMouseButtonDown (0)) {
			tap = true;
			startTouch = Input.mousePosition;
			doubleTap = Time.time - lastTap < doubleTapDelta;
			if (!doubleTap) {
				lastTap = Time.time;
			}
		} else if (Input.GetMouseButtonUp (0)) {
			startTouch = Vector2.zero;
		}
		swipeDelta = Vector2.zero;
		if (startTouch != Vector2.zero && Input.GetMouseButton (0)) {
			swipeDelta = (Vector2)Input.mousePosition - startTouch;
		}
		if (swipeDelta.sqrMagnitude > sqrDeadZone) {
			x = swipeDelta.x;
			y = swipeDelta.y;
			if (Mathf.Abs (x) > Mathf.Abs (y)) {
				if (x < 0) {
					swipeLeft = true;
				} else {
					swipeRight = true;
				}
			} else {
				if (y < 0) {
					swipeDown = true;
				} else {
					swipeUp = true;
				}
			}
			startTouch = swipeDelta = Vector2.zero;
		}
	}
	
	void CheckMobile () {
		if (Input.touches.Length != 0) {
			if (Input.touches [0].phase == TouchPhase.Began) {
				tap = true;
				startTouch = Input.mousePosition;
				doubleTap = Time.time - lastTap < doubleTapDelta;
				if (!doubleTap) {
					lastTap = Time.time;
				}
			} else if (Input.touches [0].phase == TouchPhase.Ended || Input.touches [0].phase == TouchPhase.Canceled) {
				startTouch = Vector2.zero;
			}
			swipeDelta = Vector2.zero;
			if (startTouch != Vector2.zero && Input.touches.Length != 0) {
				swipeDelta = (Vector2)Input.mousePosition - startTouch;
			}
			if (swipeDelta.sqrMagnitude > sqrDeadZone) {
				x = swipeDelta.x;
				y = swipeDelta.y;
				if (Mathf.Abs (x) > Mathf.Abs (y)) {
					if (x < 0) {
						swipeLeft = true;
					} else {
						swipeRight = true;
					}
				} else {
					if (y < 0) {
						swipeDown = true;
					} else {
						swipeUp = true;
					}
				}
				startTouch = swipeDelta = Vector2.zero;
			}
		}

	}
}
