using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SmallGraph1 : MonoBehaviour {

	public string shortTitle = "";
	public int maxPoints = 120;
	public List <float> values;
	public bool autoRange = true;
	public float minPadding = 10;
	public Vector2 range;
	public Color lineColor = new Color (34,156,213,255);
	public float lineWidth = 3;
	public string units = "°F";
	public int decimalPlaces = 0;
	
	Sprite pointSprite, lineSprite;
	RectTransform rt;
	List <RectTransform> points = new List <RectTransform> {};
	List <RectTransform> lines = new List <RectTransform> {};
	float delay = 0;
	Text minLabel, midLabel, maxLabel, valueLabel, titleLabel;
	RectTransform valueRT;
	string valueToString = "0.0";
	Transform pointContainer;

	//Temp Variables
	GameObject tempObject;
	RectTransform tempRT;
	int i1, i2;
	float f1, f2, f3, x, y;
	Vector2 v2;
	string query;
	
	void Awake () {
		rt 				= gameObject.GetComponent <RectTransform> ();
		minLabel		= transform.Find ("Min Label").gameObject.GetComponent <Text> ();
		midLabel		= transform.Find ("Mid Label").gameObject.GetComponent <Text> ();
		maxLabel 		= transform.Find ("Max Label").gameObject.GetComponent <Text> ();
		valueLabel 		= transform.Find ("Value Label").gameObject.GetComponent <Text> ();
		titleLabel 		= transform.Find ("Title Label").gameObject.GetComponent <Text> ();
		valueRT 		= valueLabel.gameObject.GetComponent <RectTransform> ();
		pointContainer 	= transform.Find ("Point Container");
		pointSprite		= Resources.Load <Sprite> ("WhiteCircle");
		lineSprite		= Resources.Load <Sprite> ("WhiteLine");
		minLabel.text = 	range.x.ToString (valueToString) + units;
		midLabel.text = 	((range.y + range.x) / 2).ToString (valueToString) + units;
		maxLabel.text = 	range.y.ToString (valueToString) + units;
		valueLabel.text = 	range.y.ToString (valueToString) + units;
		titleLabel.text = 	shortTitle;
		if (decimalPlaces <= 0) {
			valueToString = "0";
		} else {
			valueToString = "0.";
			for (i1 = 0; i1 < decimalPlaces; i1++) {
				valueToString += "0";
			}
		}
	}
		
	public void VisualUpdate (float newValue) {
		values.Add (newValue);
		valueLabel.text = newValue.ToString (valueToString) + units;
		if (autoRange && values.Count > 1) {
			range.x = values [0];
			range.y = values [0];
			for (i1 = 1; i1 < values.Count; i1++) {
				if (values [i1] < range.x) {
					range.x = values [i1];
				} else if (values [i1] > range.y) {
					range.y = values [i1];
				}
			}
			if (minPadding > 0 && range.y - range.x < minPadding) {
				f1 = minPadding - (range.y - range.x);
				range.x -= f1 / 2;
				range.y += f1 / 2;
				if (range.x < 0) {
					range.y += -range.x;
					range.x = 0;
				}
			}
			range.x = Mathf.Floor 	(range.x);
			range.y = Mathf.Ceil 	(range.y);
			minLabel.text = range.x.ToString (valueToString) + units;
			maxLabel.text = range.y.ToString (valueToString) + units;
			midLabel.text = ((range.y + range.x) / 2).ToString (valueToString) + units;
		}
		while (values.Count > maxPoints) {
			values.RemoveAt (0);
		}
		f1 = rt.sizeDelta.x / (maxPoints - 1);
		for (i1 = 0; i1 < values.Count; i1++) {
			if (points.Count < values.Count) {
				AddPoint (new Vector3 ((i1 * f1),0));
			}
			if (lines.Count < values.Count - 1) {
				AddLine ();
			}
			//x = i1 * f1;
			y = ((values [i1] - range.x) / (range.y - range.x)) * rt.sizeDelta.y;
			if (float.IsNaN(y)){
				y = rt.sizeDelta.y / 2;
			}
			//points [i1].anchoredPosition = new Vector2 (points [i1].anchoredPosition.x, y);
			v2 = points [i1].anchoredPosition;
			v2.y = y;
			points [i1].anchoredPosition = v2;
			if (i1 > 0) { //This means we are not at the last point
				
				f2 = Vector2.Distance (points [i1 - 1].anchoredPosition, points [i1].anchoredPosition);
				lines [i1 - 1].sizeDelta = new Vector2 (f2,lineWidth);
				lines [i1 - 1].anchoredPosition = Vector2.Lerp (points [i1 - 1].anchoredPosition, points [i1].anchoredPosition, .5f);
				//lines [i1 - 1].localEulerAngles = new Vector3 (0,0, Vector2.Angle (points [i1 - 1].anchoredPosition, points [i1].anchoredPosition));
				v2 = (points [i1 - 1].anchoredPosition - points [i1].anchoredPosition).normalized;
				if (v2.x == 0) {
					lines [i1 - 1].localEulerAngles = Vector3.zero;
				} else {
					//f3 = (v2.y / v2.x);
					lines [i1 - 1].localEulerAngles = new Vector3 (0,0, Mathf.Atan (v2.y / v2.x) * 180 / Mathf.PI);
				}
			}
		}
		if (y > rt.sizeDelta.y - 40) { //-40
			y = rt.sizeDelta.y - 40;
		} else if (y < 20) {
			y = 20;
		}
		valueRT.anchoredPosition = new Vector2 (valueRT.anchoredPosition.x, y);
	}
	
	void AddPoint (Vector2 point) {
		tempObject = new GameObject ("Point " + points.Count, typeof (Image));
		tempObject.transform.SetParent (pointContainer, false);
		tempObject.GetComponent <Image> ().sprite = pointSprite;
		tempRT = tempObject.GetComponent <RectTransform> ();
		tempRT.anchoredPosition = point;
		tempRT.sizeDelta = new Vector2 (lineWidth,lineWidth);
		tempRT.anchorMin = new Vector2 (0,0);
		tempRT.anchorMax = new Vector2 (0,0);
		tempRT.gameObject.GetComponent <Image> ().color = lineColor;
		points.Add (tempRT);
	}
	
	void AddLine () {
		tempObject = new GameObject ("Line " + lines.Count, typeof (Image));
		tempObject.transform.SetParent (pointContainer, false);
		tempObject.GetComponent <Image> ().sprite = lineSprite;
		tempRT = tempObject.GetComponent <RectTransform> ();
		tempRT.sizeDelta = new Vector2 (10,lineWidth);
		tempRT.anchorMin = new Vector2 (0,0);
		tempRT.anchorMax = new Vector2 (0,0);
		tempRT.gameObject.GetComponent <Image> ().color = lineColor;
		lines.Add (tempRT);
	}
}
