using System;
using UnityEngine;

public class Vehicle : MonoBehaviour
{
    [SerializeField] float moveVelocity = 4.0f;
    [SerializeField] float removeThresholdY = -8.0f;

    Rigidbody2D rb;

    public Action<Vehicle> destroyed;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (transform.localPosition.y <= removeThresholdY)
        {
            Destroy(gameObject);
            destroyed?.Invoke(this);
        }
    }

    public void Move()
    {
        rb.linearVelocityY = -moveVelocity;
    }
}