using System;
using System.ComponentModel;

namespace Linq
{
    public class PetInfo
    {
        private string ownerName;
        private string petType;
        private string name;
        private uint? age;
        
        public string OwnerName { get => ownerName; }
        public string PetType { get => petType; }
        public string Name { get => name; }
        public uint? Age { get => age; }

        public PetInfo(string petType, string ownerName = null, string name = null, uint? age = null)
        {
            if (string.IsNullOrWhiteSpace(petType))
            {
                throw new ArgumentException("Pet type can't be null or filled with whitespaces!");
            }
            this.petType = petType;
            this.name = name;
            this.ownerName = ownerName;
            this.age = age;
        }

        public override string ToString()
        {
            return $"Owner: {OwnerName}, PetType: {PetType}, Name: {Name}, Age: {Age}.";
        }

        public static PetInfo ParseString(string str)
        {
            const uint NUMBER_OF_ARGUMENTS = 4;
            
            str = str.Trim();
            if (string.IsNullOrWhiteSpace(str))
            {
                throw new ArgumentException("String to parse can't be null or filled with whitespaces!");
            }

            string[] arguments = new string[NUMBER_OF_ARGUMENTS];
            int indexBegin = 0, indexEnd = 0;
            for (int i = 0; i < NUMBER_OF_ARGUMENTS - 1; i++)
            {
                indexEnd = str.IndexOf(',', indexBegin);
                if (indexEnd >= 0)
                {
                    arguments[i] = str.Substring(indexBegin, indexEnd - indexBegin);
                    indexBegin = indexEnd + 1;
                }
                else
                {
                    throw new ArgumentException(
                        "Not enough arguments to initilize a PetInfo object from string. There must be a missing ,");
                }
            }
            arguments[NUMBER_OF_ARGUMENTS - 1] = str.Substring(indexBegin, str.Length - indexBegin);

            uint? age;
            if (arguments[NUMBER_OF_ARGUMENTS - 1].Equals(""))
            {
                age = null;
            }
            else if (UInt32.TryParse(arguments[NUMBER_OF_ARGUMENTS - 1], out var ageInt))
            {
                age = ageInt;
            }
            else
            {
                throw new ArgumentException("Argument for age is not valid!");
            }

            return new PetInfo(arguments[1], arguments[0], arguments[2], age);
        }
    }
}