using System.Collections.Generic;
using UnityEditor;
using UnityEngine;



[ExecuteInEditMode]
public class PGCTerrain : MonoBehaviour
{
    //properties Terrain
    public Terrain terrain;
    public TerrainData terrainData;
    //-----override (reset) terrian
    public bool overrideTerrain = false;
    //----random height
    public Vector2 randomHeightRange = new Vector2(0, 0.1f);
    //----Height Map Data from a texture 
    public Texture2D heightMapImage;
    public Texture2D pTexture_Vonoroi;
    public Texture2D pTexture_Midpoint;
    public Vector3 heightMapScale = new Vector3(1, 1, 1);
    public float scaleTexture = 20;
    public float offsetX = 100;
    public float offsetY = 100;
    //---- Perlin Noise
    public float perlinXScale = 0.01f;
    public float perlinYScale = 0.01f;
    public int perlinOffsetX = 0;
    public int perlinOffsetY = 0;
    //--- combine with BrownianMotion
    public int perlinOctaves = 3;
    public float perlinPersistance = 5;
    public float perlinHeightScale = 0.1f;
    //---- Multiple Perline Noise
    public List<PerlinList> perlinList = new List<PerlinList>() { new PerlinList() };
    //----- Voronoi      
    public enum VoronoiType { Voronoi_Power, Voronoi_LinPow, Voronoi_SinPow };
    public VoronoiType voronoiType = VoronoiType.Voronoi_Power;
    public int voronoiVertices = 4;
    public float voronoiFallOff = 0.1f;
    public float voronoiDropOff = 0.5f;
    public float voronoiMaxHeight = 0.5f;
    public float voronoiMinHeight = 0.1f;
    //------ Midpoint Displacement
    public float midMinHeight = -3.0f;
    public float midMaxHeight = 3.0f;
    public float midSlipPowerHeight = 3.0f;
    public float midRoughbess = 3.0f;
    //------ Smooth 
    public int amoutSmooth = 1;
    public float processSmooth = 0.0f;


    //*********************************
    void OnEnable()
    {
        //print("Init terrain data");
        terrain = GetComponent<Terrain>();
        terrainData = Terrain.activeTerrain.terrainData;
        pTexture_Vonoroi = new Texture2D(terrainData.heightmapWidth, terrainData.heightmapWidth, TextureFormat.ARGB32, false);
        pTexture_Midpoint = new Texture2D(terrainData.heightmapWidth - 1, terrainData.heightmapWidth - 1, TextureFormat.ARGB32, false);
    }

    //***********************************
    public void ResetTerrain()
    {
        print("button Reset Terrain");
        float[,] heightMap;
        heightMap = new float[terrainData.heightmapWidth, terrainData.heightmapHeight];
        for (int x = 0; x < terrainData.heightmapWidth; x++)
        {
            for (int z = 0; z < terrainData.heightmapHeight; z++)
            {
                heightMap[x, z] = 0;
            }
        }
        terrainData.SetHeights(0, 0, heightMap);
    }
    //
    public void RandomHeightTerrain()
    {
        print("button RandomTerrain");
        float[,] heightMap;
        heightMap = GetHeightMap();
        for (int x = 0; x < terrainData.heightmapWidth; x++)
        {
            for (int z = 0; z < terrainData.heightmapHeight; z++)
            {
                heightMap[x, z] += UnityEngine.Random.Range(randomHeightRange.x, randomHeightRange.y);
            }
        }
        terrainData.SetHeights(0, 0, heightMap);
    }
    //
    public void SinHeightTerrain()
    {
        float[,] heightMap;
        heightMap = GetHeightMap();
        float amplitude = 0.1f;
        float frequency = 0.1f;
        for (int x = 0; x < terrainData.heightmapWidth; x++)
        {
            for (int z = 0; z < terrainData.heightmapHeight; z++)
            {
                float y = amplitude * Mathf.Sin(x * frequency);
                heightMap[x, z] += y;
            }
        }
        terrainData.SetHeights(0, 0, heightMap);
    }
    //
    public void CreateTexture()
    {
        TextureClass textureClass = new TextureClass(scaleTexture, offsetX, offsetY);
        heightMapImage = textureClass.GenerateTexture();
    }

    public void LoadTexture()
    {
        ReadTexture(heightMapImage);
    }

    public void ReadTexture(Texture2D _heightMapImage)
    {

        float[,] heightMap;
        heightMap = GetHeightMap();
        for (int x = 0; x < terrainData.heightmapWidth; x++)
        {
            for (int z = 0; z < terrainData.heightmapHeight; z++)
            {
                // += add into the exist
                heightMap[x, z] += _heightMapImage.GetPixel(
                     (int)(x * heightMapScale.x), (int)(z * heightMapScale.z)
                                                         ).grayscale * heightMapScale.y;
            }
        }
        terrainData.SetHeights(0, 0, heightMap);
    }
    //    
    public void PerlinNoise()
    {
        float[,] heightMap = GetHeightMap();
        for (int y = 0; y < terrainData.heightmapHeight; y++)
        {
            for (int x = 0; x < terrainData.heightmapWidth; x++)
            {
                //simple perlineNoise
                //heightMap[x,y] = Mathf.PerlinNoise( (perlinOffsetX + x)  * perlinXScale, (perlinOffsetY + y) * perlinYScale);
                // using BrownianMotion
                heightMap[x, y] += BrownianMotion(
                    (perlinOffsetX + x) * perlinXScale,
                    (perlinOffsetY + y) * perlinYScale,
                    perlinOctaves,
                    perlinPersistance
                                                     ) * perlinHeightScale;

            }
        }
        terrainData.SetHeights(0, 0, heightMap);
    }

    float BrownianMotion(float x, float y, int oct, float persistance)
    {
        float result = 0;
        float frequency = 1;
        float amplitude = 1;
        float maxValue = 0;
        for (int i = 0; i < oct; i++)
        {
            result += Mathf.PerlinNoise(x * frequency, y * frequency) * amplitude;
            maxValue += amplitude;
            amplitude *= persistance;
            frequency *= 2;
        }
        return result / maxValue;
    }
    //-----
    public void MultiplePerlinNoise()
    {
        float[,] heightMap = GetHeightMap();
        for (int y = 0; y < terrainData.heightmapHeight; y++)
        {
            for (int x = 0; x < terrainData.heightmapWidth; x++)
            {
                foreach (PerlinList perlin in perlinList)
                {
                    heightMap[x, y] += BrownianMotion(
                        (x + perlin.mul_perlinOffsetX) * perlin.mul_perlinXScale,
                        (y + perlin.mul_perlinOffsetY) * perlin.mul_perlinYScale,
                        perlin.mul_perlinOctaves,
                        perlin.mul_perlinPersistance
                                                            ) * perlin.mul_perlinHeightScale;

                }
            }
        }
        terrainData.SetHeights(0, 0, heightMap);
    }

    public void AddNewPerlinRow()
    { perlinList.Add(new PerlinList()); }

    public void RemovePerLinRow()
    {
        List<PerlinList> temp = new List<PerlinList>();

        for (int i = 0; i < perlinList.Count; i++)
        {
            if (!perlinList[i].mul_remove) { temp.Add(perlinList[i]); }
        }
        //
        int beforeRemove = perlinList.Count;
        int afterRemove = temp.Count;
        if (beforeRemove == afterRemove) // no select 
        {

#if UNITY_EDITOR
            EditorUtility.DisplayDialog("Remove Perlin Noise", "Select a remove field(s) to delete a PN(s)", "Done");
#endif

        }
        else
        {
            // clear list && have to be keep at least 1
            if (temp.Count == 0)
            {
                temp.Add(perlinList[0]);

#if UNITY_EDITOR
                EditorUtility.DisplayDialog("Remove Perlin Noise", "List PN must have at least one PN ", "Done");
#endif

            }
        }
        perlinList = temp;
    }
    //-------    


    public float[,] CreateVoronoiTexture()
    {
        // texture part           
        float pValue = 0;
        Color pixCol = Color.white;
        if (!overrideTerrain)
            pTexture_Vonoroi = new Texture2D(terrainData.heightmapWidth, terrainData.heightmapHeight, TextureFormat.ARGB32, false);
        //
        float[,] heightMap = GetHeightMap();
        for (int vert = 0; vert < voronoiVertices; vert++)
        {
            Vector3 Vertice =
                new Vector3(
                UnityEngine.Random.Range(0, terrainData.heightmapWidth),
                UnityEngine.Random.Range(voronoiMinHeight, voronoiMaxHeight),
                UnityEngine.Random.Range(0, terrainData.heightmapHeight)
                            );
            if (heightMap[(int)Vertice.x, (int)Vertice.z] < Vertice.y) // check the vertice.y is coverd by something            
                heightMap[(int)Vertice.x, (int)Vertice.z] = Vertice.y; //  get a point with location(x,z) & height of y 
            else continue;

            Vector2 VerticeLocation = new Vector2(Vertice.x, Vertice.z);
            float maxDistance = Vector2.Distance(new Vector2(0, 0), new Vector2(terrainData.heightmapWidth, terrainData.heightmapHeight));

            for (int y = 0; y < terrainData.heightmapHeight; y++)
            {
                for (int x = 0; x < terrainData.heightmapWidth; x++)
                {
                    // texture part   
                    pValue = heightMap[x, y];
                    float colValue = pValue;
                    pixCol = new Color(colValue, colValue, colValue);
                    pTexture_Vonoroi.SetPixel(x, y, pixCol);
                    //
                    if ((x != Vertice.x) || (y != Vertice.z)) // the point is not the vertice point
                    {
                        float distanceToVertice = Vector2.Distance(VerticeLocation, new Vector2(x, y)) / maxDistance;
                        float height = 0;

                        switch (voronoiType)
                        {
                            case VoronoiType.Voronoi_Power:
                                height = Vertice.y - Mathf.Pow(distanceToVertice, voronoiDropOff) * voronoiFallOff; break;
                            case VoronoiType.Voronoi_LinPow:
                                height = Vertice.y - distanceToVertice * voronoiFallOff - Mathf.Pow(distanceToVertice, voronoiDropOff); break;
                            case VoronoiType.Voronoi_SinPow:
                                height = Vertice.y - Mathf.Pow(distanceToVertice * 3, voronoiFallOff) -
                                         Mathf.Sin(distanceToVertice * 2 * Mathf.PI) / voronoiFallOff; break;
                        }
                        // check value of heightmap at point(x,y)
                        // if heightmap value is higher: it exists , or will cover the new one
                        if (heightMap[x, y] < height)
                        {
                            heightMap[x, y] = height;
                            // texture part      
                            pValue = height;
                            float colValue1 = pValue;
                            pixCol = new Color(colValue1, colValue1, colValue1);
                            pTexture_Vonoroi.SetPixel(x, y, pixCol);
                            //
                        }

                    }
                }
            }
            processSmooth = (vert + 1) / (float)voronoiVertices;

#if UNITY_EDITOR
            EditorUtility.DisplayProgressBar("Voronoi Texture Creating", "Processing...", processSmooth);
#endif

        }

        pTexture_Vonoroi.Apply(false, false); // texture part    
        processSmooth = 0;

#if UNITY_EDITOR
        EditorUtility.ClearProgressBar();
#endif

        return heightMap;
        //terrainData.SetHeights(0, 0, heightMap);
    }

    public void ApplyVoronoi()
    {
        ReadTexture(pTexture_Vonoroi);
        if (overrideTerrain) overrideTerrain = false;
        Smooth();
        if (!overrideTerrain) overrideTerrain = true;
    }
    public void Create_ApplyVoronoi()
    {
        terrainData.SetHeights(0, 0, CreateVoronoiTexture());
    }
    //
    public float[,] CreateMidPointTexture()
    {
        // texture part           
        float pValue = 0;
        Color pixCol = Color.white;
        if (!overrideTerrain)
            pTexture_Midpoint = new Texture2D(terrainData.heightmapWidth - 1, terrainData.heightmapHeight - 1, TextureFormat.ARGB32, false);
        //
        float[,] heightMap = GetHeightMap();
        //print(terrainData.heightmapHeight+" & "+terrainData.heightmapWidth);
        int heightMapWidth = terrainData.heightmapWidth - 1; // example 513 - 1; 
        int squareSize = heightMapWidth; // squareX = squareY = 512

        float minHeight = midMinHeight;
        float maxHeight = midMaxHeight;
        float heightSlipping = (float)Mathf.Pow(midSlipPowerHeight, -1 * midRoughbess);
        //                            (midX, midYU)
        //               (x,cornerY)      midYU      (cornerX, cornerY)         
        // (midXL,midY)     midXL       (midX,midY)       midXR              (midXR,midY)
        //                  (x,y)         midYD       (CornerX,y)
        //                             (midX, midYD)
        // 
        int midPointX, midPointY;
        int cornerX, cornerY;
        int midXLeft, midXRight, midYUp, midYDown;
        //  (0,513-2)    (513-2,513-2) 
        //  (0,0)        (513-2,0)

        while (squareSize > 0)
        {
            //Get center midpoint 
            for (int x = 0; x < heightMapWidth; x += squareSize)
            {
                for (int y = 0; y < heightMapWidth; y += squareSize)
                {
                    cornerX = (x + squareSize);
                    cornerY = (y + squareSize);

                    midPointX = (int)(x + squareSize / 2.0f);
                    midPointY = (int)(y + squareSize / 2.0f);

                    heightMap[midPointX, midPointY] = (float)
                       ((heightMap[x, y] + heightMap[cornerX, y]
                     + heightMap[x, cornerY] + heightMap[cornerX, cornerY]) / 4.0f
                     + UnityEngine.Random.Range(minHeight, maxHeight));

                    // texture part   
                    pValue = heightMap[midPointX, midPointY];
                    float colValue = pValue;
                    pixCol = new Color(colValue, colValue, colValue);
                    pTexture_Midpoint.SetPixel(x, y, pixCol);
                    //
                }
            }
            // Get edge midpoint
            for (int x = 0; x < heightMapWidth; x += squareSize)
            {
                for (int y = 0; y < heightMapWidth; y += squareSize)
                {
                    cornerX = (x + squareSize);
                    cornerY = (y + squareSize);

                    midPointX = (int)(x + squareSize / 2.0f);
                    midPointY = (int)(y + squareSize / 2.0f);

                    // from the midpointX,Y add or subtract a distance 
                    midXLeft = (int)(midPointX - squareSize);
                    midXRight = (int)(midPointX + squareSize);
                    midYUp = (int)(midPointX + squareSize);
                    midYDown = (int)(midPointX - squareSize);

                    // check out of the range height map 512 & 0
                    if (midYUp >= heightMapWidth - 1 || midXRight >= heightMapWidth - 1
                          || midXLeft <= 0 || midYDown <= 0) continue;

                    //(midX, midYU) ~top side  midYU 
                    heightMap[midPointX, cornerY] = (float)
                        ((heightMap[midPointX, midYDown] + heightMap[midPointX, midPointY]
                        + heightMap[x, cornerY] + heightMap[cornerX, cornerY]) / 4.0f
                        + UnityEngine.Random.Range(minHeight, maxHeight));
                    // texture part      
                    pValue = heightMap[midPointX, cornerY];
                    float colValue1 = pValue;
                    pixCol = new Color(colValue1, colValue1, colValue1);
                    pTexture_Midpoint.SetPixel(x, y, pixCol);
                    //

                    //(midX, midYD) ~ bottom side  midYD
                    heightMap[midPointX, y] = (float)
                        ((heightMap[midPointX, midPointY] + heightMap[x, y]
                        + heightMap[cornerX, y] + heightMap[midPointX, midYDown]) / 4.0f
                        + UnityEngine.Random.Range(minHeight, maxHeight));
                    // texture part      
                    pValue = heightMap[midPointX, y];
                    float colValue2 = pValue;
                    pixCol = new Color(colValue2, colValue2, colValue2);
                    pTexture_Midpoint.SetPixel(x, y, pixCol);
                    //


                    // (midXL,midY) ~ left side midXL 
                    heightMap[x, midPointY] = (float)
                       ((heightMap[x, cornerY] + heightMap[x, y]
                       + heightMap[midXLeft, midPointY] + heightMap[midPointX, midPointY]) / 4.0f
                       + UnityEngine.Random.Range(minHeight, maxHeight));
                    // texture part      
                    pValue = heightMap[x, midPointY];
                    float colValue3 = pValue;
                    pixCol = new Color(colValue3, colValue3, colValue3);
                    pTexture_Midpoint.SetPixel(x, y, pixCol);
                    //

                    // (midXR,midY) ~ right side midXR     
                    heightMap[x, midPointY] = (float)
                       ((heightMap[cornerX, cornerY] + heightMap[cornerX, y]
                       + heightMap[midPointX, midPointY] + heightMap[midXRight, midPointY]) / 4.0f
                       + UnityEngine.Random.Range(minHeight, maxHeight));
                    // texture part      
                    pValue = heightMap[x, midPointY];
                    float colValue4 = pValue;
                    pixCol = new Color(colValue4, colValue4, colValue4);
                    pTexture_Midpoint.SetPixel(x, y, pixCol);
                    //

                }
            }

            squareSize = (int)(squareSize / 2.0f);  // do a half quarter each time
            minHeight *= heightSlipping; // reduce 
            maxHeight *= heightSlipping;
            //
            if (squareSize > 0)
            {
                processSmooth = heightMapWidth / squareSize;

#if UNITY_EDITOR
                EditorUtility.DisplayProgressBar("Midpoint Texture Creating", "Processing...", processSmooth);

#endif

            }

        }
        processSmooth = 0;

#if UNITY_EDITOR
        EditorUtility.ClearProgressBar();
#endif

        pTexture_Midpoint.Apply(false, false); // texture part
        //terrainData.SetHeights(0, 0, heightMap);
        return heightMap;
    }

    public void ApplyMidpointTexture()
    {
        ReadTexture(pTexture_Midpoint);

    }

    public void Create_ApplyMidpoint()
    {
        terrainData.SetHeights(0, 0, CreateMidPointTexture());
    }
    //--------------
    public void Smooth()
    {
        if (overrideTerrain)
        {

#if UNITY_EDITOR
            EditorUtility.DisplayDialog("Smooth", "Select check-box Override Terrain to apply the smoothing", "Done");
#endif            
            return;
        }
        float[,] heightMap = GetHeightMap();

        for (int i = 0; i < amoutSmooth; i++)
        {
            for (int y = 0; y < terrainData.heightmapHeight; y++)
            {
                for (int x = 0; x < terrainData.heightmapWidth; x++)
                {
                    float height = heightMap[x, y];
                    List<Vector2> neighborPoints = GetNeighborPoints(new Vector2(x, y), terrainData.heightmapWidth, terrainData.heightmapHeight);

                    foreach (Vector2 p in neighborPoints)
                    {
                        height += heightMap[(int)p.x, (int)p.y];
                    }

                    heightMap[x, y] = height / ((float)neighborPoints.Count + 1);
                }
            }
            processSmooth = (i + 1) / (float)amoutSmooth;

#if UNITY_EDITOR
            EditorUtility.DisplayProgressBar("Smooth", "Processing...", processSmooth);
#endif

        }
        processSmooth = 0.0f;

#if UNITY_EDITOR
        EditorUtility.ClearProgressBar();
#endif     

        terrainData.SetHeights(0, 0, heightMap);
    }

    public List<Vector2> GetNeighborPoints(Vector2 position, int height, int width)
    {
        List<Vector2> neighborPoints = new List<Vector2>();
        // the values are -1; 0; 1; need to be 9 points
        for (int y = -1; y <= 1; y++)
        {
            for (int x = -1; x <= 1; x++)
            {
                // do not run at point (0,0)
                if (x != 0 || y != 0)
                {
                    // return the min or max value base on input value
                    Vector2 newPoint = new Vector2(
                            Mathf.Clamp(position.x + x, 0, width - 1),
                            Mathf.Clamp(position.y + y, 0, height - 1));

                    if (!neighborPoints.Contains(newPoint)) neighborPoints.Add(newPoint);
                }
            }
        }
        return neighborPoints;
    }
    //----------------------------   
    // overrideTerrain is true = cleaning the terrain and put something new on it 
    float[,] GetHeightMap()
    {
        if (overrideTerrain) return new float[terrainData.heightmapWidth, terrainData.heightmapHeight];
        return terrainData.GetHeights(0, 0, terrainData.heightmapWidth, terrainData.heightmapHeight);

    }

}// End class PGCTerrain
//Helper Class###############################################################################
public class TextureClass
{
    public int width = 256;
    public int height = 256;
    public float scale = 20f;
    public float offsetX = 100f;
    public float offsetY = 100f;
    public TextureClass(float _scale, float _offsetX, float _offsetY)
    {
        scale = _scale;
        offsetX = _offsetX;
        offsetY = _offsetY;
    }

    public Texture2D GenerateTexture()
    {
        offsetX = UnityEngine.Random.Range(0f, 999999f);
        offsetY = UnityEngine.Random.Range(0f, 999999f);
        Texture2D texture = new Texture2D(width, height);

        // GENERATE A PERLIN NOISE MAP FOR THE TEXTURE
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Color color = CalculateColor(x, y);
                texture.SetPixel(x, y, color);
            }
        }

        texture.Apply();
        return texture;

    }

    Color CalculateColor(int x, int y)
    {
        float xCoord = (float)x / width * scale + offsetX;
        float yCoord = (float)y / height * scale + offsetY;

        float sample = Mathf.PerlinNoise(xCoord, yCoord);
        return new Color(sample, sample, sample);
    }
}
//#########################################################
[System.Serializable]
public class PerlinList
{
    public float mul_perlinXScale = 0.01f;
    public float mul_perlinYScale = 0.01f;
    public int mul_perlinOffsetX = 0;
    public int mul_perlinOffsetY = 0;
    //--- combine with BrownianMotion
    public int mul_perlinOctaves = 3;
    public float mul_perlinPersistance = 8;
    public float mul_perlinHeightScale = 0.09f;
    public bool mul_remove = false;
}
//######################################################
