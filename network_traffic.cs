using System;
using PacketDotNet;
using SharpPcap;

class Program
{
    static void Main(string[] args)
    {
        var devices = CaptureDeviceList.Instance;

        if (devices.Count < 1)
        {
            Console.WriteLine("Herhangi bir ağ cihazı bulunamadı.");
            return;
        }

        Console.WriteLine("Ağ cihazları:");
        for (int i = 0; i < devices.Count; i++)
        {
            Console.WriteLine($"{i}) {devices[i].Description}");
        }

        Console.Write("İzlemek istediğiniz cihazın numarasını seçin: ");
        int deviceIndex = int.Parse(Console.ReadLine());
        var device = devices[deviceIndex];

        device.OnPacketArrival += new PacketArrivalEventHandler(OnPacketArrival);
        device.Open(DeviceMode.Promiscuous, 1000);

        Console.WriteLine("Paketler yakalanıyor... (Çıkmak için Ctrl+C)");
        device.Capture();
    }

    private static void OnPacketArrival(object sender, CaptureEventArgs e)
    {
        
        var packet = Packet.ParsePacket(e.Packet.LinkLayerType, e.Packet.Data);

        var ethernetPacket = packet.Extract<EthernetPacket>();
        if (ethernetPacket != null)
        {
            Console.WriteLine($"Ethernet Packet: {ethernetPacket.SourceHardwareAddress} -> {ethernetPacket.DestinationHardwareAddress}");
        }

        var ipPacket = packet.Extract<IpPacket>();
        if (ipPacket != null)
        {
            Console.WriteLine($"IP Packet: {ipPacket.SourceAddress} -> {ipPacket.DestinationAddress}");
        }

        var tcpPacket = packet.Extract<TcpPacket>();
        if (tcpPacket != null)
        {
            Console.WriteLine($"TCP Packet: {tcpPacket.SourcePort} -> {tcpPacket.DestinationPort}");
        }
    }
}
