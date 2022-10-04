using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using UnityEngine;
using UnityEngine.UI;

public class ImageBasedChart : MonoBehaviour {
    
    [System.Serializable] public class DataPoint {
        public string name;
        public DateTime timestamp;
        public float value;
    }
    
    [System.Serializable] public class Series {
        public string name, displayName, tag, unit;
        public int lineWidth = 1;
        public bool barChart = false;
        public bool smoothing = true;
        public float mostRecentPoint = -1;
        public Color color;
        public List <DataPoint> dataPoints = new List <DataPoint> ();
        public int [] pixelValues;
    }
    
    public float sqlInitialDelay = 50, sqlDelay = 60, graphicsDelay = 10, displayedSeconds = 10800, minValue = 0, maxValue = 100;
    public string query;
    public List <Series> series;
    public Image image;
    public Texture2D texture;
    public Sprite sprite;
    public Vector2 imageResolution = new Vector2 (1024,512);
    
    DateTime minUTC;
    float sqlActiveDelay = 60;
    float graphicsActiveDelay = 10;
    public float secondsPerPixel;
    
    
    private readonly string connectionString = @"Data Source = tcp:172.16.5.217; user id = cipapp; password = bestinclass&1; Connection Timeout=5;";
    
    
    
    void Start() {
        sqlActiveDelay = sqlInitialDelay;
        graphicsActiveDelay = sqlInitialDelay+1;
        secondsPerPixel = displayedSeconds / imageResolution.x;
        for (int i = 0; i < series.Count; i++) {
            series [i].pixelValues = new int [Mathf.RoundToInt (imageResolution.x)];
        }
    }


    void Update() {
        sqlActiveDelay -= Time.deltaTime;
        graphicsActiveDelay -= Time.deltaTime;
        if (sqlActiveDelay <= 0) {
            sqlActiveDelay += sqlDelay;
            Refresh ();
        } else if (graphicsActiveDelay <= 0) {
            graphicsActiveDelay += graphicsDelay;
            GraphicsRefresh ();
        }
    }
    
    void Refresh () {
        string activeTag = "NULL";
        int tagInt = -1;
        DateTime activeTimestamp;
        using (SqlConnection dbCon = new SqlConnection(connectionString)) {
            SqlCommand cmd = new SqlCommand(query, dbCon);
            try {
				dbCon.Open();
				SqlDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows) {
                    while (reader.Read()) {
                        if (!reader.IsDBNull(0) && !reader.IsDBNull(1) && !reader.IsDBNull(2)) {
                            if (activeTag != reader.GetSqlString(0).ToString ()) {
                                activeTag = reader.GetSqlString(0).ToString ();
                                tagInt = -1;
                                for (int i = 0; i < series.Count; i++) {
                                    if (series [i].tag == activeTag) {
                                        tagInt = i;
                                        series [tagInt].dataPoints.Clear ();
                                        i = series.Count;
                                        break;
                                    }
                                }
                            }
                            if (tagInt >= 0) {
                                activeTimestamp = (DateTime)reader.GetSqlDateTime(1);
                                if ( (DateTime.UtcNow - activeTimestamp).TotalSeconds <= displayedSeconds) {
                                    series [tagInt].mostRecentPoint = (float) reader.GetDouble (2);
                                    if (series [tagInt].dataPoints.Count >= 2 && series [tagInt].dataPoints [series [tagInt].dataPoints.Count-1].value == series [tagInt].mostRecentPoint && series [tagInt].dataPoints [series [tagInt].dataPoints.Count-2].value == series [tagInt].mostRecentPoint) {
                                        series [tagInt].dataPoints.RemoveAt (series [tagInt].dataPoints.Count-1);
                                    } else if (series [tagInt].smoothing && series [tagInt].dataPoints.Count >= 2) {
                                        series [tagInt].dataPoints.Add (new DataPoint ());
                                        series [tagInt].dataPoints [series [tagInt].dataPoints.Count-1].timestamp = activeTimestamp.AddSeconds (-(activeTimestamp - series [tagInt].dataPoints [series [tagInt].dataPoints.Count-2].timestamp).TotalSeconds / 2) ;
                                        series [tagInt].dataPoints [series [tagInt].dataPoints.Count-1].value = (series [tagInt].mostRecentPoint + series [tagInt].dataPoints [series [tagInt].dataPoints.Count-2].value) / 2;
                                        series [tagInt].dataPoints [series [tagInt].dataPoints.Count-1].name = series [tagInt].dataPoints [series [tagInt].dataPoints.Count-1].timestamp.ToString ("yyyy-MM-dd HH:mm:ss");
                                    }
                                    series [tagInt].dataPoints.Add (new DataPoint ());
                                    series [tagInt].dataPoints [series [tagInt].dataPoints.Count-1].timestamp = activeTimestamp;
                                    series [tagInt].dataPoints [series [tagInt].dataPoints.Count-1].value = series [tagInt].mostRecentPoint;
                                    series [tagInt].dataPoints [series [tagInt].dataPoints.Count-1].name = series [tagInt].dataPoints [series [tagInt].dataPoints.Count-1].timestamp.ToString ("yyyy-MM-dd HH:mm:ss");
                                    
                                }
                            }
                        }
                    }
                }
            } catch (SqlException exception) {
                Debug.Log (exception);
			}
            graphicsActiveDelay = 1;
        }
    }
    
    void GraphicsRefresh () {
        minUTC = DateTime.UtcNow.AddSeconds (-displayedSeconds);
        if (sprite == null || sprite.name == "Clear") {
            texture = new Texture2D (Mathf.RoundToInt (imageResolution.x),Mathf.RoundToInt (imageResolution.y));
            texture.filterMode = FilterMode.Point;
            sprite = Sprite.Create (texture, new Rect (0,0,Mathf.RoundToInt (imageResolution.x),Mathf.RoundToInt (imageResolution.y)), Vector2.zero);
            image.sprite = sprite;
        }
        // Clears the graphics
        for (int x = 0; x < texture.width; x++) {
            for (int y = 0; y < texture.width; y++) {
                texture.SetPixel (x,y,Color.clear);
            }
        }
        int pixelID, pixelValue, runningPixelID;
        for (int i = 0; i < series.Count; i++) {
            pixelID = 0;
            runningPixelID = 0;
            for (int j = 0; j < series [i].dataPoints.Count; j++) {
                pixelValue = Mathf.FloorToInt (((series [i].dataPoints [j].value - minValue) / (maxValue - minValue)) * imageResolution.y);
                runningPixelID = TimeToPixel (series [i].dataPoints [j].timestamp);
                while (pixelID < runningPixelID) {
                    series [i].pixelValues [pixelID] = pixelValue;
                    pixelID++;
                }
            }
        }
        
        for (int i = 0; i < series.Count; i++) {
            if (series [i].barChart) {
                for (int x = 0; x < texture.width; x++) {
                    for (int y = 0; y < texture.width; y++) {
                        if (y < series [i].pixelValues [x]) {
                            texture.SetPixel (x,y,series [i].color);
                        }
                    }
                } 
            } else {
                for (int x = 0; x < texture.width; x++) {
                    for (int y = 0; y < texture.width; y++) {
                        if (y >= series [i].pixelValues [x] - series [i].lineWidth && y <= series [i].pixelValues [x] + series [i].lineWidth) {
                            texture.SetPixel (x,y,series [i].color);
                        }
                    }
                }
            }
        }

        texture.Apply ();
    }
    
    int TimeToPixel (DateTime d) {
        return Mathf.Clamp (Mathf.RoundToInt ((float)(d - minUTC).TotalSeconds / secondsPerPixel),0,Mathf.RoundToInt (imageResolution.x-1));
    }
}
