using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementController : MonoBehaviour
{
    public float carSpeed = 25.0f;
    [SerializeField] private float moveX = 1.0f; //переменная показывает, с какой скоростью игрок будет курицу влево и вправо

    [HideInInspector] public float _leftBoard = -3;
    [HideInInspector] public float _rightBoard = 3;

    [HideInInspector] public bool _stopGame = true;
    private float startPositionY = 0.642f;

    [HideInInspector] public float jumpSize = .0f;
    [SerializeField] private float jumpSpeed = 0.1f;

    private void Awake()
    {
        _stopGame = true;
    }

    private void Start()
    {
        _leftBoard = GameLevelController.Instance.leftBoard;
        _rightBoard = GameLevelController.Instance.RightBoard;
        startPositionY = gameObject.transform.position.y;
        jumpSize = startPositionY;
    }

    private void FixedUpdate()
    {
        if (_stopGame) return;

        if (ButtonManager.Instance.drag)
            MoveCar();

        if (gameObject.transform.position.y != jumpSize && (gameObject.transform.position.y < jumpSize - .01f || gameObject.transform.position.y > jumpSize + .01f))
            gameObject.transform.position = new Vector3(gameObject.transform.position.x, Mathf.Lerp(gameObject.transform.position.y, jumpSize, jumpSpeed), gameObject.transform.position.z);
        else
            jumpSize = startPositionY;
    }

    private void MoveCar()
    {
        float axis = ButtonManager.Instance.moveAxisX;

        float _moveXAxis = (gameObject.transform.position.x + axis * moveX);
        gameObject.transform.position = new Vector3((Mathf.Clamp(_moveXAxis, _leftBoard + gameObject.transform.localScale.x, _rightBoard - gameObject.transform.localScale.x)),
            transform.position.y, (transform.position.z + (carSpeed * Time.deltaTime)));
    }

    public void Jump(float sizeY)
    {
        if (_stopGame) return;
        jumpSize = sizeY + sizeY * 2;
    }
}
