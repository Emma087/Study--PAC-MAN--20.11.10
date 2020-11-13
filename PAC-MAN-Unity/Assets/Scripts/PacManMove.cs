using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PacManMove : MonoBehaviour
{
    public float speed; //.MoveTowards方法中最后一个参数，代表不能超过的最大值
    private Vector2 destination = Vector2.zero; //目的地，主角下一个要到达的地方

    private void Start()
    {
        destination = transform.position;
        //一上来，给了主角一个自己的位置，也就是限定了在引擎内摆好的初始位置
    }

    private void SetKeyboardMove()
    {
        if ((Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W)) && Valid(Vector2.up))
        {
            Debug.Log("上");
            destination = (Vector2) transform.position + Vector2.up;
        }

        if ((Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S)) && Valid(Vector2.down))
        {
            destination = (Vector2) transform.position + Vector2.down;
            Debug.Log("下");
        }

        if ((Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A)) && Valid(Vector2.left))
        {
            destination = (Vector2) transform.position + Vector2.left;
            Debug.Log("左");
        }

        if ((Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D)) && Valid(Vector2.right))
        {
            destination = (Vector2) transform.position + Vector2.right;
            Debug.Log("右");
        }
    }

    private void GetPlayerDirection() //获得主角的实时方向，并且播放动画
    {
        Vector2 direction = destination - (Vector2) transform.position;
        GetComponent<Animator>().SetFloat("DirectionX", direction.x);
        GetComponent<Animator>().SetFloat("DirectionY", direction.y);
    }

    private bool Valid(Vector2 direction) //射线函数检测，目的地是否为边缘，
    {
        //这个函数的参数，代表主角，即将去的目的地
        Vector2 currentPosition = transform.position; //先设定一个局部变量，存储主角当前位置
        RaycastHit2D hit = Physics2D.Linecast(currentPosition + direction, currentPosition);
        //声明一个 hit 射线变量，参数为，（射线的出发点，射线的终点）
        return (hit.collider == GetComponent<Collider2D>());
        //返回，射线接触到的 collider 是否是主角的 collider，是的话返回 true 
        //这里如果是出界了，那么射线接触到的 collider 会返回一个 false
    }

    private void FixedUpdate()
    {
        Vector2 temp = Vector2.MoveTowards(transform.position, destination, speed);
        GetComponent<Rigidbody2D>().MovePosition(temp);
        if ((Vector2) transform.position == destination)
        {
            //这句 if 限制的是，这一次的目的地已经达到以后，才可以变动下一次的目的地
            SetKeyboardMove();
            GetPlayerDirection();
        }
    }
}