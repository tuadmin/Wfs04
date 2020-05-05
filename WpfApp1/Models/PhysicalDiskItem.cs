namespace WpfApp1.Models
{
    public class PhysicalDiskItem
    {
        public PhysicalDiskItem(string id, ulong size)
        {
            DeviceId = id;
            Size = size;
        }

        public PhysicalDiskItem() {}
        
        public string DeviceId { get;}

        public ulong Size { get;}
    }
}