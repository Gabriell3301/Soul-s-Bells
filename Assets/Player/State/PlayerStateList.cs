using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateList : MonoBehaviour
{

    public bool walking;
    public bool jumping;
    public bool recentJump;
    public bool attacking;
    public bool parring;
    public bool Grounded;
    public bool isDashing;
    
    public void SetRecentJump(bool isRecentJumping)
    {
        recentJump = isRecentJumping;
    }
    public void SetDashing(bool isDash)
    {
        isDashing = isDash;
    }
    public void SetWalking(bool isWalking)
    {
        walking = isWalking;
    }
    public void SetGrounded(bool isGrounded)
    {
        Grounded = isGrounded;
    }

    public void SetAttacking(bool isAttacking)
    {
        attacking = isAttacking;
    }

    public void SetParring(bool isParring)
    {
        parring = isParring;
    }
    public void SetJumping(bool isJumping)
    {
        jumping = isJumping;
    }

}