using SQLite;
using System;

namespace MeteoApp
{
    public class Entry
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        private string _completeAddress;
        private string _street;
        private string _city;
        private string _postalCode;
        private string _country;

        public string CompleteAddress
        {
            get => _completeAddress;
            set => _completeAddress = value;
        }

        public string Street
        {
            get => _street;
            set => _street = value;
        }

        public string City
        {
            get => _city;
            set => _city = value;
        }

        public string PostalCode
        {
            get => _postalCode;
            set => _postalCode = value;
        }

        public string Country
        {
            get => _country;
            set => _country = value;
        }

        // Override del metodo Equals per il confronto degli oggetti Entry
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

        // Override di GetHashCode per supportare l'uso di Entry in collezioni hash-based
        public override int GetHashCode()
        {
            return HashCode.Combine(Id, CompleteAddress, Street, City, PostalCode, Country);
        }
    }
}
