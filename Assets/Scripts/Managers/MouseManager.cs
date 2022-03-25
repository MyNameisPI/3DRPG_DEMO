using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Cinemachine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MouseManager : Singleton<MouseManager>
{

    public Texture2D point, doorway, attack, target, arrow;

    RaycastHit hitInfo;
    public event Action<Vector3> OnMouseClicked;
    public event Action<GameObject> OnEnemyClicked;

    public GameObject UI;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }

    private void Update()
    {
        SetCursorTexture();
        if (InteractWithUI()) return;
        MouseControl();
    }

    void SetCursorTexture()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray,out hitInfo))
        {
            if (InteractWithUI())
            {
                Cursor.SetCursor(arrow, new Vector2(0, 0), CursorMode.Auto);
                return;
            }
            //«–ªª Û±ÍÃ˘Õº
            switch (hitInfo.collider.gameObject.tag)
            {
                case "Ground":
                    Cursor.SetCursor(target, new Vector2(16, 16), CursorMode.Auto);
                    break;
                case "Attackable":
                    Cursor.SetCursor(attack, new Vector2(16, 16), CursorMode.Auto);
                    break;
                case "Enemy":
                    Cursor.SetCursor(attack, new Vector2(16, 16), CursorMode.Auto);
                    break;
                case "Portal":
                    Cursor.SetCursor(doorway, new Vector2(16, 16), CursorMode.Auto);
                    break;
                case "Item":
                    Cursor.SetCursor(point, new Vector2(16, 16), CursorMode.Auto);
                    break;

                default:
                    Cursor.SetCursor(arrow, new Vector2(0, 0), CursorMode.Auto);
                    break;

            }
        }
    }

    void MouseControl()
    {
        if (Input.GetMouseButtonDown(0)&&hitInfo.collider!=null)
        {
            if (hitInfo.collider.gameObject.CompareTag("Ground"))
            {
                OnMouseClicked?.Invoke(hitInfo.point);
            }
            if (hitInfo.collider.gameObject.CompareTag("Enemy"))
            {
                OnEnemyClicked?.Invoke(hitInfo.collider.gameObject);
            }
            if (hitInfo.collider.gameObject.CompareTag("Attackable"))
            {
                OnEnemyClicked?.Invoke(hitInfo.collider.gameObject);
            }
            if (hitInfo.collider.gameObject.CompareTag("Portal"))
            {
                OnMouseClicked?.Invoke(hitInfo.point);
            }
            if (hitInfo.collider.gameObject.CompareTag("Item"))
            {
                OnMouseClicked?.Invoke(hitInfo.point);
            }
        }
    }

    bool InteractWithUI()
    {
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return true;
        else
            return false;

    }

    bool CheckGUIRayCastObjects()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;

        List<RaycastResult> list = new List<RaycastResult>();
        UI.GetComponent<GraphicRaycaster>().Raycast(eventData, list);
        return list.Count > 0;
    }


}
