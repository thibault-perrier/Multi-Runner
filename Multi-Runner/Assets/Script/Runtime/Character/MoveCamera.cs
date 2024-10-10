using System;
using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    
    [SerializeField] Transform cameraTransform;
    [SerializeField] Transform playerTransform;


    private void Start()
    {
        transform.position = cameraTransform.position;
        transform.rotation = cameraTransform.rotation;
    }


    // Update is called once per frame
    void Update()
    {
        transform.position = cameraTransform.position;
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x,playerTransform.rotation.eulerAngles.y,0);
    }
}
