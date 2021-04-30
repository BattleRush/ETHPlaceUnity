using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIInfo : MonoBehaviour
{
    public Text CameraInfo;
    public Camera Cam;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 pos = Cam.transform.position;
        var angles = Cam.transform.eulerAngles;
        var text = $"POS: {(int)pos.x}/{(int)pos.y}/{(int)pos.z} ROT: {(int)angles.x}/{(int)angles.y}/{(int)angles.z} FOV: {Cam.fieldOfView}";
        CameraInfo.text = text;
    }
}
