
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public float mouseSensitivity = 10f;
    public Transform target;
    public float distFromTarget = 2f;
    public Vector2 pitchMinMax = new Vector2(-40, 85);
    public bool lockCursor;

    public float smoothTime = 0.12f;
    Vector3 rotationSmooothVelocity;
    Vector3 currentRotation;

    float yaw;
    float pitch;

    Vector3 camMask;
    float DistanceAway = 10f;
    float DistanceUp = 1f;
    public LayerMask CamOcclusion;

    void Start()
    {
        if(lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    void LateUpdate()
    {
            yaw += Input.GetAxis("Mouse X") * mouseSensitivity;
            pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity;
            pitch = Mathf.Clamp(pitch, pitchMinMax.x, pitchMinMax.y);

            currentRotation = Vector3.SmoothDamp(currentRotation, new Vector3(pitch, yaw), ref rotationSmooothVelocity, smoothTime);
            transform.eulerAngles = currentRotation;

            transform.position = target.position - transform.forward * distFromTarget;
            
    }

    void occludeRay(Vector3 targetFollow)
    {   
        float smooth; 
        #region prevent wall clipping
        //declare a new raycast hit.
        RaycastHit wallHit = new RaycastHit();
        //linecast from your player (targetFollow) to your cameras mask (camMask) to find collisions.
        if(Physics.Linecast(targetFollow, camMask, out wallHit, CamOcclusion)){
            //the smooth is increased so you detect geometry collisions faster.
            smooth = 10f;
            //the x and z coordinates are pushed away from the wall by hit.normal.
            //the y coordinate stays the same.
            transform.position = new Vector3(wallHit.point.x + wallHit.normal.x * 0.5f, transform.position.y, wallHit.point.z + wallHit.normal.z * 0.5f);
        }
        #endregion
    }
}
