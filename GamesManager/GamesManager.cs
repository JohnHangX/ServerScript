using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using UnityEngine.UI;
using System.IO;
using System.Runtime.InteropServices;
using System;

public class GamesManager : MonoBehaviour {

    string fileDir;
    public Text showText;
    public delegate void ListenerHandler();
    public event ListenerHandler Listener;
    private Process curGame;
    private EventHandler exitGameHandler;
    DirectoryInfo[] gamesDics;
    List<string> gamesList = new List<string>();
    int curGameIndex = 0;
    void Start () {
        Application.runInBackground = true;
        exitGameHandler = new EventHandler(NextProgress);
        //string tempPath = @"E:\release\GamesOperateDemo_Data";
        //DirectoryInfo di = new DirectoryInfo(tempPath);
        DirectoryInfo di = new DirectoryInfo(Application.dataPath);
        
        fileDir = di.Parent.FullName + "/Games/";
        DirectoryInfo diParent = new DirectoryInfo(fileDir);
        gamesDics = diParent.GetDirectories();
        foreach (DirectoryInfo gameDir in gamesDics)
        {
            gamesList.Add(gameDir.Name);
            print(gameDir.Name);
        }
        //showText.text = fileDir;
        curGameIndex = 0;
        ShowWindow(GetForegroundWindow(), SW_SHOWMINIMIZED);
        SelectGame();
    }
	
	// Update is called once per frame
	void Update () {
		
	}
    //operType参数如下：
    //0: 隐藏, 并且任务栏也没有最小化图标  
    //1: 用最近的大小和位置显示, 激活  
    //2: 最小化, 激活  
    //3: 最大化, 激活  
    //4: 用最近的大小和位置显示, 不激活  
    //5: 同 1  
    //6: 最小化, 不激活  
    //7: 同 3  
    //8: 同 3  
    //9: 同 1  
    //10: 同 1 
    [DllImport("kernel32.dll")]
    public static extern int WinExec(string exeName, int operType);

    [DllImport("user32.dll")]
    public static extern void SwitchToThisWindow(IntPtr hWnd, bool fAltTab);
    const int SW_SHOWMINIMIZED = 2; //{最小化, 激活}  
    const int SW_SHOWMAXIMIZED = 3;//最大化  
    const int SW_SHOWRESTORE = 1;//还原  
    [DllImport("user32.dll")]
    public static extern bool ShowWindow(IntPtr hwnd, int nCmdShow);

    [DllImport("user32.dll")]
    static extern IntPtr GetForegroundWindow();

    void SelectGame()
    {
        string gameName = gamesList[curGameIndex];
        string exeName = fileDir + gameName + "/" + gameName + ".exe";
        StartProgress(exeName, gameName);
    }

    void StartProgress(string path, string gameName)
    {
        //ProcessStartInfo info = new ProcessStartInfo();
        //info.FileName = path;
        //info.Arguments = "";
        //info.WindowStyle = ProcessWindowStyle.Minimized;
        //print(path);
        //Process pro = Process.Start(info);
        //SwitchToThisWindow(pro.MainWindowHandle, true);
        //pro.WaitForExit();

        WinExec(path, 4);
        AddExitProcessFunc(gameName);


    }

    void AddExitProcessFunc(string gameName)
    {
        Process[] temp = Process.GetProcessesByName(gameName);
        if (temp.Length > 0)//如果查找到
        {
            //IntPtr handle = temp[0].MainWindowHandle;
            //SwitchToThisWindow(handle, true); // 激活，显示在最前
            curGame = temp[0];
            
            curGame.EnableRaisingEvents = true;
            curGame.Exited += exitGameHandler;
            if (curGame.HasExited == true) NextProgress();
            
            print("curGame: "+ curGame.ProcessName);
            curGame.WaitForExit();
            NextProgress();
        }
        else
        {
            Process.Start(gameName + ".exe");//否则启动进程
            AddExitProcessFunc(gameName);
        }
    }

    void NextProgress(object obj, EventArgs e)
    {
        print("next progress");
        curGame.Exited -= exitGameHandler;
        int gamesLen = gamesList.Count;
        curGameIndex += 1;
        if (curGameIndex >= gamesLen)
        {
            curGameIndex = 0;
            return;
        }
        SelectGame();
    }
    void NextProgress()
    {
        print("next progress");
        curGame.Exited -= exitGameHandler;
        int gamesLen = gamesList.Count;
        curGameIndex += 1;
        if (curGameIndex >= gamesLen)
        {
            curGameIndex = 0;
            return;
        }
        SelectGame();
    }

    void OnApplicationQuit()
    {
        print("OnApplicationQuit");
        if (curGame != null)
            curGame.Exited -= exitGameHandler;
    }
}
