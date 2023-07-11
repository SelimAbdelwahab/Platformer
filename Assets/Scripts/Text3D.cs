using UnityEngine;

public class Text3D : MonoBehaviour
{

    private Transform _mainCamera;
    private float _time = 0;
    private TextMesh _tm;
    // Start is called before the first frame update
    void Start()
    {
        _mainCamera = GameObject.Find("Main Camera").transform;
        _tm = GetComponent<TextMesh>();
        this.gameObject.SetActive(false);
    }

    void Awake()
    {
        _mainCamera = GameObject.Find("Main Camera").transform;
        _tm = GetComponent<TextMesh>();

        transform.position = _mainCamera.transform.position + _mainCamera.forward * 20 + Vector3.up * 8;
        transform.localEulerAngles = new Vector3(0, _mainCamera.localEulerAngles.y, 0);
    }

    // Update is called once per frame
    void Update()
    {
        _time += Time.deltaTime;
        transform.position = _mainCamera.transform.position + _mainCamera.forward * 20 + Vector3.up * 8;
        transform.localEulerAngles = new Vector3(0, _mainCamera.localEulerAngles.y, 0);

        if (_time >= 3)
        {
            if (_tm.color.a <= 0)
            {
                _tm.color = new Vector4(_tm.color.r, _tm.color.g, _tm.color.b, 1);
                _time = 0;
                this.gameObject.SetActive(false);
            } else
            {
                _tm.color = new Vector4(_tm.color.r, _tm.color.g, _tm.color.b, _tm.color.a - 1 * Time.deltaTime);
            }
            
        }
    }
}
