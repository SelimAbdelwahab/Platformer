using UnityEngine;
using UnityEngine.SceneManagement;


public class PlayerBehaviour : MonoBehaviour
{
    private const float MAX_SPEED = 1f; // Max speed 20 m/s
    private const float ACCELERATION = 0.5f; // Accelerate by 0.2 m/s^2
    public float JUMP_FORCE = 0.25f;
       
    public Vector3 _velocity = Vector3.zero;
    public float _x, _y, _z;
    
    [HideInInspector]
    public GameObject _currentSurface;

    private GameObject _mainCamera;
    public enum CollisionStates
    {
        On,
        Off,
    }

    public CollisionStates _collisionState = CollisionStates.Off;

    public GameObject _sector;
    private float _rotationX, _rotationZ;
    private GameObject _childMesh;
    private GameObject _chilLight;

    
    // Start is called before the first frame update
    void Awake()
    {
        (_x, _y, _z) = (transform.position.x, transform.position.y, transform.position.z);
        _currentSurface = GameObject.Find("Ground");
        _sector = _currentSurface;

        _mainCamera = GameObject.Find("Main Camera");

        _childMesh = GameObject.Find("Player Mesh");
        _chilLight = GameObject.Find("Player Light");
    }

    void Start()
    {
        Awake();
    }

    // Update is called once per frame
    void Update()
    {
        if (Scene._cameraMode == "3d") {
            Move3D();
        } else if (Scene._cameraMode == "side") {
            MoveSide();
        } else {
            _velocity = Vector3.zero;
        }

        if ((_collisionState != CollisionStates.On))
        {
            _velocity.y -= 1f * Time.deltaTime;
            _y += _velocity.y;
        }
        else
        {
            _y = _currentSurface.transform.position.y + (_currentSurface.transform.lossyScale.y / 2) + 0.5f;
        }

        if (Scene._cameraMode != "won")
        {
            transform.position = new Vector3(_x, _y, _z);
            _childMesh.transform.localEulerAngles = new Vector3(_rotationX, _mainCamera.transform.localEulerAngles.y, _rotationZ);
            //_chilLight.transform.localEulerAngles = new Vector3(0, _mainCamera.transform.localEulerAngles.y, 0);
        }

        if (Input.GetKey(KeyCode.P))
        {
            print("Current Sector: " + _sector.name);
            print("Current Surface: " + _currentSurface.name);
        }
        
    }

    void Move3D()
    {
        int forward = 0;
        int side = 0;

        if ((Input.GetKey("w") || Input.GetKey(KeyCode.UpArrow)) && _collisionState == CollisionStates.On)
        {
            forward = 1;
            if (_velocity.magnitude < MAX_SPEED)
            {
                _velocity.z += ACCELERATION * Time.deltaTime;
            }
        }
        else if ((Input.GetKey("s") || Input.GetKey(KeyCode.DownArrow)) && _collisionState == CollisionStates.On)
        {
            forward = -1;

            if (_velocity.magnitude < MAX_SPEED)
            {
                _velocity.z -= ACCELERATION * Time.deltaTime;
            }
        }
        else if (_collisionState == CollisionStates.On)
        {
            _velocity.z = 0;
        }

        if ((Input.GetKey("a") || Input.GetKey(KeyCode.LeftArrow)) && _collisionState == CollisionStates.On)
        {
            side = -1;

            if (_velocity.magnitude < MAX_SPEED)
            {
                _velocity.x -= ACCELERATION * Time.deltaTime;
            }
        }
        else if ((Input.GetKey("d") || Input.GetKey(KeyCode.RightArrow)) && _collisionState == CollisionStates.On)
        {
            side = 1;

            if (_velocity.magnitude < MAX_SPEED)
            {
                _velocity.x += ACCELERATION * Time.deltaTime;
            }
        }
        else if (_collisionState == CollisionStates.On)
        {
            _velocity.x = 0;
            
        }

        // Moving in both directions so divert speed
        if ((side != 0 && forward != 0) || _velocity.magnitude > MAX_SPEED)
        {
            if (Mathf.Abs(_velocity.x) > Mathf.Abs(_velocity.z))
            {
                _velocity.x -= ACCELERATION * Time.deltaTime * side;
                _velocity.z += ACCELERATION * Time.deltaTime * forward;
            } else
            {
                _velocity.x += ACCELERATION * Time.deltaTime * side;
                _velocity.z -= ACCELERATION * Time.deltaTime * forward;
            }
        }

        if (_collisionState == CollisionStates.On && Input.GetKey("space"))
        {
            _collisionState = CollisionStates.Off;
            _y = _currentSurface.transform.position.y + (_currentSurface.transform.localScale.y / 2) + 0.5f;
            _velocity.y = JUMP_FORCE; // Jump by 2 m/s
        }

        float angle = Mathf.Atan((_mainCamera.transform.position.z - transform.position.z) / (_mainCamera.transform.position.x - transform.position.x));

        if (transform.position.x > _mainCamera.transform.position.x && transform.position.z < _mainCamera.transform.position.z)
        {
            angle *= -1;
            angle = Mathf.PI - angle;
        } else if (transform.position.x > _mainCamera.transform.position.x && transform.position.z > _mainCamera.transform.position.z)
        {
            angle += Mathf.PI;
        } else if (transform.position.x < _mainCamera.transform.position.x && transform.position.z > _mainCamera.transform.position.z)
        {
            angle *= -1;
            angle = 2 * Mathf.PI - angle;
        }

        //Debug.DrawRay(new Vector3(transform.position.x, 0, transform.position.z), new Vector3(mainCamera.transform.position.x, 0, mainCamera.transform.position.z) - new Vector3(transform.position.x, 0, transform.position.z), Color.green);
        //print(angle * Mathf.Rad2Deg);

        _x -= _velocity.z * Mathf.Cos(angle);
        _z -= _velocity.z * Mathf.Sin(angle);

        _x += _velocity.x * Mathf.Cos(angle + Mathf.PI / 2);
        _z += _velocity.x * Mathf.Sin(angle + Mathf.PI / 2);

        _rotationX += _velocity.z * 20;
        _rotationZ += _velocity.x * 20;
    }

    void MoveSide()
    {
        if ((Input.GetKey("a") || Input.GetKey(KeyCode.LeftArrow)) && _collisionState == CollisionStates.On)
        {
            if (_velocity.magnitude < MAX_SPEED)
            {
                _velocity.z -= ACCELERATION * Time.deltaTime;
            }
        }
        else if ((Input.GetKey("d") || Input.GetKey(KeyCode.RightArrow)) && _collisionState == CollisionStates.On)
        {
            if (_velocity.magnitude < MAX_SPEED)
            {
                _velocity.z += ACCELERATION * Time.deltaTime;
            }
        }
        else if (_collisionState == CollisionStates.On)
        {
            _velocity.z = 0;
        }

        if (_collisionState == CollisionStates.On && (Input.GetKey("space") || Input.GetKey("w") || Input.GetKey(KeyCode.UpArrow)) && _collisionState == CollisionStates.On)
        {
            _collisionState = CollisionStates.Off;

            _y = _currentSurface.transform.position.y + (_currentSurface.transform.localScale.y / 2) + 0.5f;
            _velocity.y = JUMP_FORCE; // Jump by 2 m/s
        }
        _velocity.x = 0;

        _z += _velocity.z;
        _x = _sector.transform.position.x + _sector.transform.localScale.x / 2 - 0.5f;
    }

    void OnTriggerEnter(Collider collider)
    {

        if (collider.gameObject != _sector)
        {
            GameObject previous = _sector;
            _sector = collider.gameObject;

            if (Scene._cameraMode == "side")
            {
                Scene.ResetSide(previous);
            }
        }
        if (collider.name == "Finish")
        {
            Scene.SetView("won", true);
            _sector = GetParent(collider.gameObject);
        }

        _velocity = Vector3.zero;
        _y = collider.transform.position.y + (collider.transform.lossyScale.y / 2) + 0.5f;

        _currentSurface = collider.gameObject;
        _collisionState = CollisionStates.On;
    }

    void OnTriggerExit(Collider collider)
    {
        _collisionState = CollisionStates.Off;
    }

    //Detect collisions between the GameObjects with Colliders attached
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Portal")
        {
            return;
        }

        foreach (var item in collision.contacts)
        {
            // Debug.DrawRay(item.point, item.normal, Color.red, 2f);
            bool invalidCollision = (int)(Vector3.Angle(item.normal, Vector3.up)) >= 90;

            if (invalidCollision)
            {
                if (Scene._cameraMode == "3d")
                {
                    transform.position = item.point + item.normal * 0.55f;
                }
                else
                {
                    transform.position = new Vector3(_x, item.point.y + item.normal.y * 0.55f, item.point.z + item.normal.z * 0.55f);
                }

                _velocity = Vector3.zero;
                ZeroCoords();
                return;
            }
        }

        if (collision.gameObject.tag == "Lava")
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        else if (collision.gameObject.tag == "Projectile")
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        _collisionState = CollisionStates.On;

        _velocity.y = 0;
        _y = collision.gameObject.transform.position.y + (collision.gameObject.transform.lossyScale.y / 2) + 0.5f;

        _currentSurface = collision.gameObject;

        GameObject parent = GetParent(_currentSurface);

        _sector = parent;
    }

    void OnCollisionStay(Collision collision)
    {        
        _collisionState = CollisionStates.On;
    }

    void OnCollisionExit(Collision collision)
    {
        if ((_sector.GetComponent(typeof(BoxCollider)) as BoxCollider).bounds.Intersects((GetComponent(typeof(SphereCollider)) as SphereCollider).bounds))
            _collisionState = CollisionStates.On;
        else
            _collisionState = CollisionStates.Off;
    }

    public void ZeroCoords()
    {
        (_x, _y, _z) = (transform.position.x, transform.position.y, transform.position.z);
    }

    public static GameObject GetParent(GameObject current)
    {
        Transform parent = current.transform.parent;

        if (parent != null)
        {
            // print(parent.gameObject.name);
            return GetParent(parent.gameObject);
        }
        else
        {
            return current;
        }
    }
}
