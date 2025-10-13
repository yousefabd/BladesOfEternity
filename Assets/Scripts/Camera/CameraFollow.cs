using Unity.Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] CinemachineCamera cinemachineVirtualCamera;

    float orthographicSize;
    float targetOrthographicSize;

    private void Start()
    {
        orthographicSize = cinemachineVirtualCamera.Lens.OrthographicSize;
    }

    private void Update()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");

        float moveSpeed = 22f;
        Vector3 moveDir = new Vector3(x, y).normalized;

        transform.position += moveSpeed * Time.deltaTime * moveDir;
    }
}
