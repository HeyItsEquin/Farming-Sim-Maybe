using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    PlayerMovement movement;

    void Start()
    {
        movement = GetComponent<PlayerMovement>();
    }

    void OnMove(InputValue input)
    {
        var inputVec = input.Get<Vector2>();
        movement.Move(inputVec);
    }

    void OnSprint(InputValue input)
    {
        var inputFloat = input.Get<float>();
        movement.UpdateSprint(inputFloat > 0.5);
    }
}