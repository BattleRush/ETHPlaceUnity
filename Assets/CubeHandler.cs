using Assets.Websocket;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CubeHandler : MonoBehaviour
{
    // Start is called before the first frame update


    public GameObject cube;
    protected AudioSource placeSound;

    public Color Color;
    public GameObject plane;
    private float SpawnTime;
    private float LastExplosionTime;
    public Toggle ToggleExplosions;
    public Toggle ToggleInfo;
    public Toggle Toggle2D;
    public Canvas Canvas;
    public Camera MainCamera;

    public string Username;
    public byte[] Image;
    public bool IsBot;



    void Start()
    {
        Color = GetComponent<MeshRenderer>().material.color;
        placeSound = GetComponent<AudioSource>();
        SpawnTime = Time.time * 1000;
        plane = GameObject.Find("Plane");

        // TODO dont hardcode index
        try
        {
            var texLabels = Canvas.GetComponentsInChildren<TextMeshProUGUI>();
            texLabels[0].text = Username;
            texLabels[1].text = $"Position: {(int)transform.position.x}/{999 - (int)transform.position.z}";
            texLabels[2].text = $"Color ({Color.r * 255}, {Color.g * 255}, {Color.b * 255})";
        }
        catch (System.Exception ex)
        {

        }

        // TODO 
        Texture2D tex = new Texture2D(128, 128, TextureFormat.ARGB32, false);
        tex.LoadImage(Image);

        /*
                Texture2D texColor = new Texture2D(32, 32, TextureFormat.RGB24, false);
                int colorImageSize = 32;

                for (int i = 0; i < colorImageSize; i++)
                    for (int j = 0; j < colorImageSize; j++)
                        texColor.SetPixel(i, j, new Color(Color.r * 255, Color.g * 255, Color.b * 255));

                texColor.Apply();*/


        Color.a = 1;
        var rawImages = Canvas.GetComponentsInChildren<RawImage>();
        rawImages[0].texture = tex;
        rawImages[1].color = Color;


        // IsBot
        var panel = Canvas.gameObject.transform.GetChild(7);
        panel.gameObject.SetActive(IsBot);

        /*
        var images = Canvas.GetComponentsInChildren<Image>();
        images[2].color = Color;*/

        Canvas.enabled = ToggleInfo.isOn;
        if (Canvas.enabled)
        {
            var canvasGroup = Canvas.GetComponent<CanvasGroup>();
            if(canvasGroup != null)
                canvasGroup.alpha = 1;
        }

    }
    private bool PlayedSound = false;
    private bool PixelDrawn = false;
    // Update is called once per frame
    void Update()
    {
        //Canvas.transform.LookAt(new Vector3(MainCamera.transform.position.x * -1, MainCamera.transform.position.y, MainCamera.transform.position.z * -1));
        if (!Toggle2D.isOn)
        {
            Canvas.transform.LookAt(new Vector3(MainCamera.transform.position.x, MainCamera.transform.position.y, MainCamera.transform.position.z));
            Canvas.transform.Rotate(0, 180, 0, Space.Self);
        }
        else
        {
            Canvas.transform.rotation = Quaternion.Euler(new Vector3(90, 0, 0));
        }
        //Canvas.transform.LookAt(MainCamera.transform);
        // this is the original cube fine a better way to fix this
        if (cube.transform.position.y < -20 && cube.transform.position.y > -25)
            return;

        if (SpawnTime > 0 && SpawnTime + 1200 < Time.time * 1000 && !PlayedSound && placeSound != null)
        {
            // sound disabled for now
            //placeSound.Play();

            // some may miss a sound but thats fine as we would overload the queue (for now but find a good solution later)
            PlayedSound = true;

        }

        var pos = cube.transform.position;
        var scale = cube.transform.localScale;
        if (SpawnTime > 0 && SpawnTime + 20000 < Time.time * 1000 && pos.y < 0.6f)
        {


            if (!PixelDrawn)
            {
                var tex = plane.GetComponent<Renderer>().material.mainTexture as Texture2D;

                //Color.red;//


                tex.SetPixel((int)(999 - pos.x) + 1, (int)(999 - pos.z) + 1, Color);


                tex.Apply();
                PixelDrawn = true;
            }

            //Physics.IgnoreCollision(cube.GetComponent<Collider>(), GetComponent<Collider>());
            //var colider = cube.GetComponent<Collider>();
            //colider.enabled = false;

            var rigidBody = cube.GetComponent<Rigidbody>();
            rigidBody.useGravity = false;

            scale.y -= 0.001f;
            // we can start to sink the object
            pos.y -= 0.0005f;

            cube.transform.position = pos;
            cube.transform.localScale = scale;
        }


        // todo handle this better set the time when the block starts to lower
        if (SpawnTime + 40000 < Time.time * 1000 || scale.y < 0.001f)
        {
            // destroy
            Object.DestroyImmediate(cube);
        }
    }

    private bool Faded = true;
    private Color startcolor;
    void OnMouseEnter()
    {
        if (!ToggleInfo.isOn)
        {
            var canvasGroup = Canvas.GetComponent<CanvasGroup>();

            DoFade(canvasGroup, canvasGroup.alpha, Faded ? 1f : 0f);

            Faded = !Faded;


            //Canvas.enabled = true;
            var pos = cube.transform.position;


            //pos.x; 
            pos.y += 1.5f;

            Canvas.transform.position = pos;
        }

        var mesh = cube.GetComponent<MeshRenderer>();
        startcolor = mesh.material.color;
        mesh.material.color = Color.white;
        //SpawnTime = Time.time;

    }
    void OnMouseExit()
    {
        if (!ToggleInfo.isOn)
        {
            var canvasGroup = Canvas.GetComponent<CanvasGroup>();

            DoFade(canvasGroup, canvasGroup.alpha, Faded ? 1f : 0f);

            Faded = !Faded;
        }

        cube.GetComponent<MeshRenderer>().material.color = startcolor;
    }

    public float Duration = 1f;
    void DoFade(CanvasGroup group, float start, float end)
    {
        /*
        MainThreadWorker.Instance.AddAction(() =>
        {
            float counter = 0f;
            while (counter < Duration)
            {
                counter += Time.deltaTime;
                group.alpha = Mathf.Lerp(start, end, counter / Duration);
            }
        });*/
    }

    private bool explosionOnPlace = false;

    private void OnCollisionEnter(Collision collision)
    {
        try
        {
            explosionOnPlace = ToggleExplosions.isOn;
            if (SpawnTime + 500 < Time.time * 1000 || collision.gameObject.name == "Plane")
            {
                if (!collision.gameObject.name.StartsWith("SpawnCube("))
                {
                    Debug.Log(collision.gameObject.name);
                }

                if ((explosionOnPlace && collision.gameObject.name == "Plane")
                    || collision.gameObject.name.StartsWith("SpawnCube(") && collision.transform.position.x == transform.position.x && collision.transform.position.z == transform.position.z && LastExplosionTime + 750 < Time.time * 1000)
                {
                    var explosion = GameObject.Find("Explosion");

                    var instObj = Instantiate(explosion, new Vector3(transform.position.x, transform.position.y - 0.5f, transform.position.z), Quaternion.identity);
                    LastExplosionTime = Time.time * 1000;

                    Destroy(instObj, 5);
                }
            }
        }
        catch (System.Exception ex)
        {

        }
    }
}
