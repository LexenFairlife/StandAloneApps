using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartOnDelay : MonoBehaviour {
    
    public float delay = 172800;
    
    
    void Update () {
        delay -= Time.deltaTime;
        if (delay <= 0) {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
