using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HistorianIndicator : MonoBehaviour {

	//public HistorianGraph	historianGraph;

	public						enum 			DisplayType {none, SolidColor};
	public 						DisplayType		displayType;
	public						Color			normalColor;
	
	[HideInInspector] public	Text			valueLabel;
	[HideInInspector] public	Image			backdrop;
	[HideInInspector] public	Text			lastFailed;
	
	void Start () {
		valueLabel 		= transform.Find ("Value Label").gameObject.GetComponent <Text> ();
		backdrop		= transform.gameObject.GetComponent <Image> ();
		lastFailed 		= transform.Find ("Last Failed").gameObject.GetComponent <Text> ();
	}
}
