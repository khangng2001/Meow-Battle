using System;
using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Services
{
    public class MessageBox : MessageBoxBase
    {
        [SerializeField] private GameObject messageBox;
        [SerializeField] private Button closeBtn;
        [SerializeField] private TextMeshProUGUI messageText;

        [Header("ACCOUNT SUBJECT")]
        [SerializeField] private SignIn signInSubjectToObserve;
        [SerializeField] private Register registerSubjectToObserve;
        
        [Header("LIST ROOM SUBJECT")]
        [SerializeField] private ListRoomManager listRoomManagerSubject;
        [SerializeField] private CreateRoom createRoomSubject;
        [SerializeField] private FetchListRooms fetchListRoomsSubject;

        [Header("MAIN MENU SUBJECT")]
        [SerializeField] private ModifiedName modifiedNameSubject;
        [SerializeField] private HandlerSkinListSO handlerSkinListSO;
        [SerializeField] private ShopManager shopManager;

        //Action
        public static Action OnCloseBtn;

        private void Awake()
        {
            closeBtn.onClick.AddListener(() => {
                OnCloseBtn?.Invoke();
                messageBox.SetActive(!messageBox.activeSelf);
                EmitSoundClick.Emit();
            });
        }

        private void Start()
        {
            SubscribeEvents();
        }

        private void OnDestroy()
        {
            UnsubscribeEvents();
        }

        protected override void OnSetMessage(string message)
        {
            messageText.text = message;
            messageBox.SetActive(true);
        }
       
        private void SubscribeEvents()
        {
            shopManager.OnMessageHandlerSkinListSO += OnSetMessage;
            handlerSkinListSO.OnMessageHandlerSkinListSO += OnSetMessage;
			modifiedNameSubject.OnMessageModifieldName += OnSetMessage;
            signInSubjectToObserve.OnMessageSignIn += OnSetMessage;
            registerSubjectToObserve.OnMessageRegister += OnSetMessage;
            listRoomManagerSubject.OnNotifyLobbyHeartBeat += OnSetMessage;
            createRoomSubject.OnCreateRoomMessage += OnSetMessage;
            fetchListRoomsSubject.OnJoinRoomByIDMessage += OnSetMessage;
        }

        private void UnsubscribeEvents()
        {
            shopManager.OnMessageHandlerSkinListSO -= OnSetMessage;
            handlerSkinListSO.OnMessageHandlerSkinListSO -= OnSetMessage;
			modifiedNameSubject.OnMessageModifieldName -= OnSetMessage;
            signInSubjectToObserve.OnMessageSignIn -= OnSetMessage;
            registerSubjectToObserve.OnMessageRegister -= OnSetMessage;
            listRoomManagerSubject.OnNotifyLobbyHeartBeat -= OnSetMessage;
            createRoomSubject.OnCreateRoomMessage -= OnSetMessage;
            fetchListRoomsSubject.OnJoinRoomByIDMessage -= OnSetMessage;
            
        }

    }
}
