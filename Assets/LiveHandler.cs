using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiveHandler : MonoBehaviour
{
   /* public GameObject master;
    public string serveradress = "ws://";

    private WebSocket w = null;

    // Use this for initialization
    IEnumerator Start () {
        w = new WebSocket(new Uri(serveradress));
        yield return StartCoroutine(w.Connect());

        w.SendString ("Hello World from client");
        //w.SendString("Hi there");
        int i=0;
        while (true)
        {
            string reply = w.RecvString();
            if (reply != null)
            {
                master.GetComponent<Networker> ().onNetworkMessage (reply);
                //Debug.Log ("Received: "+reply);
                //w.SendString("Hi there"+reply);
            }
            if (w.error != null)
            {
                Debug.LogError ("Error: "+w.error);
                break;
            }
            w.SendString("helloworld");
            yield return 0;
        }
        w.Close();
    }

    public void sendString(string message){
        sendRaw(Encoding.UTF8.GetBytes (message));
    }

    public void sendRaw(byte[] send){
        w.Send (send);
    }

    // Update is called once per frame
    void Update()
    {
        
    }*/
}
