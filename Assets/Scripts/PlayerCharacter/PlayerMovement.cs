using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float playerSpeed = 5f;
    [SerializeField] float sprintSpeedOffset = 3f;

    Vector2 position;
    Vector2 inputDirection;

    bool sprinting = false;

    void Update()
    {      
        UpdatePosition();
        transform.position = position;
    }

    void UpdatePosition()
    {
        var effectiveSpeed = sprinting ? playerSpeed+sprintSpeedOffset : playerSpeed;
        position += Time.deltaTime * effectiveSpeed * inputDirection;
    }

    public void Move(Vector2 input)
    {
        inputDirection = input;
    }

    public void UpdateSprint(bool should) => sprinting = should;
}