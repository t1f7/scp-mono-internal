using GameConsole;
using System.Collections.Generic;
using Dissonance.Audio.Playback;
using Dissonance.Integrations.UNet_HLAPI;

namespace Cheat
{
    using UnityEngine;
    using UnityEngine.Networking;

    public class Cheat : NetworkBehaviour
    {
        private bool isNDown = false;
        private bool isAltDown = false;
        private bool isEndDown = false;
        private bool isDelDown = false;

        private bool isSpin = false;
        private bool isTrace = false;
        private bool isSpeedhack = false;
        private bool isNoclip = false;
        private bool isBlink = false;

        private int blinkCd;
        private float x;
        private float y;

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.LeftAlt))
            {
                if (!isAltDown)
                {
                    isAltDown = true;
                    isTrace = true;
                }
            }
            else if (isAltDown && Input.GetKeyUp(KeyCode.LeftAlt))
            {
                isAltDown = false;
                isTrace = false;
            }

            if (Input.GetKeyDown(KeyCode.Delete))
            {
                if (!isDelDown)
                {
                    isDelDown = true;
                }
            }
            else if (isDelDown && Input.GetKeyUp(KeyCode.Delete))
            {
                isDelDown = false;
                isSpin = !isSpin;
            }

            if (Input.GetKeyDown(KeyCode.End))
            {
                if (!isEndDown)
                {
                    isEndDown = true;
                }
            }
            else if (isEndDown && Input.GetKeyUp(KeyCode.End))
            {
                isEndDown = false;
                isBlink = !isBlink;
            }
            

            if (Input.GetKeyDown(KeyCode.Mouse2))
            {
                if (!isNDown)
                {
                    isNDown = true;
                    isNoclip = true;
                }
            }
            else if (isNDown && Input.GetKeyUp(KeyCode.Mouse2))
            {
                isNDown = false;
                isNoclip = false;
            }

            GameObject lp = PlayerManager.localPlayer;
            if (isNoclip && blinkCd < 1)
            {
                if (isBlink)
                {
                    //lp.transform.position += 2.5f * (Camera.main.transform.forward * (Input.GetAxis("Vertical") == 0 ? 1.0f : Input.GetAxis("Vertical")) + Camera.main.transform.right * Input.GetAxis("Horizontal"));
                    //blinkCd = 3000;
                    Vector3 myPos = lp.transform.position;
                    myPos += 10.0f * Camera.main.transform.forward * (Input.GetAxis("Vertical") == 0 ? 1.0f : Input.GetAxis("Vertical")) + Camera.main.transform.right * Input.GetAxis("Horizontal");
                    PlyMovementSync move = lp.GetComponent<PlyMovementSync>();
                    move.CallCmdSyncData(y, myPos, x);
                }
                else
                {
                    lp.transform.position += 0.5f * Camera.main.transform.forward * (Input.GetAxis("Vertical") == 0 ? 1.0f : Input.GetAxis("Vertical")) + Camera.main.transform.right * Input.GetAxis("Horizontal");
                }
            }

            int myhealth = lp.GetComponent<PlayerStats>().health;

            if (blinkCd > 0) blinkCd -= 1;

            if (myhealth > 0)
            {
                // little hacks
                CharacterClassManager ccm = lp.GetComponent<CharacterClassManager>();
                if (ccm && !isSpeedhack)
                {
                    for (int j = 0; j < ccm.klasy.Length; j++)
                    {
                        ccm.klasy[j].runSpeed *= 1.3f;
                    }
                    //lp.GetComponent<Scp096PlayerScript>.ragemultiplier_coodownduration = 5.1f;
                    isSpeedhack = true;
                }

                // pocket shoot!
                if (Input.GetKeyDown(KeyCode.H))
                {
                    foreach (GameObject gameObject2 in GameObject.FindGameObjectsWithTag("Player"))
                    {
                        gameObject2.GetComponent<Scp173PlayerScript>().CallRpcSyncAudio();
                    }
                }
                if (Input.GetKeyDown(KeyCode.G))
                {
                    RaycastHit hit;
                    if (Physics.Raycast(new Ray(Camera.main.transform.position, Camera.main.transform.forward), out hit))
                    {
                        CharacterClassManager component = hit.transform.GetComponent<CharacterClassManager>();
                        if (component != null)
                        {
                            /*Scp106PlayerScript oldman = lp.GetComponent<Scp106PlayerScript>();
                            if (!oldman) lp.AddComponent<Scp106PlayerScript>();
                            oldman.CallCmdMovePlayer(hit.transform.gameObject, ServerTime.time);
                            Hitmarker.Hit(1.5f);*/

                            //lp.GetComponent<PlayerStats>().HurtPlayer(new PlayerStats.HitInfo(40f, base.GetComponent<NicknameSync>().myNick + " (" + base.GetComponent<CharacterClassManager>().SteamId + ")", DamageTypes.Scp106, base.GetComponent<QueryProcessor>().PlayerId), ply);
                            //ply.GetComponent<PlyMovementSync>().SetPosition(UnityEngine.Vector3.down * 1997f);
                            //gameObject.GetComponent<PlayerStats>().HurtPlayer(new PlayerStats.HitInfo(1f, "WORLD", "POCKET", base.GetComponent<QueryProcessor>().PlayerId), gameObject);
                            //gameObject.GetComponent<PlayerStats>().HurtPlayer(new PlayerStats.HitInfo(1f, gameObject.GetComponent<NicknameSync>().myNick + " (" + gameObject.GetComponent<CharacterClassManager>().SteamId + ")", DamageTypes.Scp106, gameObject.GetComponent<RemoteAdmin.QueryProcessor>().PlayerId), hit.transform.gameObject);

                            bool was = false;
                            Scp939PlayerScript dog = lp.GetComponent<Scp939PlayerScript>();
                            if (dog && dog.enabled) was = true;
                            if (!dog) dog = lp.AddComponent<Scp939PlayerScript>();
                            if (!dog.enabled) dog.enabled = true;
                            dog.CallCmdShoot(hit.transform.gameObject);
                            if (!was) dog.enabled = false;

                            lp.GetComponent<Scp173PlayerScript>().CallRpcSyncAudio();
                            Hitmarker.Hit(1.5f);
                        } else
                        {
                            /*ccm.CallRpcPlaceBlood(lp.transform.position, 1, 2f);

                            lp.GetComponent<FallDamage>().CallRpcDoSound(lp.transform.position, 50);
                            ccm.CallRpcPlaceBlood(lp.transform.position, 0, UnityEngine.Mathf.Clamp(50 / 30f, 0.8f, 2f));*/
                            lp.GetComponent<FootstepSync>().CallCmdSyncFoot(true);
                        }
                    }
                }
                // hack movement
                /*PlyMovementSync move = lp.GetComponent<PlyMovementSync>();
                move.isGrounded = true;
                float rot = lp.transform.rotation.eulerAngles.y;
                Vector3 pos = lp.transform.position;
                move.CallCmdSyncData(rot, pos, lp.GetComponent<PlayerInteract>().playerCamera.transform.localRotation.eulerAngles.x);*/

                // hack radio
                // Radio radio = lp.GetComponent<Radio>();

                // activate wallhack
                //Camera wallhackCam = lp.GetComponent<Scp049PlayerScript>().plyCam.GetComponent<UnityEngine.Camera>();
                // Scp939PlayerScript dog = lp.GetComponent<Scp939PlayerScript>();
                CheckpointKiller shit = lp.GetComponent<CheckpointKiller>();
                if (shit.enabled) shit.enabled = false;
                Camera.main.renderingPath = UnityEngine.RenderingPath.VertexLit;// : UnityEngine.RenderingPath.VertexLit);
                Camera.main.cullingMask = Scp939PlayerScript.instances[0].scpVision;//this.normalVision : this.scpVision);
                Recoil recoil = lp.GetComponent<Recoil>();
                if (recoil.enabled) recoil.enabled = false;
                //dog.visionCamera.gameObject.SetActive(this.iAm939);
                //dog.visionCamera.fieldOfView = this.plyCam.fieldOfView;
                //dog.visionCamera.farClipPlane = this.plyCam.farClipPlane;
            }
        }

        private void OnGUI()
        {
            GUI.Label(new UnityEngine.Rect(10, 30, 500, 30), "Everfree v0.1");
            GUI.Label(new UnityEngine.Rect(10, 50, 500, 30), "Debug Trace (ALT): " + (isTrace ? "ON" : "OFF"));
            GUI.Label(new UnityEngine.Rect(10, 70, 500, 30), "Noclip (MOUSE3): " + (isNoclip ? "ON" : "OFF"));
            GUI.Label(new UnityEngine.Rect(10, 90, 500, 30), " > NoclipMode [" + (isBlink ? "blink" : "move") + "] (End)");
            GUI.Label(new UnityEngine.Rect(10, 110, 500, 30), "+Run Speed: " + (isSpeedhack ? "increased" : "off"));

            GameObject lp = PlayerManager.localPlayer;// FindLocalPlayer();
            PlyMovementSync move = lp.GetComponent<PlyMovementSync>();
            move.isGrounded = true;
            //move.CallCmdSyncData(rot, pos, lp.GetComponent<PlayerInteract>().playerCamera.transform.localRotation.eulerAngles.x);

            GUI.Label(new UnityEngine.Rect(10, 130, 500, 30), " > eulerAngles.x: " + lp.GetComponent<PlayerInteract>().playerCamera.transform.localRotation.eulerAngles.x);
            GUI.Label(new UnityEngine.Rect(10, 150, 500, 30), " > eulerAngles.y: " + lp.transform.rotation.eulerAngles.y.ToString());
            GUI.Label(new UnityEngine.Rect(10, 170, 500, 30), " > spinbot (Del): " + (isSpin ? "ON" : "OFF"));
            GUI.Label(new UnityEngine.Rect(10, 190, 500, 30), "PocketAttack (G)");

            if (isTrace)
            { 
                RaycastHit hit;
                if (Physics.Raycast(new Ray(Camera.main.transform.position, Camera.main.transform.forward), out hit))
                {
                    GUI.Label(new UnityEngine.Rect(Screen.width / 2, Screen.height / 2, 500, 150), "Tag:" + hit.transform.gameObject.tag + " and name is: " + hit.transform.gameObject.name);
                    Transform firstParent = hit.transform.gameObject.transform.parent;
                    if (firstParent) GUI.Label(new UnityEngine.Rect(Screen.width / 2, Screen.height / 2 + 50, 500, 150), "parent tag & name & type: " + firstParent.parent.tag + " & " + firstParent.name + " & " + firstParent.GetType().ToString());
                }
            }

            if (isNoclip)
            {
                if (isBlink)
                {
                    //lp.transform.position += 2.5f * (Camera.main.transform.forward * (Input.GetAxis("Vertical") == 0 ? 1.0f : Input.GetAxis("Vertical")) + Camera.main.transform.right * Input.GetAxis("Horizontal"));
                    //blinkCd = 3000;
                    Vector3 myPos = lp.transform.position;
                    myPos += 10.0f * Camera.main.transform.forward * (Input.GetAxis("Vertical") == 0 ? 1.0f : Input.GetAxis("Vertical")) + Camera.main.transform.right * Input.GetAxis("Horizontal");
                    move.CallCmdSyncData(y, myPos, x);
                }
                else
                {
                    lp.transform.position += 0.5f * Camera.main.transform.forward * (Input.GetAxis("Vertical") == 0 ? 1.0f : Input.GetAxis("Vertical")) + Camera.main.transform.right * Input.GetAxis("Horizontal");
                }
            }
            if (isSpin)
            {
                move.CallCmdSyncData(y, lp.transform.position, x);
                x += 3.0f;
                y += 3.0f;
                if (x > 360.0f) x = -360.0f;
                if (y > 360.0f) y = -360.0f;
            }

            Camera main = Camera.main;
            GameObject gameObject = PlayerManager.localPlayer;// FindLocalPlayer();
            int curClass = gameObject.GetComponent<CharacterClassManager>().curClass;
            foreach (GameObject gameObject2 in GameObject.FindGameObjectsWithTag("Player"))
            {
                if (gameObject2.GetComponent<NetworkIdentity>())
                {
                    NicknameSync component = gameObject2.transform.GetComponent<NicknameSync>();
                    CharacterClassManager component2 = component.GetComponent<CharacterClassManager>();
                    if (component != null)
                    {
                        Vector3 b = gameObject2.GetComponent<NetworkIdentity>().transform.position;
                        int curClass2 = component2.curClass;
                        //if (curClass2 >= 0 && (gameObject != gameObject2))
                        if (curClass2 > -1)
                        {
                            int num = (int)Vector3.Distance(main.transform.position, b);
                            Vector3 vector;
                            vector.x = main.WorldToScreenPoint(b).x;
                            vector.y = main.WorldToScreenPoint(b).y;
                            vector.z = main.WorldToScreenPoint(b).z;
                            GUI.color = getColorById(curClass2);
                            string teamNameById = GetTeamNameById(curClass2);
                            if (main.WorldToScreenPoint(b).z > 0f)
                            {
                                GUI.Label(new Rect(vector.x - 50f, (float)Screen.height - vector.y, 100f, 50f), teamNameById + " [" + num.ToString() + "]");
                                GUI.color = ((getTeamById(curClass) != getTeamById(curClass2)) ? Color.red : Color.green);
                                GUI.Label(new Rect(vector.x - 50f, (float)Screen.height - vector.y - 60f, 100f, 50f), component.myNick + "[" + gameObject2.GetComponent<PlayerStats>().health.ToString() + " HP]");
                            }
                        }
                    }
                }
            }

            // Tag: Door, name: Door 104
            foreach (GameObject gameObject2 in GameObject.FindGameObjectsWithTag("Door"))
            {
                if (gameObject2.name.Contains("Door 104"))
                {
                    GUI.color = Color.white;
                    Vector3 b = gameObject2.transform.position;
                    Vector3 vector;
                    vector.x = main.WorldToScreenPoint(b).x;
                    vector.y = main.WorldToScreenPoint(b).y;
                    vector.z = main.WorldToScreenPoint(b).z;
                    if (main.WorldToScreenPoint(b).z > 0f) GUI.Label(new Rect(vector.x + 50f, (float)Screen.height - vector.y, 100f, 50f), gameObject2.name + " " + Vector3.Distance(Camera.main.transform.position, gameObject2.transform.position));
                }
            }

            // Tag: Pickup

            // name contains: Teleport (1) / Teleport (2)!!! \ etc

        }

        /*private GameObject FindLocalPlayer()
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
        }*/

        private Color getColorById(int curTeam)
        {
            if (curTeam == 0 || curTeam == 3 || curTeam == 5 || curTeam == 10 || curTeam == 9)
            {
                return Color.red;
            }
            if (curTeam == 1)
            {
                return new Color(1f, 0.7f, 0f, 1f);
            }
            if (curTeam == 8)
            {
                return Color.green;
            }
            if (curTeam == 6)
            {
                return Color.white;
            }
            if (curTeam == 4 || curTeam == 11 || curTeam == 12 || curTeam == 13)
            {
                return Color.blue;
            }
            return new Color(1f, 0.7f, 0.3f, 0.5f);
        }

        private int getTeamById(int iId)
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
            }
            return 2;
        }

        private string GetTeamNameById(int tid)
        {
            switch (tid)
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
            }
            return "undefined (#" + tid.ToString() + ")";
        }
    }
}
