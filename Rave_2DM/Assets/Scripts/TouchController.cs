using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TouchController : MonoBehaviour, IPointerDownHandler, IDragHandler, IBeginDragHandler
{
    [SerializeField] private CameraController camera;
    private RaycastHit2D hit;

    public void OnBeginDrag(PointerEventData eventData)
    {
        camera.CameraMove(new Vector3(eventData.delta.x, eventData.delta.y, 0));
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        /*var touches = Input.touches;
        Debug.Log("Touch pos = " + touches[0].position);
        Vector3 touch = Camera.main.ScreenToWorldPoint(eventData.position);
        //if (ray.)
        Debug.Log("position = " + eventData.position);
        Debug.Log("Ray.world = " + eventData.pointerCurrentRaycast.worldPosition);
        Debug.Log("Ray.worldNormal = " + eventData.pointerCurrentRaycast.worldNormal);
    */}

    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        camera.CameraMove(new Vector3(eventData.delta.x, eventData.delta.y, 0));       
    }


}
