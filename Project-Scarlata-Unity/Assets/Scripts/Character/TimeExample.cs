using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeExample : MonoBehaviour
{
    [SerializeField] private float _cooldown;
    [SerializeField] private float _lastTimeCooldown;

    [SerializeField] private float _time;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (_lastTimeCooldown + _cooldown < Time.time)
            {
                Debug.Log("Trigger Ability");
                _lastTimeCooldown = Time.time;
            }
        }

        _time = Time.time;
    }
}
