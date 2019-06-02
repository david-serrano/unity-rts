using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class EventController : MonoBehaviour
{
    private static ArrayList eventHolder = new ArrayList();
    private static Text text;

    void Awake()
    {
        text = GetComponent<Text>();
        text.text = "No events yet!";
    }

    public static void addEvent(string e) {
        string events = "";
        eventHolder.Add(e);
        if (eventHolder.Count > 0)
        {
            if (eventHolder.Count > 5)
            {
                eventHolder.RemoveAt(0);               
            }
            foreach (string s in eventHolder)
            {
                events += s + "\r\n";
            }
        } else
        {
            events = "No events yet!";
        }
        text.text = events;
    }
}
