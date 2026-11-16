using UnityEngine;
using UnityEngine.Assertions;
using System.Collections.Generic;
using System.Collections;

using UnityRandom = UnityEngine.Random;

public class CircleManager : MonoBehaviour
{
    [SerializeField] CircleMinigame game;

    [SerializeField] Circle circlePrefab;
    [SerializeField] GameObject circleContainer;

    [SerializeField] ParticleSystem perfectIndicatorParticles;
    [SerializeField] ParticleSystem goodIndicatorParticles;
    [SerializeField] ParticleSystem missIndicatorParticles;

    List<Circle> circles = new();

    bool gameShouldEnd = false;

    int circleSpawnCount = 5;
    float circleDelay = 1.7f;

    float playAreaWidth = Screen.width*0.75f;
    float playAreaHeight = Screen.height*0.75f;

    void Start()
    {
        Assert.IsNotNull(game, "Must assign Game");

        Assert.IsNotNull(circlePrefab, "Must assign Circle Prefab");
        Assert.IsNotNull(circleContainer, "Must assign Circle Container");

        Assert.IsNotNull(perfectIndicatorParticles, "Must assign perfectIndicatorParticles");
        Assert.IsNotNull(goodIndicatorParticles, "Must assign goodIndicatorParticles");
        Assert.IsNotNull(missIndicatorParticles, "Must assign missIndicatorParticles");
    }

    public void StartSpawning()
    {
        ResetGameState();
        StartCoroutine(GameCoroutine());
    }

    void ResetGameState()
    {
        StopAllCoroutines();
        foreach (var c in circles)
            Destroy(c.gameObject);
        circles.Clear();
        gameShouldEnd = false;        
    }

    public void OnGameEnd()
    {
        ResetGameState();
    }

    IEnumerator GameCoroutine()
    {
        for (var i = 0; i < circleSpawnCount; i++)
        {
            SpawnCircle(GetRandomPoint());
            yield return new WaitForSeconds(circleDelay);
        }
        gameShouldEnd = true;
    }

    Vector2 GetRandomPoint()
    {
        var maxWidth = (Screen.width+playAreaWidth)/2;
        var minWidth = (Screen.width-playAreaWidth)/2;
        var maxHeight = (Screen.height+playAreaHeight)/2;
        var minHeight = (Screen.height-playAreaHeight)/2;
        var pos = Vector2.zero;
        do
        {
            pos = new Vector2(
                UnityRandom.Range(minWidth,maxWidth),
                UnityRandom.Range(minHeight,maxHeight)
            );
        } while (CheckPointOverlap(pos));
        return Camera.main.ScreenToWorldPoint(pos);
    }

    bool CheckPointOverlap(Vector2 p)
    {
        var radius = GetCircleRadius();
        foreach (var circle in circles)
        {
            var otherCenter = new Vector2(circle.transform.position.x,circle.transform.position.y);
            if (Vector2.Distance(p,otherCenter) <= 2*radius)
                return true;
        }
        return false;
    }

    float GetCircleRadius()
    {
        var res = 0f;
        var obj = Instantiate(circlePrefab);
        obj.transform.SetParent(circleContainer.transform);

        var c = obj.GetComponent<Circle>();
        if (c) res = c.Radius;

        Destroy(obj);
        return res;
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

            if (c.hitResult == 0)
                game.LoseHealth();

            game.AddPoints(c.hitResult*200);

            if (gameShouldEnd)
                StartCoroutine(GameEndCoroutine());
        };
        circles.Add(circle);
        return circle;
    }

    IEnumerator GameEndCoroutine()
    {
        // TODO: Implement
        yield return new WaitForSeconds(0.5f);
        game.EndGame(false);
    }

    void ShowHitResultPopup(Circle c)
    {
        var pos = new Vector2
        {
            x = c.transform.position.x,
            y = c.transform.position.y+0.2f
        };

        var p = new ParticleSystem.EmitParams
        {
            position = pos,
            startSize = c.hitResult == 0 ? 0.5f : 1.5f,
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
