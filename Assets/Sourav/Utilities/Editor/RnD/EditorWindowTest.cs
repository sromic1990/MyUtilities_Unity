using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Sourav.Utilities.EditorUtils
{
    public class EditorWindowTest : EditorWindow 
    {
        string myString = "Hello";

        [MenuItem("Window/Test")]
        public static void ShowWindow()
        {
            GetWindow<EditorWindowTest>("Test");
        }

        private void OnGUI()
        {
            //Window here
            GUILayout.Label("This is a label", EditorStyles.boldLabel);

            myString = EditorGUILayout.TextField(myString);

            if(GUILayout.Button("Press me"))
            {
                Debug.Log("Button was a pressed");
            }
        }
    }
    
}