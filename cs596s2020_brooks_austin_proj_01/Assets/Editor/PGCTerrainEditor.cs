using UnityEngine;
using UnityEditor;
using EditorGUITable;
//correct file

[CustomEditor(typeof(PGCTerrain))]
public class PGCTerrainEditor : Editor
{
    //My Properties
    SerializedProperty randomHeightRange;
    SerializedProperty heightMapScale;
    SerializedProperty heightMapImage;
    SerializedProperty perlinXScale;
    SerializedProperty perlinYScale;
    SerializedProperty perlinOffsetX;
    SerializedProperty perlinOffsetY;
    SerializedProperty perlinOctaves;
    SerializedProperty perlinPersistance;
    SerializedProperty perlinHeightScale;
    SerializedProperty resetTerrain;
    SerializedProperty voronoiFallOff;
    SerializedProperty voronoiDropOff;
    SerializedProperty voronoiMinHeight;
    SerializedProperty voronoiMaxHeight;
    SerializedProperty voronoiPeaks;
    SerializedProperty voronoiType;
    SerializedProperty sineWaves;
    SerializedProperty SineValue;
    SerializedProperty frequency;
    SerializedProperty waveHeight;
    SerializedProperty waveWidth;
    SerializedProperty QuantityRandom;
    SerializedProperty MdptheightMin;
    SerializedProperty MdptheightMax;
    SerializedProperty MdptheightDampenerPower;
    SerializedProperty Mdptroughness;
    SerializedProperty smoothAmount;

    GUITableState perlinParameterTable;
    SerializedProperty perlinParameters;

    //Foldouts
    bool SineWaveBool = false;
    bool randomHeightBool = false;
    bool perlinBool = false;
    bool multiplePerlinBool = false;
    bool veronoiBool = false;
    bool diagonalBool = false;
    bool mdptBool = false;
    bool smoothBool = false;

    void OnEnable()
    {
        sineWaves = serializedObject.FindProperty("sineWaves");
        SineValue = serializedObject.FindProperty("SineValue");
        QuantityRandom = serializedObject.FindProperty("QuantityRandom");
        perlinXScale = serializedObject.FindProperty("perlinXScale");
        perlinYScale = serializedObject.FindProperty("perlinYScale");
        perlinOffsetX = serializedObject.FindProperty("perlinOffsetX");
        perlinOffsetY = serializedObject.FindProperty("perlinOffsetY");
        perlinOctaves = serializedObject.FindProperty("perlinOctaves");
        perlinPersistance = serializedObject.FindProperty("perlinPersistance");
        perlinHeightScale = serializedObject.FindProperty("perlinHeightScale");
        perlinParameterTable = new GUITableState("perlinParameterTable");
        perlinParameters = serializedObject.FindProperty("perlinParameters");
        voronoiFallOff = serializedObject.FindProperty("voronoiFallOff");
        voronoiDropOff = serializedObject.FindProperty("voronoiDropOff");
        voronoiMinHeight = serializedObject.FindProperty("voronoiMinHeight");
        voronoiMaxHeight = serializedObject.FindProperty("voronoiMaxHeight");
        voronoiPeaks = serializedObject.FindProperty("voronoiPeaks");
        voronoiType = serializedObject.FindProperty("voronoiType");
        MdptheightMin = serializedObject.FindProperty("MdptheightMin");
        MdptheightMax = serializedObject.FindProperty("MdptheightMax");
        MdptheightDampenerPower = serializedObject.FindProperty("MdptheightDampenerPower");
        Mdptroughness = serializedObject.FindProperty("Mdptroughness");
        smoothAmount = serializedObject.FindProperty("smoothAmount");
        waveWidth = serializedObject.FindProperty("waveWidth");
        waveHeight = serializedObject.FindProperty("waveHeight");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        PGCTerrain terrain = (PGCTerrain)target;

        SineWaveBool = EditorGUILayout.Foldout(SineWaveBool, "Sine wave strength: ");
        if (SineWaveBool)
        {
            //EditorGUILayout.Slider(SineValue, 0.1f, 1, new GUIContent("Sine Value"));
            EditorGUILayout.Slider(waveWidth, 0.01f, 0.1f, new GUIContent("Sine width"));
            EditorGUILayout.Slider(waveHeight, 0.1f, 1, new GUIContent("Sine height"));

            if (GUILayout.Button("Do Sine Wave Function")) //Button that generates perlin
            {
                Debug.Log("Called in Editor");
                terrain.MySineFuntion();
            }
        }

        randomHeightBool = EditorGUILayout.Foldout(randomHeightBool, "Random height:");
        if (randomHeightBool)
        {
            EditorGUILayout.Slider(QuantityRandom, 0.1f, 1, new GUIContent("Quantity"));
            if (GUILayout.Button("Do random function"))
            {
                Debug.Log("Called in Editor");
                terrain.random();
            }
        }

        perlinBool = EditorGUILayout.Foldout(perlinBool, "Perlin:");
        if (perlinBool) //Button that generates perlin
        {
            EditorGUILayout.Slider(perlinXScale, 0, 1, new GUIContent("X scale"));
            EditorGUILayout.Slider(perlinYScale, 0, 1, new GUIContent("Y scale"));
            EditorGUILayout.Slider(perlinOffsetX, 0, 10000, new GUIContent("Offset X"));
            EditorGUILayout.Slider(perlinOffsetY, 0, 10000, new GUIContent("Offset Y"));
            EditorGUILayout.IntSlider(perlinOctaves, 1, 10, new GUIContent("Octaves"));
            EditorGUILayout.Slider(perlinPersistance, 0.1f, 10, new GUIContent("Persistance"));
            EditorGUILayout.Slider(perlinHeightScale, 0, 1, new GUIContent("Height Scale"));

            if (GUILayout.Button("Do perlin function"))
            {
                Debug.Log("Called in Editor");
                terrain.Perlin();
            }
        }

        multiplePerlinBool = EditorGUILayout.Foldout(multiplePerlinBool, "Multiple Perlin:");
        if (multiplePerlinBool)
        {
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            GUILayout.Label("Multiple Perlin Noise", EditorStyles.boldLabel);
            perlinParameterTable = GUITableLayout.DrawTable(perlinParameterTable, serializedObject.FindProperty("perlinParameters"));
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("+"))
            {
                terrain.AddNewPerlin();
            }
            if (GUILayout.Button("-"))
            {
                terrain.RemovePerlin();
            }
            EditorGUILayout.EndHorizontal();
            if (GUILayout.Button("Apply Multiple Perlin"))
            {
                terrain.multiplePerlin();
            }
        }

        veronoiBool = EditorGUILayout.Foldout(veronoiBool, "Veronoi:");
        if (veronoiBool)  //Button that generates voronoi
        {
            EditorGUILayout.Slider(voronoiFallOff, 0, 10, new GUIContent("Fall Off:"));
            EditorGUILayout.Slider(voronoiDropOff, 0, 10, new GUIContent("Drop Off:"));
            EditorGUILayout.Slider(voronoiMinHeight, 0, 1, new GUIContent("Minimum Height"));
            EditorGUILayout.Slider(voronoiMaxHeight, 0, 1, new GUIContent("Maximum Height"));
            EditorGUILayout.Slider(voronoiPeaks, 0, 10, new GUIContent("Number of peaks"));
            EditorGUILayout.PropertyField(voronoiType);

            if (GUILayout.Button("Do voronoi function"))
            {
                Debug.Log("Called in Editor");
                terrain.Voronoi();
            }
        }

        mdptBool = EditorGUILayout.Foldout(mdptBool, "Mdpt Displacement Smoothing:");
        if (mdptBool)
        {
            EditorGUILayout.PropertyField(MdptheightMin);
            EditorGUILayout.PropertyField(MdptheightMax);
            EditorGUILayout.PropertyField(MdptheightDampenerPower);
            EditorGUILayout.PropertyField(Mdptroughness);
            EditorGUILayout.PropertyField(smoothAmount);
            if (GUILayout.Button("Mdpt"))
            {
                terrain.mdpt();
            }
        }

        diagonalBool = EditorGUILayout.Foldout(diagonalBool, "Diagonal:");
        if (diagonalBool)
        {
            if (GUILayout.Button("Do Diagonal"))
            {
                terrain.DiagonalLineInYourTerrain();
            }
        }
        


        if (GUILayout.Button("Reset"))
        {
            terrain.ResetMethod();
        }

        serializedObject.ApplyModifiedProperties();
    }

}



