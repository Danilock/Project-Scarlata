using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewriters.AbilitySystem;
using Sirenix.OdinInspector;
using Cinemachine;

namespace Rewriters.Player
{
    public class PlayerAbilityHandler : MonoBehaviour
    {
        [SerializeField] private PlayerInput _input;

        private List<AbilityHolder> _holders = new List<AbilityHolder>();

        [SerializeField] private List<BaseAbility> _abilities;

        public PlayerTransformationMode TransformationMode = PlayerTransformationMode.LightMode;

        #region Dependencies
        private Character _owner;

        private readonly int Hash_DarkMode = Animator.StringToHash("DarkMode");

        private CinemachineImpulseSource _impulseSource;
        #endregion

        private void Awake()
        {
            _input = GetComponent<PlayerInput>();
            _owner = GetComponent<Character>();
            _impulseSource = GetComponent<CinemachineImpulseSource>();
        }

        private void Start()
        {
            GenerateAbilities();
        }

        // Update is called once per frame
        void Update()
        {
            if (_input.Dash)
            {
                GetAbility<Dash>().TriggerAbility();
            }
            if (_input.Attack)
            {
                GetAbility<PlayerAttack>().TriggerAbility();
            }
            if (_input.Transform)
            {
                GetAbility<PlayerTransformation>().TriggerAbility();
            }
        }

        /// <summary>
        /// Sets an state of the given ability.
        /// </summary>
        /// <param name="abilty">Ability to change.</param>
        /// <param name="state">State to set to this ability.</param>
        public void SetPlayerAbilityState(BaseAbility abilty, AbilityStates state)
        {
            AbilityHolder holder = _holders.Find(x => x.Ability == abilty);

            if (holder == null)
                return;

            holder.SetAbilityState(state);
        }
        
        public void SetDashState(int newState)
        {
            GetAbility<Dash>().SetAbilityState(newState);
        }

        /// <summary>
        /// Generates ability holders based on the abilities we set on the inspector.
        /// </summary>
        private void GenerateAbilities()
        {
            foreach(BaseAbility ability in _abilities)
            {
                AbilityHolder currentHolder = this.gameObject.AddComponent<AbilityHolder>();

                currentHolder.Ability = ability;

                _holders.Add(currentHolder);
            }
        }

        /// <summary>
        /// Returns an ability holder on the player based on the ability you pass as a parameter.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public AbilityHolder GetAbility<T>() where T : BaseAbility
        {
            return _holders.Find(x => x.Ability.GetType() == typeof(T));
        }

        /// <summary>
        /// Sets the current transformation mode on the player.
        /// </summary>
        /// <param name="mode"></param>
        public void SetLightMode(PlayerTransformationMode mode)
        {
            TransformationMode = mode;
            _owner.Animator.SetFloat(Hash_DarkMode, (float) TransformationMode);
        }

        /// <summary>
        /// Generates a simple cinemachine impulse.
        /// </summary>
        public void GenerateCinemachineImpulse() => _impulseSource.GenerateImpulse();

        public void SetTransformAbilityState(AbilityStates newState) => GetAbility<PlayerTransformation>().SetAbilityState(newState);
    }


    public enum PlayerTransformationMode
    {
        LightMode = 0,
        DarkMode = 1
    }
}