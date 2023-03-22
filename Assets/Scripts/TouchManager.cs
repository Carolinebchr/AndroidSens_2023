using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class TouchManager : MonoBehaviour
{
    private PlayerInput playerInput;

    private InputAction touchPositionAction;

    private InputAction touchPressAction;

    [SerializeField] private GameObject player;


    private void Awake()
    {
        playerInput= GetComponent<PlayerInput>();

        touchPressAction = playerInput.actions.FindAction("TouchPress");

        touchPositionAction = playerInput.actions.FindAction("TouchPosition");

    }
    private void OnEnable()
    {
        touchPressAction.performed += TouchPressed;

    }

    private void OnDisable()
    {
        touchPressAction.performed -= TouchPressed;
    }

    private void TouchPressed(InputAction.CallbackContext context)
    {

        Vector3 position = Camera.main.ScreenToWorldPoint(touchPositionAction.ReadValue<Vector2>());
        position.z = player.transform.position.z;
        player.transform.position = position;
    }

    private void Update()
    {
        if (touchPositionAction.WasPerformedThisFrame())
        {
            Vector3 position = Camera.main.ScreenToWorldPoint(touchPositionAction.ReadValue<Vector2>());
            position.z = player.transform.position.z;
            player.transform.position = position;
        }

    }

}
