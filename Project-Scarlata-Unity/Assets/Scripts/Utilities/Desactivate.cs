using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

/// <summary>
/// Custom class for objects desactivation.
/// </summary>
public class Desactivate : MonoBehaviour
{
    private enum ObjectToDesactivate { Self, Other }
    [SerializeField] private ObjectToDesactivate _objectToDesactivateType;
    [SerializeField, ShowIf("_objectToDesactivateType", ObjectToDesactivate.Other)] private GameObject _objectToDesactivate;
    [SerializeField] private DesactivationEvent _desactivationMethod = DesactivationEvent.OnEnable;
    [SerializeField] private bool _waitForSecondsBeforeDesactivate = false;
    [SerializeField, ShowIf("_waitForSecondsBeforeDesactivate", true)] private float _seconds;
    [SerializeField, ShowIf("@this._desactivationMethod == DesactivationEvent.OnTriggerEnter || this._desactivationMethod == DesactivationEvent.OnCollisionEnter")]
    private string[] _tags;
    private bool _isAlreadyDesactivating = false;
    [SerializeField, FoldoutGroup("Extra")] private bool _avoidBeingDisabledOnFirstStart = false;

    private void OnEnable()
    {
        if (_avoidBeingDisabledOnFirstStart)
        {
            _avoidBeingDisabledOnFirstStart = false;
            return;
        }

        if (_desactivationMethod != DesactivationEvent.OnEnable)
            return;

        HandleDesactivation();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (_desactivationMethod != DesactivationEvent.OnCollisionEnter || !collision.gameObject.CompareTag(_tags))
            return;

        HandleDesactivation();
    }

    /// <summary>
    /// Starts process on desactivating the object.
    /// </summary>
    private void HandleDesactivation()
    {
        //Check if has to wait for seconds and is not already in the process of desactivating.
        if (_waitForSecondsBeforeDesactivate && !_isAlreadyDesactivating)
        {
            _isAlreadyDesactivating = true;

            StartCoroutine(HandleDesactivation_CO());
            return;
        }

        DesactivateObject();
    }

    /// <summary>
    /// Waits for seconds to desactivate the object.
    /// </summary>
    /// <returns></returns>
    private IEnumerator HandleDesactivation_CO()
    {
        yield return new WaitForSeconds(_seconds);
        DesactivateObject();
    }

    /// <summary>
    /// Desactivates the object.
    /// </summary>
    private void DesactivateObject()
    {
        if (_objectToDesactivateType == ObjectToDesactivate.Self)
            this.gameObject.SetActive(false);
        else
            _objectToDesactivate?.SetActive(false);

        _isAlreadyDesactivating = false;
    }
}

public enum DesactivationEvent
{
    OnStart,
    OnEnable,
    OnCollisionEnter,
    OnTriggerEnter
}