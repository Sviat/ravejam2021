using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TouchController : MonoBehaviour, IPointerClickHandler, IDragHandler, IBeginDragHandler, IEndDragHandler
{ 
    [SerializeField] private CameraController camera;

    public void OnBeginDrag(PointerEventData eventData)
    {
        camera.CameraMove(new Vector3(eventData.delta.x, eventData.delta.y, 0));
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left && !eventData.dragging)
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(eventData.position), Vector2.zero);
            if (hit.collider != null)
            {
                hit.collider.gameObject.GetComponent<SpriteRenderer>().color = Color.black;
            }
        }
    }
    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        camera.CameraMove(new Vector3(eventData.delta.x, eventData.delta.y, 0));
        Debug.Log($"Event deltaX = {eventData.scrollDelta.x} / event deltaY = {eventData.scrollDelta.y}");
        if (eventData.scrollDelta != Vector2.zero)
            camera.CameraZoom(eventData.scrollDelta.x, eventData.scrollDelta.y);
    }

    void IEndDragHandler.OnEndDrag(PointerEventData eventData)
    {
        
    }
}
