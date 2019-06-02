using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FloatingText : MonoBehaviour
{
    public Animator animator;
    private Text textComponent;

    void Start()
    {
        Debug.Log("floating text start");
        AnimatorClipInfo[] clipInfo = animator.GetCurrentAnimatorClipInfo(0);
        Destroy(gameObject, clipInfo[0].clip.length);
        textComponent = animator.GetComponent<Text>();
    }

    public void setText(string text)
    {
        if(textComponent == null)
        {
            Debug.Log("text reference is null - trying new one");
            textComponent = transform.Find("PopupText").GetComponent<Text>();
        }

        if (textComponent != null)
        {
            Debug.Log("setting tex");
            textComponent.text = text;
        } else
        {
            Debug.Log("text reference still null");


        }
    }
}
