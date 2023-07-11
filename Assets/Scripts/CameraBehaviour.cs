using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehaviour : MonoBehaviour
{
    private Transform _player;

    private SphereCollider _player_sc;
    private ArrayList _hidden = new ArrayList();
    public static float _orthoSize = 10;

    private float _rotationX;
    private float _rotationY;

    private Vector3 _velocity = Vector3.zero;
    private Camera _c;
    // Start is called before the first frame update
    void Start()
    {
        _player = GameObject.Find("Player").transform;
        _player_sc = GameObject.Find("Player").GetComponent<SphereCollider>();

        _c = this.GetComponent<Camera>();

        _rotationX = 30;
        _rotationY = 0;

        _c.nearClipPlane = 1;
        _c.farClipPlane = 1000;

        _c.transform.localEulerAngles = new Vector3(30, 0, 0);

        _c.orthographic = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Equals))
        {
            _orthoSize -= 3 * Time.deltaTime;
            _c.orthographicSize = _orthoSize;
        }
        else if (Input.GetKey(KeyCode.Minus))
        {
            _orthoSize += 3 * Time.deltaTime;
            _c.orthographicSize = _orthoSize;
        }

        foreach (GameObject go in _hidden)
        {
            MeshRenderer renderer = go.GetComponent<MeshRenderer>();
            StandardShaderUtils.ChangeRenderMode(renderer.material, StandardShaderUtils.BlendMode.Opaque);

            Color color = renderer.material.color;
            color.a = 1f;

            renderer.material.color = color;
        }

        _hidden.Clear();

        if (Scene._cameraMode == "3d")
        {
            RaycastHit[] a = Physics.RaycastAll(this.transform.position, (_player.transform.position - this.transform.position).normalized, Mathf.Infinity, (1 << 8));
            RaycastHit[] b = Physics.RaycastAll(this.transform.position, (_player.transform.position + 0.45f * Vector3.up - this.transform.position).normalized, Mathf.Infinity, (1 << 8));

            //RaycastHit[] c = Physics.RaycastAll(this.transform.position, (player.transform.position + 0.45f * Vector3.right - this.transform.position).normalized, Mathf.Infinity, (1 << 8));
            //RaycastHit[] d = Physics.RaycastAll(this.transform.position, (player.transform.position + 0.45f * Vector3.left - this.transform.position).normalized, Mathf.Infinity, (1 << 8));
            //RaycastHit[] e = Physics.RaycastAll(this.transform.position, (player.transform.position + 0.45f * Vector3.down - this.transform.position).normalized, Mathf.Infinity, (1 << 8));

            RaycastHit[] hits = new RaycastHit[a.Length + b.Length]; //+ c.Length + d.Length + e.Length];

            a.CopyTo(hits, 0);
            b.CopyTo(hits, a.Length);
            //c.CopyTo(hits, a.Length + b.Length);
            //d.CopyTo(hits, a.Length + b.Length + c.Length);
            //e.CopyTo(hits, a.Length + b.Length + c.Length + d.Length);

            foreach (RaycastHit hit in hits)
            {
                GameObject go = hit.collider.gameObject;

                if (go.name == "Player" || go.tag == "Border" || hit.collider.bounds.Intersects(_player_sc.bounds) || _player.position.z < go.transform.position.z)
                    continue;

                MeshRenderer renderer = go.GetComponent<MeshRenderer>();
                Color color = renderer.material.color;

                StandardShaderUtils.ChangeRenderMode(renderer.material, StandardShaderUtils.BlendMode.Transparent);
                color.a = 0f;

                (go.GetComponent(typeof(MeshRenderer)) as MeshRenderer).material.color = color;

                if (!_hidden.Contains(go))
                {
                    _hidden.Add(go);
                }
            }

            if (Input.GetMouseButton(0))
            {
                float mouseX = Input.GetAxis("Mouse X") * 3;
                float mouseY = -Input.GetAxis("Mouse Y") * 3;

                _rotationX += mouseY;
                _rotationY += mouseX;

                _rotationX = Mathf.Clamp(_rotationX, 0, 30);

                Vector3 next = new Vector3(_rotationX, _rotationY);

                transform.localEulerAngles = next;
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                transform.localEulerAngles = new Vector3(30.0f, 0.0f, 0.0f);
                (_rotationX, _rotationY) = (transform.localEulerAngles.x, transform.localEulerAngles.y);
            }

            transform.position = _player.position - transform.forward * 15;
        } 
        else if (Scene._cameraMode == "side")
        {
            transform.position = new Vector3((_player.transform.position.x + 2), _player.transform.position.y, _player.position.z);
        }
        else if (Scene._cameraMode == "won")
        {
            transform.position = _player.position + 15 * Vector3.up;
        }
    }
}
