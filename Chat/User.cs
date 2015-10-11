using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Collections;

namespace Chat
{
    public class User
    {
        public Socket socket;
        public string name;

        public User(Socket sct, string nm)
        {
            socket = sct;
            name = nm;
        }

    }
}
