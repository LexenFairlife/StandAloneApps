using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Networking;
 
public class ImageDownloader : MonoBehaviour {
	
	public Camera mainCamera;
	public int delay = 5;
	public Image image;
	public string URL;
	
	
	bool updating;
	Rect rec;
	Texture myTexture;
	UnityWebRequest www;
	
	
	void Awake () {
		if (!Application.isEditor) {
			QualitySettings.vSyncCount = 0;
			Application.targetFrameRate = 1;
		} else {
			Application.targetFrameRate = 120;
		}
	}
	
    void Start() {
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
		if (System.DateTime.Now.Second % 60 == delay) {
			if (!updating) {
				updating = true;
				StartCoroutine (GetTexture ());
			}
		} else if (updating) {
			updating = false;
		}
	}
 
    IEnumerator GetTexture() {
		
        www = UnityWebRequestTexture.GetTexture(URL);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError) {
            Debug.Log(www.error);
        } else {
			DestroyImmediate(myTexture);
            myTexture = ((DownloadHandlerTexture)www.downloadHandler).texture;
			if (!image.sprite) {
				rec = new Rect (0,0,myTexture.width, myTexture.height);
			}
			image.sprite = Sprite.Create((Texture2D)myTexture,rec,Vector2.one/*new Vector2(0.5f,0.5f)*/,100);
			
			//DestroyImmediate(myTexture);
			//DestroyImmediate(www.downloadHandler);
			Resources.UnloadUnusedAssets();
			//System.CleanCache();
			www.Dispose();
			System.GC.Collect();
        }
    }
}