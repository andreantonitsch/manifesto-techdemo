using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
[CustomEditor(typeof(CityGeneration))]
public class CityGenerationEditor : Editor
{

    public override void OnInspectorGUI()
    {

        DrawDefaultInspector();

        CityGeneration script = (CityGeneration)target;

        if (GUILayout.Button("Test Blocks"))
        {
            var blocks = script.GenerateBlocks();

            //foreach (var block in blocks)
            //{
            //    var ob = Instantiate(script.BlockPrefab);
            //    ob.transform.position = new Vector3(block.center.x, 0.0f, block.center.y);
            //    ob.transform.localScale = new Vector3(block.width * script.road_size_aspas, 1.0f, block.height * script.road_size_aspas);

            //    ob.transform.parent = script.transform;
            //}

        }
        if (GUILayout.Button("Test Fill Blocks"))
        {
            foreach(var block in script.CurrentBlocks)
            {
                script.FillBlock(block);
            }
            
        }

    }


}
#endif