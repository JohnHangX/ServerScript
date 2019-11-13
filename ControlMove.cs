using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ControlMove : NetworkBehaviour
{

    void Update()
    {
        if (!isLocalPlayer)   //判断是否是本地客户端
        {
            return;
        }
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        if (x != 0 || y != 0)
        {
            transform.position += new Vector3(x, 0, y);
        }
    }
}
