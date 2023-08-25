using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackArena : MonoBehaviour
{
    [SerializeField] private PlayerController playerControl;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            if (playerControl.isEternal)
            {
                collision.GetComponent<Character>().OnHit(0f);
            } else
            {
                collision.GetComponent<Character>().OnHit(30f);
            }
        }

        if(collision.CompareTag("Enemy"))
        {
            collision.GetComponent<Character>().OnHit(30f);
        }
    }
}
