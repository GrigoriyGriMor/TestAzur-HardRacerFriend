using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.Events;

public class CarController : MonoBehaviour
{
    private bool _stopGame = true;

    [SerializeField] private Animator carAnim;
    [SerializeField] private Animator personAnim;

    [SerializeField] private AudioSource sorce;
    [SerializeField] private AudioClip jumpClip;
    [SerializeField] private AudioClip obstacleClip;
    [SerializeField] private AudioClip collectblClip;

    [SerializeField] private ParticleSystem collectblePS;

    [SerializeField] private Transform startPosition;

    private MovementController moveController;

    [SerializeField] private UnityEvent startPanelActive = new UnityEvent();

    private void Awake()
    {
        moveController = gameObject.GetComponent<MovementController>();
        _stopGame = true;
        gameObject.transform.position = startPosition.position;
        startPanelActive.Invoke();
    }

    public void StartGame()
    {
        _stopGame = false;
        moveController._stopGame = false;
        sorce.volume = 0.75f;
        GameLevelController.Instance.CreateFinish();
    }

    public void ResetGame()
    {
        carAnim.SetFloat("Blend", 0.0f);
        personAnim.SetBool("Run", false);
        GameLevelController.Instance.ResetLevel();
        carAnim.SetTrigger("StopAll");
        gameObject.transform.position = startPosition.position;
        sorce.volume = 0.01f;
        _stopGame = true;
        moveController._stopGame = true;
    }

    public void PauseGame()
    {
        moveController._stopGame = true;
        _stopGame = true;
    }

    public void ResumedGame()
    {
        moveController._stopGame = false;
        _stopGame = false;
    }

    public void LoseGame()
    {
        personAnim.SetBool("Run", false);
        moveController._stopGame = true;
        sorce.volume = 0.01f;
        _stopGame = true;
      //  startPanelActive.Invoke();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_stopGame) return;

        if (other.gameObject.GetComponent<ObstacleObject>())
        {
            if (obstacleClip != null) sorce.PlayOneShot(obstacleClip);
            InGameController.Instance.EndGame();
        }

        if (other.gameObject.GetComponent<TramplineObject>())
        {
            if (jumpClip != null) sorce.PlayOneShot(jumpClip);
            moveController.Jump(other.gameObject.GetComponent<BoxCollider>().size.y);
            InGameController.Instance.UpdatePoint(25);
            carAnim.SetTrigger("Jump");
        }

        if (other.gameObject.GetComponent<EageController>())
        {
            if (jumpClip != null) sorce.PlayOneShot(collectblClip);
            InGameController.Instance.UpdatePoint(50);
            collectblePS.Play(true);
            other.gameObject.SetActive(false);
        }

    }

    private void Update()
    {
        if (_stopGame) return;

        if (ButtonManager.Instance.drag)
        {
            sorce.volume = 0.75f;
            personAnim.SetBool("Run", true);
        }
        else
        {
            sorce.volume = 0.01f;
            personAnim.SetBool("Run", false);
        }


        float axis = ButtonManager.Instance.moveAxisX;
        carAnim.SetFloat("Blend", axis);
    }
}
