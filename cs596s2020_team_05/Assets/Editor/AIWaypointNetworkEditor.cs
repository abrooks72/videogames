using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

[CustomEditor(typeof(AIWaypointNetwork))]
public class AIWaypointNetworkEditor : Editor
{
    public override void OnInspectorGUI()
    {
        AIWaypointNetwork network = (AIWaypointNetwork)target;       
        DrawDefaultInspector();
    }

    public void OnSceneGUI()
    {
        AIWaypointNetwork network = (AIWaypointNetwork)target;
        MarkLable_DrawLine_Location(network);       
    }      

    public void MarkLable_DrawLine_Location(AIWaypointNetwork network)
    {
        GUIStyle style = new GUIStyle();
        style.normal.textColor = Color.white; // color lable
        Handles.color = Color.yellow; // color draw line      

        for (int i = 0; i < network.Waypoints.Count; i++)
        {
            string nameLocation = network.Waypoints[i].gameObject.name;
           
            Vector3 currentLocation = network.Waypoints[i].position;
            Vector3 nextLocation = network.Waypoints[GetNextLocation(i,network.Waypoints.Count)].position;
            if (currentLocation != null)
            {
                Handles.Label(currentLocation, nameLocation, style);               
                //Debug.Log("line "+ currentLocation +" & "+ nextLocation);
                Handles.DrawLine(currentLocation, nextLocation);
                          
            }               
        }         
    }

    public int GetNextLocation(int currentIndex, int total)
    { 
        //Debug.Log("current index "+ currentIndex +  " total "+ total);
        int index = currentIndex;                
        if(currentIndex < total-1)
        {
            index ++;
        }
        else
        {
            index = 0;
        }
        
        return index;
    }

   
}
