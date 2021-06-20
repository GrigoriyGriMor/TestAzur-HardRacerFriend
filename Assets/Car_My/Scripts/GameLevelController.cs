using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameLevelController : MonoBehaviour
{
    private static GameLevelController instance;
    public static GameLevelController Instance => instance;

    public float leftBoard = -3;
    public float RightBoard = 3;

    private List<Transform> allTrackAtScene = new List<Transform>();
    private List<Transform> allTramplineAtScene = new List<Transform>();
    private List<Transform> allObstacleAtScene = new List<Transform>();
    private List<Transform> allCollectblsAtScene = new List<Transform>();
    private Transform finish;

    [HideInInspector] public GameObject player;

    [SerializeField] private GameObject[] trackBloks = new GameObject[4];
    [SerializeField] private int minCountTrackAtScene = 5;
    [SerializeField] private int factCountTrackAtScene = 0;

    [SerializeField] private GameObject trampline;
    [SerializeField] private GameObject obstacle;
    [SerializeField] private GameObject collectbles;
    [SerializeField] private GameObject finishObj;
    public bool finishCreate = false;
    [SerializeField] private float gameTimer = 20.0f;

    private float returnDistance = 45.0f;
    [HideInInspector] public float trp_timeSpawn = 5.0f;

    private GameObject lastInitTrack;

    private float timeTrampline = 0;

    private bool startGame = false;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        InstanceLevel();
        for (factCountTrackAtScene = 0; factCountTrackAtScene < minCountTrackAtScene; factCountTrackAtScene++)
        {
            Transform tr_NewChunc = GetFree(allTrackAtScene);
            tr_NewChunc.gameObject.SetActive(true);

            if (lastInitTrack == null)
            {
                tr_NewChunc.GetComponent<RespawnTrackController>().Respawn(null);
            }
            else
            {
                tr_NewChunc.GetComponent<RespawnTrackController>().Respawn(lastInitTrack.transform);
            }
            lastInitTrack = tr_NewChunc.gameObject;
        }
        startGame = false;
    }

    public void StartGame()
    {
        startGame = true;
    }

    public void PauseGame()
    {
        startGame = false;
    }

    public void ResumedGame()
    {
        startGame = true;
    }

    public void ResetLevel()
    {
        StopCoroutine(FinishTimer());
        startGame = false;
        for (int i = 0; i < allObstacleAtScene.Count; i++)
            allObstacleAtScene[i].gameObject.SetActive(false);
        for (int i = 0; i < allTramplineAtScene.Count; i++)
            allTramplineAtScene[i].gameObject.SetActive(false);
        for (int i = 0; i < allCollectblsAtScene.Count; i++)
            allCollectblsAtScene[i].gameObject.SetActive(false);
        finish.gameObject.SetActive(false);

        for (int i = 0; i < allTrackAtScene.Count; i++)
        {
            if (allTrackAtScene[i].gameObject.activeInHierarchy)
            {
                allTrackAtScene[i].gameObject.SetActive(false);
                if (factCountTrackAtScene > 0) factCountTrackAtScene -= 1;
            } 
        }
        lastInitTrack = null;

        for (factCountTrackAtScene = 0; factCountTrackAtScene < minCountTrackAtScene; factCountTrackAtScene++)
        {
            Transform tr_NewChunc = GetFree(allTrackAtScene);
            tr_NewChunc.gameObject.SetActive(true);

            if (lastInitTrack == null)
            {
                tr_NewChunc.GetComponent<RespawnTrackController>().Respawn(null);
            }
            else
            {
                tr_NewChunc.GetComponent<RespawnTrackController>().Respawn(lastInitTrack.transform);
            }
            lastInitTrack = tr_NewChunc.gameObject;
        }
    }

    private void InstanceLevel()
    {
        //create track
        for (int i = 0; i < trackBloks.Length; i++)
        {
           for (int j = 0; j < 6; j++)
           {
                GameObject _go = Instantiate(trackBloks[i], new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0));
                allTrackAtScene.Add(_go.transform);

                _go.SetActive(false);
            }
        }

        //create interactive obj
        for (int i = 0; i < 6; i++)
        {
            Transform _go = Instantiate(trampline, new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0)).transform;
            allTramplineAtScene.Add(_go);
            _go.gameObject.SetActive(false);

            _go = Instantiate(obstacle, new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0)).transform;
            allObstacleAtScene.Add(_go);
            _go.gameObject.SetActive(false);
        }

        //create collectbles
        for (int i = 0; i < 10; i++)
        {
            Transform _go = Instantiate(collectbles, new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0)).transform;
            allCollectblsAtScene.Add(_go);
            _go.gameObject.SetActive(false);
        }

        finish = Instantiate(finishObj, new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0)).transform;
        finish.gameObject.SetActive(false);
    }

    private void CreateInteractiveObj()
    {
        if (!ButtonManager.Instance.drag) return;

        if (!finishCreate && timeTrampline > trp_timeSpawn)
        {
            int rubik = Random.Range(1, 100);//кидаю кубик
            if (rubik < 20)
            {
                Transform go = GetFree(allTramplineAtScene);
                go.gameObject.SetActive(true);
                go.transform.position = new Vector3(Random.Range(leftBoard + go.transform.localScale.x, RightBoard - go.transform.localScale.x), go.transform.position.y, player.transform.position.z + returnDistance * 3f);
                timeTrampline = 0;
            }
            else
            {
                Transform go = GetFree(allObstacleAtScene);
                go.gameObject.SetActive(true);
                go.transform.position = new Vector3(Random.Range(leftBoard + go.transform.localScale.x, RightBoard - go.transform.localScale.x), go.transform.position.y, player.transform.position.z + returnDistance * 3f);
                timeTrampline = 0;
            }
        }
        else
        {
            timeTrampline += 1 * Time.deltaTime;

            if (!finishCreate && Mathf.CeilToInt(timeTrampline) % 2 == 0)
            {
                Transform go = GetFree(allCollectblsAtScene);
                go.gameObject.SetActive(true);
                go.transform.position = new Vector3(Random.Range(leftBoard + go.transform.localScale.x, RightBoard - go.transform.localScale.x),
                    go.transform.position.y, player.transform.position.z + returnDistance * 3f);
            }
        }
    }

    public void CreateFinish()
    {
        StartCoroutine(FinishTimer());
    }

    private IEnumerator FinishTimer()
    {
        yield return new WaitForSeconds(gameTimer);

        finish.gameObject.SetActive(true);
        finish.transform.position = new Vector3(finish.transform.position.x,
            finish.transform.position.y, player.transform.position.z + returnDistance * 3f);
        finishCreate = true;
    }

    private void CreateTrack()
    {
        for (int i = 0; i < allTrackAtScene.Count; i++)
        {
            if (allTrackAtScene[i].gameObject.activeInHierarchy)
            {
                if (allTrackAtScene[i].transform.position.z < player.transform.position.z)
                {
                    float distance = player.transform.position.z - allTrackAtScene[i].transform.position.z;
                    Transform newGO = allTrackAtScene[i].transform.GetChild(0);
                    Transform origin = newGO.transform.GetChild(0);
                    Transform end = newGO.transform.GetChild(1);

                    float sizeZ = (end.position.z - origin.position.z);

                    if (distance >= sizeZ)
                    {
                        allTrackAtScene[i].gameObject.SetActive(false);
                        factCountTrackAtScene -= 1;
                    }
                }
            }
        }

        if (factCountTrackAtScene < minCountTrackAtScene)
        {
            Transform tr_NewChunc = GetFree(allTrackAtScene);
            tr_NewChunc.gameObject.SetActive(true);

            if (lastInitTrack == null)
            {
                tr_NewChunc.GetComponent<RespawnTrackController>().Respawn(null);
            }
            else
            {
                tr_NewChunc.GetComponent<RespawnTrackController>().Respawn(lastInitTrack.transform);
            }
            lastInitTrack = tr_NewChunc.gameObject; // revert lastInitTrack to Transform
            factCountTrackAtScene += 1;
        }
    }




    private Transform GetFree(List<Transform> pool)
    {
        List<Transform> newPool = new List<Transform>();
        for (int i = 0; i < pool.Count; i++)
        {
            if (!pool[i].gameObject.activeInHierarchy)
                newPool.Add(pool[i]);
        }

        Transform _go = newPool[Random.Range(0, newPool.Count)];

        if (_go != null)
            return _go;
        else
        {
            Debug.LogError("неверно указанны данные ввода GetFree или нет свободных объектов");
            return _go = null;
        }
    }

    private void FixedUpdate()
    {
        if (!startGame) return;

        if (player.transform.position.z > returnDistance)
            TransformAllSceneObj();

        CreateInteractiveObj();
        CreateTrack();
    }

    public void TransformAllSceneObj()
    {
        player.transform.position = new Vector3(player.transform.position.x, player.transform.position.y, player.transform.position.z - returnDistance);

        for (int i = 0; i < allTrackAtScene.Count; i++)
        {
            if (allTrackAtScene[i].gameObject.activeInHierarchy) 
                allTrackAtScene[i].position = new Vector3(allTrackAtScene[i].position.x, allTrackAtScene[i].position.y, allTrackAtScene[i].position.z - returnDistance);
        }

        for (int i = 0; i < allTramplineAtScene.Count; i++)
        {
            if (allTramplineAtScene[i].gameObject.activeInHierarchy)
                allTramplineAtScene[i].position = new Vector3(allTramplineAtScene[i].position.x, allTramplineAtScene[i].position.y, allTramplineAtScene[i].position.z - returnDistance);
        }

        for (int i = 0; i < allObstacleAtScene.Count; i++)
        {
            if (allObstacleAtScene[i].gameObject.activeInHierarchy)
                allObstacleAtScene[i].position = new Vector3(allObstacleAtScene[i].position.x, allObstacleAtScene[i].position.y, allObstacleAtScene[i].position.z - returnDistance);
        }

        for (int i = 0; i < allCollectblsAtScene.Count; i++)
        {
            if (allCollectblsAtScene[i].gameObject.activeInHierarchy)
                allCollectblsAtScene[i].position = new Vector3(allCollectblsAtScene[i].position.x, allCollectblsAtScene[i].position.y, allCollectblsAtScene[i].position.z - returnDistance);
        }

        finish.position = new Vector3(finish.position.x, finish.position.y, finish.position.z - returnDistance);
    }
}
