using HarmonyLib;
using UnityEngine;

namespace SpyciBot.LC.CozyImprovements.Improvements
{
    [HarmonyPatch]
    public static class Accessibility
    {
        static HangarShipDoor hangarShipDoor = null;

        // 
        // Launch Lever Fixes
        // - Make hitbox of launch lever huge so it's easy to pull
        //
        [HarmonyPatch(typeof(StartMatchLever), "Start")]
        [HarmonyPostfix]
        static void Postfix_StartMatchLever_Start(StartMatchLever __instance)
        {
            // Don't bother if the config option is disabled
            if (!CozyImprovements.CozyConfig.configEasyLaunchLever.Value)
                return;

            // Make the lever wide and flat, making it easy to press anywhere on the main section of the desk
            __instance.transform.localScale = new Vector3(1.139f, 0.339f, 1.539f);
            __instance.transform.localPosition = new Vector3(8.7938f, 1.479f, -7.0767f);

            // reset the playerPos so the lever pull animation is normal
            __instance.transform.GetChild(0).position = new Vector3(8.8353f, 0.2931f, -14.5767f);
        }


        // 
        // Hangar Door Button Panel Fixes
        // - This makes the panel much bigger and makes it easy to press button to open/close the doors
        //

        [HarmonyPatch(typeof(HangarShipDoor), "Start")]
        [HarmonyPostfix]
        static void Postfix_HangarShipDoor_Start(HangarShipDoor __instance)
        {
            // Don't bother if the config option is disabled
            if (!CozyImprovements.CozyConfig.configBigDoorButtons.Value)
                return;

            hangarShipDoor = __instance;
            GameObject ButtonPanel = __instance.hydraulicsDisplay.transform.parent.gameObject;

            // Make the whole panel bigger and centered on the 2 beams
            ButtonPanel.transform.localScale = new Vector3(-2f, -2f, -2f);
            ButtonPanel.transform.localPosition = new Vector3(-5.2085f, 1.8882f, -8.823f);

            // Adjust the size of the Start Button collider to match the Stop button
            GameObject StartButton = ButtonPanel.transform.Find("StartButton").gameObject;
            GameObject StopButton = ButtonPanel.transform.Find("StopButton").gameObject;

            StopButton.transform.localScale = new Vector3(-1.1986f, -0.1986f, -1.1986f);
            StartButton.transform.localScale = StopButton.transform.localScale;

            StartButton.transform.GetChild(0).localPosition = StopButton.transform.GetChild(0).localPosition;
            StartButton.transform.GetChild(0).localScale = StopButton.transform.GetChild(0).localScale;


            // Fix Emissives of buttons
            Material[] StartButtonMats = StartButton.GetComponent<MeshRenderer>().materials;
            for (int i = 0; i < StartButtonMats.Length; i++)
            {
                if (StartButtonMats[i].name == "GreenButton (Instance)")
                {
                    StartButtonMats[i].SetColor("_EmissiveColor", new Color32(39, 51, 39, 255));
                }
                if (StartButtonMats[i].name == "ButtonWhite (Instance)")
                {
                    StartButtonMats[i].SetColor("_EmissiveColor", new Color32(179, 179, 179, 255));
                }
            }
            StartButton.GetComponent<MeshRenderer>().materials = StartButtonMats;

            Material[] StopButtonMats = StopButton.GetComponent<MeshRenderer>().materials;
            for (int i = 0; i < StopButtonMats.Length; i++)
            {
                if (StopButtonMats[i].name == "RedButton (Instance)")
                {
                    StopButtonMats[i].SetColor("_EmissiveColor", new Color32(64, 24, 24, 255));
                }
                if (StopButtonMats[i].name == "ButtonWhite (Instance)")
                {
                    StopButtonMats[i].SetColor("_EmissiveColor", new Color32(179, 179, 179, 255));
                }
            }
            StopButton.GetComponent<MeshRenderer>().materials = StopButtonMats;


            // Make Buttons Interact Area Bigger

            Transform StartButtonInteract = ButtonPanel.transform.Find("StartButton").GetChild(0);
            Transform StopButtonInteract = ButtonPanel.transform.Find("StopButton").GetChild(0);


            //StartButtonInteract.GetComponent<MeshRenderer>().enabled = true;
            StartButtonInteract.GetComponent<MeshRenderer>().material.color = new Color32(39, 255, 39, 255);

            //StopButtonInteract.GetComponent<MeshRenderer>().enabled = true;
            StopButtonInteract.GetComponent<MeshRenderer>().material.color = new Color32(255, 24, 24, 255);


            Vector3 PressablePosition = new Vector3(-3.7205f, 2.0504f, -16.3018f);
            Vector3 PressableScale = new Vector3(0.7393f, 0.4526f, 0.6202f);
            Vector3 notPressableScale = new Vector3(0.003493f, 0.000526f, 0.002202f);

            StartButtonInteract.position = PressablePosition;
            StartButtonInteract.localScale = PressableScale;
            StopButtonInteract.position = PressablePosition;
            StopButtonInteract.localScale = notPressableScale;


        }

        //
        // Hangar Door Button Panel Fixes
        // - Toggle which button is usable depending on if the door is open or not
        //
        [HarmonyPatch(typeof(StartOfRound), "SetShipDoorsClosed")]
        [HarmonyPostfix]
        static void Postfix_StartOfRound_SetShipDoorsClosed(StartOfRound __instance, bool closed)
        {
            // Don't bother if the config option is disabled
            if (!CozyImprovements.CozyConfig.configBigDoorButtons.Value)
                return;

            GameObject ButtonPanel = hangarShipDoor.hydraulicsDisplay.transform.parent.gameObject;
            Transform StartButtonInteract = ButtonPanel.transform.Find("StartButton").GetChild(0);
            Transform StopButtonInteract = ButtonPanel.transform.Find("StopButton").GetChild(0);

            Vector3 PressableScale = new Vector3(0.7393f, 0.4526f, 0.6202f);
            Vector3 notPressableScale = new Vector3(0.003493f, 0.000526f, 0.002202f);

            StartButtonInteract.localScale = PressableScale;
            StopButtonInteract.localScale = PressableScale;

            if (closed)
            {
                //StartButtonInteract.GetComponent<MeshRenderer>().enabled = true;
                StartButtonInteract.localScale = PressableScale;

                //StopButtonInteract.GetComponent<MeshRenderer>().enabled = false;
                StopButtonInteract.localScale = notPressableScale;
            }
            else
            {
                //StopButtonInteract.GetComponent<MeshRenderer>().enabled = true;
                StopButtonInteract.localScale = PressableScale;

                //StartButtonInteract.GetComponent<MeshRenderer>().enabled = false;
                StartButtonInteract.localScale = notPressableScale;
            }
        }

        //
        // Teleporter Fixes
        // - Make the Teleporter Buttons bigger
        //
        [HarmonyPatch(typeof(ShipTeleporter), "Awake")]
        [HarmonyPostfix]
        static void Postfix_ShipTeleporter_Awake(ShipTeleporter __instance)
        {
            // Don't bother if the config option is disabled
            if (!CozyImprovements.CozyConfig.configBigTeleporterButtons.Value)
                return;
            GameObject TeleporterButton = __instance.buttonTrigger.gameObject.transform.parent.gameObject;
            TeleporterButton.transform.localScale = (Vector3.one * 3f);
        }

        //
        // Monitor Fixes
        // - Make the Monitor Buttons bigger
        //
        public static void adjustMonitorButtons(GameObject ButtonCube)
        {
            // Don't bother if the config option is disabled
            if (!CozyImprovements.CozyConfig.configBigMonitorButtons.Value)
                return;
            GameObject Button = ButtonCube.transform.parent.gameObject;
            Button.transform.localScale = new Vector3(1.852f, 1.8475f, 1.852f);

            if (Button.name == "CameraMonitorSwitchButton")
            {
                Button.transform.localPosition = new Vector3(-0.28f, -1.807f, -0.29f);
            }
        }
    }
}
