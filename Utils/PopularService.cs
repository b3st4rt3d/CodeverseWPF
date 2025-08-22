namespace CodeverseWPF.Utils
{
    internal class PopularService
    {
        private string Service { get; set; }
        private int Count { get; set; }
        public PopularService(string service, int count)
        {
            Service = service;
            Count = count;
        }
    }
}
