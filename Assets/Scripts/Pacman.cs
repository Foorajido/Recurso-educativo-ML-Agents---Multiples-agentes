using UnityEngine;

using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

[RequireComponent(typeof(Movement))]
public class Pacman : Agent
{
    public AnimatedSprite deathSequence;
    public SpriteRenderer spriteRenderer { get; private set; }
    public new Collider2D collider { get; private set; }
    public Movement movement { get; private set; }

    private bool onNode = false;
    public Transform ghost; //Asignar fantasma en Inspector

    public override void Initialize()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        collider = GetComponent<Collider2D>();
        movement = GetComponent<Movement>();
    }


    private void Update()
    {
        if(Vector3.Distance(transform.position, movement.movePoint.position) <= .05f){
            RequestDecision();
        }

        // Rota Pacman para mirar hacia la dirección del movimiento.
        float angle = Mathf.Atan2(movement.direction.y, movement.direction.x);
        transform.rotation = Quaternion.AngleAxis(angle * Mathf.Rad2Deg, Vector3.forward);
    }

    public void ResetState()
    {
        enabled = true;
        spriteRenderer.enabled = true;
        collider.enabled = true;
        deathSequence.enabled = false;
        deathSequence.spriteRenderer.enabled = false;
        movement.ResetState();
        gameObject.SetActive(true);
    }

    public void DeathSequence()
    {
        enabled = false;
        spriteRenderer.enabled = false;
        collider.enabled = false;
        movement.enabled = false;
        deathSequence.enabled = true;
        deathSequence.spriteRenderer.enabled = true;
        deathSequence.Restart();
    }

    public override void OnEpisodeBegin()
    {
        this.transform.parent.transform.Find("GameManager").GetComponent<GameManager>().resetEpisode();
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        var discreteActions = actionBuffers.DiscreteActions;

        if(discreteActions[0] == 1){
            movement.SetDirection(Vector2.up);
        }
        if(discreteActions[0] == 2){
            movement.SetDirection(Vector2.down);
        }
        if(discreteActions[0] == 3){
            movement.SetDirection(Vector2.left);          
        }
        if(discreteActions[0] == 4){
            movement.SetDirection(Vector2.right);        
        }  
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var discreteActionsOut = actionsOut.DiscreteActions;

        discreteActionsOut[0] = 0;

        if (Input.GetKey(KeyCode.UpArrow)) {
        discreteActionsOut[0] = 1;
        }
        else if (Input.GetKey(KeyCode.DownArrow)) {
            discreteActionsOut[0] = 2;
        }
        else if (Input.GetKey(KeyCode.LeftArrow)) {
            discreteActionsOut[0] = 3;
        }
        else if (Input.GetKey(KeyCode.RightArrow)) {
            discreteActionsOut[0] = 4;
        }   
    }

    public override void WriteDiscreteActionMask(IDiscreteActionMask actionMask)
    {

        if(onNode){
            actionMask.SetActionEnabled(0, 0, false);
        } else{
            actionMask.SetActionEnabled(0, 0, true);
        }

        //Arriba
        actionMask.SetActionEnabled(0, 1, !movement.Occupied(Vector2.up));
        //Abajo
        actionMask.SetActionEnabled(0, 2, !movement.Occupied(Vector2.down));
        //Izquierda
        actionMask.SetActionEnabled(0, 3, !movement.Occupied(Vector2.left));
  	   //Derecha
        actionMask.SetActionEnabled(0, 4, !movement.Occupied(Vector2.right));
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.gameObject.CompareTag("Node"))
        {
            onNode = true;
        }
        else{
            onNode = false;
        }    
    }

    private void OnTriggerEnter2D(Collider2D other){
        if(other.gameObject.CompareTag("Pellet"))
        {
            AddReward(1);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Ghost"))
        {
            AddReward(-10);      
            EndEpisode();      
        }
    }

    public void mazeCompleted()
    {
        AddReward(10);
        EndEpisode();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        //Posición Pacman
        sensor.AddObservation(this.transform.localPosition);  
        //Ghost position
        sensor.AddObservation(ghost.transform.localPosition);        
    }

    




}
