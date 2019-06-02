using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingTextController : MonoBehaviour
{
    private static FloatingText popupText;
    public static GameObject blah;
    private static GameObject canvas;

    public static void initialise()
    {
        canvas = GameObject.Find("Canvas");
        if(!popupText)
        {
            Debug.Log("loaded popup text");
            try
            {
                popupText = Resources.Load<FloatingText>("LoadablePrefabs/PopupTextParent");
            }
            catch (Exception e)
            {
                Debug.Log("loaded Exception: " + e.Message);
            }
        }
    }

    public static void createFloatingText(string text, Transform location)
    {
        Debug.Log("object pos x: " + location.position.x + " / y:" + location.position.y + " / z: "+ location.position.z);

        Vector2 screenPosition = Camera.main.WorldToScreenPoint(location.position);
        Debug.Log("isntantiating flaoting text" + screenPosition.x + " / " + screenPosition.y);
        try
        {
            FloatingText instance = Instantiate(popupText);
            if(instance != null)
            {
                //instance.transform.SetParent(canvas.transform, false);
                Vector3 adjustedPosition = location.position;

                adjustedPosition.y = 6;

                instance.transform.position = adjustedPosition;
                instance.setText(text);
            } else
            {
                Debug.Log("instance is null");
            }
          
        } catch (Exception e)
        {
            Debug.Log("Exception: " + e.Message + "---" + e.StackTrace);
           
        }
     
    }
}
