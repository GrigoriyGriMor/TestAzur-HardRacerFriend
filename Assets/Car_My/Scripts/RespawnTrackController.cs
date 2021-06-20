using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnTrackController : MonoBehaviour
{
    public void Respawn(Transform pos)
    {
        if (pos == null)
        {
            gameObject.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, 0);
        }
        else
        {
            Transform prevTrackEnd = pos.GetChild(0).GetChild(1);

            Transform newTrackOrigin = gameObject.transform.transform.GetChild(0);
            Transform newTrackStart = newTrackOrigin.GetChild(0);
            newTrackStart.SetParent(gameObject.transform.parent);
            gameObject.transform.SetParent(newTrackStart);

            newTrackStart.position = prevTrackEnd.position;
            gameObject.transform.SetParent(newTrackStart.parent);
            newTrackStart.SetParent(newTrackOrigin);
            newTrackStart.SetAsFirstSibling();
        }
    }
}
