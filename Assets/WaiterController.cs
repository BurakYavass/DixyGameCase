using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class WaiterController : ObjectID
{
    public static WaiterController current;
    [SerializeField] private bool onWork = false;
    [SerializeField] private Animator waitorAnimator;
    [SerializeField] private WaiterStackList waiterStackList;
    [SerializeField] private NavMeshAgent waiterAgent;
    [SerializeField] private Transform barPoint;
    private AgentAI _customer;
    public List<AgentAI> customerPoint = new List<AgentAI>();

    public bool serving = false;
    private bool walking = false;
    private bool _waitingWine = false;
    private bool _once = false;


    private void Awake()
    {
        if (current == null)
        {
            current = this;
        }
    }
    
    void Start()
    {
        GameEventHandler.current.CustomerServeWaiting += OnCustomerServeWaiting;
    }

    private void OnDestroy()
    {
        GameEventHandler.current.CustomerServeWaiting -= OnCustomerServeWaiting;
    }
    
    private void OnCustomerServeWaiting(AgentAI customerObje)
    {
        customerPoint.Add(customerObje);
    }
    
    void Update()
    {
        AnimationControl();

        AgentControl();
    }

    private void WaitingCustomerCheck()
    {
        for (int i = 0; i < customerPoint.Count; i++)
        {
            if (!onWork && waiterStackList.stackList.Count > 1)
            {
                onWork = true;
                _customer = customerPoint[i];
                return;
            }
        }
    }

    private void AgentControl()
    {
        if (_customer && _customer.waitingServe && waiterStackList.stackList.Count >1 )
        {
            _waitingWine = false;
            if (!serving)
            {
                walking = true;
            }
            
            waiterAgent.isStopped = false;
            var customerPos = _customer.transform.position;
            waiterAgent.SetDestination(customerPos + Vector3.right);
            waiterAgent.updateRotation = true;
            var dist = Vector3.Distance(transform.position, customerPos);
            
            if (dist < 7)
            {
                walking = false;
                if (_customer.waitingServe)
                {
                    waiterStackList.ServeGlass(_customer.dropPoint,_customer);
                    waiterAgent.isStopped = true;
                    serving = true;
                    StartCoroutine(WaitServe());
                }
                else
                {
                    WaitingCustomerCheck();
                    _customer = null;
                }
            }
        }
        else if(waiterStackList.stackList.Count < 2)
        {
            serving = false;
            waiterAgent.isStopped = false;
            waiterAgent.destination = barPoint.position;
            waiterAgent.updateRotation = true;
            if (!_waitingWine)
            {
                walking = true;
            }
            
            var dist = Vector3.Distance(transform.position, barPoint.position);
            if (dist < 3)
            {
                walking = false;
                _waitingWine = true;
                waiterAgent.isStopped = true;
                BarController.current.WaiterOnBar(1);
                waiterAgent.updateRotation = false;
                waiterAgent.transform.rotation = Quaternion.Euler(barPoint.forward);
            }
        }
        else if (!_customer && waiterStackList.stackList.Count > 1)
        {
            onWork = false;
            WaitingCustomerCheck();
            waiterAgent.isStopped = true;
            waiterAgent.updateRotation = true;
            _waitingWine = false;
            serving = false;
        }
    }

    private void AnimationControl()
    {
        if (walking)
        {
            waitorAnimator.SetBool("walking", true);

            waitorAnimator.SetBool("Idle", false);
            if (waiterStackList.stackList.Count > 1)
            {
                waitorAnimator.SetBool("Idle", false);
                waitorAnimator.SetBool("carry", true);
                waitorAnimator.SetBool("walking", false);
                waitorAnimator.SetBool("carryidle", false);
            }
            else
            {
                waitorAnimator.SetBool("carry", false);
            }
        }
        else if (!walking)
        {
            waitorAnimator.SetBool("walking", false);
            waitorAnimator.SetBool("Idle", true);
            if (waiterStackList.stackList.Count > 1)
            {
                waitorAnimator.SetBool("carryidle", true);
                waitorAnimator.SetBool("Idle", false);
                waitorAnimator.SetBool("carry", false);
                waitorAnimator.SetBool("walking", false);
            }
            else
            {
                waitorAnimator.SetBool("carryidle", false);
            }
        }
    }

    private IEnumerator WaitServe()
    {
        yield return new WaitForSeconds(1.0f);
        customerPoint.Remove(_customer);
        _customer = null;
        yield return new WaitForSeconds(3.0f);
        onWork = false;
        WaitingCustomerCheck();
        yield return null;
    }
}
