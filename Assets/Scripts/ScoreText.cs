using UnityEngine;
using TMPro;
using PrimeTween;

public class ScoreText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textMesh;
    [SerializeField] private float floatDistance = 100f;
    [SerializeField] private float duration = 1f;

    public void Setup(int score, Color color)
    {
        if (textMesh == null) textMesh = GetComponentInChildren<TextMeshProUGUI>();
        
        textMesh.text = "+" + score.ToString();
        textMesh.color = color;

        // Reset state
        transform.localScale = Vector3.zero;
        
        // Animation sequence
        Sequence.Create()
            .Group(Tween.LocalPosition(transform, transform.localPosition + Vector3.up * floatDistance, duration, Ease.OutQuad))
            .Group(Tween.Scale(transform, 1f, duration * 0.2f, Ease.OutBack))
            .Group(Tween.Alpha(textMesh, 0f, duration, Ease.InQuad, startDelay: duration * 0.5f))
            .OnComplete(() => Destroy(gameObject));
    }
}
