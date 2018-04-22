﻿using System.Collections;
using System.Collections.Generic;
using Microsoft.Win32;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;


public class Enemy : MonoBehaviour, IDamageable
{
    [SerializeField] private float maxHealthPoints = 100f;
    [SerializeField] private float attackRadius = 4f;
    [SerializeField] float chaseRadius = 6f;
    [SerializeField] float damagePerShot = 9;
    [SerializeField] float secondsBetweenShots = 0.5f;
  

    [SerializeField] GameObject projectileToUse;

    [SerializeField] GameObject projectileSocket;

    private float currentHealthPoints = 100f;

    AICharacterControl aiCharacterControl = null;

    private GameObject player = null;

    bool isAttacking = false;

    public float healthAsPercentage
    {
        get { return currentHealthPoints /  maxHealthPoints;  }
    }

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        aiCharacterControl = GetComponent<AICharacterControl>();
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(player.transform.position, transform.position);

        if (distanceToPlayer <= attackRadius && !isAttacking)
        {
            isAttacking = true;
            InvokeRepeating("SpawnProjectile", 0f, secondsBetweenShots); // TODO switch to coroutines
        }
        
        if(distanceToPlayer > attackRadius)
        {
            isAttacking = false;
            CancelInvoke();
        }

         if (distanceToPlayer <= chaseRadius)
        {
            aiCharacterControl.SetTarget(player.transform);
        }
        else
        {
            aiCharacterControl.SetTarget(transform);
        }
    }

    void SpawnProjectile()
    {
        GameObject newProjectile = Instantiate(projectileToUse, projectileSocket.transform.position, Quaternion.identity);
        var projectileComponent = newProjectile.GetComponent<Projectile>();
        projectileComponent.SetDamage(damagePerShot);
        Vector3 unitVectorToPlayer = (player.transform.position - projectileSocket.transform.position).normalized;
        
        newProjectile.GetComponent<Rigidbody>().velocity = unitVectorToPlayer * projectileComponent.projectileSpeed;
    }

    public void TakeDamage(float damage)
    {
        currentHealthPoints = Mathf.Clamp(currentHealthPoints - damage, 0f, maxHealthPoints);
    }

    void OnDrawGizmos()
    {
        // Draw attack spehere
        Gizmos.color = new Color(255f, 0f, 0, .5f);
        Gizmos.DrawWireSphere(transform.position, attackRadius);

        // Draw move spehere
        Gizmos.color = new Color(0f, 0f, 255f, .5f);
        Gizmos.DrawWireSphere(transform.position, chaseRadius);

        
    }

}
