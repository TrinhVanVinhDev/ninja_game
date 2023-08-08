using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Input;
using UnityEngine;

public class PlayerController : Character
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private LayerMask groundedLayer;
    [SerializeField] private float speed = 200;
    [SerializeField] private float jumpForce = 350;
    [SerializeField] private Kunai kunaiPrefab;
    [SerializeField] private Transform kunaiPoint;
    [SerializeField] private GameObject attackArena;

    private bool isGrounded = true;
    private bool isJumping = false;
    private bool isActtack = false;
    //private bool isDeath = false;

    private float horizontal;
    private float horizontalKey;

    private int coinTotal = 0;

    private Vector3 savePoint;

    private void Awake()
    {
        coinTotal = PlayerPrefs.GetInt("coin");
    }

    // Update is called once per frame
    void Update()
    {
        if(isDeath)
        {
            return;
        }

        isGrounded = CheckGrounded();

        horizontalKey = Input.GetAxisRaw("Horizontal");

        if (isActtack)
        {
            rb.velocity = Vector2.zero;
            return;
        }
        if(isGrounded)
        {
            if (isJumping)
            {
                return;
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                Jump();
            }

            if (Input.GetKeyDown(KeyCode.Z))
            {
                Attack();
            }

            if (Input.GetKeyDown(KeyCode.C))
            {
                Throw();
            }

            if (Mathf.Abs(horizontalKey) > 0.1f)
            {
                Run(horizontalKey);
            }
            else if (Mathf.Abs(horizontal) > 0.1f)
            {
                Run(horizontal);
            }
            else
            {
                ChangeAnim("is_idle");
                rb.velocity = Vector2.zero;
            }

        }

        if (!isGrounded && rb.velocity.y < 0)
        {
            ChangeAnim("is_jumpOut");
            isJumping = false;
        }

        if (Mathf.Abs(horizontalKey) > 0.1f)
        {
            Run(horizontalKey);
        } else if (Mathf.Abs(horizontal) > 0.1f)
        {
            Run(horizontal);
        }
    }

    public override void OnInit()
    {
        base.OnInit();
        UIManager.instance.SetPoint(0);
        ChangeAnim("is_idle");
        isActtack = false;
        DeActiveAttackArena();
        transform.position = savePoint;
        SavePoint();
    }

    public override void OnDespawn()
    {
        base.OnDespawn();
        OnInit();
    }

    public void SetMove(float horizontal)
    {
        this.horizontal = horizontal;
    }

    public void Attack()
    {
        if (CheckGrounded())
        {
            ChangeAnim("is_attack");
            isActtack = true;
            Invoke(nameof(ResetAttack), 0.5f);
            ActiveAttackArena();
            Invoke(nameof(DeActiveAttackArena), 0.5f);
        }
    }

    public void Throw()
    {
        if (CheckGrounded())
        {
            ChangeAnim("is_throw");
            isActtack = true;
            Invoke(nameof(ResetAttack), 0.5f);

            Instantiate(kunaiPrefab, kunaiPoint.position, kunaiPoint.rotation);
        }
    }

    public void Jump()
    {
        if (CheckGrounded())
        {
            isJumping = true;
            ChangeAnim("is_jumpin");
            rb.AddForce(jumpForce * Vector2.up);
        }
    }

    protected override void OnDeath()
    {
        base.OnDeath();
    }

    private bool CheckGrounded()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 1.1f, groundedLayer);

        return hit.collider != null;
    }

    private void Run(float horizontalMoving)
    {
        ChangeAnim("is_run");
        rb.velocity = new Vector2(horizontalMoving * Time.fixedDeltaTime * speed, rb.velocity.y);
        transform.rotation = Quaternion.Euler(new Vector3(0, horizontalMoving > 0 ? 0 : 180, 0));
    }

    private void ResetAttack()
    {
        isActtack = false;
        ChangeAnim("is_idle");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Coin")
        {
            coinTotal++;
            PlayerPrefs.SetInt("coin", coinTotal);
            UIManager.instance.SetPoint(coinTotal);
            Destroy(collision.gameObject);
        }

        if(collision.tag == "DeathZone")
        {
            //isDeath = true;
            base.hp = 0;
            ChangeAnim("is_die");
            Invoke(nameof(OnInit), 1f);
        }
    }

    private void ActiveAttackArena()
    {
        attackArena.SetActive(true);
    }

    private void DeActiveAttackArena()
    {
        attackArena.SetActive(false);
    }

    internal void SavePoint()
    {
        savePoint = transform.position;
    }
}
