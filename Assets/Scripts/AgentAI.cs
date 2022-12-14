using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class AgentAI : ObjectID
{
    [SerializeField] private GameObject _uiGameObject;
    [SerializeField] private Animator _animator;
    [SerializeField] private NavMeshAgent _agent;
    private Camera _camera;
    public Transform destinationPoint;
    public Vector3 forward;
    private Vector3 desiredPosition;
    public Transform dropPoint;

    public bool arriveDestination = false;
    public bool waitingServe;
    public bool agentLeaving = false;
    private bool _finished = false;
    private bool once = false;
    public int wine = 0;
    

    void Start()
    {
        AgentControl();
        desiredPosition = destinationPoint.position;
    }

    private void OnDestroy()
    {
        StopCoroutine(KillingHimself());
    }

    void Update()
    {
        if (!_finished)
        {
            _agent.destination = desiredPosition;
            AgentControl();
        }
        
        if (Math.Abs(transform.position.x - destinationPoint.position.x) < 0.2f)
        {
            arriveDestination = true;
            _agent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
            _agent.updateRotation = false;
            _agent.transform.rotation = Quaternion.Euler(forward);
        }
    }
    

    private void AgentControl()
    {
        if (arriveDestination && wine == 0)
        {
            waitingServe = true;
            _animator.SetBool("Walking",false);
            _animator.SetBool("Sitting",true);
            if (!once)
            {
                once = true;
                StartCoroutine(ServeWaiting());
                GameEventHandler.current.WaitingServe(this);
            }
            
            _uiGameObject.SetActive(true);
            _uiGameObject.transform.LookAt(Camera.main.transform.forward);
        }
        else if(wine != 0)
        {
            if (WaiterController.current)
            {
                WaiterController.current.AgentAIDelete(this);
            }
            waitingServe = false;
            _animator.SetBool("Walking",true);
            _animator.SetBool("Sitting",false);
        }

        if (agentLeaving)
        {
            _agent.updateRotation = true;
            waitingServe = false;
            _animator.SetBool("Walking",true);
            StartCoroutine(KillingHimself());
        }
    }

    public void StateChange()
    {
        wine += 1;
        
        StopCoroutine(Drink());
        StartCoroutine(Drink());
    }

    private IEnumerator ServeWaiting()
    {
        
        yield return new WaitForSeconds(10.0f);
        yield return null;
    }

    private IEnumerator Drink()
    {
        GameManager.current.PlayerMoneyIncrease(50,transform.position ,this);
        _uiGameObject.SetActive(false);
        yield return new WaitForSeconds(10.0f);
        _animator.SetBool("GetUp",true);
        agentLeaving = true;
        arriveDestination = false;
        DeskArea.current.DeskStateChange(destinationPoint.name);
        yield return null;
    }

    private IEnumerator KillingHimself()
    {
        desiredPosition = new Vector3(39.0f, 2.0f, -14.0f);

        if (Math.Abs(transform.position.x - desiredPosition.x) < 0.5f)
        {
            _finished = true;
            Destroy(this.gameObject);
        }
        else
        {
            yield return new WaitForSeconds(10.0f);
            Destroy(this.gameObject);
            yield return null;
        }
        
    }
}
