using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerlinNoiseDisplay : MonoBehaviour
{
    public Renderer textureRenderer;

    public int meshWidth;
    public int meshLength;
    public float noiseScale;

    public int octaves;
    [Range(0, 1)]
    public float persistance;
    public float lacunarity;

    public int seed;
    public Vector2 offset;

    public bool autoUpdate;

    public TreeType[] Trees;
    public List<GameObject> spawnedTrees;

    public void DrawNoiseMap()
    {
        float[,] perlinNoise = PerlinNoise.GenerateNoiseMap(meshWidth, meshLength, seed, noiseScale, octaves, persistance, lacunarity, offset);

        int width = perlinNoise.GetLength(0);
        int length = perlinNoise.GetLength(1);

        Texture2D texture = new Texture2D(width, length);

        Color[] colourMap = new Color[width * length];

        for (int y = 0; y < length; y++)
        {
            for (int x = 0; x < width; x++)
            {
                colourMap[y * width + x] = Color.Lerp(Color.black, Color.white, perlinNoise[x, y]);

                for (int index = 0; index < Trees.Length; index++)
                {
                    float currentRange = perlinNoise[x, y];
                    if (currentRange <= Trees[index].range)
                    {
                        GameObject tree = Instantiate(Trees[index].tree, new Vector3((x * meshLength)/2, 1, (y * meshWidth)/2), Trees[index].tree.transform.rotation);
                        break;
                    }
                }
            }
        }

        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.SetPixels(colourMap);
        texture.Apply();

        textureRenderer.sharedMaterial.mainTexture = texture;
        textureRenderer.transform.localScale = new Vector3(width, 1, length);
    }

    void OnValidate()
    {
        if (meshWidth < 1)
        {
            meshWidth = 1;
        }
        if (meshLength < 1)
        {
            meshLength = 1;
        }
        if (lacunarity < 1)
        {
            lacunarity = 1;
        }
        if (octaves < 0)
        {
            octaves = 0;
        }
    }

    [System.Serializable]
    public class TreeType
    {
        public string name;
        public float range;
        public GameObject tree;
    }
}
