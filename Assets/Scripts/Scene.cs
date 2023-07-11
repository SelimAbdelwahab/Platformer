using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Scene : MonoBehaviour
{
    private static float _timeAlive = 0;
    public static string _cameraMode = "3d";

    static private PlayerBehaviour _playerBehaviour;
    static private GameObject _player;
    static private GameObject _goMainCamera;
    static private Camera _mainCamera;
    static private GameObject[] _anchors;

    static private Text _timeText;
    static private GameObject _wonText;

    private float _timeSinceWon = 0;
    static private GameObject _invalidText;
    static private Vector3 _pos;
    static private string _activeScene;

    // Start is called before the first frame update
    void Awake()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;

        _goMainCamera = GameObject.Find("Main Camera");
        _mainCamera = _goMainCamera.GetComponent(typeof(Camera)) as Camera;
        _anchors = GameObject.FindGameObjectsWithTag("Anchor");
        _cameraMode = "3d";

        _player = GameObject.Find("Player");
        _playerBehaviour = _player.GetComponent(typeof(PlayerBehaviour)) as PlayerBehaviour;

        _wonText = GameObject.Find("Won Text");
        _wonText.SetActive(false);

        _timeText = GameObject.Find("Time Text").GetComponent<Text>();

        _invalidText = GameObject.Find("Invalid Switch");
       
        if (_activeScene == null)
        {
            SceneManager.LoadScene("intro");
            _activeScene = SceneManager.GetActiveScene().name;
        } else if (_activeScene != SceneManager.GetActiveScene().name)
        {
            _activeScene = SceneManager.GetActiveScene().name;
            _timeAlive = 0;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // print(_pos);
        if (_cameraMode != "won")
        {
            _timeAlive += Time.deltaTime;
            _timeText.text = "Time: " + Mathf.Round(_timeAlive) + "s";
        } else
        {
            _timeText.text = "Time: " + _timeAlive + "s";
        }

        if (_player.transform.position.y <= -20)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        
        if (_cameraMode != "won")
        {
            if (Input.GetKeyDown(KeyCode.C))
            {
                if (_cameraMode == "3d")
                    SetView("side", true);
                else
                    SetView("3d", true);
            }
        } else
        {
            _timeSinceWon += Time.deltaTime;

            if (_timeSinceWon >= 3)
            {
                SceneManager.LoadScene("Intro");
            }
        }
    }

    public static void SetView(string view, bool move)
    {
        if (_cameraMode == "3d")
        {
            _invalidText.SetActive(false);
        }
        else if (_cameraMode == "won")
        {
            _wonText.SetActive(false);
        }

        _cameraMode = view;
        SwitchView(move);
    }

    private static bool ValidShift()
    {
        for (var i = 0; i < _playerBehaviour._sector.transform.childCount; i++)
        {
            GameObject anchor = _playerBehaviour._sector.transform.GetChild(i).gameObject;
            try
            {
                BoxCollider abc = anchor.GetComponent(typeof(BoxCollider)) as BoxCollider;

                // print(new Vector3(anchor.transform.position.x + abc.center.x, _player.transform.position.y, _player.transform.position.z));
                // print(abc.bounds);
                if (abc.bounds.Contains(new Vector3(_playerBehaviour._sector.transform.position.x, _playerBehaviour._y, _playerBehaviour._z)))
                {
                    return false;
                }
            } catch
            {
                continue;
            }
            
        }

        return true;
    }

    public static void SwitchView(bool move)
    {
        if (_cameraMode == "side")
        {
            _pos = _playerBehaviour.transform.position;
            _mainCamera.transform.localEulerAngles = new Vector3(0, 270, 0);

            _mainCamera.orthographic = true;
            _mainCamera.orthographicSize = CameraBehaviour._orthoSize;
            _mainCamera.nearClipPlane = 1;
            _mainCamera.farClipPlane = 1000;

            SphereCollider psc = _player.GetComponent(typeof(SphereCollider)) as SphereCollider;
            psc.center = new Vector3(-(_playerBehaviour._sector.transform.localScale.x / 2 - 0.5f), 0, 0);

            foreach (GameObject go in GameObject.FindGameObjectsWithTag("Ground"))
            {
                for (var i = 0; i < go.transform.childCount; i++)
                {
                    try
                    {
                        GameObject child = go.transform.GetChild(i).gameObject;
                        
                        if (child.tag == "Border")
                            continue;

                        BoxCollider abc = child.GetComponent(typeof(BoxCollider)) as BoxCollider;
                        abc.center = Vector3.zero;

                        if (go.name == _playerBehaviour._sector.name)
                            abc.center = new Vector3(-child.transform.localPosition.x / child.transform.localScale.x, 0, 0);
                        else
                            child.SetActive(false);
                    }
                    catch
                    {
                        continue;
                    }
                }
            }

            if (!ValidShift())
            {
                _player.transform.position = _pos;
                _playerBehaviour.ZeroCoords();
                SetView("3d", false);
                _invalidText.SetActive(true);
            }

            foreach (GameObject go in GameObject.FindGameObjectsWithTag("Ground"))
            {
                BoxCollider bc = go.GetComponent(typeof(BoxCollider)) as BoxCollider;
                bc.center = Vector3.zero;

                if (go == _playerBehaviour._sector)
                    continue;

                bc.center = new Vector3((_playerBehaviour._sector.transform.localPosition.x - go.transform.localPosition.x) / go.transform.localScale.x, 0, 0);
            }

        } else if (_cameraMode == "3d")
        {
            _mainCamera.nearClipPlane = 1;
            _mainCamera.farClipPlane = 1000;

            _mainCamera.transform.localEulerAngles = new Vector3(30, 0, 0);

            _mainCamera.orthographic = false;

            SphereCollider psc = _player.GetComponent(typeof(SphereCollider)) as SphereCollider;

            if (_playerBehaviour._currentSurface != null && _playerBehaviour._currentSurface != _playerBehaviour._sector && move)
            {
                _player.transform.position = new Vector3(_playerBehaviour._currentSurface.transform.position.x, _player.transform.position.y, _player.transform.position.z);
                _playerBehaviour.ZeroCoords();
            } else if (move)
            {
                _playerBehaviour.transform.position = _pos;
                _playerBehaviour.ZeroCoords();
            }

            
            foreach (GameObject go in GameObject.FindGameObjectsWithTag("Ground"))
            {
                for (var i = 0; i < go.transform.childCount; i++)
                {
                    try
                    {
                        GameObject child = go.transform.GetChild(i).gameObject;

                        if (child.tag == "Border")
                            continue;

                        BoxCollider abc = child.GetComponent(typeof(BoxCollider)) as BoxCollider;
                        abc.center = Vector3.zero;

                        child.SetActive(true);

                    }
                    catch
                    {
                        continue;
                    }
                }
            }

            foreach (GameObject go in GameObject.FindGameObjectsWithTag("Ground"))
            {
                BoxCollider bc = go.GetComponent(typeof(BoxCollider)) as BoxCollider;
                bc.center = Vector3.zero;
            }

            psc.center = Vector3.zero;

        }
        else if (_cameraMode == "won")
        {
            GameObject finish = GameObject.Find("Finish");
            Vector3 pos = finish.transform.position;
            pos.y += finish.transform.localScale.y / 2 + 0.5f;

            _player.transform.position = pos;
            _playerBehaviour.ZeroCoords();

            _mainCamera.transform.localEulerAngles = new Vector3(90, 0, 0);

            _wonText.SetActive(true);
        }
    }

    private static void ShiftPlayer()
    {
        bool shifted = false;
        foreach (GameObject anchor in _anchors)
        {
            BoxCollider abc = anchor.GetComponent(typeof(BoxCollider)) as BoxCollider;

            // Will place player above anchor if the players 2d projection intersects the anchors 2d projection
            if (abc.bounds.Intersects(_player.GetComponent<SphereCollider>().bounds) || abc.bounds.Contains(new Vector3(0, _playerBehaviour._y, _playerBehaviour._z)))
            {
                if (_playerBehaviour._y - 0.5 <= anchor.transform.position.y)
                {
                    _playerBehaviour._z = anchor.transform.position.z - (anchor.transform.lossyScale.z / 2f) - 0.5f;
                    shifted = true;
                    break;
                }
                else
                {
                    _playerBehaviour._y = anchor.transform.position.y + (anchor.transform.localScale.y / 2) + 0.5f;
                    shifted = false;
                    break;
                }

            }
        }

        if (shifted)
            ShiftPlayer();
    }

    public static void ResetSide(GameObject previous)
    {
        //    for (var i = 0; i < previous.transform.childCount; i++)
        //    {
        //        try
        //        {
        //            GameObject child = previous.transform.GetChild(i).gameObject;
        //            print(child.name);
        //            if (child.tag == "Border")
        //                continue;

        //            BoxCollider abc = child.GetComponent(typeof(BoxCollider)) as BoxCollider;
        //            abc.center = Vector3.zero;
        //        }
        //        catch
        //        {
        //            continue;
        //        }
        //    }

        //    SphereCollider psc = _player.GetComponent(typeof(SphereCollider)) as SphereCollider;
        //    psc.center = new Vector3(-(_playerBehaviour._sector.transform.localScale.x / 2 - 0.5f), 0, 0);

        //    foreach (GameObject go in GameObject.FindGameObjectsWithTag("Ground"))
        //    {
        //        for (var i = 0; i < go.transform.childCount; i++)
        //        {
        //            try
        //            {
        //                GameObject child = go.transform.GetChild(i).gameObject;

        //                if (child.tag == "Border")
        //                    continue;

        //                BoxCollider abc = child.GetComponent(typeof(BoxCollider)) as BoxCollider;
        //                abc.center = Vector3.zero;

        //                child.SetActive(true);

        //            }
        //            catch
        //            {
        //                continue;
        //            }
        //        }
        //    }

        //    ShiftPlayer();

        // print(true);

        _playerBehaviour.transform.position = new Vector3(_playerBehaviour._sector.transform.position.x + _playerBehaviour._sector.transform.lossyScale.x/2, _playerBehaviour._sector.transform.position.y + 1, _player.transform.position.z);
        _playerBehaviour.ZeroCoords();

        SetView("3d", false);

    }
}
