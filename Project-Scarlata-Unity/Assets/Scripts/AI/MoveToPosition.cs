using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathBerserker2d;

public class MoveToPosition : MonoBehaviour
{
    [SerializeField] private Vector2 _positionToMove;
    [SerializeField] private NavAgent _agent;

    [ContextMenu("Move")]
    public void Move()
    {
        _agent.PathTo(_positionToMove);
    }
}
