using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    public float moveSpeed;
    [Header("Rotate config")]
    public float minXRot;
    public float maxXRot;
    private float curXRot;
    public float rotateSpeed;

    [Header("Zoom config")]
    public float minZoom;
    public float maxZoom;
    public float zoomSpeed;

    private float curZoom;
    // reference to the camera object
    private Camera cam;

    private float mouseX;

    //Indicates if the camera is rotating
    private bool rotating;

    private Vector2 moveDirection;


    private void Start()
    {
        cam = Camera.main;
        curZoom = cam.transform.localPosition.y;

        curXRot = -50;

    }

    public void OnZoom(InputAction.CallbackContext context)
    {
        //getting the scroll wheel value
        curZoom += context.ReadValue<Vector2>().y * zoomSpeed;
        //clamping it between min/max zoom values
        curZoom = Mathf.Clamp(curZoom, minZoom, maxZoom);
    }


    public void OnRotateToggle(InputAction.CallbackContext context)
    {
        rotating = context.ReadValueAsButton();
        if (!rotating) mouseX = 0; //To avoid infinite spin
    }

    public void OnRotate(InputAction.CallbackContext context)
    {
        //If clicking right button mouse
        if(rotating)
        {
            mouseX = context.ReadValue<Vector2>().x;
            float mouseY = context.ReadValue<Vector2>().y;

            curXRot += -mouseY * rotateSpeed;
            curXRot = Mathf.Clamp(curXRot, minXRot, maxXRot);                   
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        //Read the input value that is being sent by the Input system
        moveDirection = context.ReadValue<Vector2>();
    }

    private void LateUpdate()
    {
        //applying ZOOM to the camera
        cam.transform.localPosition = Vector3.up * curZoom;

        //applying ROTATION to the anchor
        transform.eulerAngles = new Vector3(curXRot, transform.eulerAngles.y + (mouseX * rotateSpeed), 0.0f);

        //applying MOVEMENT to the camera
        Vector3 forward = cam.transform.forward;
        forward.y = 0.0f;
        forward.Normalize();

        Vector3 right = cam.transform.right;

        //get a local direction
        Vector3 dir = forward * moveDirection.y + right * moveDirection.x;

        dir *= moveSpeed * Time.deltaTime;

        transform.position += dir;
    }


}
