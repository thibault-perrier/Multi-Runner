using System;
using UnityEngine;
using UnityEngine.Serialization;

public class MoveCamera : MonoBehaviour
{
    [SerializeField] Transform cameraTransformInPlayer;
    [SerializeField] Transform playerTransform;


    private void Start()
    {
        transform.position = cameraTransformInPlayer.position;
        transform.rotation = cameraTransformInPlayer.rotation;
    }


    // Update is called once per frame
    void Update()
    {
        transform.position = cameraTransformInPlayer.position;
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x,playerTransform.rotation.eulerAngles.y,0);
    }
}
