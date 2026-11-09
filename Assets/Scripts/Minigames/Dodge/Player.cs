using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] DodgeMinigame game;
    [SerializeField] GameObject lanesContainer;
    List<Lane> lanes = new();

    int currentLane = 1;

    float laneTransitionDuration = 0.1f;
    float laneTransitionCoyote = 0.1f;
    
    float lastTransitionAttempt = 0;
    int lastTransitionDirection = 0;

    bool inLaneTransition = false;

    void Start()
    {
        Assert.IsNotNull(game);
        Assert.IsNotNull(lanesContainer);
        foreach(Transform child in lanesContainer.transform)
        {
            var obj = child.gameObject;
            var lane = obj.GetComponent<Lane>();
            Assert.IsNotNull(lane);
            lanes.Add(lane);
        }
    }

    void OnLose()
    {
        Destroy(gameObject);
        lanes.Clear();
    }

    System.Collections.IEnumerator LaneTransitionCoroutine(float targetX)
    {
        inLaneTransition = true;

        var timeElapsed = 0f;
        var startX = transform.localPosition.x;
        while (timeElapsed < laneTransitionDuration)
        {
            timeElapsed += Time.deltaTime;

            var t = timeElapsed/laneTransitionDuration;
            var te = 1-(1-t)*(1-t);
            var x = Mathf.Lerp(startX,targetX,te);

            transform.localPosition = new Vector2(
                x,
                transform.localPosition.y
            );

            yield return null;
        }

        inLaneTransition = false;

        var diff = Time.time-lastTransitionAttempt;
        if (diff <= laneTransitionCoyote)
            UpdatePosition(currentLane+lastTransitionDirection);
    }

    void UpdatePosition(int newLane)
    {
        newLane = Math.Clamp(newLane,0,lanes.Count-1);

        if (inLaneTransition)
        {
            lastTransitionAttempt = Time.time;
            lastTransitionDirection = newLane-currentLane;
            return;
        }
        lastTransitionAttempt = 0;
        lastTransitionDirection = 0;

        var lane = lanes.ElementAtOrDefault(newLane);
        if (!lane) return;

        currentLane = newLane;
        StartCoroutine(LaneTransitionCoroutine(lane.transform.localPosition.x));
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Vehicle"))
        {
            game.LoseGame();
            OnLose();
        }
    }

    void OnMoveLeft(InputValue input)
    {
        if (!input.isPressed) return;

        UpdatePosition(currentLane-1);
    }

    void OnMoveRight(InputValue input)
    {
        if (!input.isPressed) return;

        UpdatePosition(currentLane+1);
    }
}