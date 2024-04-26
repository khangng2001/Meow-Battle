using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerNotify : NetworkBehaviour
{
    public void Notify(ulong localClientID)
    {
        ClientRpcParams clientRpcParams = new ClientRpcParams() 
        { Send = new ClientRpcSendParams
            {
                TargetClientIds = new ulong[] { localClientID }
            }
        };

        NotifyClientRpc(clientRpcParams);
    }

    [ClientRpc]
    private void NotifyClientRpc(ClientRpcParams clientRpcParams = default)
    {
        if (IsOwner) return;

        KillNotification.OnKillNotification(1);
    }

    [ClientRpc]
    public void NotifyAllClientRpc(string whoKill, string whoDie)
    {
        KillNotification.OnKillNotificationAll(whoKill, whoDie);
    }

    [ClientRpc]
    public void NotifyAllSuicidalClientRpc(string whoSuicidal)
    {
        KillNotification.OnSuicidalNotificationAll(whoSuicidal);
    }
}
