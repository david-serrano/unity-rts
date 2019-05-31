using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldCamera : MonoBehaviour
{
    
 
//this value is used to further clamp the camera away from the edge.
//Transform will not travel closer than nonPassibleBorderWidth from a terrain edge

    public struct BoxLimit
    {
        public float leftLimit;
        public float rightLimit;
        public float topLimit;
        public float bottomLimit;
    }

    float terrainSizeFactor = 5f;

    public static BoxLimit cameraLimits = new BoxLimit();
    public static BoxLimit mouseScrollLimits = new BoxLimit();
    public static WorldCamera Instance;

    private float cameraMoveSpeed = 60f;
    private float shiftBonus = 45f;
    private float mouseBoundary = 25f;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
       // Debug.Log("starting camera");
        cameraLimits.leftLimit = 10f;
        cameraLimits.rightLimit = 240f;
        cameraLimits.topLimit = 204f;
        cameraLimits.bottomLimit = -20f;

        mouseScrollLimits.leftLimit = mouseBoundary;
        mouseScrollLimits.rightLimit = mouseBoundary;
        mouseScrollLimits.topLimit = mouseBoundary;
        mouseScrollLimits.bottomLimit = mouseBoundary;
        
        GameObject terrainMain = GameObject.Find("TerrainMain");
        
        var myTerrainTransform = terrainMain.transform;

        cameraLimits.leftLimit = -myTerrainTransform.localScale.x * terrainSizeFactor;
        cameraLimits.rightLimit = myTerrainTransform.localScale.x * terrainSizeFactor;
        cameraLimits.topLimit = -myTerrainTransform.localScale.z * terrainSizeFactor;
        cameraLimits.bottomLimit = myTerrainTransform.localScale.z * terrainSizeFactor;
    }

    void Update()
    {
        if(CheckIfUserCameraInput())
        {
           // Debug.Log("camera input detected");
            Vector3 cameraDesiredMove = GetDesiredTranslation();

            if(isDesiredPositionOverBoundaries(cameraDesiredMove))
            {
                Debug.Log("desired move over boundaries - moving");
                this.transform.Translate(cameraDesiredMove);
            } else
            {
                Debug.Log("movement out of boundaries :(");
            }
        } else
        {
           // Debug.Log("no input detected");
        }
    }

    public bool CheckIfUserCameraInput()
    {
        bool keyboardMove;
        bool mouseMove;
        bool canMove;

        if(WorldCamera.AreCameraKeyboardButonsPressed())
        {
            keyboardMove = true;
        } else
        {
            keyboardMove = false;
        }

        if(WorldCamera.IsMousePositionWithinBoundaries())
        {
            mouseMove = true;
        } else
        {
            mouseMove = false;
        }

        if(keyboardMove || mouseMove)
        {
            canMove = true;
        } else
        {
            canMove = false;
        }

        return canMove;
    }

    public Vector3 GetDesiredTranslation()
    {
        float moveSpeed = 0f;
        float desiredX = 0f;
        float desiredY = 0f;
        float desiredZ = 0f;

        if(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            moveSpeed = (cameraMoveSpeed * shiftBonus) * Time.deltaTime;
        } else
        {
            moveSpeed = cameraMoveSpeed * Time.deltaTime;
        }

        if(Input.GetKey(KeyCode.W))
        {
            desiredZ = moveSpeed;
           desiredY = moveSpeed*3/4;
        }

        if (Input.GetKey(KeyCode.S))
        {
            desiredZ = -moveSpeed;
            desiredY = -moveSpeed * 3/4;
        }

        if (Input.GetKey(KeyCode.A))
        {
            desiredX = moveSpeed * -1;
        }

        if (Input.GetKey(KeyCode.D))
        {
            desiredX = moveSpeed;
        }

        return new Vector3(desiredX, desiredY, desiredZ);
          
    }

    public bool isDesiredPositionOverBoundaries(Vector3 desiredPosition)
    {
        bool overBoundaries = true;
        float moveX = this.transform.position.x + desiredPosition.x;
        float moveZ = this.transform.position.z + desiredPosition.z;
       if (moveX < cameraLimits.leftLimit || moveX > cameraLimits.rightLimit) {

                overBoundaries = false;
        }
        if (moveZ < cameraLimits.topLimit - 2*terrainSizeFactor || moveZ > cameraLimits.bottomLimit - 4*terrainSizeFactor)
        {

            overBoundaries = false;
        }

        return overBoundaries;
    }

    public static bool AreCameraKeyboardButonsPressed()
    {
        if(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
        {
          //  Debug.Log("wasd pressed");
            return true;
        } else
        {
            return false;
        }
    }

    public static bool IsMousePositionWithinBoundaries()
    {
        if(
            (Input.mousePosition.x < mouseScrollLimits.leftLimit && Input.mousePosition.x > -5) ||
            (Input.mousePosition.x > (Screen.width - mouseScrollLimits.rightLimit) && Input.mousePosition.x < (Screen.width + 5)) ||
            (Input.mousePosition.y < mouseScrollLimits.bottomLimit && Input.mousePosition.y > -5) ||
            (Input.mousePosition.y > (Screen.height - mouseScrollLimits.topLimit) && Input.mousePosition.y < (Screen.height + 5))
            )
        {
          //  Debug.Log("mouse move within limits");
            return true;
        } else
        {
            return false;
        }
    }
}
