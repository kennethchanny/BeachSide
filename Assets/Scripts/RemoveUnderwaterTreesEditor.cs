using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RemoveUnderWaterTrees))]
public class RemoveUnderwaterTreesEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        RemoveUnderWaterTrees underwaterTreeScript = (RemoveUnderWaterTrees)target; 

        if(GUILayout.Button("RemoveUnderwaterTrees"))
        {
            underwaterTreeScript.RemoveTreesUnderWater();
        }
    }
}
