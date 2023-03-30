using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveToTarget : MonoBehaviour
{
    public float lerpSpeed = 10f;

    private bool canMove = false;
    private Vector3 target = Vector3.zero;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnEnable()
    {
        canMove = false;
        target = ArcadeManager.instance.grabCoinTransform.position;
        canMove= true;
    }


    private void FixedUpdate()
    {
        Move(); 
    }

    private void Move()
    {
        if (!canMove)
            return;

        float distance = Vector3.Distance(transform.position, target);
        if (distance > 0.1f)
            transform.position = Vector3.Lerp(transform.position, target, Time.deltaTime * lerpSpeed);
        else
            Destroy(gameObject);
    }
}
