using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class PlaceBoardHistory
{
    public int PlaceBoardHistoryId;

    // 4 bytes
    public short XPos;
    public short YPos;

    // 3 bytes
    public byte R;
    public byte G;
    public byte B;


    public ulong DiscordUserId;
    // 8bytes
    public ulong SnowflakeTimePlaced;

    // 1 bit
    public bool Removed;
}

[System.Serializable]
public class SpriteDataCollection
{
    public PlaceBoardHistory[] data;
}

public class DataHandler : MonoBehaviour
{
    public GameObject spawnee;
    protected AudioSource placeSound;

    bool _threadRunning;
    Thread _thread;
    SpriteDataCollection data;
    int counter = 0;
    public Text PixelInfo;

    public GameObject plane;


    public Terrain TerrainMain;


    void Start()
    {/*
        int xRes = TerrainMain.terrainData.heightmapResolution;
        int yRes = TerrainMain.terrainData.heightmapResolution;
        float[,] heights = TerrainMain.terrainData.GetHeights(0,0, xRes, yRes);

        for (int i = 0; i < xRes; i++)
        {
            for (int j = 0; j < yRes; j++)
            {
                heights[i, j] = 0f;
            }
        }
        TerrainMain.terrainData.SetHeights(0, 0, heights);


        plane = GameObject.Find("Plane");
        placeSound = GetComponent<AudioSource>();
        try
        {
            var dataString = File.ReadAllText("I:\\Download\\0-100000.json");
            //dataString = "{\"data\":" + dataString + "}";

            data = JsonUtility.FromJson<SpriteDataCollection>(dataString);

        }
        catch(System.Exception ex)
        {

        }


        // Begin our heavy work on a new thread.
        _thread = new Thread(ThreadedWork);
        //_thread.Start();*/
    }

    void ThreadedWork()
    {
        _threadRunning = true;
        bool workDone = false;

        // This pattern lets us interrupt the work at a safe point if neeeded.
        while (_threadRunning && !workDone)
        {
            // Do Work...
        }
        _threadRunning = false;
    }

    void OnDisable()
    {
        // If the thread is still running, we should shut it down,
        // otherwise it can prevent the game from exiting correctly.
        if (_threadRunning)
        {
            // This forces the while loop in the ThreadedWork function to abort.
            _threadRunning = false;

            // This waits until the thread exits,
            // ensuring any cleanup we do after this is safe. 
            _thread.Join();
        }

        // Thread is guaranteed no longer running. Do other cleanup tasks.
    }

    // Start is called before the first frame update



    private float LastPlacement;
    // Update is called once per frame
    void Update()
    {/*
        // draw direct pixels
        var tex = plane.GetComponent<Renderer>().material.mainTexture as Texture2D;
        if (LastPlacement < Time.time * 1000 - 25)
        {
            try
            {

                LastPlacement = Time.time * 1000;
                int step = Random.Range(10,20);
                int until = Mathf.Min(counter + step, data.data.Length);

                for (; counter < until; counter++)
                {
                    var item = data.data[counter];
                    Color col = new Color32(item.R, item.G, item.B, 0);// , (byte)Random.Range(0, 255), 0);
                                                                       //Random.Range (0, 2) == 1 ? Color.black : Color.white;

                    //Color col = xBlock % 2 == 0 ? Color.red : Color.green;
                    //Color col = Random.Range(0, 2) == 1 ? Color.black : Color.white;

                    //var instObj = Instantiate(spawnee, new Vector3(Random.Range(0, gridSize) * 1.0F, 10, Random.Range(0, gridSize) * 1.0F), Quaternion.identity);

                    if (false)
                    {
                        // cube mode -> lags
                        var instObj = Instantiate(spawnee, new Vector3(item.XPos * 1.0F + 0.5F, 3, 999 - item.YPos * 1.0F + 0.5F), Quaternion.identity);
                        instObj.GetComponent<MeshRenderer>().material.color = col;
                        instObj.AddComponent<AudioSource>();

                        var audio = instObj.GetComponent<AudioSource>();
                        audio.clip = placeSound.clip;
                    }
                    else
                    {

                        //Color.red;//

                        int padding = 24;

                        float[,] heights = new float[,] {
                          {0.01f, 0.02f},
                          {0.03f, 0.03f},
                        };

                        int x = (padding + (int)(999 - item.XPos));
                        int y = (padding + (int)item.YPos);

                        if (x < 500 && y < 500)
                        {
                            //heights[(padding + (int)(999 - item.XPos)), (padding + (int)item.YPos)] = 0.02f;
                            //TerrainMain.terrainData.SetHeights(x, item.YPos, heights);
                        }

                        tex.SetPixel((int)(999 - item.XPos), (int)item.YPos, col);


                    
                    }


                    PixelInfo.text = $"Pixels: {counter.ToString("N0")} \r\nLast by: {item.DiscordUserId}";
                }
                
                tex.Apply();
                //PixelDrawn = true;
            }
            catch (System.Exception ex)
            {

            }
        }*/
    }
}
