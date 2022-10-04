using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MembraneSchedule : MonoBehaviour {

	public List <float> last6Gal;
	public int maxGal = 120000;
	public Color lineColor = new Color (34,156,213,255);
	public Color targetColor = new Color (34,156,213,255);
	public float lineWidth = 3;
	
	Sprite pointSprite, lineSprite;
	RectTransform rt;
	List <RectTransform> last6Points	= new List <RectTransform> {};
	List <RectTransform> last6Lines 	= new List <RectTransform> {};
	List <RectTransform> lines			= new List <RectTransform> {};
	Transform pointContainer;
	Text valueLabel;
	RectTransform valueRT;
	
	//Temp Variables
	GameObject tempObject;
	RectTransform tempRT;
	int i1, i2;
	float f1, f2, f3, x, y;
	Vector2 v2;
	
	void Awake () {
		rt 				= gameObject.GetComponent <RectTransform> ();
		pointContainer 	= transform.Find ("Point Container");
		pointSprite		= Resources.Load <Sprite> ("WhiteCircle");
		lineSprite		= Resources.Load <Sprite> ("WhiteLine");
		valueLabel 		= transform.Find ("Value Label").gameObject.GetComponent <Text> ();
		valueRT 		= valueLabel.gameObject.GetComponent <RectTransform> ();
		//78 hours total
		f1 = rt.sizeDelta.x / 168;
		for (i1 = 0; i1 < 168; i1++) {
			AddPoint (new Vector2 (f1 * i1, 0), lineColor);
			last6Points.Add (tempRT);
		}
		for (i1 = 0; i1 < 167; i1++) {
			AddLine ((last6Points [i1].anchoredPosition + last6Points [i1].anchoredPosition) / 2, lineColor);
			last6Lines.Add (tempRT);
		}
		Last6Points ();
	}
	
	public void Last6Points () {
		for (i1 = 0; i1 < last6Gal.Count; i1++) {
			y = ((float)last6Gal[i1] / maxGal) * rt.sizeDelta.y;
			
			last6Points [i1].anchoredPosition = new Vector2 (last6Points [i1].anchoredPosition.x, y);
		}
		for (i1 = 0; i1 < last6Lines.Count; i1++) {
			UpdateLine (last6Points [i1].anchoredPosition, last6Points [i1 + 1].anchoredPosition, last6Lines [i1]);
		}
		valueLabel.text = CompressedValue (last6Gal [last6Gal.Count - 1]);
		y = last6Points [last6Gal.Count - 1].anchoredPosition.y;
		//x = last6Points [47].anchoredPosition.x;
		if (y < 40) { y = 40;}
		valueRT.anchoredPosition = new Vector2 (valueRT.anchoredPosition.x, y);
	}
	
	void UpdateLine (Vector2 a, Vector2 b, RectTransform InputRt) {
		f2 = Vector2.Distance (a, b);
		InputRt.sizeDelta = new Vector2 (f2,lineWidth);
		InputRt.anchoredPosition = (a + b) / 2;
		v2 = (a - b).normalized;
		if (v2.x == 0) {
			InputRt.localEulerAngles = Vector3.zero;
		} else {
			InputRt.localEulerAngles = new Vector3 (0,0, Mathf.Atan (v2.y / v2.x) * 180 / Mathf.PI);
		}
	}
	
	void AddPoint (Vector2 point, Color inputColor) {
		tempObject = new GameObject ("Point", typeof (Image));
		tempObject.transform.SetParent (pointContainer, false);
		tempObject.GetComponent <Image> ().sprite = pointSprite;
		tempRT = tempObject.GetComponent <RectTransform> ();
		tempRT.anchoredPosition = point;
		tempRT.sizeDelta = new Vector2 (lineWidth,lineWidth);
		tempRT.anchorMin = new Vector2 (0,0);
		tempRT.anchorMax = new Vector2 (0,0);
		tempRT.gameObject.GetComponent <Image> ().color = inputColor;
	}
	
	void AddLine (Vector2 pos, Color inputColor) {
		tempObject = new GameObject ("Line " + lines.Count, typeof (Image));
		tempObject.transform.SetParent (pointContainer, false);
		tempObject.GetComponent <Image> ().sprite = lineSprite;
		tempRT = tempObject.GetComponent <RectTransform> ();
		tempRT.anchoredPosition = pos;
		tempRT.sizeDelta = new Vector2 (10,lineWidth);
		tempRT.anchorMin = new Vector2 (0,0);
		tempRT.anchorMax = new Vector2 (0,0);
		tempRT.gameObject.GetComponent <Image> ().color = inputColor;
	}
	
	string CompressedValue (float inputFloat) {
		if (inputFloat < 1000) {
			return inputFloat.ToString ("0 gal");
		} else if (inputFloat < 1000000) {
			return (inputFloat / 1000).ToString ("0.0k gal");
		}
		return "";
	}
}
