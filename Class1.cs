using Steamworks;
using UnityEngine;
using UnityEngine.UIElements;
using System.Reflection;

namespace MyPuckMod
{
    public class Class1 : IPuckMod
    {

        public static bool lobbyCreateSuccess;
        public static LobbyCreated_t lobbyData;
        public static Button mainMenuSettingsButton;

                //no clue what this does
        static readonly FieldInfo _mainMenuSettingsButtonField = typeof(UIMainMenu)
            .GetField("settingsButton",
            BindingFlags.Instance | BindingFlags.NonPublic);
            
        private static void LocateReferenceButtons()
        {
            mainMenuSettingsButton = (Button)_mainMenuSettingsButtonField.GetValue(UIMainMenu.Instance);
            Debug.Log($"Located main menu settings button: {mainMenuSettingsButton}");
        }

        public static void AddHoverEffectsForButton(Button button)
        {
            button.RegisterCallback<MouseEnterEvent>(new EventCallback<MouseEnterEvent>((evt) =>
            {
                button.style.backgroundColor = Color.white;
                button.style.color = Color.black;
            }));
            button.RegisterCallback<MouseLeaveEvent>(new EventCallback<MouseLeaveEvent>((evt) =>
            {
                button.style.backgroundColor = new StyleColor(new Color(0.25f, 0.25f, 0.25f));
                button.style.color = Color.white;
            }));
        }

        Button addRankedButton(Button referenceButton)
        {
            Button button = new Button
            {
                text = "LOBBY PRACTICE - 2",
                style =
                {
                    backgroundColor = new StyleColor(new Color(0.25f, 0.25f, 0.25f)),
                    unityTextAlign = TextAnchor.MiddleLeft,
                    width = new StyleLength(new Length(100, LengthUnit.Percent)),
                    minWidth = new StyleLength(new Length(100, LengthUnit.Percent)),
                    maxWidth = new StyleLength(new Length(100, LengthUnit.Percent)),
                    height = referenceButton.style.height,
                    minHeight = referenceButton.style.minHeight,
                    maxHeight = referenceButton.style.maxHeight,
                    marginTop = 8,
                    paddingTop = 8,
                    paddingBottom = 8,
                    paddingLeft = 15
                }
            };
            AddHoverEffectsForButton(button);
            button.RegisterCallback<ClickEvent>(ButtonClicked);
            return button;
        }

        void addRankedButton(UIMainMenu mainMenu)
        {
            VisualElement containerVisualElement = mainMenuSettingsButton.parent;
            if (containerVisualElement == null)
            {
                Debug.LogError("Container VisualElement not found (parent of settingsButton missing)!");
                return;
            }

            Button reskinMenuButton = addRankedButton(mainMenuSettingsButton);
            containerVisualElement.Insert(2, reskinMenuButton);
        }

        public bool OnEnable()
        {
            LocateReferenceButtons();
            addRankedButton(UIMainMenu.Instance);

            Debug.Log("Hello world from PartyingSystem!");
            Debug.Log("Creating Steam Lobby...");

            CallResult<LobbyCreated_t> handler = new CallResult<LobbyCreated_t>();
            SteamAPICall_t created = SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, 6);
            handler.Set(created, LobbyCreated);

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
            MonoBehaviourSingleton<EventManager>.Instance.TriggerEvent("Event_Client_OnMainMenuClickPractice", null);
        }//*/

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
            SteamMatchmaking.SetLobbyGameServer(new CSteamID(lobbyData.m_ulSteamIDLobby), 0x7f000001, 7777, new CSteamID());
        }

        public bool OnDisable()
        {
            return true;
        }
    }
}