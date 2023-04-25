using UnityEngine;
using DG.Tweening;

public class FloatingTicket : MonoBehaviour
{
    public float moveAmount=3;
    public float rotateAmount = 3000;
    public float colorSpeed;
    Color ticketColor;
    SpriteRenderer sprite;

    private void Start()
    {
        sprite = transform.GetChild(0).GetComponent<SpriteRenderer>();
        ticketColor = sprite.color;
    }

    private void Update()
    {
        ticketColor.a = Mathf.Lerp(ticketColor.a, 0, Time.deltaTime * colorSpeed);
        sprite.color = ticketColor;
        if (ticketColor.a <= 0.5f)
        {
            //DOTween.KillAll();
            ticketColor.a = 1;
            transform.position = new Vector3(0, 0, 0);
            this.gameObject.SetActive(false);
        }
    }

    public void FloatingEffet()
    {
        transform.DOMoveY(transform.position.y + moveAmount, 3);
        transform.DORotate(new Vector3(0, rotateAmount, 0), 3, RotateMode.FastBeyond360).SetEase(Ease.OutQuint);
    }
}
