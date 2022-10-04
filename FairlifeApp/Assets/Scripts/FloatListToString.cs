using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatListToString : MonoBehaviour {
    
	public bool test = false;
	public float [] input;
	public byte [] middleBytes;
	public string middle;
	public List <float> output;
	
	//Taken from https://en.wikipedia.org/wiki/List_of_j%C5%8Dy%C5%8D_kanji
	public List <string> characters;
	
	byte [] fourBytes = new byte [4];
	int i1;
	
	void Start () {
		string output = "\"" + characters [0]+"\"";
		
		for (i1 = 1; i1 < characters.Count; i1++) {
			output += ", \"" + characters [i1] + "\"";
		}
		Debug.Log (output);
	}
	
    void Update () {
        if (test) {
			test = false;
			middleBytes = new byte [input.Length * 4];
			Buffer.BlockCopy (input, 0, middleBytes, 0, middleBytes.Length);
			middle = "";
			for (i1 = 0; i1 < middleBytes.Length; i1++) {
				middle += characters [middleBytes [i1]];
			}
			output.Clear ();
			for (i1 = 0; i1 < middleBytes.Length; i1 += 4) {
				fourBytes [0] = (byte) characters.FindIndex (x => x.Contains (middle [i1  ].ToString ()));
				fourBytes [1] = (byte) characters.FindIndex (x => x.Contains (middle [i1+1].ToString ()));
				fourBytes [2] = (byte) characters.FindIndex (x => x.Contains (middle [i1+2].ToString ()));
				fourBytes [3] = (byte) characters.FindIndex (x => x.Contains (middle [i1+3].ToString ()));
				output.Add (BitConverter.ToSingle (fourBytes, 0));
			}
			//Debug.Log (characters.FindIndex (x => x.Contains (middle [0].ToString ())));
		}
    }

}
