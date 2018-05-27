using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using InputDetection.Scripts;
using Sourav.Utilities.Scripts;

namespace InputDetection.EditorUtils
{
    public class LoadInputModule : Editor 
    {
        [MenuItem("ProjectUtility/InputModule/Load Module")]
        public static void LoadModule()
        {
            CheckGameObjectsPresetInScene<InputSingleTouch> checkGameObject = new CheckGameObjectsPresetInScene<InputSingleTouch>();
            GameObjectSerachResult gSearchResult = checkGameObject.CheckForGameObject();
            GameObject gObj = null;

            if(gSearchResult.numberOfObjects >= 1)
            {
                gObj = gSearchResult.foundGameObjects[0];
                if (gObj != null)
                {
                    gObj.SetActive(true);
                }
                if(gSearchResult.numberOfObjects > 1)
                {
                    for (int i = 1; i < gSearchResult.foundGameObjects.Count; i++)
                    {
                        DestroyImmediate(gSearchResult.foundGameObjects[i]);
                    }
                }
                Show_ObjectAlreadyExist();
            }
            else
            {
                CreatePrefabInstance createPrefab = new CreatePrefabInstance("Prefabs/Input");
            }
           
        }

        public static void Show_ObjectAlreadyExist()
        {
            EditorUtility.DisplayDialog("Input Module Already Present In Scene", "Input module is already present in scene. Cannot create more than one active module.", "Ok");
        }
    }
}
