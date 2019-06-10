using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;

public class EnemyBehaviorManager : MonoBehaviour
{
    struct Aggro
    {
        // damage and abilities increase agrro value
        public Player player;
        public float aggro;
        public bool hasSeenInLOS;

        public Aggro(Player p)
        {
            player = p;
            aggro = 0f;
            hasSeenInLOS = false;
        }

        public void SetAsSeen()
        {
            hasSeenInLOS = true;
        }

        public void AddAggro(float toAdd)
        {
            aggro += toAdd;
        }
    }
    EnemyAbilities enemyAbilities;

    Ability[] _abilities;

    List<Aggro> _playersInside;
    NavMeshAgent agent;

    float _aggroFalloff = 0.1f;
    float _aggroPerDamage = 1f;

    [SerializeField]
    EnemyTriggerEnter AggroTrigger;
    float triggerRange;
    public delegate void OnEnterAggroTrigger(Collider other);
    OnEnterAggroTrigger OnAggro; // this tells the trigger which method to call

    [SerializeField]
    LayerMask playerMask;
    [SerializeField]
    float _rotationSpeed = 0.2f;

    Aggro currentTarget = new Aggro(null);
    bool _navActive = false;

    bool enemyActive = true;

    private void Start()
    {
        if (AggroTrigger == null)
        {
            enemyActive = false;
            return;
        }

        enemyAbilities = GetComponent<EnemyAbilities>();
        triggerRange = AggroTrigger.GetComponent<SphereCollider>().radius;
        _abilities = enemyAbilities.GetAbilities();

        agent = GetComponent<NavMeshAgent>();

        OnAggro += OnAggroEnter;
        AggroTrigger.SetDelegate(OnAggro);

        _playersInside = new List<Aggro>();

        StartCoroutine(ManageAggro());
    }

    IEnumerator ManageAggro()
    {
        while(Application.isPlaying)
        {
            if(currentTarget.player != null)
            {
                bool usedAbility = false;
                bool withinRange = false;

                int indexToUse = -1;
                float lowestRange = 100000f;

                // check if within attack range
                // get ability index with the lowest range that can currently attack
                for (int i = 0; i<_abilities.Length;i++) 
                {
                    float abilityRange = _abilities[i].GetRange();

                    bool inRange = abilityRange > Vector3.Distance(transform.position, currentTarget.player.transform.position);
                    bool inLOS = Physics.Raycast(transform.position,
                        currentTarget.player.transform.position - transform.position,
                        abilityRange, playerMask.value);

                    // chosen ability must be within range, in LOS, available, 
                    // and the lowest range ability that can be cast
                    if (inRange && inLOS && enemyAbilities.IsAvailable(i) &&
                        abilityRange < lowestRange)
                    {
                        indexToUse = i;                            
                    }

                    if (inRange)
                        withinRange = true;
                }
                // ability was chosen and was successfully cast
                if (indexToUse != -1f && enemyAbilities.ActivateAbility(indexToUse))
                {
                    usedAbility = true;
                    Debug.Log($"Enemy at {transform.localPosition} cast ability {_abilities[indexToUse].abilityName} at {currentTarget.player.name}");
                }

                if (!usedAbility && !withinRange) // no available abilities within range, move towards tracked player
                    SetNavTarget();
            }

            yield return new WaitForFixedUpdate();
        }
    }

    // TODO



        // if enemy is hit with an attack, add to aggro if not in it



        // TODO


    private void FixedUpdate()
    {
        if (!enemyActive)
            return;

        for(int i=0;i<_playersInside.Count;i++)
        {
            if(!_playersInside[i].hasSeenInLOS)
            {
                if (Physics.Raycast(transform.position, _playersInside[i].player.transform.position - transform.position, triggerRange, playerMask.value))
                {
                    _playersInside[i].SetAsSeen();

                    if (currentTarget.player == null) // this is the first player
                        currentTarget = _playersInside[0]; // make them the current target
                }
            }
        }
        
        // turn to face player
        if(currentTarget.player != null)
        {
            Vector3 dir = (currentTarget.player.transform.position - transform.position).normalized;
            dir.y = 0; // keep it horizontal

            if (dir != Vector3.zero)
            {
                Quaternion rot = Quaternion.LookRotation(dir);
                transform.rotation = Quaternion.Slerp(transform.rotation, rot, _rotationSpeed);
            }
        }
    }

    void SetNavTarget()
    {
        CalculateHighestAggro();

        _navActive = true;
        agent.destination = currentTarget.player.transform.position;
    }

    void CalculateHighestAggro()
    {
        if (_playersInside.Count == 0)
            return;

        float highestVal = -1;
        int highestIndex = -1;

        foreach (Aggro playerAggro in _playersInside)
        {
            if (playerAggro.aggro > highestVal)
                highestIndex = _playersInside.IndexOf(playerAggro);
        }

        currentTarget = _playersInside[highestIndex];
    }

    public void OnAggroEnter(Collider other)
    {
        if (other.tag != "Player")
            return;

        Player player = other.gameObject.GetComponent<Player>();

        // start following player if not already in 
        foreach (Aggro p in _playersInside)
        {
            if(player == p.player)
            {
                // player already being tracked
                return;
            }
        }

        // add the player to tracking
        _playersInside.Add(new Aggro(player));

        // if in line of sight, start tracking
        if (Physics.Raycast(transform.position, other.transform.position - transform.position, Mathf.Infinity, playerMask.value))
        {
            if (_playersInside.Count == 1) // this is the first player
                currentTarget = _playersInside[0]; // make them the current target
        }
    }

    public void Damaged(GameObject playerObject, float damage)
    {
        if (!enemyActive)
            return;

        Player player = playerObject.GetComponent<Player>();
        Aggro playerAggro = _playersInside.Find(x => x.player == player);

        if(playerAggro.player != new Aggro().player) // player is already in list
        {
            // just add the aggro
            playerAggro.AddAggro(damage * _aggroPerDamage);
            Debug.Log($"{damage * _aggroPerDamage} aggro has been added from the {damage} damage");
        }
        else
        {
            // add to list
            _playersInside.Add(new Aggro(player));
            _playersInside[_playersInside.Count - 1].AddAggro(damage * _aggroPerDamage);
            _playersInside[_playersInside.Count - 1].SetAsSeen();
            Debug.Log($"{damage * _aggroPerDamage} aggro has been added from the {damage} damage and added player to list");
        }

        foreach (Aggro p in _playersInside)
        {
            if (player == p.player)
            {
                // player already being tracked
                return;
            }
        }
    }

    public void OnForgetPlayer(Collider other)
    {
        // go back to starting position
    }
}
