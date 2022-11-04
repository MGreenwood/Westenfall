using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;

public class EnemyBehaviorManager : MonoBehaviour
{
    class Aggro
    {
        // damage and abilities increase agrro value
        public readonly Player player;
        public float aggro;
        public bool hasSeenInLOS;

        public Aggro(Player p)
        {
            player = p;
            aggro = 0f;
            hasSeenInLOS = false;
        }

        public Aggro()
        {
            player = null;
            aggro = 0;
            hasSeenInLOS = false;
        }

        public void SetAsSeen()
        {
            this.hasSeenInLOS = true;
        }

        public void AddAggro(float toAdd)
        {
            this.aggro += toAdd;
        }
    }
    EnemyAbilities enemyAbilities;

    Ability[] _abilities;

    List<Aggro> _playersInside;
    NavMeshAgent agent;

    readonly float _aggroFalloff = 0.1f;
    readonly float _aggroPerDamage = 1f;

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
            if (currentTarget.player != null)
            {
                bool usedAbility = false;
                bool withinRange = false;

                int indexToUse = -1;
                float lowestRange = 100000f;

                // check if within attack range
                // get ability index with the lowest range that can currently attack
                for (int i = 0; i < _abilities.Length; i++)
                {
                    float abilityRange = _abilities[i].GetRange();
                    RaycastHit hit;
                    bool inRange = abilityRange > Vector3.Distance(transform.position, currentTarget.player.transform.position);
                    bool inLOS = Physics.Linecast(transform.position,
                        transform.position + (currentTarget.player.transform.position - transform.position).normalized * _abilities[i].GetRange(), out hit,
                        ~playerMask) && hit.transform.tag == "Player";

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
                    agent.destination = transform.position;
                }

                //
                { 
                    RaycastHit hit;
                    bool inLOS = Physics.Linecast(transform.position,
                        currentTarget.player.transform.position + (currentTarget.player.transform.position - transform.position).normalized * 100f, out hit,
                        ~playerMask) && hit.transform.tag == "Player";

                    if ((!usedAbility && !withinRange) || !inLOS) // no available abilities within range, move towards tracked player
                        SetNavTarget();
                }
                //
            }

            yield return new WaitForFixedUpdate();
        }
    }

    private void FixedUpdate()
    {
        if (!enemyActive)
            return;

        for(int i=0;i<_playersInside.Count;i++)
        {
            if(!_playersInside[i].hasSeenInLOS)
            {
                RaycastHit hit;
                if (Physics.Raycast(transform.position,
                        (_playersInside[i].player.transform.position - transform.position).normalized, out hit,
                        Mathf.Infinity, ~playerMask) && hit.transform.tag == "Player")
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

        // get range of lowest range ability
        float lowest = 100f;
        for(int i = 0; i < _abilities.Length; ++i)
            if(_abilities[i].GetRange() < lowest && enemyAbilities.IsAvailable(i))
                lowest = _abilities[i].GetRange();

        // get within range
        Vector3 newTarget = currentTarget.player.transform.position + (transform.position - currentTarget.player.transform.position).normalized * (lowest * 0.65f);

        if (lowest != 100f)
            agent.SetDestination(newTarget);
        else
            agent.SetDestination(currentTarget.player.transform.position);
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
            if(player.GetInstanceID() == p.player.GetInstanceID())
            {
                // player already being tracked
                return;
            }
        }

        // add the player to tracking
        _playersInside.Add(new Aggro(player));

        // if in line of sight, start tracking
        RaycastHit hit;
        if (Physics.Raycast(transform.position,
                (other.transform.position - transform.position).normalized, out hit,
                Mathf.Infinity, ~playerMask) && hit.transform.tag == "Player")
        {
            if (_playersInside.Count == 1) // this is the first player
                currentTarget = _playersInside[0]; // make them the current target

            _playersInside[_playersInside.Count - 1].SetAsSeen();
        }
    }

    public void Damaged(GameObject playerObject, float damage)
    {
        if (!enemyActive)
            return;

        Player player = playerObject.GetComponent<Player>();
        Aggro playerAggro = _playersInside?.Find(x => x.player == player);

        if(playerAggro != null) // player is already in list
        {
            // just add the aggro
            playerAggro.AddAggro(damage * _aggroPerDamage);
        }
        else
        {
            // add to list
            _playersInside.Add(new Aggro(player));
            _playersInside[_playersInside.Count - 1].AddAggro(damage * _aggroPerDamage);
            _playersInside[_playersInside.Count - 1].SetAsSeen();
            CalculateHighestAggro();
        }

        // not sure why this is here 9/12/20
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
