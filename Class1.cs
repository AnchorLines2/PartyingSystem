using Steamworks;
using UnityEngine;
using UnityEngine.UIElements;
using System.Reflection;
using HarmonyLib;
using System;
using Unity.Netcode;

namespace MyPuckMod
{
    public class Class1 : IPuckMod
    {
        public static bool lobbyCreateSuccess;
        public static LobbyCreated_t lobbyData;
        static readonly Harmony harmony = new Harmony("ANCHOR.PartyingSystem");

        public bool OnEnable()
        {
            try
            {
                // Patched all functions we have defined to be patched
                harmony.PatchAll();
                Debug.Log("Party Patch Success");
            }
            catch (Exception e)
            {
                Debug.LogError($"Harmony patch failed: {e.Message}");
                return false;
            }

            //no clue what this does
            FieldInfo _mainMenuSettingsButtonField = typeof(UIMainMenu)
                .GetField("settingsButton",
                BindingFlags.Instance | BindingFlags.NonPublic);

            Button mainMenuSettingsButton = (Button)_mainMenuSettingsButtonField.GetValue(UIMainMenu.Instance);
            Debug.Log($"Located main menu settings button: {mainMenuSettingsButton}");

            ButtonManager.addRankedButton(mainMenuSettingsButton, ButtonClicked);

            Debug.Log("Hello world from PartyingSystem!");
            Debug.Log("Creating Steam Lobby...");

            CallResult<LobbyCreated_t> handler = new CallResult<LobbyCreated_t>();
            SteamAPICall_t created = SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, 6);
            handler.Set(created, LobbyCreated);



            return true;
        }

        public bool OnDisable()
        {
            try
            {
                // Patched all functions we have defined to be patched
                harmony.UnpatchSelf();
            }
            catch (Exception e)
            {
                Debug.LogError($"Harmony unpatch failed: {e.Message}");
                return false;
            }

            return true;
        }

        public void LobbyCreated(LobbyCreated_t result, bool IOFailure)
        {
            lobbyCreateSuccess = !IOFailure;
            lobbyData = result;
            if (IOFailure)
            {
                Debug.Log("Lobby Creation Failed.");
                return;
            }

            //if (pCallback.m_eResult == EResult.k_EResultOK) {}
            Debug.Log("Lobby Creation Successful.");

            lobbyDataUpdate = Callback<LobbyGameCreated_t>.Create(lobbyUpdate);
        }

        private Callback<LobbyGameCreated_t> lobbyDataUpdate;
        public void lobbyUpdate(LobbyGameCreated_t pCallback)
        {
            Debug.Log("LobbyDataUpdate fired for lobby: " + (CSteamID)pCallback.m_ulSteamIDLobby);
            //MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnMainMenuClickPractice", null);

            string ipAddress = pCallback.m_unIP + "";
            ushort port = pCallback.m_usPort;
            string password = "";
            ConnectionManager.Instance.Client_StartClient(ipAddress, port, password);
        }

        public static void ButtonClicked(ClickEvent clickEvent)
        {
            Debug.Log("PS Button Clicked");
            if (!lobbyCreateSuccess)
            {
                Debug.Log("No Lobby Present");
                return;
            }

            //MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnMainMenuClickPractice", null);
            //https://partner.steamgames.com/doc/api/ISteamMatchmaking#SetLobbyGameServer

            uint ipAddress = 0x7f000001;
            ushort port = 7777;
            string password = "";

            //(port, name, maxPlayers, password, voip, isPublic, ownerSteamId, uPnP = false)
            ServerManager.Instance.Client_StartHost(port, "LOBBY PRACTICE", 12, password, true, false, MonoBehaviourSingleton<StateManager>.Instance.PlayerData.steamId, false);
            SteamMatchmaking.SetLobbyGameServer(new CSteamID(lobbyData.m_ulSteamIDLobby), ipAddress, port, new CSteamID());
        }

        [HarmonyPatch(typeof(NetworkManager), "StartClient")]
        public class BringPartyIntoGame
        {
            [HarmonyPrefix]
            public static bool Prefix()
            {
                Debug.Log("Writing from harmony patch! (StartClient)");
                return true;
            }
        }
    }
}