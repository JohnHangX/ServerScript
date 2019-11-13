using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameQuit : MonoBehaviour {
    public Button QuitBtn;
    void Start()
    {
        QuitBtn.onClick.AddListener(OnClickQuit);
    }
	public void OnClickQuit()
    {
        print("quit");
        Application.Quit();
    }
}
