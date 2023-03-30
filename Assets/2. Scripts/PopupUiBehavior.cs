using DG.Tweening;
using UnityEngine;

public class PopupUiBehavior : MonoBehaviour
{
    private Vector3 originalScale;
    private void Awake()
    {
        originalScale = transform.localScale;
    }
    private void OnEnable()
    {
        transform.DOPunchScale(originalScale * 0.3f, 0.2f, 0, 1f);
    }
}
