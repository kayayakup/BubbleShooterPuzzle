using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PrimeTween;

public class BackgroundManager : MonoBehaviour
{
    [Header("Sprites")]
    public Sprite[] backgroundSprites;

    [Header("References")]
    public Image backgroundA;
    public Image backgroundB;

    [Header("Timing")]
    public float cycleInterval = 5f;
    public float transitionDuration = 1.5f;

    private int currentIndex = 0;
    private bool usingA = true;

    void Start()
    {
        if (backgroundSprites == null || backgroundSprites.Length == 0)
        {
            Debug.LogWarning("BackgroundManager: No sprites assigned!");
            return;
        }

        // Initialize first background
        backgroundA.sprite = backgroundSprites[0];
        backgroundA.color = Color.white;
        backgroundB.color = new Color(1, 1, 1, 0); // Transparent
        
        StartCoroutine(BackgroundCycleRoutine());
    }

    IEnumerator BackgroundCycleRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(cycleInterval);

            if (backgroundSprites.Length <= 1) continue;

            // Move to next sprite
            currentIndex = (currentIndex + 1) % backgroundSprites.Length;
            Sprite nextSprite = backgroundSprites[currentIndex];

            if (usingA)
            {
                // Switch from A to B
                backgroundB.sprite = nextSprite;
                Tween.Alpha(backgroundB, 0.21f, transitionDuration);
                Tween.Alpha(backgroundA, 0f, transitionDuration);
            }
            else
            {
                // Switch from B to A
                backgroundA.sprite = nextSprite;
                Tween.Alpha(backgroundA, 0.21f, transitionDuration);
                Tween.Alpha(backgroundB, 0f, transitionDuration);
            }

            usingA = !usingA;
        }
    }
}
