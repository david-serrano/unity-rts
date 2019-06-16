using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Common : MonoBehaviour
{
    public static void CopyTransform(Transform dst, Transform src)
    {
        dst.localPosition = src.localPosition;
        dst.localRotation = src.localRotation;
        dst.localScale = src.localScale;
    }

    /**
     * General utility methods
     **/
    public static float unsigned(float val)
    {
        if (val < 0f)
        {
            val *= -1;
        }
        return val;
    }

    public static bool FloatToBool(float f)
    {
        if (f < 0f)
        {
            return false;
        }
        return true;
    }

    public static bool shiftKeysDown()
    {
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
