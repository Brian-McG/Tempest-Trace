using UnityEngine;
using UnityEditor;
using System.Collections;

public class EditorQuit : MonoBehaviour
{
    void Update()
    {
        if(Application.isEditor)
        {
            if(Input.GetKeyDown(KeyCode.Escape))
            {
                EditorApplication.isPlaying = false;
            }
        }
    }
}
