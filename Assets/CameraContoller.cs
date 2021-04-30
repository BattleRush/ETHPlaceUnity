using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraContoller : MonoBehaviour
{
    public float panSpeed = 20f;
    public float panBorderThickness = 10f;

    public Vector2 panLimit;

    public float scrollSpeed = 20f;

    public float minY = 10f;
    public float maxY = 250f;
    Camera cam;
    public Material frontPlane;

    public GameObject block;


    void SampleTerain()
    {
        int chunkY = 25;
        int chunkZ = 25;
        int chunkX = 25;
        float Flatscale = 1f;

        float posX = 0;
        float posZ = 0;

        for (int y = 0; y < chunkY; y++)
        {

            for (int z = 0; z < chunkZ; z++)
            {

                for (int x = 0; x < chunkX; x++)
                {
                    //Terrain algorithm
                    float flatNoise = Mathf.Round(Mathf.PerlinNoise((transform.position.x + x) * Flatscale, (transform.position.z + z) * Flatscale) * 10);
                    float height = flatNoise;

                    Vector3 pos = new Vector3(posX, height, posZ);
                    GameObject newBlock = Instantiate(block, pos, Quaternion.identity) as GameObject;
                    newBlock.transform.parent = this.transform;
                    posX += 1;
                }
                posX = transform.position.x;
                posZ += 1;
            }
            posX = transform.position.x;
            posZ = transform.position.z;
        }
    }

    void Start()
    {
        //SampleTerain();
        cam = GetComponent<Camera>();

        // Create the Plane
        //GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);

        //plane.transform.localScale = new Vector3(100, 1, 100);

        //Vector3 pos = plane.transform.position;

        //pos.x = 500;
        //pos.z = 500;

        //plane.transform.position = pos;

        //plane.size
        // Load the Image from somewhere on disk
        var filePath = "I:\\Download\\base.png";
        if (System.IO.File.Exists(filePath))
        {
            // Image file exists - load bytes into texture
            var bytes = System.IO.File.ReadAllBytes(filePath);
            var tex = new Texture2D(1, 1);
            tex.LoadImage(bytes);
            frontPlane.mainTexture = tex;

            // Apply to Plane
            //MeshRenderer mr = plane.GetComponent<MeshRenderer>();
            //mr.material = frontPlane;
        }


    }

    //https://gist.github.com/gunderson/d7f096bd07874f31671306318019d996

    float mainSpeed = 25.0f; //regular speed
    float shiftAdd = 100.0f; //multiplied by how long shift is held.  Basically running
    float maxShift = 200.0f; //Maximum speed when holdin gshift
    float camSens = 0.1f; //How sensitive it with mouse
    private Vector3 lastMouse = new Vector3(255, 255, 255); //kind of in the middle of the screen, rather than at the top (play)
    private float totalRun = 1.0f;



    public GameObject cameraOrbit;

    public float rotateSpeed = 0.25f;


    private Vector3 screenPos;
    private float angleOffset;


    public float speed = 50.5f;
    private float rotationX = float.MinValue;
    private float rotationY = float.MinValue;


    public float dragSpeed = 2;
    private Vector3 dragOrigin;

    void Update()
    {
        if (Input.GetMouseButton(1) && !is2DEnabled)
        {
            //var horizontalRotation = -Input.GetAxis("Mouse Y") * Time.deltaTime * speed;
            //var verticalRotation = Input.GetAxis("Mouse X") * Time.deltaTime * speed;
            //transform.Rotate(horizontalRotation, verticalRotation, 0);
            //transform.rotation = Quaternion.Euler(Mathf.Clamp(transform.rotation.eulerAngles.x, 11.22872f, 12.0f), Mathf.Clamp(transform.rotation.eulerAngles.y, 178.5f, 181.5f), 0);

            //Camera.main.transform.Rotate(new Vector3(0, Input.GetAxis("Mouse X") * rotateSpeed, 0), Space.World);
            //Camera.main.transform.Rotate(new Vector3(Input.GetAxis("Mouse Y") * rotateSpeed, 0, 0), Space.World);
            
            if(rotationX == float.MinValue)
            {
                rotationX = transform.eulerAngles.x;
                rotationY = transform.eulerAngles.y;
            }
            
            rotationY -= rotateSpeed * Input.GetAxis("Mouse X");
            rotationX += rotateSpeed * Input.GetAxis("Mouse Y");

            rotationX = Mathf.Clamp(rotationX, -60, 90);

            //var xClamp = Mathf.Clamp(Camera.main.transform.rotation.eulerAngles.x, -80, 80);
            //var yClamp = Mathf.Clamp(Camera.main.transform.rotation.eulerAngles.y, -180, 180);

            transform.eulerAngles = new Vector3(rotationX, rotationY, 0);



            //transform.rotation = Quaternion.Euler(Mathf.Clamp(transform.rotation.eulerAngles.x, 11.22872f, 12.0f), Mathf.Clamp(transform.rotation.eulerAngles.y, 178.5f, 181.5f), 0);
        }


        /*  if (Input.GetMouseButtonDown(0))
          {
              dragOrigin = Input.mousePosition;
              return;
          }

          if (!Input.GetMouseButton(0)) return;

          Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - dragOrigin);
          Vector3 move = new Vector3(pos.x * dragSpeed, 0, pos.y * dragSpeed);

          transform.Translate(move, Space.World);*/

        //if (Input.GetMouseButton(1))
        //{
        //    transform.Rotate(new Vector3(Input.GetAxis("Mouse Y") * speed, -Input.GetAxis("Mouse X") * speed, 0));
        //    X = transform.rotation.eulerAngles.x;
        //    Y = transform.rotation.eulerAngles.y;
        //    transform.rotation = Quaternion.Euler(X, Y, 0);
        //}


        /*  if (Input.GetMouseButton(1))
              {
                  float h = rotateSpeed * Input.GetAxis("Mouse X");
                  float v = rotateSpeed * Input.GetAxis("Mouse Y");

                  /*if (transform.eulerAngles.z + v <= 0.1f || transform.eulerAngles.z + v >= 179.9f)
                          v = 0;*/
        /*
                            transform.eulerAngles = new Vector3(transform.eulerAngles.x, 0, transform.eulerAngles.z + v);

                        }*/
        /*
                                        lastMouse = Input.mousePosition - lastMouse ;
                    lastMouse = new Vector3(-lastMouse.y * camSens, lastMouse.x * camSens, 0 );
                    lastMouse = new Vector3(transform.eulerAngles.x + lastMouse.x , transform.eulerAngles.y + lastMouse.y, 0);
                    transform.eulerAngles = lastMouse;
                    lastMouse =  Input.mousePosition;*/

        //Mouse  camera angle done.  


        Vector3 pos = transform.position;

        if (is2DEnabled)
        {
            int speed = (Input.GetKey(KeyCode.LeftShift) ? 10 : 2);
            if (Input.GetKey("w") || Input.mousePosition.y >= Screen.height - panBorderThickness && panBorderThickness > 0)
                pos.z += panSpeed * Time.deltaTime * speed;

            if (Input.GetKey("s") || Input.mousePosition.y <= panBorderThickness && panBorderThickness > 0)
                pos.z -= panSpeed * Time.deltaTime * speed;

            if (Input.GetKey("d") || Input.mousePosition.x >= Screen.width - panBorderThickness && panBorderThickness > 0)
                pos.x += panSpeed * Time.deltaTime * speed;

            if (Input.GetKey("a") || Input.mousePosition.x <= panBorderThickness && panBorderThickness > 0)
                pos.x -= panSpeed * Time.deltaTime * speed;

            transform.position = pos;
        }
        else
        {
            if (Input.GetKey("w"))
                transform.localPosition += transform.forward * Time.deltaTime * (Input.GetKey(KeyCode.LeftShift) ? 10 : 1) * panSpeed;

            if (Input.GetKey("s"))
                transform.localPosition -= transform.forward * Time.deltaTime * (Input.GetKey(KeyCode.LeftShift) ? 10 : 1) * panSpeed;

            if (Input.GetKey("d"))
                transform.localPosition += transform.right * Time.deltaTime * (Input.GetKey(KeyCode.LeftShift) ? 10 : 1) * panSpeed;

            if (Input.GetKey("a"))
                transform.localPosition -= transform.right * Time.deltaTime * (Input.GetKey(KeyCode.LeftShift) ? 10 : 1) * panSpeed;
        }

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        pos.y -= scroll * scrollSpeed * 100f * Time.deltaTime * (Input.GetKey(KeyCode.LeftShift) ? 10 : 1);

        pos.x = Mathf.Clamp(pos.x, -100, panLimit.x);
        pos.y = Mathf.Clamp(pos.y, minY, maxY);
        pos.z = Mathf.Clamp(pos.z, -100, panLimit.y);

        //transform.position = pos;

        cam.fieldOfView -= scroll * 10f;
        cam.fieldOfView = Mathf.Clamp(cam.fieldOfView, 1, 120);

    }
    private bool is2DEnabled = false;
    public void valueChanged(Toggle t)
    {
        if (t.isOn)
        {
            Vector3 pos = transform.position;
            pos.x = 500;
            pos.y = 500;
            pos.z = 500;
            //cam.fieldOfView = 90f;
            transform.position = pos;
            transform.eulerAngles = new Vector3(90, 0, 0);
            //t.GetComponentInChildren<Text>().text = "Toggle is on";
        }
        else
        {
            Vector3 pos = transform.position;
            pos.x = 500;
            pos.y = 400;
            pos.z = 180;
            //cam.fieldOfView = 90f;
            transform.position = pos;
            transform.eulerAngles = new Vector3(70, 0, 0);
            //t.GetComponentInChildren<Text>().text = "Toggle is off";
        }
        is2DEnabled = t.isOn;
    }

   
}
