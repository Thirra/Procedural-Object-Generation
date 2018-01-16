using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The types of trees that can be generated, with variables that can be adjusted as needed.
/// </summary>
[System.Serializable]
public class TreeType
{
    public string name;
    [Range(0, 1)]
    public float startRange;
    [Range(0, 1)]
    public float endRange;
    [Range(0, 100)]
    public float spawnDensity;
    public GameObject tree;
    [HideInInspector]
    public List<GameObject> spawnedTrees;
}

[ExecuteInEditMode]
public class TreeGenerationDisplay : MonoBehaviour
{
    //Current procedural generation types
    public enum GenerationType
    {
        PerlinNoise,
        Texture
    }

    public GenerationType generationType;

    public List<TreeType> Trees;

    #region Mesh Variables
    public Renderer textureRenderer;
    public GameObject mesh;
    public int meshWidth;
    public int meshLength;
    #endregion

    #region Texture Variables
    public Texture2D textureMap;
    #endregion

    #region PerlinNoise Variables
    public float noiseScale;
    public int octaves;
    [Range(0, 1)]
    public float persistance;
    public float lacunarity;
    public int seed;
    public Vector2 offset;
    #endregion

    /// <summary>
    /// Drawing the perlin noise map to a mesh and instantiating trees to specific pixel colour ranges.
    /// </summary>
    /// <param name="treeTypeIndex"></param>
    public void DrawNoiseMap(int treeTypeIndex)
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

                float currentRange = perlinNoise[x, y];
                if (currentRange >= Trees[treeTypeIndex].startRange && currentRange <= Trees[treeTypeIndex].endRange)
                {
                    GameObject tree = Instantiate(Trees[treeTypeIndex].tree, new Vector3(((x - (meshWidth/2)) * (meshWidth/5)), 1, ((y - (meshLength/2))) * (meshLength/5)), Trees[treeTypeIndex].tree.transform.rotation);
                    Trees[treeTypeIndex].spawnedTrees.Add(tree);
                    break;
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

    /// <summary>
    /// Applying a texture to a mesh, changing it to greyscale and instantiating trees based off colour pixel ranges.
    /// </summary>
    /// <param name="treeTypeIndex"></param>
    public void DrawTextureMap(int treeTypeIndex)
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

                    if (l > Trees[treeTypeIndex].startRange && l < Trees[treeTypeIndex].endRange)
                    {
                        Vector3 thisPosition = new Vector3((((x * meshWidth / 2) / 25) - (meshWidth * 5f)), 10, ((y * meshLength) / 25) - (meshLength * 5f));
                        int randomNumber = Random.Range(0, 100);

                        Collider[] overlapObjects = Physics.OverlapSphere(thisPosition, 5);
                        if (overlapObjects.Length == 0 && randomNumber <= Trees[treeTypeIndex].spawnDensity)
                        {
                            GameObject tree = Instantiate(Trees[treeTypeIndex].tree, thisPosition, Trees[treeTypeIndex].tree.transform.rotation);
                            Trees[treeTypeIndex].spawnedTrees.Add(tree);
                        }
                    }
                }
            }
            textureMap.Apply();
            textureRenderer.sharedMaterial.mainTexture = textureMap;
        }
    }

    /// <summary>
    /// Checking how far above the mesh the objects have been instantiated through a raycast, so that trees can be placed onto meshes of varying heights.
    /// </summary>
    /// <param name="tree"></param>
    public void CheckMeshHeight(GameObject tree)
    {
        float maxDistance = (tree.GetComponent<Collider>().bounds.size.y / 2) + 0.1f;

        RaycastHit hit;
        if (Physics.Raycast(tree.transform.position, -Vector3.up, out hit))
        {
            if (hit.distance > maxDistance)
            {
                float amountToMove = hit.distance - maxDistance;
                Vector3 position = tree.transform.position;
                position.y -= amountToMove;
                tree.transform.position = position;
            }
        }
    }

    /// <summary>
    /// A function to perform the tree movement according to mesh height.
    /// </summary>
    /// <param name="treeIndex"></param>
    public void MoveGameObjects(int treeIndex)
    {
        for (int index = 0; index < Trees[treeIndex].spawnedTrees.Count; index++)
        {
            CheckMeshHeight(Trees[treeIndex].spawnedTrees[index]);
        }
    }

    /// <summary>
    /// Deleting all of the objects in a list of trees. The button for this needs to be pressed more than once to entirely remove all of the trees.
    /// </summary>
    /// <param name="index"></param>
    public void DeleteObjects(int index)
    {
        for (int i = 0; i < Trees[index].spawnedTrees.Count; i++)
        {
            DestroyImmediate(Trees[index].spawnedTrees[i]);
            Trees[index].spawnedTrees.Remove(Trees[index].spawnedTrees[i]);
        }
    }

    /// <summary>
    /// Validating some values so that no error occurs.
    /// </summary>
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
}
