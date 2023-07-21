using System;
using UnityEngine;
using Utilities;

public class PlayerInputManager : MonoBehaviour
{
    public bool isDragging;
    public Action<Vector3> onDragging;
    public Action<Vector3> onLeftButtonDown;
    public Action onLeftButtonUp;
    public Action<Vector3> onRightButtonDown;


    public static PlayerInputManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (isDragging) HandleDragging();

        if (Input.GetMouseButtonDown(0)) HandleLeftMouseButtonDown();
        else if (Input.GetMouseButtonUp(0)) HandleLeftMouseButtonUp();
        else if (Input.GetMouseButtonDown(1)) HandleRightMouseButtonDown();
    }

    private void HandleLeftMouseButtonDown()
    {
        var position = Helpers.Camera.ScreenToWorldPoint(Input.mousePosition);

        onLeftButtonDown?.Invoke(position);
    }

    private void HandleLeftMouseButtonUp()
    {
        onLeftButtonUp?.Invoke();
    }


    private void HandleDragging()
    {
        var position = Helpers.Camera.ScreenToWorldPoint(Input.mousePosition);
        position.z = 0;
        onDragging?.Invoke(position);
    }


    private void HandleRightMouseButtonDown()
    {
        var position = Helpers.Camera.ScreenToWorldPoint(Input.mousePosition);
        onRightButtonDown?.Invoke(position);
    }
}