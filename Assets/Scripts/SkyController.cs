using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyController : MonoBehaviour
{
    private Transform playerTransform;
    private Vector3 playerPos;

    // Start is called before the first frame update
    void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        playerPos = playerTransform.position;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        playerPos = playerTransform.position;
        transform.position = new Vector3(transform.position.x, transform.position.y, playerPos.z);
    }
}
