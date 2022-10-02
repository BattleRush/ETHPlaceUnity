using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Texture2DEditor : MonoBehaviour
{
    Texture2D t2D;
    RectTransform rect;

    public Plane plane;

    private Material material;
    private Texture2D texture;
    // Start is called before the first frame update
    void Start()
    {
        material = GetComponent<Renderer>().material;
        texture = material.mainTexture as Texture2D;
        t2D = new Texture2D(1000, 1000);
        t2D.filterMode = FilterMode.Point;
        try
        {
            t2D.SetPixels32(texture.GetPixels32());
        }
        catch (Exception ex)
        {

        }
        t2D.Apply();

        material.mainTexture = t2D;
        var pixelData = t2D.GetPixels32();

        //print($"Total pixels: ${pixelData.Length}");

        var colorIndex = new List<Color>();

        /*for (int i = 0; i < pixelData.Length; i++)
        {
            var color = pixelData[i];


            if (colorIndex.IndexOf(color) == -1)
                colorIndex.Add(color);
        }*/

        //print($"Indexed colors: ${colorIndex.Count}");

    }

    // Update is called once per frame
    void Update()
    {
        /*
        if (Input.GetMouseButton(0))
        {
            var tex = GetComponent<Renderer>().material.mainTexture as Texture2D;

            //Color.red;//

            var randColor = new Color32((byte)Random.Range(0, 255), (byte)Random.Range(0, 255), (byte)Random.Range(0, 255), 0);

            for (int y = 0; y < 45; y++)
            {

                int randX = Random.Range(0, 999);
                int randY = Random.Range(0, 999);
                tex.SetPixel(randX, randY, randColor);

            }

            tex.Apply();
        }*/
    }
}
