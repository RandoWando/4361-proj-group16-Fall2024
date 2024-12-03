using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static Shmoove.AbilityManager;

namespace Shmoove
{
    // manages the abilities. who would've thought
    public class AbilityManager : MonoBehaviour
    {
        // thirdPersonController to be referenced
        [SerializeField] 
        private ThirdPersonController playerController;

        public DashAbility dashAbility;

        // ability abstract base class to be used
        [System.Serializable]
        public abstract class Ability
        {
            // ability specifications
            public string name;
            public float cooldownTime;
            public float activeTime;
            [HideInInspector] 
            public float currentCooldown;

            // methods for activating and removing abilities and their effects
            public abstract void Activate(ThirdPersonController playerController);
            public abstract void OnAbilityEnd(ThirdPersonController playerController);
        }

        // list of our abilities to be handled and used
        public List<Ability> abilities = new List<Ability>();

        // on script initialization, do this
        private void Start()
        {
            // initialize abilities
            abilities.Add(dashAbility);
        }

        // called many times a second, decrements cooldown timers per ability
        private void Update()
        {
            foreach (var ability in abilities)
            {
                // decrement remaining cooldown duration
                if (ability.currentCooldown > 0)
                {
                    ability.currentCooldown -= Time.deltaTime;
                }
            }
        }

        // ability use method, calls cooldown coroutine 
        public bool UseAbility(string abilityName)
        {
            // checking thirdPersonController for noabilities status
            if (playerController.activeStatuses.Contains(ThirdPersonController.statusEffects.noAbilities)
                || playerController.activeStatuses.Contains(ThirdPersonController.statusEffects.noControl))
            {
                //Debug.Log("Can't use abilities due to status effect!");
                return false;
            }

            // grabbing ability from our list
            Ability ability = abilities.Find(x => x.name == abilityName);
            if (ability != null && ability.currentCooldown <= 0)
            {
                // call ability
                ability.Activate(playerController);
                // call cooldown
                StartCoroutine(AbilityCoroutine(ability));
                return true;
            }

            // if ability doesn't exist
            return false;
        }

        // coroutine for cooldown of ability (aka timer)
        private IEnumerator AbilityCoroutine(Ability ability)
        {
            //Debug.Log($"Using ability: {ability.name}");
            ability.currentCooldown = ability.cooldownTime;
            yield return new WaitForSeconds(ability.activeTime);
            // cleanup and finishing effects call
            ability.OnAbilityEnd(playerController);
            //Debug.Log($"Ability ended: {ability.name}");
        }

        // check function to see if a given ability is available
        public bool IsAbilityReady(string abilityName)
        {
            Ability ability = abilities.Find(x => x.name == abilityName);
            return ability != null && ability.currentCooldown <= 0;
        }
    }

    // TODO: REDO THE CAMERA DIRECTION THING. ITS BROKED
    // speciifc ability classes to encapsulate what each should do
    [System.Serializable]
    public class DashAbility : Ability
    {
        // making it available in the editor and defining it
        [SerializeField] 
        private float dashForce = 30f;

        // constructor with the ability values and such
        public DashAbility()
        {
            name = "Dash";
            cooldownTime = 5f;
            activeTime = 0.5f;
        }


        // for reference i think unity will allow us to have an update function in this ability class format
        // this is important for more complex abilities


        // function for activation of the specific ability, passes in playercontroller
        public override void Activate(ThirdPersonController playerController)
        {
            // calculating the players look direction for dash
            Vector3 dashDirection = playerController.playerCamera.transform.forward;

            Debug.Log($"Force applied: {Vector3.Magnitude(dashDirection*dashForce)}");

            // applying force to dash with
            playerController.GetComponent<Rigidbody>().AddForce(dashDirection*dashForce, ForceMode.Impulse);
        }

        // called when we finish the effect
        public override void OnAbilityEnd(ThirdPersonController playerController)
        {
            // any cleanup or additional effects here
            
        }
    }
}
