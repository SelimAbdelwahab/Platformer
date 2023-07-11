using UnityEngine;

public class Portal : MonoBehaviour
{
    public GameObject _spouse;
    public float _timeSince = 0;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        _timeSince += Time.deltaTime;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "Player" && _timeSince > 1)
        {
            _timeSince = 0;
            collision.transform.position = _spouse.transform.position + Vector3.up;

            PlayerBehaviour pb = collision.gameObject.GetComponent<PlayerBehaviour>();
            pb.ZeroCoords();
            pb._velocity = Vector3.zero;
            pb._collisionState = 0;
            pb._currentSurface = _spouse.transform.parent.gameObject;

            try
            {
                Portal spouseP = _spouse.GetComponent<Portal>();
                if (_spouse != null)
                {
                    spouseP._timeSince = 0;
                }
            } catch
            {
                return;
            }
        }
    }
}
