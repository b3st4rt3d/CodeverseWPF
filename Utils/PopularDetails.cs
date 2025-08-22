namespace CodeverseWPF.Utils
{
    internal class PopularDetails
    {
        private string Detail { get; set; }
        private string Brand { get; set; }
        private string Type { get; set; }
        private int Count { get; set; }

        public PopularDetails(string detail, string brand, string type, int count)
        {
            Detail = detail;
            Brand = brand;
            Type = type;
            Count = count;
        }
    }
}
