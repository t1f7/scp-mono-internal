using RemoteAdmin;
using UnityEngine;
using UnityEngine.Networking;


namespace Cheat
{
    public class Cheat : NetworkBehaviour
    {
        private int _blinkCd;
        private bool _isAltDown;
        private bool _isBlink;
        private bool _isDelDown;
        private bool _isEndDown;
        private bool _isNDown;
        private bool _isNoclip;
        private bool _isSpeedhack;

        private bool _isSpin;
        private bool _isTrace;

        private float _x;
        private float _y;

        private const KeyCode Key_noRecoil = KeyCode.G;
        private const KeyCode Key_Trace = KeyCode.LeftAlt;
        private const KeyCode Key_Spin = KeyCode.Delete;

        private void Start()
        {
            Memory.Init();
        }

        public bool IsValidClass(int cl)
        {
            return !(cl == 2 || cl == -1);
        }


        //private System.Random rand = new System.Random(); rand.Next(1, 999);
        public void Update()
        {
            ESP.Update();

            if (Input.GetKeyDown(KeyCode.LeftAlt))
            {
                if (!_isAltDown)
                {
                    _isAltDown = true;
                    _isTrace = true;
                }
            }
            else if (_isAltDown && Input.GetKeyUp(KeyCode.LeftAlt))
            {
                _isAltDown = false;
                _isTrace = false;
            }

            if (Input.GetKeyDown(KeyCode.Delete))
            {
                if (!_isDelDown) _isDelDown = true;
            }
            else if (_isDelDown && Input.GetKeyUp(KeyCode.Delete))
            {
                _isDelDown = false;
                _isSpin = !_isSpin;
            }

            if (Input.GetKeyDown(KeyCode.End))
            {
                if (!_isEndDown) _isEndDown = true;
            }
            else if (_isEndDown && Input.GetKeyUp(KeyCode.End))
            {
                _isEndDown = false;
                _isBlink = !_isBlink;
            }

            if (Input.GetKeyDown(KeyCode.Mouse2))
            {
                if (!_isNDown)
                {
                    _isNDown = true;
                    _isNoclip = true;
                }
            }
            else if (_isNDown && Input.GetKeyUp(KeyCode.Mouse2))
            {
                _isNDown = false;
                _isNoclip = false;
            }

            var lp = FindLocalPlayer();//PlayerManager.localPlayer;
            if (lp != null)
            {
                if (_isNoclip && _blinkCd < 1)
                {
                    if (_isBlink)
                    {
                        var myPos = lp.transform.position;
                        myPos += 10.0f * Camera.main.transform.forward *
                                 (Input.GetAxis("Vertical") == 0 ? 1.0f : Input.GetAxis("Vertical")) +
                                 Camera.main.transform.right * Input.GetAxis("Horizontal");
                        var move = lp.GetComponent<PlyMovementSync>();
                        move.position = myPos;
                        move.CallCmdSyncData(_y, myPos, _x);
                    }
                    else
                    {
                        lp.transform.position +=
                            0.05f * Camera.main.transform.forward *
                            (Input.GetAxis("Vertical") == 0 ? 1.0f : Input.GetAxis("Vertical")) +
                            Camera.main.transform.right * Input.GetAxis("Horizontal");
                    }
                }

                var myhealth = lp.GetComponent<PlayerStats>().health;

                if (_blinkCd > 0) _blinkCd -= 1;

                if (myhealth <= 0) return;
                // little hacks
                var ccm = lp.GetComponent<CharacterClassManager>();
                if (ccm && !_isSpeedhack)
                {
                    foreach (var t in ccm.klasy)
                        t.runSpeed *= 1.3f;
                    _isSpeedhack = true;

                    // testing
                    PlayerManager.localPlayer.GetComponent<FallDamage>().enabled = false;
                }

                if (Input.GetKeyDown(KeyCode.G))
                {
                    Memory.SetSendPacket(false);
                }
                if (Input.GetKeyDown(KeyCode.H))
                {
                    Memory.SetSendPacket(true);
                }
                if (Input.GetKeyDown(KeyCode.F5))
                {
                    Memory.SetWallhack(!Memory._bWallhack);
                }
                if (Input.GetKeyDown(KeyCode.F6))
                {
                    Memory.SetRadio(!Memory._bAllRadio);
                }
            }
        }

        private void OnGUI()
        {
            GUI.Label(new Rect(10, 30, 500, 30), "Everfree v0.2.2");
            GUI.Label(new Rect(10, 50, 500, 30), "Debug Trace (ALT): " + (_isTrace ? "ON" : "OFF"));
            GUI.Label(new Rect(10, 70, 500, 30), "Noclip (MOUSE3): " + (_isNoclip ? "ON" : "OFF"));
            GUI.Label(new Rect(10, 90, 500, 30), " > NoclipMode [" + (_isBlink ? "blink" : "move") + "] (End)");
            GUI.Label(new Rect(10, 110, 500, 30), "+Run Speed: " + (_isSpeedhack ? "increased" : "off"));

            // I placed it OnGUI because when onGUI is called then hook is 100% accurate.
            // If you know a better place feel free to suggest ;)
            Memory.Hook();

            var lp = PlayerManager.localPlayer;
            if (!lp) return;

            /*move.isGrounded = true; // not sure if this actually helps*/

            Aimbot.Update(lp);

            GUI.Label(new Rect(10, 130, 500, 30), "Spinbot (Del): " + (_isSpin ? "ON" : "OFF"));
            var ccm = lp.GetComponent<CharacterClassManager>();
            GUI.Label(new Rect(10, 150, 500, 30), "Stop position sync [G / H]: " + (Memory._bSendPatched ? "ON" : "OFF"));
            GUI.Label(new Rect(10, 170, 500, 30), "Wallhack [F5]: " + (Memory._bWallhack ? "ON" : "OFF"));
            GUI.Label(new Rect(10, 190, 500, 30), "Listen ALL [F6]: " + (Memory._bAllRadio ? "ON" : "OFF"));
            GUI.Label(new Rect(10, 210, 500, 30), "AimbotTarget [C - choose, F - attack, T - rage]: -"+ (Aimbot.targetNick) +"-");
            GUI.Label(new Rect(10, 230, 500, 30), "ESP items [F4]: " + (ESP._bShowDropEsp ? "ON" : "OFF"));
            GUI.Label(new Rect(10, 250, 500, 30), "ESP locations [F7]: " + (ESP._bShowLocations ? "ON" : "OFF"));
            

            if (_isTrace)
                if (Physics.Raycast(new Ray(Camera.main.transform.position, Camera.main.transform.forward),
                    out var hit))
                {
                    GUI.Label(new Rect(Screen.width / 2f, Screen.height / 2f, 500, 150),
                        "Tag:" + hit.transform.gameObject.tag + " and name is: " + hit.transform.gameObject.name);
                    var firstParent = hit.transform.gameObject.transform.parent;
                    if (firstParent)
                        GUI.Label(new Rect(Screen.width / 2f, Screen.height / 2f + 50, 500, 150),
                            "parent tag & name & type: " + firstParent.parent.tag + " & " + firstParent.name + " & " +
                            firstParent.GetType());
                    //if (Input.GetKeyDown(KeyCode.Delete))
                     //   Destroy(hit.transform.gameObject);
                }

            if (_isNoclip)
            {
                if (_isBlink)
                {
                    var myPos = lp.transform.position;
                    myPos[1] += 15.0f;
                    var move = lp.GetComponent<PlyMovementSync>();
                    move.CallCmdSyncData(_y, myPos, _x);
                    myPos += 1.3f * Camera.main.transform.forward;
                    move.CallCmdSyncData(_y, myPos, _x);
                    myPos[1] -= 10.0f;
                    move.CallCmdSyncData(_y, myPos, _x);
                }
                else
                {
                    var myPos = lp.transform.position;
                    myPos += 50f * Camera.main.transform.forward +
                             Camera.main.transform.right * Input.GetAxis("Horizontal");
                    lp.GetComponent<PlyMovementSync>().position = myPos;
                }
            }

            if (_isSpin)
            {
                var move = lp.GetComponent<PlyMovementSync>();
                move.CallCmdSyncData(_y, lp.transform.position, _x);
                _x += 3.0f;
                _y += 3.0f;
                if (_x > 360.0f) _x = -360.0f;
                if (_y > 360.0f) _y = -360.0f;
            }

            var main = Camera.main;
            //var player = PlayerManager.localPlayer; // FindLocalPlayer();

            ESP.Render(lp, main, ccm);
        }

        private GameObject FindLocalPlayer()
        {
            GameObject[] array = GameObject.FindGameObjectsWithTag("Player");
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i].GetComponent<NetworkIdentity>().isLocalPlayer)
                {
                    return array[i];
                }
            }
            return null;
        }
    }
}