using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyParentGameOject : MonoBehaviour
{
   void DestroyGameObject()
    {
        Destroy(this.gameObject.transform.parent.gameObject);
    }
}
