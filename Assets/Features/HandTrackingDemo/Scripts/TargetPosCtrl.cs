using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetPosCtrl : MonoBehaviour
{
    public TargetPosition[] targetPositions;


    private void Awake()
    {
        targetPositions = GetComponentsInChildren<TargetPosition>();
    }

    private void OnEnable()
    {
        if(targetPositions.Length == 0)
            targetPositions = GetComponentsInChildren<TargetPosition>();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
