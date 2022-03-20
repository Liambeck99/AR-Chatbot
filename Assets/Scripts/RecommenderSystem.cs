using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using System.IO;

public class RecommenderSystem 
{
    private string message;

    public RecommenderSystem()
    {
       
    }


    public List<NLUReturnValues> loadNLUJSON()
    {
        //load list of NLU jsons from getNLUjson class
        string NLUpath = Application.persistentDataPath + "/NLUResponse.json";
        StreamReader r = new StreamReader(NLUpath);
        string jsonString = r.ReadToEnd();
        var result = Regex.Split(jsonString, "\r\n|\r|\n");

        string returntext = "";
        double returnrel = 0;
        int j = 0;
        int k = 1;
        string jsonchunk = "";

        //create a list ready to store results
        List<NLUReturnValues> recommendedSocietityList = new List<NLUReturnValues>() ;

        foreach (var line in result)
        {
            //Console.WriteLine(line);
            //Console.WriteLine("j: {0}", j);
            //Console.WriteLine("k: {0}", k);
            if (j > 7)
            {

                if (k % 5 == 0)
                {
                    string nocomma = line.Remove(line.Length - 1, 1);
                    jsonchunk = jsonchunk + nocomma;
                    //Console.WriteLine(jsonchunk);

                    nludatamodel m = JsonConvert.DeserializeObject<nludatamodel>(jsonchunk);

                    returntext = m.text;
                    returnrel = m.relevance;

                    //Console.WriteLine(returntext);
                    //Console.WriteLine(returnrel);

                    jsonCompare(returntext, returnrel);
                    //var pls = Tuple.Create(m.text, m.relevance);

                    jsonchunk = "";
                }
                else
                {
                    jsonchunk = jsonchunk + line + "\n";
                }
                k = k + 1;
            }

            j = j + 1;

        }

        //create object to store recommended society values
        NLUReturnValues recommendedSociety = new NLUReturnValues();
        recommendedSociety.keyword = returntext;
        recommendedSociety.score = returnrel;

        //add recommended society to the recommended list
        recommendedSocietityList.Add(recommendedSociety);
        
        return (recommendedSocietityList);

    }

    public bool isScoreValid(double score)
    {

        if (score > 0.5)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool jsonCompare(string keyword, double score)
    {

        if (isScoreValid(score) == false)
        {
            //Console.WriteLine("Score invalid");
            return (false);
        }

        else
        {
            //Console.WriteLine("Score valid");

            string keywordMatch = keyword;
            bool matchFound = false;
            StreamReader r = new StreamReader("sockey.json");
            string jsonString = r.ReadToEnd();

            var result = Regex.Split(jsonString, "\r\n|\r|\n");

            string comparison = "";
            int j = 0;
            foreach (var line in result)
            {
                if (j == 4)
                {
                    j = 0;
                    string nocomma = line.Remove(line.Length - 1, 1);
                    comparison = comparison + nocomma;
                    userdatamodel m = JsonConvert.DeserializeObject<userdatamodel>(comparison);
                    comparison = "";

                    if (m.keyword == keywordMatch)
                    {
                        matchFound = true;
                        Debug.Log("MATCH FOUND");
                        //Console.WriteLine("Keyword: {0}", m.keyword);
                        //Console.WriteLine("Name: {0}", m.name);
                        //Console.WriteLine("Link: {0}", m.link);
                        string nameout = m.name;
                        string linkout = m.link;
                        Debug.Log(returnRecMessage(nameout, linkout));
                    }
                }
                else
                {
                    comparison = comparison + line + "\n";
                    j = j + 1;
                }
            }
            if (matchFound == false)
            {
                //Console.WriteLine("No Match Found");
            }
            return (true);
        }
    }

    public string returnRecMessage(string name, string link)
    {
        string msg = ("From your interests, you may like the society: " + name);
        return (msg);
    }

    class userdatamodel
    {
        public string keyword { get; set; }
        public string name { get; set; }
        public string link { get; set; }
    }

    class nludatamodel
    {
        public string text { get; set; }
        public double relevance { get; set; }
        public int count { get; set; }
    }

    public struct NLUReturnValues
    {
        public string keyword;
        public double score;
    }
}
 