using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    public float moveSpeed = 6f;
    public float mouseSensitivity = 100f;

    private CharacterController controller;
    private Camera playerCamera;

    private float verticalRotation = 0f;
    private float verticalVelocity = 0f;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        playerCamera = Camera.main;
    }

    void Update()
    {
        // Mouse look
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -90f, 90f);

        transform.Rotate(Vector3.up * mouseX);
        playerCamera.transform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);

        // Keyboard movement
        float horizontalMovement = Input.GetAxis("Horizontal") * moveSpeed;
        float verticalMovement = Input.GetAxis("Vertical") * moveSpeed;

        verticalVelocity += Physics.gravity.y * Time.deltaTime;

        Vector3 moveDirection = new Vector3(horizontalMovement, verticalVelocity, verticalMovement);
        moveDirection = transform.TransformDirection(moveDirection);

        controller.Move(moveDirection * Time.deltaTime);
    }
}