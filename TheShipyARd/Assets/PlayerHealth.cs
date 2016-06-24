﻿using UnityEngine;
using System.Collections;

public class PlayerHealth : MonoBehaviour {

	public float health = 100f;

	private bool playerDead;

	void playerDying()
	{
		playerDead = true;
	}

	void playerDead()
	{
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (health <= 0f) 
		{
			// Player currently dying
			if (!playerDead) {
				playerDying();
			} 
			// Player is already dead
			else 
			{
				playerDead();
			}
		}
	
	}

	public void takeDamage(float amount)
	{
		health -= amount;
	}
}
