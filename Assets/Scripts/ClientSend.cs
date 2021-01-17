using Networking;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientSend : MonoBehaviour
{
   public static void SendTCPData(Packet _packet)
   {
        _packet.WriteLength();
        Client.Instance.Tcp.SendData(_packet);
   }

    public static void WelcomeReceived()
    {
        using (Packet _packet = new Packet((int)ClientPackets.WELCOME_RECEIVED))
        {
            _packet.Write(Client.Instance.MyId);
            _packet.Write(UIManager.Instance.UsernameField.text);

            SendTCPData(_packet);
        }
    }

    public static void SpawnReceived()
    {
        using (Packet _packet = new Packet((int)ClientPackets.SPAWN_PLAYER_RECEIVED))
        {
            _packet.Write(Client.Instance.MyId);

            SendTCPData(_packet);
        }
    }

    public static void SendRevealTile(int x, int y)
    {
        using (Packet _packet = new Packet((int)ClientPackets.SEND_REVEAL_TILE))
        {
            _packet.Write(Client.Instance.MyId);
            _packet.Write(x);
            _packet.Write(y);

            SendTCPData(_packet);
        }
    }

    public static void SendFlag(int x, int y)
    {
        using (Packet _packet = new Packet((int)ClientPackets.SEND_FLAG))
        {
            _packet.Write(Client.Instance.MyId);
            _packet.Write(x);
            _packet.Write(y);

            SendTCPData(_packet);
        }
    }
}
