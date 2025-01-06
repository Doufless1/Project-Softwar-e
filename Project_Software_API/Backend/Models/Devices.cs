namespace Project_Software_API.Properties.Backend.Models;


public class Devices
{
    public static List<string> GetDevices(){
        return
        [
            "lht-wierden",
            "mkr-wierden",
            "lht-saxion",
            "mkr-saxion",
            "mkr-gronau",
            "lht-gronau",
            "ibfkloranew",
            "lht-tester"
        ];
    }
}