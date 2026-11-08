using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;
using UnityRandom = UnityEngine.Random;

public class Circle : MonoBehaviour
{
    [SerializeField] SpriteRenderer outerSprite;
    [SerializeField] SpriteRenderer innerSprite;

    [SerializeField] GameObject outerCircle;
    [SerializeField] GameObject innerCircle;

    [SerializeField] AnimationCurve fadeInCurve = AnimationCurve.EaseInOut(0,0,1,1);

    bool fullyVisible = false;

    float fadeInDuration = 2.0f;
    float fadeInTimeElapsed = 0;

    float fadeOutDuration = 0.1f;
    float fadeOutTimeElapsed = 0;

    bool shouldFadeout = false;

    float lifetime = 2f;

    Color circleColor;
    Color innerCircleColor;

    Color perfectColor = new Color(1,0.83f,0.38f,1);

    float currentAlpha = 0;

    float failShakeDuration = 0.3f;
    float failShakeMagnitude = 0.08f;

    float perfectWindow = 0.5f;

    bool inPerfectWindow = false;

    public int hitResult = 0;

    public Action<Circle> destroyed;

    void Awake()
    {
        Assert.IsNotNull(outerSprite);
        Assert.IsNotNull(innerSprite);

        Assert.IsNotNull(outerCircle);
        Assert.IsNotNull(innerCircle);

        circleColor = outerSprite.color;
        innerCircleColor = innerSprite.color;
        circleColor.a = 0;
        innerCircleColor.a = 0;

        outerSprite.color = circleColor;
        innerSprite.color = innerCircleColor;
    }

    void Update()
    {
        if (!fullyVisible)
        {
            fadeInTimeElapsed += Time.deltaTime;
            currentAlpha = GetCurrentAlpha();

            if (fadeInTimeElapsed >= fadeInDuration)
            {
                fullyVisible = true;
                StartCoroutine(DisappearCoroutine());
                StartCoroutine(PerfectWindowCoroutine());
            }
        }
        else if (shouldFadeout)
        {
            fadeOutTimeElapsed += Time.deltaTime;
            currentAlpha = 1f - Mathf.Clamp01(fadeOutTimeElapsed/fadeOutDuration);

            if (fadeOutTimeElapsed >= fadeOutDuration)
            {
                destroyed?.Invoke(this);
                Destroy(gameObject);
                return;
            }
        }

        ApplyColors();
    }

    IEnumerator PerfectWindowCoroutine()
    {
        inPerfectWindow = true;
        yield return new WaitForSeconds(perfectWindow);
        inPerfectWindow = false;
    }

    void ApplyColors()
    {
        var outer = inPerfectWindow ? perfectColor : circleColor;
        outer.a = currentAlpha;
        outerSprite.color = outer;
        
        var inner = innerCircleColor;
        inner.a = currentAlpha;
        innerSprite.color = inner;
    }

    public void SetCircleColor(Color c)
    {
        circleColor = new Color(c.r,c.g,c.b,0);
    }

    IEnumerator DisappearCoroutine()
    {
        yield return new WaitForSeconds(lifetime);
        shouldFadeout = true;
    }

    void OnClick(InputValue input)
    {
        var mousePos = Mouse.current.position.ReadValue();
        if (SelfContainsPoint(mousePos))
        {
            if (!fullyVisible)
            {
                Shake();
                return;
            }

            if (inPerfectWindow)
                hitResult = 2;
            else
                hitResult = 1;

            shouldFadeout = true;
        }
    }

    void Shake()
    {
        StartCoroutine(ShakeCoroutine());
    }

    IEnumerator ShakeCoroutine()
    {
        float elapsed = 0;
        while (elapsed < failShakeDuration)
        {
            var pos = UnityRandom.insideUnitCircle * failShakeMagnitude;
            outerCircle.transform.localPosition = pos;
            innerCircle.transform.localPosition = pos;
            elapsed += Time.deltaTime;
            yield return null;
        }
        outerCircle.transform.localPosition = Vector2.zero;
        innerCircle.transform.localPosition = Vector2.zero;
    }

    bool SelfContainsPoint(Vector2 p)
    {
        var radius = outerSprite.bounds.size.x/2;
        var center = new Vector2(transform.position.x,transform.position.y);

        var pn = Camera.main.ScreenToWorldPoint(p);

        return Vector2.Distance(pn,center) < radius;
    }

    float GetCurrentAlpha()
    {
        var t = Mathf.Clamp01(fadeInTimeElapsed/fadeInDuration);
        return fadeInCurve.Evaluate(t);
    }
}