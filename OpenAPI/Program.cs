﻿using System.Net.Http;
using System;
using Newtonsoft;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Data;

namespace OpenAPI
{
    class Program
    {
        class DangerousObject
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public bool Is_potentially_hazardous_asteroid { get; set; }

            public string Nasa_jpl_url { get; set; }

            public override string ToString()
            {
                return $"Id: {Id} \t Jméno: {Name.Substring(0, (Name.Length > 20 ? 20 : Name.Length)),-20} " +
                    $"\t Nebezpečný: {Is_potentially_hazardous_asteroid,-20}" +
                    $" Odkaz: {Nasa_jpl_url} \n";
            }
        }

        static void Main()
        {
            // je nutné se přihlásit k odběru API na https://api.nasa.gov/
            var client = new HttpClient();
            var response = client.GetAsync("https://api.nasa.gov/neo/rest/v1/feed?" +
                "start_date=2020-11-18&end_date=2020-11-19" +
                "&api_key=QK36Tn9laJtbIDk2hO2PFuU9JiAxnubncCq7nF6f").Result.Content.ReadAsStringAsync().Result;

            // celý obsah parsovaný jako json objekty
            // response je obyčejný string a neumožňuje nám dělat logické operace
            // (jako třeba odfiltrovat všechno, co nesouvisí se seznameme zajímavých asteroidů
            JObject asteroids = JObject.Parse(response);

            Console.WriteLine(asteroids);

            Console.WriteLine("Stiskněte Enter");
            Console.ReadLine();

            // Vybere jenom data, která se týdají asteroidů jednoho dne
            JArray asteroidsData = (JArray)asteroids["near_earth_objects"]["2020-11-19"];

            List<DangerousObject> dangerousList = new List<DangerousObject>();
            for (int i = 0; i < asteroidsData.Count; i++)
            {
                DangerousObject dObj = asteroidsData[i].ToObject<DangerousObject>();
                dangerousList.Add(dObj);
            }
            dangerousList.ForEach((c) => Console.WriteLine(c));

            Console.WriteLine("Stiskněte Enter");
            Console.ReadLine();

            // vybere data všech asteroidů v rozmezí start_date=2020-11-18 & end_date=2020-11-19
            // nevšímá si ostatních informací v asteroids
            string asteroidsDataForAllDates = ((JObject)asteroids["near_earth_objects"]).ToString();
            // Deserializace do dictionary, kde klíčem je datum a hodnotou je pole 
            // asteroidů
            Dictionary<string, DangerousObject[]> nearEarthObjects =
                JsonConvert.DeserializeObject<Dictionary<string, DangerousObject[]>>(asteroidsDataForAllDates);
            foreach(var v in nearEarthObjects)
            {
                Console.WriteLine($"{v.Key}: ");
                foreach(var t in v.Value)
                {
                    Console.WriteLine(t);
                }
            }
        }
    }
}
