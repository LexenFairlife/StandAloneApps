using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;

public class UploadScreenShot : MonoBehaviour {

	int resWidth = 1920;//3840; 
	int resHeight = 1080;//2160;

	public bool disableCamera = false;
	Camera mainCamera;
	
	public bool takePerMinute;
	public bool run;
	float runTime;
	bool takingSnapShot;
	bool tookScreenShot = false;
	byte[] bytes;
	Rect rect;
	RenderTexture rt;
	public string imageName;
	public string savePath;
	Texture2D screenShot;
	Uri uri;
	WebClient client;
	
	void Start () {
		mainCamera = Camera.main;
		if (disableCamera) {mainCamera.enabled = false;}
		//imageName = Application.dataPath + "/" + gameObject.name + ".png";
		client = new System.Net.WebClient();
		//uri = new Uri("ftp://172.16.5.59/ScreenImages/" + new FileInfo(imageName).Name);
		client.Credentials = new System.Net.NetworkCredential("ubuntu", "1");
		rect = new Rect(0, 0, resWidth, resHeight);
	}
 	
	void Update () {
		if (takePerMinute) {
			if (runTime > 60) {
				if (!takingSnapShot && !tookScreenShot && DateTime.UtcNow.Second == 1) {
				    run = true;
				}
			} else {
				 runTime += Time.deltaTime;
			}
		}
		if (run) {
			takingSnapShot = true;
			run = false;
			StartCoroutine (TakeScreenShot ());
		}
	}
	
	public IEnumerator TakeScreenShot () {
		Debug.Log ("Taking Snap Shot");
		//imageName = Application.dataPath + "/" + imageName + ".png";
		uri = new Uri(/*"ftp://172.16.5.59/ScreenImages/" */ savePath + new FileInfo(Application.dataPath + "/" + imageName + ".png").Name);
		
		if (disableCamera) {mainCamera.enabled = true;}
		rt = new RenderTexture(resWidth, resHeight, 24);
		mainCamera.targetTexture = rt;
		mainCamera.Render();
		RenderTexture.active = rt;														
		screenShot = new Texture2D(resWidth, resHeight, TextureFormat.RGB24, false);
		screenShot.ReadPixels(rect, 0, 0);				
		mainCamera.targetTexture = null;			                                    
		RenderTexture.active = null;												//	yield return new WaitForSeconds (.1f);															
		bytes = screenShot.EncodeToPNG();											//	yield return new WaitForSeconds (.1f);								
		File.WriteAllBytes(Application.dataPath + "/" + imageName + ".png", bytes);										//	yield return new WaitForSeconds (.1f);								
		if (disableCamera) {mainCamera.enabled = false;}							//	yield return new WaitForSeconds (.1f);
		try {
			client.UploadFileAsync(uri, "STOR", Application.dataPath + "/" + imageName + ".png");
		} catch (WebException exception) {
			Debug.Log (exception);
			//Debug.Log ("Upload Failed");
		}
		yield return new WaitForSeconds (5);
		try {
			bytes = client.UploadFile(uri,Application.dataPath + "/" + imageName + ".png");										
			if (System.Text.Encoding.ASCII.GetString(bytes).Length > 0) {
				Debug.Log (System.Text.Encoding.ASCII.GetString(bytes));
			}
		} catch (WebException exception) {
			Debug.Log (exception);
			//Debug.Log ("Upload Failed");
		}

		yield return new WaitForSeconds (1);
		Destroy(rt);
		Destroy (screenShot);															yield return new WaitForSeconds (1);
		

		System.GC.Collect();
		//Debug.Log ("Snapshot Complete");
		takingSnapShot = false;
		yield return null;
	}

}
