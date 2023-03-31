using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverUiManager : MonoBehaviour
{
    [SerializeField] private GameObject backgroundBase;
    [SerializeField] private float backgroundMoveSpeed;

    private Vector2 moveDir;
    private float moveX = 1;
    private float moveY = 1;
    private float xScreenHalfSize;
    private float yScreenHalfSize;

    void Start()
    {
        yScreenHalfSize = Screen.height / 2;
        xScreenHalfSize = Screen.width / 2;
        moveDir = new Vector2(moveX, moveY);
    }

    void Update()
    {
        MoveBackground();
    }

    private void MoveBackground()
    {
        float x = backgroundBase.GetComponent<RectTransform>().anchoredPosition.x;
        float y = backgroundBase.GetComponent<RectTransform>().anchoredPosition.y;

        if (x < -xScreenHalfSize)
            moveX = UnityEngine.Random.Range(0, 1f);
        else if (x > xScreenHalfSize)
            moveX = UnityEngine.Random.Range(-1f, 0);
        if (y < -yScreenHalfSize)
            moveY = UnityEngine.Random.Range(0, 1f);
        else if (y > yScreenHalfSize)
            moveY = UnityEngine.Random.Range(-1f, 0);

        moveDir.x = moveX;
        moveDir.y = moveY;
        backgroundBase.GetComponent<RectTransform>().anchoredPosition += backgroundMoveSpeed * Time.deltaTime * moveDir.normalized;
    }
}
