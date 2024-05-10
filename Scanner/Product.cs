using SQLite;

namespace Scanner
{
    public class Product
    {
        [PrimaryKey, AutoIncrement] public int Id { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }
        public string Barcode { get; set; }
    }
}