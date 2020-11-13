using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pacdots : MonoBehaviour
{
    public bool isSuperPacdot = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.name == "Pacman") //有可能是小鬼碰到的我，所以要限定碰到我的是不是主角
        {
            if (isSuperPacdot)
            {
                GameManager.Instance.OnEatPacdot(gameObject);
                GameManager.Instance.OnEatSuperPacdot();
                Destroy(gameObject);
            }
            else
            {
                GameManager.Instance.OnEatPacdot(gameObject);
                //如果发生碰撞的是，Pacman，那么就是说主角吃掉了我
                Destroy(gameObject); //我被吃掉了，删除我自己
            }
        }
    }
}