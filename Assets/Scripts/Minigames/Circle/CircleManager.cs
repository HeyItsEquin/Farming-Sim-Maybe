using UnityEngine;
using UnityEngine.Assertions;
using System.Collections.Generic;
using System.Collections;

using UnityRandom = UnityEngine.Random;

public class CircleManager : MonoBehaviour
{
    [SerializeField] Circle circlePrefab;
    [SerializeField] GameObject circleContainer;

    [SerializeField] ParticleSystem perfectIndicatorParticles;
    [SerializeField] ParticleSystem goodIndicatorParticles;
    [SerializeField] ParticleSystem missIndicatorParticles;

    List<Circle> circles = new();

    int circleSpawnCount = 5;
    float circleDelay = 1.7f;

    float playAreaWidth = Screen.width*0.75f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Assert.IsNotNull(circlePrefab, "Must assign Circle Prefab");
        Assert.IsNotNull(circleContainer, "Must assign Circle Container");

        Assert.IsNotNull(perfectIndicatorParticles);
        Assert.IsNotNull(goodIndicatorParticles);
        Assert.IsNotNull(missIndicatorParticles);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartGame()
    {
        StartCoroutine(GameCoroutine());
    }

    IEnumerator GameCoroutine()
    {
        for (var i = 0; i < circleSpawnCount; i++)
        {
            SpawnCircle(GetRandomPos());
            yield return new WaitForSeconds(circleDelay);
        }
    }

    Vector2 GetRandomPos()
    {
        var maxWidth = (Screen.width+playAreaWidth)/2;
        var minWidth = (Screen.width-playAreaWidth)/2;
        var pos = new Vector2(
            UnityRandom.Range(minWidth,maxWidth),
            UnityRandom.Range(0,Screen.height)
        );
        return Camera.main.ScreenToWorldPoint(pos);
    }

    Circle SpawnCircle(Vector2 position)
    {
        var circle = Instantiate(circlePrefab);
        circle.transform.position = position;
        circle.transform.SetParent(circleContainer.transform);
        circle.SetCircleColor(RandomColor());
        circle.destroyed += (c) =>
        {
            circles.Remove(c);
            ShowHitResultPopup(c);
        };
        circles.Add(circle);
        return circle;
    }

    void ShowHitResultPopup(Circle c)
    {
        var pos = new Vector2
        {
            x = c.transform.position.x,
            y = c.transform.position.y+10
        };

        var p = new ParticleSystem.EmitParams
        {
            position = pos,
            startSize = 15,
            applyShapeToPosition = false
        };

        switch(c.hitResult)
        {
            case 0:
                missIndicatorParticles.Emit(p,1);
                break;
            case 1:
                goodIndicatorParticles.Emit(p,1);
                break;
            case 2:
                perfectIndicatorParticles.Emit(p,2);
                break;
        }
    }

    Color RandomColor()
    {
        Color[] colors =
        {
            Color.red,
            Color.blue,
            Color.gray,
            Color.green,
            Color.magenta
        };

        return colors[UnityRandom.Range(0,colors.Length-1)];
    }
}
