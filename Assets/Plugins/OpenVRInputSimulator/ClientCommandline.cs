using System.Linq;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public static class ClientCommandline
{
    static readonly string[] dummy = new string[2]; 
    public static void deciveOffsets(string[] args)
    {
        var argsWithDummy = dummy.Concat(args).ToArray();
        deviceOffsets(argsWithDummy.Length, argsWithDummy);
    }
    [DllImport("client_commandline")] static extern void deviceOffsets(int argc, string[] argv);
}
