using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class EnemyHandler : MonoBehaviour
{

    private Transform playerTransform;

    [SerializeField]
    private float reachTime;

    
    private Text scoreText;

    private float timeAlive;

    private float moveSpeed;
    private Vector3 moveDirection;

    private bool hasExploded;

    private int patternPosition;

    private Vector3 targetPosition;

    private Transform patternTransform;

    public void Start()
    {
        patternPosition = 0;

        Material spriteMaterial = GetComponent<SpriteRenderer>().material;


        ParticleSystemRenderer explosionSystemRenderer = gameObject.GetComponent<ParticleSystemRenderer>();
        explosionSystemRenderer.material = spriteMaterial;

        hasExploded = false;
        timeAlive = 0;
        moveSpeed = 0;
    }

    public void SetScoreText(Text scoreText)
    {
        this.scoreText = scoreText;
    }
    public void SetMoveTransforms(Transform playerTransform, Transform patternTransform) 
    {
        this.playerTransform = playerTransform;
        this.patternTransform = patternTransform;
    }

    public float GetReachTime()
    {
        return reachTime;
    }

    // Update is called once per frame
    void Update()
    {
        // Only moves once target set and also hasn't exploded
        if (playerTransform != null && !hasExploded) 
        {
            SetTargetPosition();
            float remainingDistance = CalculateMoveDistanceDistance();
            timeAlive += Time.deltaTime;

            moveSpeed = CalculateMoveSpeed(remainingDistance);
            moveDirection = CalculateMoveDirection();

            
            //Stops moving when close
            if (remainingDistance > 0.4f) 
            {
                MoveTowardsTarget();
                RotateTowardTarget();
            }

            if (timeAlive > reachTime * 1.5f) 
            {
                Destroy(gameObject);
                ChangeScore(-100);

            }

            
        }
        
    }

    private float CalculateMoveDistanceDistance()
    {
        Vector3[] patternPositions = GetPatternPositions();

        float totalDistance = 0;
        for (int i = patternPosition; i < patternPositions.Length - 1; i++)
        {
            totalDistance += (patternPositions[i] - patternPositions[i + 1]).magnitude;
        }

        totalDistance += (playerTransform.position  - transform.position).magnitude;

        return totalDistance;
    }


    private Vector3[] GetPatternPositions() 
    {
        Vector3[] patternPositions = new Vector3[GetPatternLineRenderer().positionCount];
        GetPatternLineRenderer().GetPositions(patternPositions);

        return patternPositions;
    }

    private LineRenderer GetPatternLineRenderer()
    {
        return patternTransform.GetComponent<LineRenderer>();
    }

    private void SetTargetPosition()
    {
        if (patternPosition < GetPatternLineRenderer().positionCount)
        {
            // Sets target position before check to prevent crash
            Vector3[] patternPositions = GetPatternPositions();           
            targetPosition = patternTransform.TransformPoint(patternPositions[patternPosition]) ;

            if ((targetPosition - transform.position).magnitude < 0.8f)
            {
                patternPosition++;
            }

            
        }
        else 
        {
            targetPosition = playerTransform.position;
        }
    }

    private void RotateTowardTarget() 
    {
        Vector3 dir = targetPosition - transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

    }


    private float CalculateMoveSpeed(float totalDistance) 
    {
        float speed = totalDistance / (reachTime - timeAlive);

        return speed;
    }

    private Vector3 CalculateMoveDirection()
    {
        return (targetPosition - transform.position).normalized;
    }

    private void MoveTowardsTarget() 
    {
        
        transform.position += moveSpeed * moveDirection * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("AttackZone")) 
        {
            if (!hasExploded )
            {
                GetComponent<AudioSource>().PlayOneShot(GetComponent<AudioSource>().clip);
                GetComponent<SpriteRenderer>().enabled = false;
                GetComponent<ParticleSystem>().Play();
                GetComponent<BoxCollider2D>().enabled = false;
                ChangeScore(10);
                StartCoroutine("WaitForExplosion");

            }

            hasExploded = true;
        }
      
    }

    private void ChangeScore(int changeAmount) 
    {
        int currentScore = Convert.ToInt32(scoreText.name);
        currentScore += changeAmount;

        scoreText.name = currentScore.ToString();
        scoreText.text = "Score: " + currentScore.ToString();
        if (currentScore < 0)
        {
            scoreText.color = Color.red;
        }
        else
        {
            scoreText.color = Color.white;
        }
    }


    IEnumerator WaitForExplosion()
    {
        
        yield return new WaitForSeconds(gameObject.GetComponent<ParticleSystem>().main.duration);
        Destroy(gameObject);
    }
}
