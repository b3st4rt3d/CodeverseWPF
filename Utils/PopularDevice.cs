namespace CodeverseWPF.Utils
{
    internal class PopularDevice
    {
        private string Device {  get; set; }
        private int Count { get; set; }
        public PopularDevice(string device, int count)
        {
            Device = device;
            Count = count;
        }
    }
}
