using RestSharp;
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
                return $"Id: {Id} \t Jméno: {Name.Substring(0, (Name.Length>20 ? 20 : Name.Length)),-20} " + 
                    $"\t Nebezpečný: {Is_potentially_hazardous_asteroid, -20}" + 
                    $" Odkaz: {Nasa_jpl_url} \n";
            }
        }

        static void Main(string[] args)
        {
            // je nutné se přihlásit k odběru API na https://rapidapi.com/dimas/api/NasaAPI/endpoints
            var client = new RestClient("https://nasaapidimasv1.p.rapidapi.com/getAsteroids");
            var request = new RestRequest(Method.POST);
            request.AddHeader("x-rapidapi-host", "NasaAPIdimasV1.p.rapidapi.com");
            request.AddHeader("x-rapidapi-key", "e386c6d8fcmshe260585231da736p1b775djsn22735edacde9");
            request.AddHeader("content-type", "application/x-www-form-urlencoded");
            IRestResponse response = client.Execute(request);

            // celý obsah parsovaný jako json objekty
            JObject asteroids = JObject.Parse(response.Content);

            //{
            //  "callback": "success",
            //  "contextWrites": {
            //      "to": { ....
            JArray asteroidsData = (JArray)asteroids["contextWrites"]["to"]["near_earth_objects"];
            // asteroids["contextWrites"]["to"]["near_earth_objects"] je pole

            List<DangerousObject> dangerousList = new List<DangerousObject>();

            for (int i = 0; i<asteroidsData.Count; i++)
            {
                DangerousObject dObj = asteroidsData[i].ToObject<DangerousObject>();
                dangerousList.Add(dObj);
            }

            dangerousList.ForEach((c) => Console.WriteLine(c));

            Console.ReadLine();
        }
    }
}
