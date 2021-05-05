using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace Linq
{
    class DataBase
    {
        private IEnumerable upperAge;
        private IEnumerable<IGrouping<string, PetInfo>> petTypeGrouping;
        private IEnumerable petTypeMinAge;
        private IEnumerable petTypeDiffAge;

        private readonly IList<PetInfo> petInfos;
        
        public DataBase(IList<PetInfo> infos)
        {
            petInfos = infos;
        }
        
        private static uint DialogChoice(string questionText, string[] variants)
        {
            Console.WriteLine(questionText);
            for (int i = 0; i < variants.Length; i++)
            {
                Console.WriteLine((i + 1) + ". " + variants[i]);
            }

            return (uint) GetIntUserInput(1, variants.Length);
        }

        private static int GetIntUserInput(int lowerBound = Int32.MinValue, int upperBound = Int32.MaxValue)
        {
            while (true) //while input is not valid (it must be int between 1 and amount of variants)
            {
                var input = Console.ReadLine();
                if (Int32.TryParse(input, out var intInput) && intInput >= lowerBound && intInput <= upperBound)
                {
                    return intInput;
                }

                Console.WriteLine("Неправильный ввод, попробуте снова.");
            }
        }

        static void Main(string[] args)
        {
            const string FILENAME_INPUT = "input.txt";
            const string FILENAME_OUTPUT = "output.txt";
            StreamReader reader = new StreamReader(FILENAME_INPUT);
            List<PetInfo> petInfos = new List<PetInfo>();

            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                petInfos.Add(PetInfo.ParseString(line));
            }

            DataBase dataBase = new DataBase(petInfos);

            Console.WriteLine("Добро пожаловать в мастер запросов по базе домашних питомцев.");
            var answer = DialogChoice("Введите номер нужного запроса.", new[]
            {
                "Вывести всех владельцев животных, возраст которых не превышает задаваемого значения.",
                "Вывести минимальный возраст питомцев каждого вида.",
                "По каждому виду животных вывести разницу между максимальным и минимальным значением возраста."
            });

            IEnumerable result = null;
            switch (answer)
            {
                case 1:
                {
                    Console.WriteLine("Введите максимальный возраст питомца: ");
                    uint maxAge = (uint) GetIntUserInput(0);
                    result = dataBase.GetOwnersWithPetsYoungerThan(maxAge);
                    break;
                }
                case 2:
                {
                    result = dataBase.GetMinAges();
                    break;
                }
                case 3:
                {
                    result = dataBase.GetDiffAges();
                    break;
                }
            }

            var answer2 = DialogChoice("Введите номер подходящего для вас варианта вывода результата.", new[]
            {
                "Вывод в консоль.",
                "Вывол в файл " + FILENAME_OUTPUT + "."
            });

            switch (answer2)
            {
                case 1:
                {
                    foreach (var o in result)
                    {
                        Console.WriteLine(o);
                    }

                    break;
                }
                case 2:
                {
                    using (var writer = new StreamWriter(FILENAME_OUTPUT, true))
                    {
                        foreach (var o in result)
                        {
                            writer.WriteLine(o);
                        }
                    }

                    break;
                }
            }
        }

        private IEnumerable GetOwnersWithPetsYoungerThan(uint maxAge)
        {
            return
                from p in petInfos
                where p.Age <= maxAge
                select p.OwnerName;
        }

        private IEnumerable GetMinAges()
        {
            return petTypeMinAge ??= 
                from type in GetPetTypeGrouping()
                where type.Any(x => x.Age != null)
                select new { key = type.Key, value = type.Min(x => x.Age) };
        }

        private IEnumerable GetDiffAges()
        {
            return petTypeDiffAge ??=
                from type in GetPetTypeGrouping()
                where type.Count(x => x.Age != null) > 2
                select new { key = type.Key, value = type.Max(x => x.Age) - type.Min(x => x.Age) };

        }

        private IEnumerable<IGrouping<string, PetInfo>> GetPetTypeGrouping()
        {
            return petTypeGrouping ??= 
                from pet in petInfos
                group pet by pet.PetType;
        }
    }
}