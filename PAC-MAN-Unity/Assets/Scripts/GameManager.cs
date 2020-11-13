using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance; //这个叫做 单例

    public static GameManager Instance
    {
        get { return _instance; }
    }

    public GameObject pacman;
    public GameObject blinky;
    public GameObject clyde;
    public GameObject inky;
    public GameObject pinky;

    public bool isSuperPacman = false; //默认主角不是超级状态

    public List<int> usingIndex = new List<int>(); //代表已经挑选完的那一条路径
    private List<int> rawIndex = new List<int> {0, 1, 2, 3}; //代表的是 4条预制体路径
    private List<GameObject> pacdotsGos = new List<GameObject>(); //声明一个集合，存放所有的豆子

    public GameObject startCountDownPrefab; //开始动画
    public GameObject gameOverPrefab; //游戏结束动画
    public GameObject winPrefab; //赢了的动画
    public GameObject startPanel; //开始游戏的界面
    public GameObject gamePanel; //游戏中的分数记录界面
    public AudioClip startClip; //321倒计时时候播放的音乐

    public Text remainText; //剩余的豆子数量
    public Text nowText; //已经吃的豆子数量字体
    public Text scoreText; //分数的字体
    private int pacdotNumber = 0; //场景中所有豆子的数量
    private int nowEat = 0; //已经吃掉的豆子的数量
    public int score = 0; //分数

    private void Awake()
    {
        _instance = this;
        Screen.SetResolution(1024,768,false);
        int rawIndexCount = rawIndex.Count; //如果不拿出来单独的声明，后面的数组数量会有动态变化的错误
        for (int i = 0; i < rawIndexCount; i++)
        {
            int tempIndex = Random.Range(0, rawIndex.Count); //声明一个变量，拿到随机到的路径的编号
            usingIndex.Add(rawIndex[tempIndex]); //把选中的路径，放进 usingIndex 集合中
            Debug.Log("增加后的UsingIndex个数" + usingIndex.Count);
            rawIndex.RemoveAt(tempIndex); //把已经挑选的路径从数组中拿出去，防止重复
        }

        foreach (Transform t in GameObject.Find("Obstacle").transform)
        {
            //用一个循环来获取，每一个豆子，豆子的整体
            pacdotsGos.Add(t.gameObject);
        }

        pacdotNumber = GameObject.Find("Obstacle").transform.childCount; //豆子的数量获取
    }

    private void Start()
    {
        SetGameState(false);
    }

    public void OnStartButton() //点击了开始游戏按钮以后，启动协程，播放声音，隐藏开始界面
    {
        StartCoroutine(PlayerStartCountDown());
        AudioSource.PlayClipAtPoint(startClip, new Vector3(0, 0, -5)); //声音越靠近摄像机的位置，越大
        startPanel.SetActive(false); //游戏开始以后隐藏游戏开始界面
    }

    IEnumerator PlayerStartCountDown()
    {
        //协程，生成倒计时动画，等待 4秒删除动画，游戏状态为动起来，延迟生成豆子，分数界面显示，播放音乐
        GameObject go = Instantiate(startCountDownPrefab);
        yield return new WaitForSeconds(3.5f);
        Destroy(go);
        SetGameState(true);
        Invoke("CreateSuperPacdot", 10f); //游戏开始后 10秒，开始生成超级豆子
        gamePanel.SetActive(true);
        GetComponent<AudioSource>().Play();
    }

    public void OnExitButton()
    {
        Application.Quit(); //这个代表软件退出
    }

    public void OnEatPacdot(GameObject go) //如果吃的是普通豆子，直接从数组中移除被吃掉的那个就行了
    {
        nowEat++;
        score += 100;
        pacdotsGos.Remove(go);
    }

    public void OnEatSuperPacdot() //如果是超级豆子，那么
    {
        score += 200;
        Invoke("CreateSuperPacdot", 10f);
        isSuperPacman = true;
        FreezeEnemies();
        StartCoroutine(RecoverEnemies());
    }

    IEnumerator RecoverEnemies() //把敌人恢复，并且接触主角的超级状态
    {
        yield return new WaitForSeconds(3f); //运行到这里，会等 3秒，再执行让主角从无敌状态恢复
        UnFreezeEnemies();
        isSuperPacman = false;
    }

    private void CreateSuperPacdot() //随机从豆子数组中，生成一个超级豆子
    {
        if (pacdotsGos.Count < 80)
        {
            return;
        }

        int tempIndex = Random.Range(0, pacdotsGos.Count); //生成一个随机的 索引数
        pacdotsGos[tempIndex].transform.localScale = new Vector3(3, 3, 3); //将其放大
        pacdotsGos[tempIndex].GetComponent<Pacdots>().isSuperPacdot = true; // 豆子是否是大豆子，改为真
    }

    private void FreezeEnemies() //冻结所有的敌人，并且透明度降低
    {
        blinky.GetComponent<GhostMove>().enabled = false;
        clyde.GetComponent<GhostMove>().enabled = false;
        inky.GetComponent<GhostMove>().enabled = false;
        pinky.GetComponent<GhostMove>().enabled = false;

        blinky.GetComponent<SpriteRenderer>().color = new Color(0.7f, 0.7f, 0.7f);
        clyde.GetComponent<SpriteRenderer>().color = new Color(0.7f, 0.7f, 0.7f);
        inky.GetComponent<SpriteRenderer>().color = new Color(0.7f, 0.7f, 0.7f);
        pinky.GetComponent<SpriteRenderer>().color = new Color(0.7f, 0.7f, 0.7f);
    }

    private void UnFreezeEnemies() //解冻所有敌人，恢复原样
    {
        blinky.GetComponent<GhostMove>().enabled = true;
        clyde.GetComponent<GhostMove>().enabled = true;
        inky.GetComponent<GhostMove>().enabled = true;
        pinky.GetComponent<GhostMove>().enabled = true;

        blinky.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f);
        clyde.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f);
        inky.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f);
        pinky.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f);
    }

    private void SetGameState(bool state) //游戏的状态
    {
        //所有的幽灵，和主角，全部按照游戏的状态，判定为是开始还是停止状态
        pacman.GetComponent<PacManMove>().enabled = state;
        blinky.GetComponent<GhostMove>().enabled = state;
        clyde.GetComponent<GhostMove>().enabled = state;
        inky.GetComponent<GhostMove>().enabled = state;
        pinky.GetComponent<GhostMove>().enabled = state;
    }

    private void Update()
    {
        if (nowEat == pacdotNumber && pacman.GetComponent<PacManMove>().enabled != false)
        {
            // 条件卡的比较死，为的是不浪费性能
            gamePanel.SetActive(false);
            Instantiate(winPrefab);
            StopAllCoroutines(); //游戏胜利以后，停止所有的协程
            SetGameState(false);
        }

        if (nowEat == pacdotNumber)
        {
            if (Input.anyKeyDown)
            {
                SceneManager.LoadScene(0);
            }
        }

        if (gamePanel.activeInHierarchy) //如果 UI界面在 Hierarchy 窗口显示了
        {
            remainText.text = "Remain:\n" + (pacdotNumber - nowEat);
            nowText.text = "Eaten:\n" + nowEat;
            scoreText.text = "Score:\n" + score;
        }
    }
}