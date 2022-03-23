using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RotateCamera : MonoBehaviour
{
    [SerializeField] GameObject player;
    [SerializeField] float rotateSpeed;

    public PlayerActionsExample playerInput;
    private InputAction move;
    float horizontalInput;

    void Start()
    {
        playerInput = new PlayerActionsExample();
        move = playerInput.Player.Move;
        move.Enable();
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 input = move.ReadValue<Vector2>();
        horizontalInput = input.x;
    }

    void LateUpdate()
    {
        transform.Rotate(Vector3.up, rotateSpeed * horizontalInput * Time.deltaTime);

        if (player.transform.position.y > 0)
            transform.position = player.transform.position;
    }
}
