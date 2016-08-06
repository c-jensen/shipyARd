using UnityEngine;
using System.Collections;

public class PlayerScript : MonoBehaviour {

    public float health = 100f;
    public int score = 0;

    private bool playerDead;

    void playerDying()
    {
        playerDead = true;
    }

    void playerIsDead()
    {

    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (health <= 0f)
        {
            // Player currently dying
            if (!playerDead)
            {
                playerDying();
            }
            // Player is already dead
            else
            {
                playerIsDead();
            }
        }

    }

    public void takeDamage(float amount)
    {
        health -= amount;
    }
}
