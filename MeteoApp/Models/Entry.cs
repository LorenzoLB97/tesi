using SQLite;

namespace MeteoApp
{
    public class Entry
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string CompleteAddress { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is not Entry other)
                return false;

            return Id == other.Id &&
                   CompleteAddress == other.CompleteAddress &&
                   Street == other.Street &&
                   City == other.City &&
                   PostalCode == other.PostalCode &&
                   Country == other.Country;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, CompleteAddress, Street, City, PostalCode, Country);
        }
    }
}
