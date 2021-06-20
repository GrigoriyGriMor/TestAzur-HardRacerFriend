using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturningToPool : MonoBehaviour
{
    private float deactivateDistance = 50;
    private Transform player;

    private void Awake()
    {
        if (GameLevelController.Instance.player != null) player = GameLevelController.Instance.player.transform;
    }

    private void FixedUpdate()
    {
        if (gameObject.transform.position.z < player.position.z)
        {
            float distance = player.position.z - gameObject.transform.position.z;
            if (distance > deactivateDistance)
                gameObject.SetActive(false);
        }
    }
}
