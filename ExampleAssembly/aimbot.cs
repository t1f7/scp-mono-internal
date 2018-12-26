using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace Cheat
{
    class Aimbot
    {
        static public string targetNick = "no target";
        static private UnityEngine.GameObject target;
        static private bool _isNDown;
        static private bool _isRage;

        static public void Update(UnityEngine.GameObject localPlayer)
        {
            if (UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.T))
            {
                if (!_isNDown)
                {
                    _isNDown = true;
                    _isRage = true;
                }
            }
            else if (_isNDown && UnityEngine.Input.GetKeyUp(UnityEngine.KeyCode.T))
            {
                _isNDown = false;
                _isRage = false;
            }

            if (UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.C))
            {
                SelectTarget();
            }
            if (UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.F))
            {
                AimAt(target, localPlayer);
            }
            if (_isRage)
            {
                SelectTarget();
                AimAt(target, localPlayer);
            }
        }

        static private void AimAt(UnityEngine.GameObject target, UnityEngine.GameObject lp)
        {
            var ccm = target.GetComponent<CharacterClassManager>();
            if (ccm && ccm.curClass != 2)
            {
                var weapon = lp.GetComponent<WeaponManager>();
                UnityEngine.Vector3 localEulerAngles2 = UnityEngine.Camera.main.transform.localEulerAngles;
                UnityEngine.Vector3 localPosition = UnityEngine.Camera.main.transform.localPosition;
                var at = target.transform.position;
                UnityEngine.Camera.main.transform.LookAt(at);
                UnityEngine.RaycastHit raycastHit;
                UnityEngine.Physics.Raycast(new UnityEngine.Ray(UnityEngine.Camera.main.transform.position, UnityEngine.Camera.main.transform.forward), out raycastHit, 10000f, weapon.raycastMask);
                if (raycastHit.collider != null)
                {
                    HitboxIdentity hitboxIdentity = raycastHit.collider.GetComponent<HitboxIdentity>();
                    if (hitboxIdentity == null)
                    {
                        hitboxIdentity = raycastHit.collider.gameObject.AddComponent<HitboxIdentity>();
                    }
                    hitboxIdentity.id = "HEAD";
                }

                try
                {
                    weapon.GetType().GetMethod("Shoot", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(weapon, new object[0]);
                }
                catch
                {
                }
                UnityEngine.Camera.main.transform.localPosition = localPosition;
                UnityEngine.Camera.main.transform.localEulerAngles = localEulerAngles2;
            }
        }

        static private void SelectTarget()
        {
            float num = 15f;
            UnityEngine.GameObject y = null;
            foreach (UnityEngine.GameObject gameObject in PlayerManager.singleton.players)
            {
                UnityEngine.Vector3 position = gameObject.transform.position;
                if (CheckValidTarget(gameObject) && UnityEngine.Vector3.Angle(UnityEngine.Camera.main.transform.forward, position - UnityEngine.Camera.main.transform.position) < num)
                {
                    y = gameObject;
                    num = UnityEngine.Vector3.Angle(UnityEngine.Camera.main.transform.forward, position - UnityEngine.Camera.main.transform.position);
                }
            }
            if (y == null)
            {
                target = null;
                targetNick = "no target";
                return;
            }
            target = y;

            if (target.GetComponent<UnityEngine.Networking.NetworkIdentity>())
            {
                var component = target.transform.GetComponent<NicknameSync>();
                targetNick = component.myNick;
            }
        }

        static private bool CheckValidTarget(UnityEngine.GameObject player)
        {
            return player != PlayerManager.localPlayer;
        }
    }
}
