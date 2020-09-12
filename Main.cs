﻿using MelonLoader;
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
using DataHelper;
using TMPro;

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
        public static bool shownpc = false;
        public static bool imreload = false;
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
                            Weapon.value.ModifyBulletInMagzine(100, 100);//无限子弹，不用换弹
                            if (Weapon.value.WeaponAttr.Radius != 500f)
                                Weapon.value.WeaponAttr.Radius = 500f;//爆炸范围
                        }
                        if (imreload)
                        {
                            Weapon.value.ReloadBulletImmediately(); //消耗子弹，光速换弹
                        }

                        if (Weapon.value.WeaponAttr.Accuracy != 10000) 
                            Weapon.value.WeaponAttr.Accuracy = 10000;//子弹不扩散
                        if (Weapon.value.WeaponAttr.AttDis != 500f)
                            Weapon.value.WeaponAttr.AttDis = 500f;//射程
                        //Weapon.value.WeaponAttr.Pierce = 99; //穿透力，改了好像无效果?

                        if (Weapon.value.WeaponAttr.BulletSpeed >= 50f && Weapon.value.WeaponAttr.BulletSpeed != 55f)//弹道速度,某些武器改了会打不到怪
                        {
                            if (Weapon.value.WeaponAttr.BulletSpeed != 500f)
                                Weapon.value.WeaponAttr.BulletSpeed = 500f;
                        }
                        else if (Weapon.value.WeaponAttr.BulletSpeed == 30f) 
                        {
                            if (Weapon.value.WeaponAttr.BulletSpeed != 500f)
                                Weapon.value.WeaponAttr.BulletSpeed = 500f;
                        }
                        if (Weapon.value.WeaponAttr.Stability != 10000)
                            Weapon.value.WeaponAttr.Stability = 10000;//后坐力
                        if (Weapon.value.WeaponAttr.Radius > 0f)
                        {
                            if (Weapon.value.WeaponAttr.Radius < 9f)
                                Weapon.value.WeaponAttr.Radius = 9f;//爆炸范围(会影响爆炸类武器、火标和电手套)
                        }
                    }
                }
                if (Input.GetKeyUp(KeyCode.Home))//暴力模式全图锁
                {
                    shownpc = !shownpc;
                }
                if (Input.GetKeyUp(KeyCode.Delete))
                {
                    //if (WarShopManager.ShopItemDict.count > 0) 
                    //{
                    //    bool first = true;
                    //    WarShopObject fobj = null;
                    //    foreach (KeyValuePair<int, WarShopObject> shopitem in WarShopManager.ShopItemDict)
                    //    {
                    //        if (shopitem.value.PropertyDict.ContainsKey("Inscription"))
                    //        {
                    //            List<int> Inscription = new List<int>();
                    //            Inscription.Add(4820); Inscription.Add(4821); Inscription.Add(4822); Inscription.Add(4823); Inscription.Add(4824); Inscription.Add(4825); Inscription.Add(4827);
                    //            shopitem.value.PropertyDict["Inscription"] = Inscription;
                    //        }
                    //
                    //    }
                    //}
                }
                if (Input.GetKeyUp(KeyCode.V))
                {
                    imreload = !imreload;
                }
                //War_Shop_Event
                //InitWarShop


                if (Input.GetKeyUp(KeyCode.Insert))//暴力模式全图锁
                {
                    limitangle = !limitangle;
                }
                
                //if (Input.GetMouseButton(1)) 
                if (Input.GetKey(KeyCode.F))//按F键自瞄(请按个人喜好修改)
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

                                    visible = false;//判断怪物是否可见
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
                if (shownpc)
                {
                    foreach (var obj in NewPlayerManager.PlayerDict)
                    {
                        var val = obj.Value;
                        if (val.centerPointTrans == null) continue;
                        if (!ShowObject(val)) continue;
                        var screenPos = CameraManager.MainCameraCom.WorldToScreenPoint(val.centerPointTrans.transform.position);
                        if (screenPos.z > 0)
                        {
                            var dist = Vector3.Distance(HeroMoveManager.HeroObj.centerPointTrans.position, val.centerPointTrans.position).ToString("0.0");
                            GUI.Label(new Rect(screenPos.x, Screen.height - screenPos.y, 800, 50), FightTypeToString(val) + "(" + dist + "m)");
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

        public bool ShowObject(NewPlayerObject obj)
        {
            if (obj.FightType == ServerDefine.FightType.NWARRIOR_DROP_EQUIP)
                return true;
            else if (obj.FightType == ServerDefine.FightType.NWARRIOR_DROP_RELIC)
                return true;
            else if (obj.FightType == ServerDefine.FightType.NWARRIOR_NPC_SMITH)
                return true;
            else if (obj.FightType == ServerDefine.FightType.NWARRIOR_NPC_SHOP)
                return true;
            else if (obj.FightType == ServerDefine.FightType.WARRIOR_OBSTACLE_NORMAL && (obj.Shape == 4406 || obj.Shape == 4419 || obj.Shape == 4427))
                return true;
            else if (obj.FightType == ServerDefine.FightType.NWARRIOR_NPC_EVENT)
                return true;
            else if (obj.FightType == ServerDefine.FightType.NWARRIOR_NPC_ITEMBOX)
                return true;
            return false;

        }
        public String FightTypeToString(NewPlayerObject obj)
        {
            if (obj.FightType == ServerDefine.FightType.NWARRIOR_DROP_EQUIP)
                return DataMgr.GetWeaponData(obj.Shape).Name + " +" + obj.DropOPCom.WeaponInfo.SIProp.Grade.ToString();
            else if (obj.FightType == ServerDefine.FightType.NWARRIOR_DROP_RELIC)
                return DataMgr.GetRelicData(obj.DropOPCom.RelicSid).Name;
            else if (obj.FightType == ServerDefine.FightType.NWARRIOR_NPC_SMITH)
                return "工匠";
            else if (obj.FightType == ServerDefine.FightType.NWARRIOR_NPC_SHOP)
                return "商人";
            else if (obj.FightType == ServerDefine.FightType.WARRIOR_OBSTACLE_NORMAL && (obj.Shape == 4406 || obj.Shape == 4419 || obj.Shape == 4427))
                return "秘境";
            else if (obj.FightType == ServerDefine.FightType.NWARRIOR_NPC_EVENT)
                return "宝箱";
            else if (obj.FightType == ServerDefine.FightType.NWARRIOR_NPC_ITEMBOX)
                return "宝箱";
            return "unk";
        }


    }
}
