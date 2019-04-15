using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cheat
{
    using UnityEngine;
    using UnityEngine.Networking;

    class ESP
    {
        static public bool _bShowDropEsp;
        static public bool _bShowLocations;

        static private Color COLOR_GRAY = new Color(1f, 0.7f, 0.3f, 0.5f);
        static private Color COLOR_DROP = new Color(1f, 1f, 0.7f, 0.5f);
        static private Color COLOR_WTF = new Color(1f, 0.7f, 0f, 1f);
        static private Color COLOR_LOCATION = new Color(1f, 0.65f, 1f, 0.6f);

        static public void Update()
        {
            if (Input.GetKeyDown(KeyCode.F4))
            {
                _bShowDropEsp = !_bShowDropEsp;
            }
            if (Input.GetKeyDown(KeyCode.F7))
            {
                _bShowLocations = !_bShowLocations;
            }
        }
        static public void Render(GameObject player, Camera main, CharacterClassManager ccm)
        {
            var curClass = player.GetComponent<CharacterClassManager>().curClass;
            foreach (var gameObject2 in GameObject.FindGameObjectsWithTag("Player"))
                if (gameObject2.GetComponent<NetworkIdentity>())
                {
                    var component = gameObject2.transform.GetComponent<NicknameSync>();
                    var component2 = component.GetComponent<CharacterClassManager>();
                    var b = gameObject2.GetComponent<NetworkIdentity>().transform.position;
                    var pos2d = main.WorldToScreenPoint(b);
                    if (!(pos2d.z > 0f)) continue;
                    var curClass2 = component2.curClass;
                    //if (curClass2 >= 0 && (gameObject != gameObject2))
                    if (curClass2 <= -1) continue;
                    var num = (int)Vector3.Distance(main.transform.position, b);
                    GUI.color = GetColorById(curClass2);
                    GUI.Label(new Rect(pos2d.x - 50f, Screen.height - pos2d.y, 100f, 50f),
                        ccm.klasy[curClass2].fullName + " [" + num + "]");
                    GUI.color = GetTeamById(curClass) != GetTeamById(curClass2) ? Color.red : Color.green;
                    GUI.Label(new Rect(pos2d.x - 50f, Screen.height - pos2d.y - 60f, 100f, 50f), component.myNick);
                    /*Vector3 vector;
                    vector.x = main.WorldToScreenPoint(b).x;
                    vector.y = main.WorldToScreenPoint(b).y;
                    vector.z = main.WorldToScreenPoint(b).z;
                    GUI.color = GetColorById(curClass2);
                    GUI.Label(new Rect(vector.x - 50f, Screen.height - vector.y, 100f, 50f),
                        ccm.klasy[curClass2].fullName + " [" + num + "]");
                    GUI.color = GetTeamById(curClass) != GetTeamById(curClass2) ? Color.red : Color.green;
                    GUI.Label(new Rect(vector.x - 50f, Screen.height - vector.y - 60f, 100f, 50f), component.myNick);*/
                }

            // Tag: Pickup
            if (_bShowDropEsp)
            {
                var inv = player.GetComponent<Inventory>();
                foreach (Pickup pickup in Object.FindObjectsOfType<Pickup>())
                {
                    if (Mathf.Abs(Camera.main.transform.position[2] - pickup.transform.position[2]) > 300f) continue;
                    var pos2d = main.WorldToScreenPoint(pickup.transform.position);
                    if (!(pos2d.z > 0f)) continue;
                    //if (Vector3.Angle(Camera.main.transform.forward, pickup.transform.position - Camera.main.transform.position) < 70f)
                    GUI.color = COLOR_DROP;
                    GUI.Label(new Rect(pos2d.x - 20f, Screen.height - pos2d.y - 20f, pos2d.x + 40f, Screen.height - pos2d.y + 50f), inv.availableItems[pickup.info.itemId].label + " at " + (int)Vector3.Distance(Camera.main.transform.position, pickup.transform.position) + "m");
                }
            }
            if (_bShowLocations)
            {
                GUI.color = COLOR_LOCATION;
                DisplayLocation("SCP_914", GameObject.FindGameObjectWithTag("914_use").transform.position);
                Lift[] array2 = Object.FindObjectsOfType<Lift>();
                for (int i = 0; i < array2.Length; i++)
                {
                    foreach (Lift.Elevator elevator in array2[i].elevators)
                    {
                        if (Mathf.Abs(Camera.main.transform.position[2] - elevator.door.transform.position[2]) > 300) continue;
                        if (Mathf.Abs(PlayerManager.localPlayer.transform.position.y - elevator.door.transform.position.y) < 150f)
                        {
                            DisplayLocation("Lift", elevator.door.transform.position);
                        }
                    }
                }
                foreach (TeslaGate teslaGate in Object.FindObjectsOfType<TeslaGate>())
                {
                    if (Mathf.Abs(Camera.main.transform.position[2] - teslaGate.transform.position[2]) > 300) continue;
                    DisplayLocation("Tesla", teslaGate.transform.position);
                }
                foreach (PocketDimensionTeleport pocketDimensionTeleport in Object.FindObjectsOfType<PocketDimensionTeleport>())
                {
                    if (Mathf.Abs(Camera.main.transform.position[2] - pocketDimensionTeleport.transform.position[2]) > 300) break;
                    if (pocketDimensionTeleport.GetTeleportType() == PocketDimensionTeleport.PDTeleportType.Exit)
                    {
                        DisplayLocation("Exit", pocketDimensionTeleport.transform.position);
                    }
                }
                foreach (Generator079 generator079 in Object.FindObjectsOfType<Generator079>())
                {
                    if (Mathf.Abs(Camera.main.transform.position[2] - generator079.transform.position[2]) > 300) break;
                   
                        DisplayLocation("Generator", generator079.transform.position);
                 
                }
            }
        }

        static private void DisplayLocation(string name, Vector3 thePosition)
        {
            var pos2d = Camera.main.WorldToScreenPoint(thePosition);
            if (!(pos2d.z > 0f)) return;
            //if (Vector3.Angle(Camera.main.transform.forward, thePosition - Camera.main.transform.position) < 70f)
            //{
                //Vector3 vector = Camera.main.WorldToScreenPoint(thePosition);
                /*GUI.Label(new Rect(vector.x - 20f, (float)Screen.height - vector.y - 20f, vector.x + 40f, (float)Screen.height - vector.y + 50f), string.Concat(new object[]
                {
                name,
                " [",
                (int)Vector3.Distance(Camera.main.transform.position, thePosition),
                "m]"
                }));*/
                GUI.Label(new Rect(pos2d.x - 20f, Screen.height - pos2d.y - 20f, pos2d.x + 40f, Screen.height - pos2d.y + 50f), name + " [" + (int)Vector3.Distance(Camera.main.transform.position, thePosition) + "m]");
            //}
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
                case 16:
                case 17:
                case 7:
                    return Color.red;
                case 1:
                    return COLOR_WTF;
                case 8:
                    return Color.green;
                case 6:
                    return Color.white;
                case 4:
                case 11:
                case 12:
                case 13:
                    return Color.blue;
                default: return COLOR_GRAY;
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
                    return 1;
                default: return 2;
            }
        }
    }
}
