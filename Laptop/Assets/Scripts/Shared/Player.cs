using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player
{
    public int id;
    public string username;
    public string instrumentType;
    public string IP;

    public Player(int _id, string _username, string _IP, string _instrumentType = null)
    {
        id = _id;
        username = _username;
        IP = _IP;
        instrumentType = _instrumentType;
    }
}
