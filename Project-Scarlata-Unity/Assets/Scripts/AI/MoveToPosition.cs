using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveToPosition : MonoBehaviour
{

    [SerializeField] private Transform _destination;
    [SerializeField] private PathBerserker2d.NavAgent _agent;

    // Start is called before the first frame update
    void Start()
    {
        _agent.PathTo(_destination.position);
    }
}
