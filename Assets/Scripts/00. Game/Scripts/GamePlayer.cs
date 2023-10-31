using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class GamePlayer : NetworkBehaviour
{
    public void CreateRoom()
    {
        var manager = RoomManager.singleton;

        manager.StartHost();
    }
}
