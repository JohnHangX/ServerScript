using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MyJsonClass
{
    public Dictionary<string, bool> map = new Dictionary<string, bool>();
    public int level;
    public float timeElapsed;
    public string playerName;
    public List<string> list = new List<string>();
}

public class UIController : MonoBehaviour {
    public GameObject Canvas;
    SocketManager socketMgr;
    public post postObj;
    Dictionary<string, bool> map = new Dictionary<string, bool>();
    // Use this for initialization
    void Start () {
        socketMgr = SocketManager.Instance;
        string a = "a";
        MyJsonClass jsonObj = new MyJsonClass();
        jsonObj.map.Add("DisConnect", true);
        jsonObj.map.Add("aaaa", false);
        jsonObj.list.Add("list000000");
        jsonObj.list.Add("list11111111111");
        jsonObj.playerName = a;
        string jsonStr = JsonUtility.ToJson(jsonObj);
        print(jsonStr);
    }
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyUp(KeyCode.Space))
        {
            if(SocketManager.Instance.myCurrent == SocketManager.Character.Server)
            {

            }
            else if(SocketManager.Instance.myCurrent == SocketManager.Character.Client)
            {
                SocketManager.Instance.ClientSendMsg("On Click Space");
            }

        }
	}

    public void OnClickServer()
    {
        SocketManager.Instance.InitServer((o) => { print(o); });
        CloseUI();
        SocketManager.Instance.myCurrent = SocketManager.Character.Server;
    }

    public void OnClickClinet()
    {
        SocketManager.Instance.ClientConnectServer(SocketManager.Instance.ip, SocketManager.Instance.port, (o) => { print(o); });
        CloseUI();
        SocketManager.Instance.myCurrent = SocketManager.Character.Client;

        //postObj.Post(SocketManager.Instance.ip, "111");
    }

    void CloseUI()
    {
        Canvas.gameObject.SetActive(false);
    }

    public void OnApplicationQuit()
    {
        SocketManager.Instance.OnApplicationQuit();
    }
}
