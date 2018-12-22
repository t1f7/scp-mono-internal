using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using RemoteAdmin;
using UnityEngine;
using UnityEngine.Networking;
// ReSharper disable PossibleNullReferenceException

//using System.Reflection;
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
                }

                if (Input.GetKeyDown(KeyCode.G))
                {
                    Memory.SetSendPacket(false);
                }
                if (Input.GetKeyDown(KeyCode.H))
                {
                    Memory.SetSendPacket(true);
                }
            }

            /*
             * Reverse it later
             * var shit = lp.GetComponent<CheckpointKiller>();
            if (shit.enabled) shit.enabled = false;
            Camera.main.renderingPath = RenderingPath.VertexLit; // : UnityEngine.RenderingPath.VertexLit);
            Camera.main.cullingMask =
                Scp939PlayerScript.instances[0].scpVision; //this.normalVision : this.scpVision);*/
        }

        private void OnGUI()
        {
            GUI.Label(new Rect(10, 30, 500, 30), "Everfree v0.2");
            GUI.Label(new Rect(10, 50, 500, 30), "Debug Trace (ALT): " + (_isTrace ? "ON" : "OFF"));
            GUI.Label(new Rect(10, 70, 500, 30), "Noclip (MOUSE3): " + (_isNoclip ? "ON" : "OFF"));
            GUI.Label(new Rect(10, 90, 500, 30), " > NoclipMode [" + (_isBlink ? "blink" : "move") + "] (End)");
            GUI.Label(new Rect(10, 110, 500, 30), "+Run Speed: " + (_isSpeedhack ? "increased" : "off"));

            // I placed it OnGUI because when onGUI is called then hook is 100% accurate.
            // If you know a better place feel free to suggest ;)
            Memory.Hook();

            var lp = PlayerManager.localPlayer; // FindLocalPlayer();
            var move = lp.GetComponent<PlyMovementSync>();
            move.isGrounded = true; // not sure if this actually helps

            GUI.Label(new Rect(10, 130, 500, 30), "Spinbot (Del): " + (_isSpin ? "ON" : "OFF"));
            var ccm = lp.GetComponent<CharacterClassManager>();
            GUI.Label(new Rect(10, 140, 500, 30), "Stop position sync [G / H]: " + (Memory._bSendPatched ? "ON" : "OFF"));

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
                move.CallCmdSyncData(_y, lp.transform.position, _x);
                _x += 3.0f;
                _y += 3.0f;
                if (_x > 360.0f) _x = -360.0f;
                if (_y > 360.0f) _y = -360.0f;
            }

            var main = Camera.main;
            var player = PlayerManager.localPlayer; // FindLocalPlayer();
            var curClass = player.GetComponent<CharacterClassManager>().curClass;
            foreach (var gameObject2 in GameObject.FindGameObjectsWithTag("Player"))
                if (gameObject2.GetComponent<NetworkIdentity>())
                {
                    var component = gameObject2.transform.GetComponent<NicknameSync>();
                    var component2 = component.GetComponent<CharacterClassManager>();
                    var b = gameObject2.GetComponent<NetworkIdentity>().transform.position;
                    var curClass2 = component2.curClass;
                    //if (curClass2 >= 0 && (gameObject != gameObject2))
                    if (curClass2 <= -1) continue;
                    var num = (int) Vector3.Distance(main.transform.position, b);
                    Vector3 vector;
                    vector.x = main.WorldToScreenPoint(b).x;
                    vector.y = main.WorldToScreenPoint(b).y;
                    vector.z = main.WorldToScreenPoint(b).z;
                    GUI.color = GetColorById(curClass2);
                    var teamNameById = GetTeamNameById(curClass2);
                    if (!(main.WorldToScreenPoint(b).z > 0f)) continue;
                    GUI.Label(new Rect(vector.x - 50f, Screen.height - vector.y, 100f, 50f),
                        teamNameById + " [" + num + "]");
                    GUI.color = GetTeamById(curClass) != GetTeamById(curClass2) ? Color.red : Color.green;
                    /*GUI.Label(new Rect(vector.x - 50f, Screen.height - vector.y - 60f, 100f, 50f),
                        component.myNick + "[" + gameObject2.GetComponent<PlayerStats>().health + " HP]");*/
                    GUI.Label(new Rect(vector.x - 50f, Screen.height - vector.y - 60f, 100f, 50f), component.myNick);
                }

            // Tag: Door, name: Door 104
            foreach (var gameObject2 in GameObject.FindGameObjectsWithTag("Door"))
                if (gameObject2.name.Contains("Door 104"))
                {
                    GUI.color = Color.white;
                    var b = gameObject2.transform.position;
                    Vector3 vector;
                    vector.x = main.WorldToScreenPoint(b).x;
                    vector.y = main.WorldToScreenPoint(b).y;
                    vector.z = main.WorldToScreenPoint(b).z;
                    if (main.WorldToScreenPoint(b).z > 0f)
                        GUI.Label(new Rect(vector.x + 50f, Screen.height - vector.y, 100f, 50f),
                            gameObject2.name + " " + Vector3.Distance(Camera.main.transform.position,
                                gameObject2.transform.position));
                }

            // Tag: Pickup

            // name contains: Teleport (1) / Teleport (2)!!! \ etc
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
        private static Color GetColorById(int curTeam)
        {
            switch (curTeam)
            {
                case 0:
                case 3:
                case 5:
                case 10:
                case 9:
                    return Color.red;
                case 1:
                    return new Color(1f, 0.7f, 0f, 1f);
                case 8:
                    return Color.green;
                case 6:
                    return Color.white;
                case 4:
                case 11:
                case 12:
                case 13:
                    return Color.blue;
                default: return new Color(1f, 0.7f, 0.3f, 0.5f);
            }
        }

        private static int GetTeamById(int iId)
        {
            switch (iId)
            {
                case 1:
                case 8:
                    return 0;
                case 4:
                case 6:
                case 11:
                case 12:
                case 13:
                case 15:
                case 16:
                case 17:
                    return 1;
                default: return 2;
            }
        }

        private static string GetTeamNameById(int iId)
        {
            switch (iId)
            {
                case -1:
                    return "Server Admin";
                case 0:
                    return "SCP-173";
                case 1:
                    return "Class D";
                case 2:
                    return "Spectator";
                case 3:
                    return "SCP-106";
                case 4:
                    return "MTF Scientist";
                case 5:
                    return "SCP-049";
                case 6:
                    return "Scientist";
                case 8:
                    return "Chaos Insurgency";
                case 9:
                    return "SCP-096";
                case 10:
                    return "SCP-049-2";
                case 11:
                    return "MTF Lieutenant";
                case 12:
                    return "MTF Commander";
                case 13:
                    return "MTF Guard";
                case 15:
                    return "facility Guard";
                case 16:
                    return "scp-939-53";
                case 17:
                    return "scp-939-89";
                default: 
                    return "undefined (#" + iId + ")";
            }
        }
    }
}