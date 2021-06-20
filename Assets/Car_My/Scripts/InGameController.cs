using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class InGameController : MonoBehaviour
{
    private static InGameController instance; 
    public static InGameController Instance => instance;

    [SerializeField] private WorldCurver worldCurver;

    [SerializeField] private Animator cameraAnim;
    [SerializeField] private float clipSize = 120.0f;
    [SerializeField] private Text startStText;
    private float _stStart = 0;
    private bool _st = false;

    public GameObject StartButton;
    [SerializeField] private GameObject InputSystem;
    [SerializeField] private Text pointText;
    [SerializeField] private Text distanceText;
    [SerializeField] private Text pointPlusText;

    public GameObject player;
    private float playerSpeed = 0;
    private MovementController playerController;
    [SerializeField] private GameLevelController GLController;
    [SerializeField] private float timeSpawnOst = 3.0f;

    [SerializeField] private GameObject finishPanel;
    [SerializeField] private Text finishResult;


    private float timer;
    private float distance;
    private int pointValue = 0;

    private int state = 0;

    public bool stopGame = true;

    public void StartGame()//Запуск игры после
    {
        cameraAnim.SetTrigger("StartPresent");
        if (StartButton != null) StartButton.SetActive(false); else Debug.LogError("добавьте в метод CarController StartButton");
        StartCoroutine(StartPresentAnim());
        Diffuse(0);
        pointValue = 0;
        pointText.text = pointValue.ToString();
    }

    public void EndGame()
    {
        finishPanel.SetActive(true);
        finishResult.text = "You Result: " + pointValue;
        player.GetComponent<CarController>().LoseGame();
        GLController.PauseGame();
        stopGame = true;
    }

    public void PauseGame()//остановка уровня без обновления
    {
        player.GetComponent<CarController>().PauseGame();
        GLController.PauseGame();
        stopGame = true;
    }

    public void ResumedGame()
    {
        SceneManager.LoadScene(0);
        /*player.GetComponent<CarController>().ResumedGame();
        GLController.ResumedGame();
        stopGame = false;*/
    }

    public void PlayGame()//запуск уровня
    {
        _stStart = 4;
        _st = true;

        cameraAnim.SetTrigger("Play");

        if (startStText != null) startStText.gameObject.SetActive(true); else Debug.LogError("добавьте в метод");
        if (StartButton != null) StartButton.SetActive(false); else Debug.LogError("добавьте в метод CarController StartButton");
        if (InputSystem != null) InputSystem.SetActive(true); else Debug.LogError("добавьте в метод CarController StartButton");
    }

    public void ResetGame()// обновление уровня и всех данных на нем
    {
        cameraAnim.SetTrigger("StartPresent");
        StartCoroutine(StartPresentAnim());
        pointPlusText.color = new Color(pointPlusText.color.r, pointPlusText.color.g, pointPlusText.color.b, 0);
        distanceText.text = 0 + " m";
        Diffuse(0);
        pointValue = 0;
        pointText.text = pointValue.ToString();
        //if (StartButton != null) StartButton.SetActive(true); else Debug.LogError("добавьте в метод CarController StartButton");
        if (InputSystem != null) InputSystem.SetActive(false); else Debug.LogError("добавьте в метод CarController StartButton");
        worldCurver.changeViaTime = false;
        stopGame = true;
        GLController.ResetLevel();
        player.GetComponent<CarController>().ResetGame();
    }

    private IEnumerator StartPresentAnim()
    {
        yield return new WaitForSeconds(clipSize / 60);
        StartButton.SetActive(true);
    }

    public void UpdatePoint(int point)
    {
        pointValue += point;
        pointText.text = pointValue.ToString();
        pointPlusText.text = "+" + point.ToString();
        StartCoroutine(AlfaUpgrade());
    }

    private IEnumerator AlfaUpgrade()
    {
        pointPlusText.color = new Color(pointPlusText.color.r, pointPlusText.color.g, pointPlusText.color.b, 1);

        yield return new WaitForSeconds(1.5f);
        pointPlusText.color = new Color(pointPlusText.color.r, pointPlusText.color.g, pointPlusText.color.b, 0);
    }

    private void StInStart()
    {
        if (_stStart > 1)
        {
            _stStart -= 1 * Time.deltaTime;
            startStText.text = Mathf.FloorToInt(_stStart).ToString();
        }
        else
        {
            if (_stStart > 0.5f)
            {
                _stStart -= 1 * Time.deltaTime;
                startStText.text = "GO";
            }
            else
            {
                if (_st)
                {
                    player.GetComponent<CarController>().StartGame();
                    GLController.StartGame();
                    worldCurver.changeViaTime = true;
                    if (startStText != null) startStText.gameObject.SetActive(false); else Debug.LogError("добавьте в метод");
                    stopGame = false;
                    _st = false;
                }
            }
        }
    }

    private void Diffuse(int _state)
    {
        switch (_state)
        {
            case 0:
                GLController.trp_timeSpawn = timeSpawnOst;
                playerController.carSpeed = playerSpeed;
                timer = 0;
                state = 0;
                break;
            case 1:
                GLController.trp_timeSpawn = timeSpawnOst / 1.2f;
                playerController.carSpeed = playerSpeed * 1.2f;
                break;
            case 2:
                GLController.trp_timeSpawn = timeSpawnOst / 1.4f;
                playerController.carSpeed = playerSpeed * 1.4f;
                break;
        }
    }

    public void Update()
    {
        if (_st)
            StInStart();

        if (stopGame) return;

        timer += 1 * Time.deltaTime;//немного хитрим, т.к. игрок не отличит время от дистации
        if (Mathf.FloorToInt(timer) % 1 == 0) distanceText.text = (Mathf.FloorToInt(timer) * 10 / 2).ToString() + " m";

        if (timer > 10 && state == 0)
        {
            state += 1;
            Diffuse(state);
        }
        else
        if (timer > 20 && state == 1)
        {
            state += 1;
            Diffuse(state);
        }


        if (distance < 5)
            distance += 1 * Time.deltaTime;
        else
        {
            UpdatePoint(5);
            distance = 0;
        }
    }

    private void Start()
    {
        instance = this;
        pointPlusText.color = new Color(pointPlusText.color.r, pointPlusText.color.g, pointPlusText.color.b, 0);
        pointValue = 0;
        pointText.text = pointValue.ToString();
        GLController.player = player;

        playerController = player.GetComponent<MovementController>();
        playerSpeed = playerController.carSpeed;
        Diffuse(0);
        if (startStText != null) startStText.gameObject.SetActive(false); else Debug.LogError("добавьте в метод");
    }

    private void Awake()
    {
        stopGame = true;
        if (worldCurver == null) Debug.LogError("Не присвоен метод worldCurver в GameLevelController");
        else
            worldCurver.changeViaTime = false;

        if (StartButton != null) StartButton.SetActive(false); else Debug.LogError("добавьте в метод CarController StartButton");
        if (InputSystem != null) InputSystem.SetActive(false); else Debug.LogError("добавьте в метод CarController StartButton");
    }
}
