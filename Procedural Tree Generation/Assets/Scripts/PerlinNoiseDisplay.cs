using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerlinNoiseDisplay : MonoBehaviour
{
    public enum GenerationType
    {
        PerlinNoise,
        Texture
    }

    public GenerationType generationType;

    public Renderer textureRenderer;

    public Texture2D textureMap;

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

    public void ProceduralGeneration()
    {
        switch (generationType)
        {
            case GenerationType.PerlinNoise:
                {
                    DrawNoiseMap();
                }
                break;

            case GenerationType.Texture:
                {
                    DrawTextureMap();
                }
                break;
        }
    }

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
                    if (currentRange >= Trees[index].startRange && currentRange <= Trees[index].endRange)
                    {
                        GameObject tree = Instantiate(Trees[index].tree, new Vector3(((x - (meshWidth/2)) * (meshWidth/5)), 1, ((y - (meshLength/2))) * (meshLength/5)), Trees[index].tree.transform.rotation);
                        spawnedTrees.Add(tree);
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

    public void DrawTextureMap()
    {
        if (textureMap != null)
        {
            Color32[] pixels = textureMap.GetPixels32();

            for (int x = 0; x < textureMap.width; x++)
            {
                for (int y = 0; y < textureMap.height; y++)
                {
                    Color32 pixel = pixels[x + y * textureMap.width];
                    int p = ((256 * 256 + pixel.r) * 256 + pixel.b) * 256 + pixel.g;
                    int b = p % 256;
                    p = Mathf.FloorToInt(p / 256);
                    int g = p % 256;
                    p = Mathf.FloorToInt(p / 256);
                    int r = p % 256;
                    float l = (0.2126f * r / 255f) + 0.7152f * (g / 255f) + 0.0722f * (b / 255f);
                    Color c = new Color(l, l, l, 1);            
                    textureMap.SetPixel(x, y, c);

                    for (int index = 0; index < Trees.Length; index++)
                    {
                        if (l >= Trees[index].startRange && l <= Trees[index].endRange)
                        {
                            GameObject tree = Instantiate(Trees[index].tree, new Vector3((((x * meshWidth/2)/25) - (meshWidth * 5.1f)), 1, ((y * meshLength)/25) - (meshLength * 5.1f)), Trees[index].tree.transform.rotation);
                            spawnedTrees.Add(tree);
                        }
                    }
                }
            }
            textureMap.Apply();

            textureRenderer.sharedMaterial.mainTexture = textureMap;
        }
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

    public void DeleteObjects()
    {
        for (int index = 0; index < spawnedTrees.Count; index++)
        {
            DestroyImmediate(spawnedTrees[index]);
            spawnedTrees.Remove(spawnedTrees[index]);
        }
    }

    [System.Serializable]
    public class TreeType
    {
        public string name;
        public float startRange;
        public float endRange;
        public GameObject tree;
    }
}
