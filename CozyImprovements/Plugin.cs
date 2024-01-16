﻿using BepInEx;
using HarmonyLib;
using UnityEngine;
using System.Collections.Generic;
using Unity.Netcode;
using System.Reflection;
using GameNetcodeStuff;

using System.Security.Permissions;
using System.ComponentModel;
using SpyciBot.LC.CozyImprovements.Improvements;
using System;

[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]

namespace SpyciBot.LC.CozyImprovements
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    [HarmonyPatch]
    public class CozyImprovements : BaseUnityPlugin
    {
        static new GameObject gameObject = null;
        static Terminal TermInst = null;

        public static Config CozyConfig { get; internal set; }
        private void Awake()
        {
            // Plugin startup logic
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_NAME} - {PluginInfo.PLUGIN_GUID} - {PluginInfo.PLUGIN_VERSION} is loaded!");
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
            CozyConfig = new(base.Config);
        }


        // 
        // Terminal Fixes
        //
        [HarmonyPatch(typeof(Terminal), "Start")]
        [HarmonyPostfix]
        static void Postfix_Terminal_Start(Terminal __instance)
        {
            TermInst = __instance;
            if (CozyImprovements.CozyConfig.configTerminalMonitorAlwaysOn.Value)
            {
                //  Make terminal display the Store list on startup
                TermInst.LoadNewNode(TermInst.terminalNodes.specialNodes[1]);
            }
            if (CozyImprovements.CozyConfig.configTerminalGlow.Value)
            {
                // Force terminal light to always be turned on/visible
                TermInst.terminalLight.enabled = true;
            }
        }

        [HarmonyPatch(typeof(Terminal), "waitUntilFrameEndToSetActive")]
        [HarmonyPrefix]
        static void Prefix_Terminal_WaitUntilFrameEndToSetActive(Terminal __instance, ref bool active)
        {
            TermInst = __instance;
            if (CozyImprovements.CozyConfig.configTerminalMonitorAlwaysOn.Value)
            {
                // Force terminal canvas to always be turned on/visible
                active = true;
            }
        }

        [HarmonyPatch(typeof(Terminal), "SetTerminalInUseClientRpc")]
        [HarmonyPostfix]
        static void Postfix_Terminal_SetTerminalInUseClientRpc(Terminal __instance, bool inUse)
        {
            TermInst = __instance;
            if (CozyImprovements.CozyConfig.configTerminalGlow.Value)
            {
                // Force terminal light to always be turned on/visible
                TermInst.terminalLight.enabled = true;
            }
        }


        //
        // Run on Client and Host
        //

        [HarmonyPatch(typeof(StartOfRound), "OnPlayerConnectedClientRpc")]
        [HarmonyPostfix]
        static private void PostfixOnPlayerConnectedClientRpc(StartOfRound __instance, ulong clientId, int connectedPlayers, ulong[] connectedPlayerIdsOrdered, int assignedPlayerObjectId, int serverMoneyAmount, int levelID, int profitQuota, int timeUntilDeadline, int quotaFulfilled, int randomSeed)
        {
            // This will trigger on every client every time a client joins, so only do stuff if it's the joining client
            if (clientId == NetworkManager.Singleton.LocalClientId)
            {
                DoAllTheThings();
            }
        }

        [HarmonyPatch(typeof(StartOfRound), "LoadUnlockables")]
        [HarmonyPostfix]
        static void PostfixLoadUnlockables(StartOfRound __instance)
        {
            // This will only trigger on the host
            DoAllTheThings();
        }


        //
        // All The Things™️
        //

        private static void DoAllTheThings()
        {
            try {
                manageInteractables();
            }
            catch (Exception ex){
                Debug.LogError($"Caught Exception in manageInteractables(): {ex}");
            }
            try {
                manageStorageCupboard();
            }
            catch (Exception ex)
            {
                Debug.LogError($"Caught Exception in manageStorageCupboard(): {ex}");
            }
        }

        private static void manageInteractables()
        {
            PlayerControllerB localPlayerController = GameNetworkManager.Instance.localPlayerController;
            GameObject[] array = GameObject.FindGameObjectsWithTag("InteractTrigger");
            for (int i = 0; i < array.Length; i++)
            {
                //Debug.Log($"{i} -- {array[i].name}");
                if (array[i].name == "LightSwitch")
                {
                    if (CozyImprovements.CozyConfig.configLightSwitchGlow.Value)
                    {
                        // Make the light switch panel glow green and make the switch glow red
                        makeEmissive(array[i], new Color32(182, 240, 150, 102), 0.02f);
                        makeEmissive(array[i].transform.GetChild(0).gameObject, new Color32(241, 80, 80, 10), 0.15f);
                    }
                }
                if (array[i].name == "Trigger" && array[i].transform.parent.gameObject.name == "ChargeStationTrigger")
                {
                    if (CozyImprovements.CozyConfig.configChargeStationGlow.Value)
                    {
                        GameObject ChargeStation = array[i].transform.parent.parent.gameObject;

                        // Add a yellow glow to the ChargeStation
                        GameObject lightObject = new GameObject("ChargeStationLight");
                        Light lightComponent = lightObject.AddComponent<Light>();
                        lightComponent.type = LightType.Point;
                        lightComponent.color = new Color32(240, 240, 140, 255);
                        lightComponent.intensity = 0.05f;
                        lightComponent.range = 0.3f;
                        //lightComponent.spotAngle = 179.0f;
                        lightComponent.shadows = LightShadows.Soft;

                        lightObject.layer = LayerMask.NameToLayer("Room");
                        lightObject.transform.localPosition = new Vector3(0.5f, 0.0f, 0.0f);
                        //lightObject.transform.rotation = Quaternion.Euler(0, 180, 0);
                        lightObject.transform.SetParent(ChargeStation.transform, false);
                    }
                }
                if (array[i].name == "Cube (2)" && array[i].transform.parent.gameObject.name.StartsWith("CameraMonitor"))
                {
                    if (CozyImprovements.CozyConfig.configBigMonitorButtons.Value)
                    {
                        Accessibility.adjustMonitorButtons(array[i].gameObject);
                    }
                }
            }
        }
        private static void makeEmissive(GameObject gameObject, Color32 glowColor, float brightness = 0.02f)
        {
            MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();
            Material EmMaterial = meshRenderer.material;

            EmMaterial.SetColor("_EmissiveColor", (Color) glowColor * brightness);
            meshRenderer.material = EmMaterial;
        }
        private static void manageStorageCupboard()
        {

            PlaceableShipObject[] array = GameObject.FindObjectsOfType<PlaceableShipObject>();
            for (int i = 0; i < array.Length; i++)
            {
                StartOfRound sorInst = StartOfRound.Instance;
                UnlockableItem unlockableItem = sorInst.unlockablesList.unlockables[array[i].unlockableID];
                int unlockableType = unlockableItem.unlockableType;
                if (unlockableItem.unlockableName == "Cupboard")
                {
                    /*
                    Debug.Log("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
                    Debug.Log("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
                    Debug.Log("~~~~~~~~~~~~~~~~~~~~    LoadUnlockables    ~~~~~~~~~~~~~~~~~~~~~");
                    Debug.Log(padString(unlockableItem.unlockableName, '~', 65));
                    Debug.Log(padString("" + unlockableType, '~', 65));
                    Debug.Log(padString("" + unlockableItem.spawnPrefab, '~', 65));
                    Debug.Log("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
                    Debug.Log("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
                    Debug.Log("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
                    */
                    gameObject = array[i].parentObject.gameObject;
                    break;
                }
            }
            if (gameObject == null)
            {
                return;
            }


            // Don't bother if the config option is disabled
            if (CozyImprovements.CozyConfig.configStorageLights.Value)
            {
                spawnStorageLights(gameObject);
            }
        }
        private static void spawnStorageLights(GameObject storageCloset)
        {
            /*
            MeshRenderer renderer = gameObject.GetComponent<MeshRenderer>();
            Vector3 size = renderer.bounds.size;
            Debug.Log("Renderer bounds: " + size);
            */
            float midPoint = -1.1175f;

            List<float> heightList = new List<float> { 2.804f, 2.163f, 1.48f, 0.999f };
            float lightOffset = 0.55f;

            // Top Shelf
            float shelfHeight = heightList[0];
            AttachLightToStorageCloset(storageCloset, new Vector3(midPoint - lightOffset, 0.4f, shelfHeight));
            AttachLightToStorageCloset(storageCloset, new Vector3(midPoint, 0.4f, shelfHeight));
            AttachLightToStorageCloset(storageCloset, new Vector3(midPoint + lightOffset, 0.4f, shelfHeight));
            // 2nd Shelf
            shelfHeight = heightList[1];
            AttachLightToStorageCloset(storageCloset, new Vector3(midPoint - lightOffset, 0.4f, shelfHeight));
            AttachLightToStorageCloset(storageCloset, new Vector3(midPoint, 0.4f, shelfHeight));
            AttachLightToStorageCloset(storageCloset, new Vector3(midPoint + lightOffset, 0.4f, shelfHeight));
            // 3rd Shelf
            shelfHeight = heightList[2];
            AttachLightToStorageCloset(storageCloset, new Vector3(midPoint - lightOffset, 0.4f, shelfHeight), 2.0f);
            AttachLightToStorageCloset(storageCloset, new Vector3(midPoint, 0.4f, shelfHeight), 2.0f);
            AttachLightToStorageCloset(storageCloset, new Vector3(midPoint + lightOffset, 0.4f, shelfHeight), 2.0f);
            // Bottom Shelf
            shelfHeight = heightList[3];
            AttachLightToStorageCloset(storageCloset, new Vector3(midPoint - lightOffset, 0.4f, shelfHeight));
            AttachLightToStorageCloset(storageCloset, new Vector3(midPoint, 0.4f, shelfHeight));
            AttachLightToStorageCloset(storageCloset, new Vector3(midPoint + lightOffset, 0.4f, shelfHeight));
        }
        private static void AttachLightToStorageCloset(GameObject storageCloset, Vector3 lightPositionOffset, float intensity = 3.0f)
        {
            // Create lightbulb object
            GameObject lightObject = new GameObject("StorageClosetLight");
            MeshFilter meshFilter = lightObject.AddComponent<MeshFilter>();
            MeshRenderer meshRenderer = lightObject.AddComponent<MeshRenderer>();

            GameObject tempSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            meshFilter.mesh = tempSphere.GetComponent<MeshFilter>().mesh;

            // Make lightbulb glow
            Material offWhiteMaterial = new Material(Shader.Find("HDRP/Lit"));
            Color lightSphereColor = new Color32(249, 240, 202, 255); // Off-white color
            offWhiteMaterial.SetColor("_BaseColor", lightSphereColor); // Set the base color (albedo)

            float emissiveIntensity = 1.0f;
            offWhiteMaterial.SetColor("_EmissiveColor", lightSphereColor * emissiveIntensity);
            meshRenderer.material = offWhiteMaterial;

            GameObject.DestroyImmediate(tempSphere);

            // Add light beam from lightbulb
            Light lightComponent = lightObject.AddComponent<Light>();
            lightComponent.type = LightType.Spot;
            lightComponent.color = lightSphereColor;
            lightComponent.intensity = intensity;
            lightComponent.range = 1.05f;
            lightComponent.spotAngle = 125.0f;
            lightComponent.shadows = LightShadows.Soft;

            lightObject.layer = LayerMask.NameToLayer("Room");
            lightObject.transform.localScale = new Vector3(0.125f, 0.125f, 0.04f);
            lightObject.transform.localPosition = lightPositionOffset;
            lightObject.transform.rotation = Quaternion.Euler(170, 0, 0);
            lightObject.transform.SetParent(storageCloset.transform, false);

        }


        //
        // Utilities
        //
        private static string padString(string baseStr, char padChar, int width)
        {
            int paddingWidth = width - (baseStr.Length + 8);
            int padLeft = paddingWidth / 2 + (baseStr.Length + 8);
            string paddedStr = ("    " + baseStr + "    ").PadLeft(padLeft, padChar).PadRight(width, padChar);
            return paddedStr;
        }
        private static void obviousDebug(string baseStr)
        {
            Debug.Log("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
            Debug.Log("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
            Debug.Log("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
            Debug.Log(padString("" + baseStr, '~', 65));
            Debug.Log("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
            Debug.Log("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
            Debug.Log("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
        }

    }
}
