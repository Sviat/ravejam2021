using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Vector3 newPosition;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float zoomSpeed;

    private Camera camera;

    private void Start()
    {
        newPosition = transform.position;
        moveSpeed = 3f;
        zoomSpeed = 0.25f;
        camera = GetComponent<Camera>();
    }

    void Update()
    {
        if (Vector3.Distance(newPosition, transform.position) > 0.01f)
            transform.position = Vector3.Lerp(transform.position, newPosition, moveSpeed * Time.deltaTime);

        if (Input.mouseScrollDelta != Vector2.zero)
            CameraZoom(Input.mouseScrollDelta.x, Input.mouseScrollDelta.y);
    }

    public void CameraMove(Vector3 deltaPosition)
    {
        newPosition -= deltaPosition * Time.deltaTime;
    }

    public void CameraZoom(float deltaX, float deltaY)
    {
        float newSize = camera.orthographicSize - deltaY * zoomSpeed;
        if (newSize > 3 && newSize <= 15)
            camera.orthographicSize = newSize;    
    }

    
}
