using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[DisallowMultipleComponent]

public class moveObject : MonoBehaviour
{
    [SerializeField] Vector3 movePos;
    [SerializeField] [Range(0,1)] float moveProgress;
    [SerializeField] float moveSpeed;
    Vector3 startPos;


    void Start()
    {
        startPos = transform.position;  
    }

   
    void Update()
    {
        moveProgress = Mathf.PingPong(Time.time*moveSpeed, 1);
        Vector3 offset = movePos * moveProgress;
        transform.position = startPos + offset;
    }
}
