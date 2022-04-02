using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class B_Awoken : Boss_State
{
    [SerializeField] Transform center;
    [SerializeField] Transform[] waypoints;
    [SerializeField] int randWP;
    [SerializeField] float speed = 5;
    Vector3 movementVec,player;
    [SerializeField] float timer;
    [SerializeField] int end;

    //>>>CHASE VARS>>>

    [SerializeField] float radius;
    [SerializeField] LayerMask environmentLayer;

    public override void BossOnCollisionEnter(Boss_StateMachine bsm, Collider collider)
    {
        //throw new System.NotImplementedException();
    }

    public override void StartState(Boss_StateMachine bsm)
    {
        bsm.agent.enabled = false;
        bsm.transform.position = center.transform.position;
        randWP = Random.Range(0, waypoints.Length);
        movementVec = new Vector3(waypoints[randWP].position.x, 0, waypoints[randWP].position.z);
        timer = 0;
        end = Random.Range(20, 30);
        radius = bsm.agent.radius * 0.9f;
    }

    public override void UpdateState(Boss_StateMachine bsm)
    {
        timer += Time.deltaTime * 1;
        if (timer >= end)
        {
            MiddlePhaseChange(bsm);
        }
        else
        {
            AwokenActive(bsm);
        }

        player = new Vector3(bsm.player.transform.position.x, bsm.transform.position.y, bsm.player.transform.position.z);
        bsm.transform.LookAt(player);
    }

    bool isGrounded;
    int state = 0;
    void MiddlePhaseChange(Boss_StateMachine bsm)
    {
        if (state == 0)
        {
            if (Vector3.Distance(bsm.transform.position, center.transform.position) <= 1)
            {
                state++;
            }
            else
            {
                movementVec = center.transform.position - bsm.transform.position;
                movementVec.y = 0;
                bsm.transform.position += movementVec.normalized * (speed / 4) * Time.deltaTime;
            }
        }
        else
        {
            bsm.transform.position += Vector3.down * (speed / 4) * Time.deltaTime;
            isGrounded = Physics.CheckSphere(bsm.transform.position, radius, environmentLayer);
            if (isGrounded)
            {
                bsm.agent.enabled = true;
                bsm.BossSwitchState(bsm.chaseState);
            }
        }
    }

    void AwokenActive(Boss_StateMachine bsm)
    {
        WaypointDistanceCheck(bsm.transform);
        bsm.transform.position += movementVec.normalized * speed * Time.deltaTime;
        Debug.DrawRay(bsm.transform.position, movementVec);
        Debug.Log("Awoken State");
    }

    void WaypointDistanceCheck(Transform bsm)
    {
        if (Vector3.Distance(bsm.transform.position, waypoints[randWP].position) <= 2)
        {
            randWP = Random.Range(0, waypoints.Length);
        }
        else
        {
            movementVec = waypoints[randWP].position - bsm.transform.position;
            movementVec.y = 0;
        }
    }
}