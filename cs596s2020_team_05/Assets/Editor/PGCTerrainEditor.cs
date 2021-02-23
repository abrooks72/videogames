using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PGCTerrain))]
[CanEditMultipleObjects]

public class PGCTerrainEditor : Editor
{
    //------------------properties-------------------
    //random height
    SerializedProperty randomHeightRange;
    //sin height
    SerializedProperty sinHeightRange;
    // overrideTerrain
    SerializedProperty overrideTerrain;
    // height map from image 
    SerializedProperty heightMapScale;
    SerializedProperty heightMapImage;
    SerializedProperty pTexture_Vonoroi;
    SerializedProperty pTexture_Midpoint;
    SerializedProperty scaleTexture;
    SerializedProperty offsetX;
    SerializedProperty offsetY;
    // perlin noise 
    SerializedProperty perlinXScale;
    SerializedProperty perlinYScale;
    SerializedProperty perlinOffsetX;
    SerializedProperty perlinOffsetY;
    //--- combine with BrownianMotion
    SerializedProperty perlinPersistance;
    SerializedProperty perlinOctaves;
    SerializedProperty perlinHeightScale;
    //--------List perline   
    SerializedProperty perlinList;
    //-------- Voronoi
    SerializedProperty voronoiFallOff;
    SerializedProperty voronoiDropOff;
    SerializedProperty voronoiMinHeight;
    SerializedProperty voronoiMaxHeight;
    SerializedProperty voronoiType;
    SerializedProperty voronoiVertices;
    //--------Midpoint Displacement
    SerializedProperty midMinHeight;
    SerializedProperty midMaxHeight;
    SerializedProperty midSlipPowerHeight;
    SerializedProperty midRoughbess;
    //--------Smooth
    SerializedProperty amoutSmooth;
    SerializedProperty processSmooth;
    //pop down-----------------------
    bool showRandom = false;
    bool showSin    = false;
    bool showLoadTexture = false;
    bool showPerlineNoise = false;
    bool showListPerlin = false;
    bool showVoronoi = false;
    bool showMidPoint = false;
    bool showSmooth = false;    

    void OnEnable()
    {
        // link value of variable       
        randomHeightRange = serializedObject.FindProperty("randomHeightRange");
        sinHeightRange   = serializedObject.FindProperty("sinHeightRange");
        overrideTerrain = serializedObject.FindProperty("overrideTerrain");
        heightMapImage = serializedObject.FindProperty("heightMapImage");
        pTexture_Vonoroi        = serializedObject.FindProperty("pTexture_Vonoroi");
        pTexture_Midpoint       = serializedObject.FindProperty("pTexture_Midpoint");
        heightMapScale = serializedObject.FindProperty("heightMapScale");
        scaleTexture   = serializedObject.FindProperty("scaleTexture");
        offsetX        = serializedObject.FindProperty("offsetX");
        offsetY        = serializedObject.FindProperty("offsetY");
        perlinXScale = serializedObject.FindProperty("perlinXScale");
        perlinYScale = serializedObject.FindProperty("perlinYScale");
        perlinOffsetX = serializedObject.FindProperty("perlinOffsetX");
        perlinOffsetY = serializedObject.FindProperty("perlinOffsetY");
        perlinOctaves = serializedObject.FindProperty("perlinOctaves");
        perlinPersistance = serializedObject.FindProperty("perlinPersistance");
        perlinHeightScale = serializedObject.FindProperty("perlinHeightScale");
        perlinList = serializedObject.FindProperty("perlinList");
        voronoiFallOff = serializedObject.FindProperty("voronoiFallOff");
        voronoiDropOff = serializedObject.FindProperty("voronoiDropOff");
        voronoiMaxHeight = serializedObject.FindProperty("voronoiMaxHeight");
        voronoiMinHeight = serializedObject.FindProperty("voronoiMinHeight");
        voronoiType = serializedObject.FindProperty("voronoiType");
        voronoiVertices = serializedObject.FindProperty("voronoiVertices");
        midMaxHeight = serializedObject.FindProperty("midMaxHeight");
        midMinHeight = serializedObject.FindProperty("midMinHeight");
        midSlipPowerHeight = serializedObject.FindProperty("midSlipPowerHeight");
        midRoughbess = serializedObject.FindProperty("midRoughbess");
        amoutSmooth = serializedObject.FindProperty("amoutSmooth");
        processSmooth = serializedObject.FindProperty("processSmooth");
        //---

    }

    public override void OnInspectorGUI()
    {
        // begin     
        serializedObject.Update();     

        // body--------------    open ----
        PGCTerrain terrain = (PGCTerrain)target;  // Editor calls cript custom
        // override Terrain
        EditorGUILayout.PropertyField(overrideTerrain);
        // reset Terrain button
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        if (GUILayout.Button("Reset Terrain"))
        {
            terrain.ResetTerrain();
        }
        // random height button
        showRandom = EditorGUILayout.Foldout(showRandom,"Random"); // check pop dowm
        if(showRandom)
        {
            EditorGUILayout.LabelField("",GUI.skin.horizontalSlider);    // create slide bar     
            GUILayout.Label("Set Height Between Random Values", EditorStyles.boldLabel);  // set lable name
            EditorGUILayout.PropertyField(randomHeightRange);              // set value of property randomHeighRang
            if(GUILayout.Button("Random Heights"))                         // check button press
            {
                terrain.RandomHeightTerrain();    // call method
            }
        }
        // sin height button
        showSin = EditorGUILayout.Foldout(showSin, "Sin Wave"); // check pop dowm
        if (showSin)
        {
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);    // create slide bar     
            GUILayout.Label("Set Height Wave by Sin", EditorStyles.boldLabel);  // set lable name           
            if (GUILayout.Button("Sin Wave"))                         // check button press
            {
                terrain.SinHeightTerrain();    // call method
            }
        }
        // height map from image 
        showLoadTexture = EditorGUILayout.Foldout(showLoadTexture, "Load Texture");
        if (showLoadTexture)
        {
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            GUILayout.Label("Load HeightMap Form Texture", EditorStyles.boldLabel);
            GUILayout.Label("Texture Name   :"+ terrain.heightMapImage.name);          
            GUILayout.Space(5);            
            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical();
            GUIStyle style = new GUIStyle(GUI.skin.label);
            style.alignment = TextAnchor.UpperCenter;
            style.fixedWidth = 100;      
            Texture2D texture =  (Texture2D)EditorGUILayout.ObjectField(terrain.heightMapImage, typeof(Texture2D), false, GUILayout.Width(100), GUILayout.Height(100));
            terrain.heightMapImage = texture;
            GUILayout.EndVertical();
            GUILayout.BeginVertical();         
            EditorGUILayout.Slider(scaleTexture, 1, 20, new GUIContent("Scale Texture"));
            EditorGUILayout.Slider(offsetX,0,1000, new GUIContent("OffSet X"));
            EditorGUILayout.Slider(offsetY, 0, 1000, new GUIContent("OffSet Y"));
            if (GUILayout.Button("Create Texture",GUILayout.Height(40)))
            {
                terrain.CreateTexture();
            }
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
            GUILayout.Space(5);
            EditorGUILayout.PropertyField(heightMapScale);
            GUILayout.Space(15);           
            if (GUILayout.Button("Load Texture"))
            {
                terrain.LoadTexture();
            }
            GUILayout.Space(15);
        }
        // perlin noise  
        showPerlineNoise = EditorGUILayout.Foldout(showPerlineNoise, "Simple Perlin Noise");
        if (showPerlineNoise)
        {
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            GUILayout.Label("Perlin Noise", EditorStyles.boldLabel);
            EditorGUILayout.Slider(perlinXScale, 0, 1, new GUIContent("X Scale")); // input variable control, min, max value
            EditorGUILayout.Slider(perlinYScale, 0, 1, new GUIContent("Y Scale"));
            EditorGUILayout.IntSlider(perlinOffsetX, 0, 10000, new GUIContent("X Offset"));
            EditorGUILayout.IntSlider(perlinOffsetY, 0, 10000, new GUIContent("Y Offset"));
            EditorGUILayout.IntSlider(perlinOctaves, 1, 10, new GUIContent("Octaves"));           
            EditorGUILayout.Slider(perlinPersistance, 0.1f, 10, new GUIContent("Persistance"));
            EditorGUILayout.Slider(perlinHeightScale, 0, 1, new GUIContent("Height Scale"));
            if (GUILayout.Button("Perline_Noise"))
            {
                terrain.PerlinNoise();
            }
        }
        // List Perlin 
        showListPerlin = EditorGUILayout.Foldout(showListPerlin, " List Perlin Noise");
        if (showListPerlin)
        {
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            GUILayout.Label("List Perlin Noise", EditorStyles.boldLabel);           
            EditorGUILayout.PropertyField(perlinList, true); // true ~ display the list
            GUILayout.Space(15);
            //---
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("+"))
            {
                terrain.AddNewPerlinRow();
            }
            if (GUILayout.Button("-"))
            {
                terrain.RemovePerLinRow();
            }
            EditorGUILayout.EndHorizontal();
            //---
            if (GUILayout.Button("Apply Multiple Perlin Noise"))
            {
                terrain.MultiplePerlinNoise();
            }
        }
        // Voronoi 
        showVoronoi = EditorGUILayout.Foldout(showVoronoi, "Voronoi");
        if (showVoronoi)
        {
            EditorGUILayout.PropertyField(voronoiType, new GUIContent("Function Type"));
            EditorGUILayout.IntSlider(voronoiVertices, 1, 10, new GUIContent("Vertice Count"));
            EditorGUILayout.Slider(voronoiFallOff, 0, 10, new GUIContent("FallOff"));
            EditorGUILayout.Slider(voronoiDropOff, 0, 10, new GUIContent("DropOff"));
            EditorGUILayout.Slider(voronoiMaxHeight, 0, 1, new GUIContent("Max Height"));
            EditorGUILayout.Slider(voronoiMinHeight, 0, 1, new GUIContent("Min Height"));    
            
            //
            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical();
            GUIStyle style = new GUIStyle(GUI.skin.label);
            style.alignment = TextAnchor.UpperCenter;
            style.fixedWidth = 100;      
            Texture2D texture =  (Texture2D)EditorGUILayout.ObjectField(terrain.pTexture_Vonoroi, typeof(Texture2D), false, GUILayout.Width(100), GUILayout.Height(100));
            terrain.pTexture_Vonoroi = texture;
            GUILayout.EndVertical();
            GUILayout.Space(5);
            GUILayout.BeginVertical();
            if(GUILayout.Button("Create Voronoi Texture",GUILayout.Height(30)))
            {
                terrain.CreateVoronoiTexture();
            }
            GUILayout.Space(5);
            if (GUILayout.Button("Apply Voronoi Texture",GUILayout.Height(30)))
            {
                terrain.ApplyVoronoi();
            }
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
            if(GUILayout.Button("Create & Apply Voronoi"))
            {
                terrain.Create_ApplyVoronoi();
            }
            //   
            
        }
        // Midpoint Displacement 
        showMidPoint = EditorGUILayout.Foldout(showMidPoint, "Midpoind Displacement");
        if (showMidPoint)
        {
            EditorGUILayout.PropertyField(midSlipPowerHeight);
            EditorGUILayout.PropertyField(midRoughbess);
            EditorGUILayout.PropertyField(midMinHeight);
            EditorGUILayout.PropertyField(midMaxHeight);
            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical();
            GUIStyle style = new GUIStyle(GUI.skin.label);
            style.alignment = TextAnchor.UpperCenter;
            style.fixedWidth = 100;      
            Texture2D texture =  (Texture2D)EditorGUILayout.ObjectField(terrain.pTexture_Midpoint, typeof(Texture2D), false, GUILayout.Width(100), GUILayout.Height(100));
            terrain.pTexture_Midpoint = texture;
            GUILayout.EndVertical();
            GUILayout.Space(5);
            GUILayout.BeginVertical();
            if(GUILayout.Button("Create Midpoint Texture",GUILayout.Height(30)))
            {
                terrain.CreateMidPointTexture();
            }
            GUILayout.Space(5);
            if (GUILayout.Button("Apply Midpoint Texture",GUILayout.Height(30)))
            {
                terrain.ApplyMidpointTexture();
            }
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
            if(GUILayout.Button("Create & Apply Midpoint"))
            {
                terrain.Create_ApplyMidpoint();
            }
            //   
            
        }
        // Smooth 
        showSmooth = EditorGUILayout.Foldout(showSmooth, "Smooth");
        if (showSmooth)
        {
            EditorGUILayout.IntSlider(amoutSmooth, 1, 10);           
            if (GUILayout.Button("Smooth"))
            {
                terrain.Smooth();
            }
        }
        // body--------------    close ----

        // end     
        serializedObject.ApplyModifiedProperties(); 

    }

}// End Class PGCTerrainEditor-----
