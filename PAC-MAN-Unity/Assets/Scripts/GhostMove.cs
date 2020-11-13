using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.SceneManagement;

public class GhostMove : MonoBehaviour
{
    //public Transform[] wayPoints; //路线数组，用来存储所有的路线点位置
    public GameObject[] wayPointGoWay; //创建一个游戏物体变量数组，存放敌人可选择的几条路线
    private List<Vector3> wayPoints = new List<Vector3>(); //创建一个 List 集合存放路线预制体
    private Vector3 startPosition; //这是每一个幽灵初始的位置，就是在地图上摆好的那个位置

    public float speed; //幽灵的移动速度

    private int index = 0; //幽灵当前在哪一个路径点


    private void Start()
    {
        startPosition = transform.position + new Vector3(0, 3, 0); //每一个鬼，出来都是走 3个单位，出老家
        LoadAPath();
    }

    private void GetGhostDirection() //获得幽灵的实时方向，并且播放动画
    {
        Vector2 direction = wayPoints[index] - transform.position;
        GetComponent<Animator>().SetFloat("DirectionX", direction.x);
        GetComponent<Animator>().SetFloat("DirectionY", direction.y);
    }

    private void LoadAPath()
    {
        wayPoints.Clear(); //每次加载新路径之前先清空一下
        //将敌人要走的路线，用随机函数拿出来一条，然后用循环，把每一个点位置都用 Transform 拿出来


        int usingIndex = GetComponent<SpriteRenderer>().sortingOrder - 2;
        Debug.Log(usingIndex);
        int wayIndex = GameManager.Instance.usingIndex[usingIndex];
        Debug.Log(wayIndex);
        foreach (Transform t in (wayPointGoWay[wayIndex]).transform)
        {
            wayPoints.Add(t.position); //并且把那些点的位置，存放到 List 集合当中
        }

        //一下两句代码，是为了适配不同的幽灵的，起点和终点
        wayPoints.Insert(0, startPosition); //这句使用代码，加了路径起始点的，移动点坐标，Insert( 第一个也就是 0，加什么)
        wayPoints.Add(startPosition); //这句话加了，路径终点的坐标，Add 默认加在数组的最后一位
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.name == "Pacman")
        {
            if (GameManager.Instance.isSuperPacman)
            {
                //幽灵碰见了超级吃豆人，被遣返老家
                transform.position = startPosition - new Vector3(0, 3, 0);
                index = 0;
                GameManager.Instance.score += 500; //吃到幽灵 奖励 500分
            }
            else
            {
                //Destroy(other.gameObject); //删除掉碰到我的那个物体，也就是限定条件的 主角
                other.gameObject.SetActive(false);
                GameManager.Instance.gamePanel.SetActive(false);
                Instantiate(GameManager.Instance.gameOverPrefab);
                Invoke("ReStart", 3f);
            }
        }
    }

    private void FixedUpdate()
    {
        if (transform.position != wayPoints[index])
        {
            Vector2 temp =
                Vector2.MoveTowards(transform.position, wayPoints[index], speed);
            GetComponent<Rigidbody2D>().MovePosition(temp);
        }
        else
        {
            // //index++; 这么写不高级，看下面的写法
            // index = (index + 1) % wayPoints.Count;
            // //使用 取余/取模运算符，达成一个不断循环的状态
            index++;
            if (index >= wayPoints.Count)
            {
                index = 0;
                LoadAPath();
            }
        }

        GetGhostDirection();
    }

    private void ReStart() //重置场景
    {
        SceneManager.LoadScene(0);
    }
}