using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoxelFarm : MonoBehaviour
{
    public GameObject currentBlock;

    // Start is called before the first frame update
    void Start()
    {
        //generateTerrain();
    }

    void generateTerrain()
    {
        int cols = 250;
        int rows = cols;

        float amp = 10f;
        float freq = 10f;

        for (int i = 0; i < cols; i++)
        {
            for (int j = 0; j < rows; j++)
            {
                float y = Mathf.PerlinNoise(i / freq, j / freq) * amp;
                GameObject newBlock = GameObject.Instantiate(currentBlock);

                newBlock.transform.position = new Vector3(i, y, j);
            }
        }
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
