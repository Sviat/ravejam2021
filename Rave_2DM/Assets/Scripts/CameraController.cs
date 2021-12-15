using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Vector3 newPosition;
    [SerializeField] private float moveSpeed;

    private void Start()
    {
        newPosition = transform.position;
        moveSpeed = 3f;
    }

    void Update()
    {
        if (Vector3.Distance(newPosition, transform.position) > 0.01f)
        {
            transform.position = Vector3.Lerp(transform.position, newPosition, moveSpeed * Time.deltaTime);
            //transform.position = Vector3.MoveTowards(transform.position, newPosition, moveSpeed * Time.deltaTime);
        }

        if (Input.touchCount == 1)
        {
            Debug.Log("Touch detected");
            var posTouch = Input.GetTouch(0).position;
            Vector3 rayDirection = Camera.main.ScreenToWorldPoint(posTouch);

            RaycastHit2D hit = Physics2D.Raycast(rayDirection, Vector2.zero);
            if (hit.collider != null)
            {
                Debug.Log("Target Position: " + hit.collider.gameObject.transform.position);
                Destroy(hit.collider.gameObject);
            }

        }
    }

    public void CameraMove(Vector3 deltaPosition)
    {
        newPosition -= deltaPosition * Time.deltaTime;
    }

    public void CameraZoom()
    {

    }
}
