using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public Transform cameraTransform; 
    private Transform backgroundTransform;  
    private float initialBackgroundX;
    private float initialCameraX;

    void Start()
    {
        backgroundTransform = GetComponent<Transform>();
        // Guardar las posiciones iniciales
        initialBackgroundX = backgroundTransform.position.x;
        initialCameraX = cameraTransform.position.x;
    }

    void Update()
    {
        float deltaX = cameraTransform.position.x - initialCameraX;

        backgroundTransform.position = new Vector3(initialBackgroundX + deltaX, backgroundTransform.position.y, backgroundTransform.position.z);
    }
}