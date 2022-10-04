using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

public class ImagesDownloader : MonoBehaviour {
	
	public Camera mainCamera;
	public List<Image> images;
	public List<string> URLs;
	
	bool updating = true;
	int i1;
	Rect rec;
	List<Texture> textures = new List <Texture> {};
	UnityWebRequest www;
	
	void Awake () {
		if (!Application.isEditor) {
			QualitySettings.vSyncCount = 0;
			Application.targetFrameRate = 1;
		} else {
			Application.targetFrameRate = 60;
		}

	}
	
    void Start() {
		for (i1 = 0; i1 < images.Count; i1++) {
			textures.Add (null);
		}
        StartCoroutine(GetTexture());
    }
	
	void Update () {
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
		if (System.DateTime.Now.Second % 60 == 10) {
			if (!updating) {
				updating = true;
				StartCoroutine (GetTexture ());
			}
		} else if (updating) {
			updating = false;
		}
	}
 
    IEnumerator GetTexture() {
		for (i1 = 0; i1 < URLs.Count; i1++) {
		
		}
		
		
		
		for (i1 = 0; i1 < URLs.Count; i1++) {
			www = UnityWebRequestTexture.GetTexture(URLs[i1]);
			yield return www.SendWebRequest();

			if (www.isNetworkError || www.isHttpError) {
				Debug.Log(www.error);
			} else {
				if (textures [i1] && textures [i1] != null) {DestroyImmediate(textures[i1]);}
				textures[i1] = ((DownloadHandlerTexture)www.downloadHandler).texture;
				if (!images[i1].sprite) {
					rec = new Rect (0,0,textures[i1].width, textures[i1].height);
				}
				images[i1].sprite = Sprite.Create((Texture2D)textures[i1],rec,Vector2.one/*new Vector2(0.5f,0.5f)*/,100);
			}
		}
		Resources.UnloadUnusedAssets();
		www.Dispose();
		System.GC.Collect();
    }
}