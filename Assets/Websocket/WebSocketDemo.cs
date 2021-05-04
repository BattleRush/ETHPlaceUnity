using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

// Use plugin namespace
using HybridWebSocket;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using Assets.Websocket;
using UnityEngine.UI;
using Assets.Data;
using System.Net;

public class WebSocketDemo : MonoBehaviour
{
    public GameObject spawnee;
    private AudioSource placeSound;
    public AudioClip Clip;
    public Plane plane;
    // Use this for initialization

    public Text WebsocketStatusText;

    private int MessageCount = 0;

    public Dropdown Mode;
    public Button LoadChunks;
    public Text LoadChunksStatus;
    public Scrollbar TimelapseScrollbar;
    public Button ReconnectButton;

    private static List<DiscordUser> DiscordUsers = new List<DiscordUser>();

    public static List<PixelHistoryEntry> PixelHistory = new List<PixelHistoryEntry>();

    private int TotalPixels = 0;

    private short TotalChunks = 10;
    private int CurrentChunk = 0;
    private bool LoadingChunks = false;

    private static WebSocket WS;
    public Text PixelCount;

    private int ChunkSize = -1;



    /// <summary>
    /// 0 | ID
    /// 1-2 | X Pos (int 16)
    /// 3-4 | Y Pos (int 16)
    /// 5 | R color (int 8)
    /// 6 | G color (int 8)
    /// 7 | B color (int 8)
    /// 8-9 | user id (int 16)
    /// Total: 10 bytes
    /// </summary>
    /// <param name="data"></param>
    void ProcessLivePixel(byte[] data)
    {
        if (!LiveMode)
            return; // currently in timelapse mode
        TotalPixels++;
        try
        {
            byte[] xBytes = data.Skip(1).Take(2).ToArray();
            byte[] yBytes = data.Skip(3).Take(2).ToArray();

            short x = BitConverter.ToInt16(xBytes, 0);
            short y = (short)(999 - BitConverter.ToInt16(yBytes, 0));

            byte R = data[5];
            byte G = data[6];
            byte B = data[7];

            int userId = data[8];

            var user = DiscordUsers.SingleOrDefault(i => i.UserId == userId);

            Color col = new Color32(R, G, B, 0);
            int height = 10; // TODO find out if there are currently any at this pos then increase this height

            MainThreadWorker.Instance.AddAction(() =>
            {
                try
                {
                    var instObj = Instantiate(spawnee, new Vector3(x * 1.0F + 0.5F, height, y * 1.0F + 0.5F), Quaternion.identity);

                    var handler = instObj.GetComponent<CubeHandler>();
                    handler.Username = user?.Username;// + $" (ID:{user?.UserId})";
                    handler.Image = user?.Image ?? new byte[0];
                    handler.IsBot = user?.IsBot == true;

                    instObj.GetComponent<MeshRenderer>().material.color = col;
                    instObj.AddComponent<AudioSource>();
                    var colider = instObj.GetComponent<Collider>();
                    colider.enabled = true;
                    //var rigidBody = instObj.GetComponent<Rigidbody>();
                    //rigidBody.useGravity = true;

                    var audio = instObj.GetComponent<AudioSource>();
                    audio.clip = Clip;

                    PixelCount.text = TotalPixels.ToString("N0") + " Pixels";

                }
                catch (Exception ex)
                {
                    Debug.LogError("Error ProcessLivePixel MainThreadWorker: " + ex.Message);

                }
            });
        }
        catch (Exception ex)
        {
            Debug.LogError("Error ProcessLivePixel: " + ex.Message);

        }
    }

    /// <summary>
    /// TODO
    /// </summary>
    /// <param name="data"></param>
    void ProcessFullImage(byte[] data)
    {
        int size = 1000;

        MainThreadWorker.Instance.AddAction(() =>
        {
            try
            {
                var tex = GetComponent<Renderer>().material.mainTexture as Texture2D;
                int index = 1; // we skip the first byte (the packet id)

                for (int y = 0; y < size; y++)
                {
                    for (int x = 0; x < size; x++)
                    {
                        var randColor = new Color32(data[index], data[index + 1], data[index + 2], 0);
                        index += 3;
                        tex.SetPixel(999 - x, y, randColor);
                    }
                }

                tex.Apply();
            }
            catch (Exception ex)
            {
                // TODO log
            }
        });
    }

    /// <summary>
    /// 0 | ID
    /// 1-2 | Amount of users loaded
    /// USER REPEAT (rel) Total 199
    /// 0-1 | user id (int 16)
    /// 2-97 Username (utf8 3 bytes per char)
    /// 98-197 | Url of the Profile (10 chars around spare) ASCII 1 byte per char
    /// 198 | IsBot 1 byte (could move 1 bit to user id as we wont need all 16 bits but me lazy)
    /// </summary>
    /// <param name="data"></param>
    void ProcessUsers(byte[] data)
    {
        try
        {
            byte[] totalUsersBytes = data.Skip(1).Take(2).ToArray();

            short totalUsers = BitConverter.ToInt16(totalUsersBytes, 0);

            List<DiscordUser> discordUsers = new List<DiscordUser>();
            int index = 3;

            for (int i = 0; i < totalUsers; i++)
            {
                var userIdBytes = data.Skip(index).Take(2).ToArray();
                short userId = BitConverter.ToInt16(userIdBytes, 0);

                int usernameByteCount = 32 * 3;
                int urlByteCount = 100;
                var userName = Encoding.UTF8.GetString(data.Skip(index + 2).Take(usernameByteCount).ToArray());
                var url = Encoding.ASCII.GetString(data.Skip(index + 2 + usernameByteCount).Take(urlByteCount).ToArray());
                bool isBot = data[index + 2 + usernameByteCount + urlByteCount] == 1;

                index += 2 + usernameByteCount + urlByteCount + 1;

                var discordUser = new DiscordUser()
                {
                    UserId = userId,
                    Username = userName.Trim('\0'),
                    AvatarUrl = url.Trim('\0'),
                    IsBot = isBot
                };

                var avatarUrl = discordUser.AvatarUrl;
                if (avatarUrl.Contains("?"))
                    discordUser.AvatarUrl = avatarUrl.Substring(0, avatarUrl.IndexOf("?"));

                try
                {
                    if (!string.IsNullOrWhiteSpace(discordUser.AvatarUrl))
                    {
                        // SEND WS FOR IMAGE

                        var request = new byte[3];
                        request[0] = (byte)MessageEnum.GetUserProfileImage_Request;
                        request[1] = userIdBytes[0];
                        request[2] = userIdBytes[1];

                        WS.Send(request);
                    }
                    else
                    {
                        discordUser.Image = new byte[0];
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError(ex);
                }

                discordUsers.Add(discordUser);
            }

            DiscordUsers = discordUsers;
        }
        catch (Exception ex)
        {
            Debug.LogError(ex);
        }
    }

    /// <summary>
    /// 0 | ID
    /// 1-2 | UserId
    /// Remaining image bytes[]
    /// <param name="data"></param>
    void ProcessUserImage(byte[] data)
    {
        try
        {
            short userId = (short)(data[1] + data[2] * 256);//BitConverter.ToInt16(chunkIdBytes, 0);

            var imageBytes = data.Skip(3).ToArray();

            DiscordUsers.Single(i => i.UserId == userId).Image = imageBytes;
        }
        catch (Exception ex)
        {

        }
    }

    void ProcessChunk(byte[] data)
    {
        try
        {

            int index = 1;

            //byte[] chunkIdBytes = data.Skip(1).Take(2).ToArray();
            short chunkId = (short)(data[1] + data[2] * 256);//BitConverter.ToInt16(chunkIdBytes, 0);
            index += 2;


            Debug.Log($"received ChunkId-{chunkId} size {data.Length}");


            // dynamic chunksizing
            if (ChunkSize < 0)
            {
                ChunkSize = (data.Length - 3) / 12;
            }

            int size = ChunkSize;




            for (int i = 0; i < size; i++)
            {
                /*if (i % 1000 == 0)
                {
                    Debug.Log($"STEP {i} ChunkId-{chunkId} Time in ms: {watch.ElapsedMilliseconds} | {DateTime.Now.ToString("HH:mm:ss.fff")}");
                    watch.Restart();
                }*/

                //byte[] pixelHistoryIdBytes = data.Skip(index).Take(4).ToArray();
                // todo bitshit stuff
                int pixelHistoryId = data[index] + data[index + 1] * 256 + data[index + 2] * 65536 + data[index + 3] * 16777216;

                index += 4;
                /*
                byte[] xBytes = data.Skip(index).Take(2).ToArray();
                byte[] yBytes = data.Skip(index + 2).Take(2).ToArray();
                */
                short x = (short)(data[index] + data[index + 1] * 256);
                short y = (short)(data[index + 2] + data[index + 3] * 256);

                index += 4;

                byte r = data[index];
                byte g = data[index + 1];
                byte b = data[index + 2];
                index += 3;

                byte userId = data[index];
                index += 1;

                var pixelHistoryItem = new PixelHistoryEntry()
                {
                    Id = pixelHistoryId,
                    X = x,
                    Y = y,
                    R = r,
                    G = g,
                    B = b,
                    UserId = userId
                };

                PixelHistory.Add(pixelHistoryItem);
            }

            CurrentChunk++;

            if (CurrentChunk < TotalChunks && LoadingChunks)
            {
                try
                {
                    MainThreadWorker.Instance.AddAction(() =>
                    {
                        Debug.Log($"Processing ChunkId-{chunkId}");
                        LoadChunksStatus.text = $"Chunk {CurrentChunk.ToString("N0")}/{TotalChunks.ToString("N0")}";
                        LoadChunksStatus.color = Color.yellow;
                    });
                    byte[] newChunkIdBytes = BitConverter.GetBytes(CurrentChunk);

                    var request = new byte[3];
                    request[0] = (byte)MessageEnum.GetChunk_Request;
                    request[1] = newChunkIdBytes[0];
                    request[2] = newChunkIdBytes[1];

                    WS.Send(request);
                }
                catch (Exception ex)
                {
                    // for test only
                }

                // send request for next chunk
                /* byte[] newChunkIdBytes = BitConverter.GetBytes(CurrentChunk);

                 var request = new byte[3];
                 request[0] = (byte)MessageEnum.GetChunk_Request;
                 request[1] = newChunkIdBytes[0];
                 request[2] = newChunkIdBytes[1];

                 WS.Send(request);*/
            }
            else
            {
                // ensure order in the end
                //PixelHistory = PixelHistory.OrderBy(i => i.Id).ToList();

                MainThreadWorker.Instance.AddAction(() =>
                {
                    Debug.Log($"Processing Done");
                    LoadChunksStatus.text = $"Chunk {CurrentChunk.ToString("N0")}/{TotalChunks.ToString("N0")} (DONE)";
                    LoadChunksStatus.color = Color.green;
                    TimelapseScrollbar.gameObject.SetActive(true);
                });
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Error on process: " + ex.Message);
        }
    }


    void Start()
    {
        ReconnectButton.gameObject.SetActive(false);
        // disable timelapse stuff
        LoadChunksStatus.gameObject.SetActive(false);
        LoadChunks.gameObject.SetActive(false);
        TimelapseScrollbar.gameObject.SetActive(false);

        placeSound = GetComponent<AudioSource>();

        // Create WebSocket instance


        WS = WebSocketFactory.CreateInstance("wss://websocket.battlerush.dev:9000/place");
        //WS = WebSocketFactory.CreateInstance("ws://52.142.4.222:9000/place"); // TODO config
        //WS = WebSocketFactory.CreateInstance("ws://127.0.0.1:9000/place");
        //WS = ws;

        // Add OnOpen event listener
        WS.OnOpen += () =>
        {
            Debug.Log("WS connected!");
            Debug.Log("WS state: " + WS.GetState().ToString());

            MainThreadWorker.Instance.AddAction(() =>
            {
                WebsocketStatusText.text = "WS: Connected";
                WebsocketStatusText.color = Color.green;
            });

            // request the image 

            //var requestImage = new byte[1];


            // 01 - request full image
            // 02 - response of full image
            // 03 - live pixel
            // 04 - request get total pixel count
            // 05 - response get total pixel count
            // 06 - request timelapse 2. and 3. byte chunk id (each chunk is 100k placements) (max 65k chunks = 6.5B Placements)
            // 07 - response timelapse
            // 08? - request userinfos
            // 09? - response userinfos

            WS.Send(new byte[1] { (byte)MessageEnum.FullImage_Request });
            WS.Send(new byte[1] { (byte)MessageEnum.GetUsers_Request });
            WS.Send(new byte[1] { (byte)MessageEnum.TotalChunksAvailable_Request });
            WS.Send(new byte[1] { (byte)MessageEnum.TotalPixelCount_Request });


        };

        // Add OnMessage event listener
        WS.OnMessage += (byte[] msg) =>
        {
            try
            {
                MessageCount++;

                MainThreadWorker.Instance.AddAction(() =>
                {
                    WebsocketStatusText.text = $"WS: OK ({MessageCount.ToString("N0")})";
                    WebsocketStatusText.color = Color.green;
                });


                MessageEnum messageType = (MessageEnum)msg[0];


                switch (messageType)
                {
                    case MessageEnum.FullImage_Response:
                        ProcessFullImage(msg);
                        break;
                    case MessageEnum.LivePixel:
                        ProcessLivePixel(msg);
                        break;
                    case MessageEnum.TotalPixelCount_Response:
                        // total pixels is int32 (4 bytes)
                        // TODO move to method
                        int totalPixels = BitConverter.ToInt32(msg.Skip(1).ToArray(), 0);

                        TotalPixels = totalPixels;

                        break;
                    case MessageEnum.TotalChunksAvailable_Response:
                        // total chunks is int16 (2 bytes)
                        // TODO move to method

                        short totalChunks = BitConverter.ToInt16(msg.Skip(1).ToArray(), 0);

                        TotalChunks = totalChunks;

                        break;
                    case MessageEnum.GetChunk_Response:
                        ProcessChunk(msg);
                        break;
                    case MessageEnum.GetUsers_Response:
                        ProcessUsers(msg);
                        break;

                    case MessageEnum.GetUserProfileImage_Response:
                        ProcessUserImage(msg);
                        break;

                    case MessageEnum.FullImage_Request:
                    case MessageEnum.TotalPixelCount_Request:
                    case MessageEnum.GetChunk_Request:
                    case MessageEnum.TotalChunksAvailable_Request:
                    case MessageEnum.GetUsers_Request:
                    case MessageEnum.GetUserProfileImage_Request:
                        // yea i may have broken something :/
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("Error on message: " + ex.Message);
            }

        };


        // Add OnError event listener
        WS.OnError += (string errMsg) =>
        {
            MainThreadWorker.Instance.AddAction(() =>
            {
                WebsocketStatusText.text = $"WS: Error ({errMsg})";
                WebsocketStatusText.color = Color.red;
            });
            Debug.Log("WS error: " + errMsg);
        };



        // Add OnClose event listener
        WS.OnClose += (WebSocketCloseCode code) =>
        {
            MainThreadWorker.Instance.AddAction(() =>
            {
                WebsocketStatusText.text = $"WS: Disconnected ({code.ToString()})";
                WebsocketStatusText.color = Color.red;
                ReconnectButton.gameObject.SetActive(true);
            });
            Debug.Log("WS closed with code: " + code.ToString());
        };

        // Connect to the server
        WS.Connect();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private bool LiveMode = true;
    //Ouput the new value of the Dropdown into Text
    public void DropdownValueChanged(Dropdown change)
    {
        var value = change.value;

        if (value == 0)
        {
            // Live mode
            LoadChunksStatus.gameObject.SetActive(false);
            LoadChunks.gameObject.SetActive(false);
            LoadingChunks = false; // cance chunk loading
            LiveMode = true;
            WS.Send(new byte[1] { (byte)MessageEnum.FullImage_Request });
            // send new WS request to reload the image
        }
        else if (value == 1)
        {
            // Timelapse mode
            LoadChunksStatus.gameObject.SetActive(true);
            LoadChunks.gameObject.SetActive(true);

            if (CurrentChunk == TotalChunks)
            {
                LoadChunksStatus.text = $"Chunk {CurrentChunk}/{TotalChunks} (DONE)";
                LoadChunksStatus.color = Color.green;
            }
            else
            {
                CurrentChunk = 0;
                LoadChunksStatus.text = $"Chunk {CurrentChunk}/{TotalChunks}";
                LoadChunksStatus.color = Color.red;
            }


            LiveMode = false;
        }

        // m_Text.text = "New Value : " + change.value;
    }

    public void ButtonClicked(int buttonId)
    {
        try
        {
            Debug.Log("Button clicked = " + buttonId);
            if (buttonId == 0)
            {
                // load only if the chunk loading hasnt been done
                if (CurrentChunk < TotalChunks)
                {
                    // TODO do resume logic
                    LoadingChunks = true;

                    var request = new byte[3];
                    request[0] = (byte)MessageEnum.GetChunk_Request;
                    request[1] = 1; // first chunk
                    request[2] = 0;

                    WS.Send(request);
                }
            }
            else if (buttonId == 1)
            {
                DrawTimelapse();
            }
            else if (buttonId == 2)
            {
                ReconnectButton.gameObject.SetActive(false);

                WS.Connect();
            }
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.ToString());
        }
    }

    async Task DrawTimelapse()
    {

        int steps = 4_001;
        int totalPixels = TotalChunks * ChunkSize;

        int pixelsPerStep = totalPixels / steps;

        int pixelIndex = 0;


        var tex = GetComponent<Renderer>().material.mainTexture as Texture2D;
        var defaultGray = new Color32(54, 57, 63, 0);
        for (int i = 0; i < 1000; i++)
        {
            for (int j = 0; j < 1000; j++)
            {
                tex.SetPixel(i, j, defaultGray);
            }
        }
        MainThreadWorker.Instance.AddAction(() => { tex.Apply(); });

        for (int i = 0; i < steps; i++)
        {
            for (int j = 0; j < pixelsPerStep; j++)
            {
                if (LiveMode)
                    return; // cancel

                try
                {
                    var pixelHistoryEntry = PixelHistory.ElementAt(pixelIndex);
                    var randColor = new Color32(pixelHistoryEntry.R, pixelHistoryEntry.G, pixelHistoryEntry.B, 0);

                    tex.SetPixel(999 - pixelHistoryEntry.X, pixelHistoryEntry.Y, randColor);
                    //tex.SetPixel(UnityEngine.Random.Range(0, 1000), UnityEngine.Random.Range(0, 1000), randColor);;

                    pixelIndex++;

                    /*if (pixelIndex % UnityEngine.Random.Range(5, 20) == 0)
                    {
                        //MainThreadWorker.Instance.AddAction(() => { PixelCount.text = pixelIndex.ToString("N0"); });
                    }*/
                }
                catch (Exception ex)
                {

                }
            }


            var delay = DelayAsync(0.025f); // doesnt work in webgl yet

            // Draw the pixels
            MainThreadWorker.Instance.AddAction(() =>
            {
                TimelapseScrollbar.value = pixelIndex / (float)totalPixels;
                PixelCount.text = pixelIndex.ToString("N0") + " Pixels";
                tex.Apply();
            });

            await delay;
        }
    }

    public static async Task DelayAsync(float secondsDelay)
    {
        float startTime = Time.time;
        while (Time.time < startTime + secondsDelay) await Task.Yield();
    }
}
