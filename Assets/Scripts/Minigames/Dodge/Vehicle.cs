using System;
using UnityEngine;

public class Vehicle : MonoBehaviour
{
    [SerializeField] float moveVelocity = 4.0f;
    [SerializeField] float removeThresholdY = -8.0f;

    float spawnY = 8f;

    public static float BaseVelocity = 4.0f;

    Rigidbody2D rb;

    public Action<Vehicle> destroyed;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        BaseVelocity = moveVelocity;
    }

    void Update()
    {
        if (transform.localPosition.y <= removeThresholdY)
        {
            Destroy(gameObject);
            destroyed?.Invoke(this);
        }
    }

    public void SpawnAt(Lane l)
    {
        transform.localPosition = new Vector2(l.GetX(), spawnY);
    }

    public void SetMoveVelocity(float v) => moveVelocity = v;

    public void Move()
    {
        rb.linearVelocityY = -moveVelocity;
    }
}