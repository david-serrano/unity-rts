using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//should be attached to all controllable units in game, walkable or not
public class Unit : MonoBehaviour
{
    //for mouse.cs
    public Vector2 screenPos;
    public bool onScreen;
    public bool selected = false;

    private void Update()
    {
        if (!selected)
        {
            screenPos = Camera.main.WorldToScreenPoint(this.transform.position);

            if(Mouse.unitWithinScreenSpace(screenPos))
            {
                if(!onScreen)
                {
                    Mouse.unitsOnScreen.Add(this.gameObject);
                    onScreen = true;
                }
            } else
            {
                if(onScreen)
                {
                    Mouse.removeUnitFromOnScreen(this.gameObject);
                }
            }
        } 
    }
}
