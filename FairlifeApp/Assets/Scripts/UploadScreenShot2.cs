#if !UNITY_WEBGL
using StackExchange.Redis;
#endif
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using UnityEngine;

public class UploadScreenShot2 : MonoBehaviour {
    
	int resWidth = 1920;//3840; 
	int resHeight = 1080;//2160;
    
	Rect rect;
	RenderTexture rt;
    int i1;
    
    Camera mainCamera;
    public string imageName, redisLocation;
    public bool takePicture, takingPicture, tookPicture;
    float runTime = 0;
    
    Texture2D snap;
    
    //Upload
    byte[] bytes, bytes2;
	Uri uri;
	WebClient client;
    
    #if !UNITY_WEBGL
    // Redis Information
    ConnectionMultiplexer redis;
    IDatabase db;
    #endif
    
    // TSQL Information
    private readonly string         connectionStringPBI01 = @"Data Source = tcp:172.16.5.217; user id = cipapp; password = bestinclass&1; Connection Timeout=5;";
    
    
    
    void Start () {
        client = new System.Net.WebClient();
        client.Credentials = new System.Net.NetworkCredential("ubuntu", "1");
        //snap = new Texture2D(resWidth, resHeight);
        snap = new Texture2D(resWidth, resHeight, TextureFormat.RGB24, false);
        rt = new RenderTexture(resWidth, resHeight, 24);
        mainCamera = Camera.main;
        if (imageName.Length <= 0) {
            imageName = gameObject.name;
        }
        uri = new Uri("ftp://172.16.5.59/ScreenImages/" + imageName + ".png");
        client.Credentials = new System.Net.NetworkCredential("ubuntu", "1");
        
        #if !UNITY_WEBGL
        if (redisLocation.Length > 1) {
            redis = ConnectionMultiplexer.Connect("172.16.5.59:6379");
            db = redis.GetDatabase();
        }
        #endif
    }
    
    void Update () {
        if (runTime > 60) {
            if (!takingPicture && !tookPicture && DateTime.Now.Second == 1) {
                takePicture = true;
            } else {
                tookPicture = false;
            }
        } else {
             runTime += Time.deltaTime;
             if (tookPicture) {
                 tookPicture = false;
             }
        }
		if (takePicture) {
			takePicture = false;
            takingPicture = true;
			StartCoroutine (Picture ());
		}
    }
    
    IEnumerator Picture () {

		mainCamera.targetTexture = rt;
		mainCamera.Render();
        RenderTexture.active = rt;
        snap.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        snap.Apply();
        mainCamera.targetTexture = null;			                                    
		RenderTexture.active = null;
        
        yield return new WaitForSeconds (1); 
        
        bytes = snap.EncodeToPNG();
        
        
        #if !UNITY_WEBGL
        yield return new WaitForSeconds (1);
        
        try {
            if (redisLocation.Length > 1) {
                db.StringSet(redisLocation, Convert.ToBase64String(bytes));
            }
        } catch (Exception e) {
            Debug.Log (e);
        }
        #endif
        
        yield return new WaitForSeconds (1);
        
        try {
            client.UploadData(uri, bytes);
        } catch (WebException exception) {
            if (UnityEngine.Application.isEditor) {Debug.Log (exception);}
        }
        
        yield return new WaitForSeconds (1);
        
        try {
            bytes2 = client.UploadData(uri,bytes);
            if (System.Text.Encoding.ASCII.GetString(bytes2).Length > 0) {
                Debug.Log (System.Text.Encoding.ASCII.GetString(bytes2));
            }
        } catch (WebException exception) {
            if (UnityEngine.Application.isEditor) {Debug.Log (exception);}
        }
   
        takingPicture = false;
        tookPicture = true;
        yield return null;
    }
}
