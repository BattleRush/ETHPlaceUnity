using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public Transform spawnPos;
    public GameObject spawnee;

    public GameObject plane;


    private AudioSource placeSound;
    public int size = 25;



    // Start is called before the first frame update
    void Start()
    {
        placeSound = GetComponent<AudioSource>();
        if (false)
        {

            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    //spawnee.GetComponent<Renderer>().material.color = 

                    var instObj = Instantiate(spawnee, new Vector3(i * 1.0F, 5, j * 1.0F), Quaternion.identity);
                    instObj.GetComponent<MeshRenderer>().material.color = new Color32((byte)Random.Range(0, 255), (byte)Random.Range(0, 255), (byte)Random.Range(0, 255), 0);

                    //Object.Destroy(instObj, 10.0f); // should destroy instantiated Projectile after 10s

                }
            }
        }

        int gridSize = 1000;
        // bottom left
        /*
                var instObj2 = Instantiate(spawnee, new Vector3(0, 10, 0), Quaternion.identity);
                instObj2.GetComponent<MeshRenderer>().material.color = Color.blue;


                // bottom right

                instObj2 = Instantiate(spawnee, new Vector3(gridSize, 10, 0), Quaternion.identity);
                instObj2.GetComponent<MeshRenderer>().material.color = Color.yellow;


                // top left
                instObj2 = Instantiate(spawnee, new Vector3(0, 10, gridSize), Quaternion.identity);
                instObj2.GetComponent<MeshRenderer>().material.color = Color.green;

                // top right
                instObj2 = Instantiate(spawnee, new Vector3(gridSize, 10, gridSize), Quaternion.identity);
                instObj2.GetComponent<MeshRenderer>().material.color = Color.red;*/

    }

    // Update is called once per frame

    private float LastPlacement;
    private int xBlock = 0;
    private int yBlock = 0;



    IEnumerator ExecuteAfterTime(float time, System.Action task)
    {

        yield return new WaitForSeconds(time);
        task();
    }

    void Update()
    {

        //var tex = plane.GetComponent<Renderer>().material.mainTexture as Texture2D;

        //Color.red;//


        //tex.SetPixel(0, 0, Color.red);
        //tex.SetPixel(999, 0, Color.green);
        //tex.SetPixel(0, 999, Color.blue);
        //tex.SetPixel(999, 999, Color.yellow);

        //tex.Apply();



        if (false /*Input.GetMouseButton(0) || *//*LastPlacement < Time.time * 1000 - 50*/)
        {
            LastPlacement = Time.time * 1000;

            int gridSize = 50;


            // for(int i = 0; i < size; i++){
            Color col = new Color32((byte)Random.Range(0, 255), (byte)Random.Range(0, 255), (byte)Random.Range(0, 255), 0);// , (byte)Random.Range(0, 255), 0);
            //Random.Range (0, 2) == 1 ? Color.black : Color.white;

            //Color col = xBlock % 2 == 0 ? Color.red : Color.green;
            //Color col = Random.Range(0, 2) == 1 ? Color.black : Color.white;

            //var instObj = Instantiate(spawnee, new Vector3(Random.Range(0, gridSize) * 1.0F, 10, Random.Range(0, gridSize) * 1.0F), Quaternion.identity);



            int height = 10; // was 10

            var instObj = Instantiate(spawnee, new Vector3(xBlock * 1.0F + 0.5F, height, yBlock * 1.0F + 0.5F), Quaternion.identity);

            instObj.GetComponent<MeshRenderer>().material.color = col;
            instObj.AddComponent<AudioSource>();


            var audio = instObj.GetComponent<AudioSource>();
            audio.clip = placeSound.clip;

            /*StartCoroutine(ExecuteAfterTime(2f, () =>
            {



            }));*/






            xBlock++;
            if (xBlock > gridSize)
            {
                xBlock = 0;
                yBlock++;
            }

            if (yBlock > gridSize)
            {
                xBlock = 0;
                yBlock = 0;
            }

            //}
        }

    }
}
