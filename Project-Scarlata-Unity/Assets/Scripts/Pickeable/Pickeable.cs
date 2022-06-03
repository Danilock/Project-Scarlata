using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector;

namespace Rewriters.Items
{
    /// <summary>
    /// Base class for pickeable objects.
    /// </summary>
    [RequireComponent(typeof(Collider2D), typeof(Rigidbody2D), typeof(SpriteRenderer))]
    public abstract class Pickeable : MonoBehaviour
    {
        [ReadOnly, SerializeField] protected bool CanBePickedUp = true;

        [SerializeField, FoldoutGroup("Pickeable Settings")] protected OptionsOncePicked OnPickObject = OptionsOncePicked.DisableOnPick;
        [SerializeField, FoldoutGroup("Pickeable Settings")] protected CollisionType CollisionTypeValue = CollisionType.OnTriggerEnter;
        [SerializeField, FoldoutGroup("Pickeable Settings"), ShowIf("@this.CollisionTypeValue == CollisionType.OnTriggerEnter || this.CollisionTypeValue == CollisionType.OnCollisionEnter")] 
        protected string[] Tags;
        
        [ShowIf("OnPickObject", OptionsOncePicked.DisableForFewSeconds), FoldoutGroup("Pickeable Settings"), SerializeField] 
        protected float SecondsDisabled = 1f;

        [ReadOnly, FoldoutGroup("Dependencies"),SerializeField] protected SpriteRenderer Renderer;
        [ReadOnly, FoldoutGroup("Dependencies"),SerializeField] protected Collider2D Collider;
        [ReadOnly, FoldoutGroup("Dependencies"),SerializeField] protected Rigidbody2D Rigidbody;

        #region Unity Methods
        protected virtual void Awake()
        {
            Renderer = GetComponent<SpriteRenderer>();
            Collider = GetComponent<Collider2D>();
            Rigidbody = GetComponent<Rigidbody2D>();
        }
        #endregion

        /// <summary>
        /// Picks an object.
        /// </summary>
        /// <param name="owner">The owner that picked up this object.</param>
        public virtual void Pick(GameObject owner)
        {
            if (!CanBePickedUp)
                return;

            OnPick(owner);

            //Decides what to do once the object is picked up(Disable for ever, Disable for few seconds or destroy it).
            switch (OnPickObject)
            {
                case OptionsOncePicked.DestroyOnPick: Destroy(this.gameObject); break;
                case OptionsOncePicked.DisableOnPick: this.gameObject.SetActive(false); break;
                case OptionsOncePicked.DisableForFewSeconds: StartCoroutine(DisableForFewSeconds_CO()); break;
            }
        }

        protected abstract void OnPick(GameObject owner);

        protected virtual void OnCollisionEnter2D(Collision2D collision)
        {
            if (CollisionTypeValue != CollisionType.OnCollisionEnter && collision.gameObject.CompareTag(Tags))
                return;

            Pick(collision.gameObject);
        }

        protected virtual void OnTriggerEnter2D(Collider2D collider)
        {
            if (CollisionTypeValue != CollisionType.OnTriggerEnter && collider.gameObject.CompareTag(Tags))
                return;

            Pick(collider.gameObject);
        }

        /// <summary>
        /// Hides the object for few seconds by disabling its components.
        /// </summary>
        /// <returns></returns>
        protected virtual IEnumerator DisableForFewSeconds_CO()
        {
            Collider.enabled = false;
            Renderer.enabled = false;
            CanBePickedUp = false;

            yield return new WaitForSeconds(SecondsDisabled);

            Collider.enabled = true;
            Renderer.enabled = true;
            CanBePickedUp = true;
        }
    }

    public enum OptionsOncePicked
    {
        DisableOnPick,
        DestroyOnPick,
        DisableForFewSeconds,
        Nothing
    }
}