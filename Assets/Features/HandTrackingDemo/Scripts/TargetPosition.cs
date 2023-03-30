using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TargetPosition : MonoBehaviour
{
    public Vector3 target1Pos = Vector3.zero;
    public Vector3 target2Pos = Vector3.zero;
    public Vector3 target3Pos = Vector3.zero;

    private Transform[] childTransforms;

    private void Awake()
    {
        SetUpPositions();
    }

    private void OnEnable()
    {
        if (target1Pos == Vector3.zero)
            SetUpPositions();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void SetUpPositions()
    {
        childTransforms = GetComponentsInChildren<Transform>();
        //idx 0 is the parent transform + 3 positions
        if (childTransforms.Length != 4)
            return;

        target1Pos = childTransforms[1].position;
        target2Pos = childTransforms[2].position;
        target3Pos = childTransforms[3].position;
    }
}
