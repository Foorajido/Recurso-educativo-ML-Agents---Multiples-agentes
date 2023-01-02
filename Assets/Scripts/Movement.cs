using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Movement : MonoBehaviour
{
    public float speed = 8f;
    public float speedMultiplier = 1f;
    public Vector2 initialDirection;
    public LayerMask obstacleLayer;

    public new Rigidbody2D rigidbody { get; private set; }
    public Vector2 direction { get; private set; }
    public Vector2 nextDirection { get; private set; }
    public Vector3 startingPosition { get; private set; }

    public Transform movePoint; //Esta posición se asigna desde el inspector y corresponde al punto donde debe moverse la entidad.
    
    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        startingPosition = transform.position;

        movePoint.parent = null; //Se remueve el padre del objeto para que no dependa del transform del padre y pueda moverse libremente.
    }

    private void Start()
    {
        ResetState();
    }

    public void ResetState()
    {
        speedMultiplier = 1f;
        direction = initialDirection;
        nextDirection = Vector2.zero;
        transform.position = startingPosition;
        rigidbody.isKinematic = false;
        enabled = true;

        movePoint.position = startingPosition; //Al reiniciarse la posicion de movePoint también debe volver a la posición inicial.
    }

    private void Update()
    {
        // Intenta moverse hacia la siguiente dirección mientras se encuentre en la cola
        // para hacer el movimiento más responsivo.
        if (nextDirection != Vector2.zero) {
            SetDirection(nextDirection);
        }
    }

    private void FixedUpdate()
    {

        if(Vector3.Distance(transform.position, movePoint.position) <= .05f && !Occupied(direction))
        {
            if(direction.x == -1)
            {
                movePoint.position += new Vector3(-1f, 0f, 0f);
            }
            else if(direction.x == 1)
            {
                movePoint.position += new Vector3(1f, 0f, 0f);
            }
            else if(direction.y == -1)
            {
                movePoint.position += new Vector3(0f, -1f, 0f);
            }
            else if(direction.y == 1)
            {
                movePoint.position += new Vector3(0f, 1f, 0f);
            }
                
        }
        
        transform.position = Vector3.MoveTowards(transform.position, movePoint.position, speed * Time.deltaTime); //Esto mueve a la entidad HACIA movePoint en un determinado tiempo
        
    }

    public void SetDirection(Vector2 direction, bool forced = false)
    {
        // Sólo establece la dirección si el tile en esa dirección se encuentra disponible
        // si no es el caso se establece como la siguiente dirección de manera que se establezca
        // automáticamente cuando se encuentre disponible.
        if (forced || !Occupied(direction))
        {
            this.direction = direction;
            nextDirection = Vector2.zero;
        }
        else
        {
            nextDirection = direction;
        }
    }

    public bool Occupied(Vector2 direction)
    {
        // Si no se encuentra ningún collider significa que no hay ningún obstáculo en esa dirección.
        RaycastHit2D hit = Physics2D.BoxCast(transform.position, Vector2.one * 0.75f, 0f, direction, 1f, obstacleLayer);
        return hit.collider != null;
    }

}
