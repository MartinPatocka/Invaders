using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    //Config param
    [Header("Player")]
    [SerializeField] public float moveSpeed = 10f;
    [SerializeField] public int health = 300;

    [Header("Level1")]
    [SerializeField] public float moveLevel1 = 12f;
    [SerializeField] public int scoreLevel1 = 10000;
    [SerializeField] public float projectileSpeedLevel1 = 25f;
    [SerializeField] public GameObject laserPrefabLevel1;
    [SerializeField] public AudioClip shootingSoundLevel1;

    [Header("Level2")]
    [SerializeField] public float moveLevel2 = 15f;
    [SerializeField] public int scoreLevel2 = 30000;
    [SerializeField] public float projectileSpeedLevel2 = 30f;
    [SerializeField] public GameObject laserPrefabLevel2;
    [SerializeField] public AudioClip shootingSoundLevel2;

    [Header("Level3")]
    [SerializeField] public float moveLevel3 = 15f;
    [SerializeField] public int scoreLevel3 = 50000;
    [SerializeField] public float projectileSpeedLevel3 = 35f;
    [SerializeField] public GameObject laserPrefabLevel3;
    [SerializeField] public AudioClip shootingSoundLevel3;

    [Header("Projectile")]
    [SerializeField] public float projectileSpeed = 10f;
    [SerializeField] public GameObject laserPrefab;
    [SerializeField] public float projectileFiringPeriod = 0.1f;

    [Header("Visual Efects")]
    [SerializeField] public GameObject deathVFX;
    [SerializeField] public float durationOfExplosion = 1f;

    [Header("Audio")]
    [SerializeField] public AudioClip deathSound;
    [SerializeField] public AudioClip shootingSound;
    [SerializeField] [Range(0, 1)] public float deathSoundVolume = 0.75f;
    [SerializeField] [Range(0, 1)] public float shootingSoundVolume = 0.75f;

    //Reference
    Coroutine firingCoroutine;

    //States
    float xMin;
    float xMax;
    float yMin;
    float yMax;

    

    // Start is called before the first frame update
    void Start()
    {
        SetUpMoveBoundaries();
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        Fire();
        LevelUp();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        DamageDealer damageDealer = other.gameObject.GetComponent<DamageDealer>();
        if (!damageDealer) { return; }
        ProcessHit(damageDealer);
    }

    private void ProcessHit(DamageDealer damageDealer)
    {
        health -= damageDealer.GetDamage();
        damageDealer.Hit();
        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        FindObjectOfType<Level>().LoadGameOver();
        Destroy(gameObject);
        GameObject explosion = Instantiate(deathVFX, transform.position, transform.rotation);
        Destroy(explosion, durationOfExplosion);
        AudioSource.PlayClipAtPoint(deathSound, Camera.main.transform.position, deathSoundVolume);
    }

    private void Fire()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            firingCoroutine = StartCoroutine(FireContinously());
        }
        if (Input.GetButtonUp("Fire1"))
        {
            StopCoroutine(firingCoroutine);
        }

    }

    IEnumerator FireContinously()
    {
        while (true)
        { 
        GameObject laser = Instantiate(
           laserPrefab,
           transform.position,
           Quaternion.identity) as GameObject;
        laser.GetComponent<Rigidbody2D>().velocity = new Vector2(0, projectileSpeed);
        AudioSource.PlayClipAtPoint(shootingSound, Camera.main.transform.position, shootingSoundVolume);
        yield return new WaitForSeconds(projectileFiringPeriod);
        Input.GetButtonUp("Fire1");
        }
    }

    private void Move()
    {
        var deltaX = Input.GetAxis("Horizontal") * Time.deltaTime * moveSpeed;
        var deltaY = Input.GetAxis("Vertical") * Time.deltaTime * moveSpeed;

        var newXPos = Mathf.Clamp(transform.position.x + deltaX, xMin, xMax);
        var newYPos = Mathf.Clamp(transform.position.y + deltaY, yMin, yMax);
        transform.position = new Vector2(newXPos, newYPos);
    }

    private void SetUpMoveBoundaries()
    {
        Camera gameCamera = Camera.main;
        xMin = gameCamera.ViewportToWorldPoint(new Vector3(0.05f, 0, 0)).x;
        xMax = gameCamera.ViewportToWorldPoint(new Vector3(0.95f, 0, 0)).x;
        yMin = gameCamera.ViewportToWorldPoint(new Vector3(1, 0.035f, 0)).y;
        yMax = gameCamera.ViewportToWorldPoint(new Vector3(0, 0.975f, 0)).y;
    }

    public int GetHealth()
    {
        return health;
    }

    public bool LevelUp()
    {
        if(FindObjectOfType<GameSession>().GetScore() > scoreLevel1)
        {
            moveSpeed = moveLevel1;
            projectileSpeed = projectileSpeedLevel1;
            laserPrefab = laserPrefabLevel1;
            shootingSound = shootingSoundLevel1;
        }

        if (FindObjectOfType<GameSession>().GetScore() > scoreLevel2)
        {
            moveSpeed = moveLevel2;
            projectileSpeed = projectileSpeedLevel2;
            laserPrefab = laserPrefabLevel2;
            shootingSoundLevel1 = shootingSoundLevel2;
        }

        if (FindObjectOfType<GameSession>().GetScore() > scoreLevel3)
        {
            moveSpeed = moveLevel3;
            projectileSpeed = projectileSpeedLevel3;
            laserPrefab = laserPrefabLevel3;
            shootingSound = shootingSoundLevel3;
        }
        return true;
    }

    public void SetHealth()
    {
        if (LevelUp())
        {
            health = 400;
        }
    }

}
