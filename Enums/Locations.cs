namespace enums;

public interface ILocationEnum
{
    public Locations GetLocationEnum();
    public Gateways GetGatewayEnum();
}

public enum Locations
{
    Wierden,
    Saxion,
    Gronau
}

public enum Gateways
{
    centrumEnschede,
    kerlinkAwmUt,
    loraGronauCentrum,
    packetbroker,
    slotWierden
}

public class LocationEnum : ILocationEnum
{
    private readonly Locations location_;
    public LocationEnum(Locations location) => location_ = location;
    public string GetLocation() => location_.ToString();
    public Locations GetLocationEnum() => location_;
    public Gateways GetGatewayEnum() => Gateways.centrumEnschede;   // Default gateway (not used)
}

public class GatewayEnum : ILocationEnum
{
    private readonly Gateways gateway_;
    public GatewayEnum(Gateways gateway) => gateway_ = gateway;
    public string GetGateway() => gateway_.ToString();
    public Gateways GetGatewayEnum() => gateway_;
    public Locations GetLocationEnum() => Locations.Wierden;   // Default location (not used)
}