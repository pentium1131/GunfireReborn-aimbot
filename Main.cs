using MelonLoader;
using UnityEngine;
using UnityEngine.UI;
using UnhollowerRuntimeLib;
using Il2CppSystem.Collections.Generic;
using System;
using System.IO;
using HeroCameraName;
using Item;
using BulletChange;
using OCServerMoveNS;

namespace zcMod
{
    public static class BuildInfo
    {
        public const string Name = "zcMod"; // Name of the Mod.  (MUST BE SET)
        public const string Description = "Gunfire Reborn Aimbot"; // Description for the Mod.  (Set as null if none)
        public const string Author = "zhuchong"; // Author of the Mod.  (Set as null if none)
        public const string Company = null; // Company that made the Mod.  (Set as null if none)
        public const string Version = "1.0.0"; // Version of the Mod.  (MUST BE SET)
        public const string DownloadLink = null; // Download Link for the Mod.  (Set as null if none)
    }

    public class zcMod : MelonMod
    {
        public static bool showobjinfo = false;
        public static bool showallobjinfo = false;
        public static bool limitangle = true;
        public override void OnApplicationStart() // Runs after Game Initialization.
        {
            MelonLogger.Log("OnApplicationStart");
        }

        public override void OnLevelIsLoading() // Runs when a Scene is Loading or when a Loading Screen is Shown. Currently only runs if the Mod is used in BONEWORKS.
        {
            MelonLogger.Log("OnLevelIsLoading");
        }

        public override void OnLevelWasLoaded(int level) // Runs when a Scene has Loaded.
        {
            MelonLogger.Log("OnLevelWasLoaded: " + level.ToString());
        }

        public override void OnLevelWasInitialized(int level) // Runs when a Scene has Initialized.
        {
            MelonLogger.Log("OnLevelWasInitialized: " + level.ToString());
        }

        public override void OnUpdate() // Runs once per frame.
        {
            try
            {
                if (HeroCameraManager.HeroObj != null && HeroCameraManager.HeroObj.BulletPreFormCom != null && HeroCameraManager.HeroObj.BulletPreFormCom.weapondict != null) 
                {
                    foreach (KeyValuePair<int, WeaponPerformanceObj> Weapon in HeroCameraManager.HeroObj.BulletPreFormCom.weapondict)
                    {
                        if (!limitangle)
                        {
                            Weapon.value.ModifyBulletInMagzine(100, 100);//无限子弹
                            Weapon.value.WeaponAttr.Radius = 500f;//爆炸范围
                        }
                        
                        Weapon.value.WeaponAttr.Accuracy = 10000;
                        Weapon.value.WeaponAttr.AttDis = 500f;
                        Weapon.value.WeaponAttr.Pierce = 99;
                        if (Weapon.value.WeaponAttr.BulletSpeed >= 50f && Weapon.value.WeaponAttr.BulletSpeed != 55f)
                        {
                            Weapon.value.WeaponAttr.BulletSpeed = 500f;
                        }
                        else if (Weapon.value.WeaponAttr.BulletSpeed == 30f) 
                        {
                            Weapon.value.WeaponAttr.BulletSpeed = 500f;
                        }
                        Weapon.value.WeaponAttr.Stability = 10000;
                        if (Weapon.value.WeaponAttr.Radius > 0f)
                        {
                            Weapon.value.WeaponAttr.Radius = 9f;//爆炸范围
                        }
                    }
                }
                if (Input.GetKeyUp(KeyCode.Insert))//暴力模式
                {
                    limitangle = !limitangle;
                }
                if (Input.GetKey(KeyCode.F))//按F键自瞄
                {
                    List<NewPlayerObject> monsters = NewPlayerManager.GetMonsters();
                    if (monsters != null)
                    {
                        
                        Vector3 campos = CameraManager.MainCamera.position;

                        Transform nearmons = null;
                        float neardis = 99999f;
                        foreach (NewPlayerObject monster in monsters)
                        {
                            Transform montrans = monster.BodyPartCom.GetWeakTrans();
                            if (montrans == null)
                            {
                                continue;
                            }
                            Vector3 vec = CameraManager.MainCameraCom.WorldToViewportPoint(montrans.position);
                            if (limitangle)
                            {
                                if (vec.z <= 0) continue;
                                vec.y = 0;
                                vec.x = 0.5f - vec.x;
                                vec.x = Screen.width * vec.x;
                                vec.z = 0f;
                                if (vec.magnitude > 150f) continue;//自瞄范围
                            }
                            vec = montrans.position - campos;
                            float curdis = vec.magnitude;
                            Ray ray = new Ray(campos, vec);
                            var hits = Physics.RaycastAll(ray, curdis);
                            bool visible = true;
                            foreach(var hit in hits)
                            {
                                if (showallobjinfo)
                                {
                                    MelonLogger.Log(hit.collider.name);
                                    MelonLogger.Log(hit.collider.gameObject.layer.ToString());
                                }
                                
                                if (hit.collider.gameObject.layer == 0 || hit.collider.gameObject.layer == 30 || hit.collider.gameObject.layer == 31) //&& hit.collider.name.Contains("_")
                                {

                                    if (showobjinfo)
                                    {
                                        MelonLogger.Log(hit.collider.name);
                                        MelonLogger.Log(hit.collider.gameObject.layer.ToString());
                                    }

                                    visible = false;
                                    break;
                                }
                            }
                            if (visible)
                            {
                                
                                if (curdis < neardis)
                                {
                                    neardis = curdis;
                                    nearmons = montrans;
                                }
                            }


                        }
                        if (nearmons != null)
                        {
                            Vector3 objpos = new Vector3();
                            objpos.x = HeroCameraManager.HeroObj.gameTrans.position.x;
                            objpos.y = HeroCameraManager.HeroObj.gameTrans.position.y + 1f;
                            objpos.z = HeroCameraManager.HeroObj.gameTrans.position.z;
                            Vector3 fw = nearmons.position - objpos;
                            fw.y += 0.12f;
                            Quaternion rot = Quaternion.LookRotation(fw);
                            HeroCameraManager.HeroObj.gameTrans.rotation = rot;
                            fw = nearmons.position - campos;
                            fw.y += 0.12f;
                            Quaternion rot2 = Quaternion.LookRotation(fw);
                            CameraManager.MainCamera.rotation = rot2;

                            //AutoAimat.Rotation(nearmons, 500f);

                        }
                    }
                }

            }
            catch
            {

            }
        }

        public override void OnFixedUpdate() // Can run multiple times per frame. Mostly used for Physics.
        {
        }

        public override void OnLateUpdate() // Runs once per frame after OnUpdate and OnFixedUpdate have finished.
        {
        }

        public override void OnGUI() // Can run multiple times per frame. Mostly used for Unity's IMGUI.
        {
            try
            {
                Dictionary<int, NewPlayerObject> hidedoors = NewPlayerManager.PlayerDict;
                if (hidedoors != null)
                {
                    foreach (KeyValuePair<int, NewPlayerObject> door in hidedoors)
                    {
                        Vector3 vec = CameraManager.MainCameraCom.WorldToViewportPoint(door.value.gameTrans.position);
                        if (vec.z > 0f)
                        {
                            vec.y = 1f - vec.y;
                            float scrx = Screen.width * vec.x;
                            float scry = Screen.height * vec.y;
                            Rect rect = new Rect(scrx - 25f, scry - 10f, 50f, 20f);
                            
                            if(door.value.FightType== ServerDefine.FightType.WARRIOR_OBSTACLE_NORMAL)
                            {
                                if (door.value.ComLst.Count > 0)
                                {
                                    if (door.value.ComLst[0].luaparent.modelData.Name == "隐藏门")
                                    {
                                        Vector3 dis = door.value.gameTrans.position - CameraManager.MainCamera.position;
                                        GUI.Label(rect, dis.magnitude.ToString("f1"));
                                    }
                                }
                            }
                        }

                    }
                }
                return;
            }
            catch
            {
                MelonLogger.Log("OnGUIbug");
            }
        }

        public override void OnApplicationQuit() // Runs when the Game is told to Close.
        {
            MelonLogger.Log("OnApplicationQuit");
        }

        public override void OnModSettingsApplied() // Runs when Mod Preferences get saved to UserData/modprefs.ini.
        {
            MelonLogger.Log("OnModSettingsApplied");
        }

        public override void VRChat_OnUiManagerInit() // Runs upon VRChat's UiManager Initialization. Only runs if the Mod is used in VRChat.
        {
            MelonLogger.Log("VRChat_OnUiManagerInit");
        }
    }
}