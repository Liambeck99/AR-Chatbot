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

    public List<NLUReturnValues3> loadNLUJSON()
    {
        //load list of NLU jsons from getNLUjson class

        string filePath = Path.Combine(Application.persistentDataPath, "data");
        string NLUpath = Path.Combine(filePath, "NLUResponse.json");

        StreamReader r = new StreamReader(NLUpath);
        string jsonString = r.ReadToEnd();
        r.Dispose();
        var result = Regex.Split(jsonString, "\r\n|\r|\n");

        string returntext = "";
        double returnrel = 0;
        int j = 0;
        int k = 1;
        string jsonchunk = "";

        //create a list ready to store results
        List<NLUReturnValues3> recommendedSocietityList = new List<NLUReturnValues3>();

        foreach (var line in result)
        {
            // Discards start of NLU file (JSONs start at line 7)
            if (j > 7)
            {

                // End of JSON chunk (5 lines each)
                if (k % 5 == 0)
                {
                    // Checks for file end
                    if (line == "    }")
                    {
                        jsonchunk = jsonchunk + line;
                    }
                    // Removes comma from end of JSON
                    else
                    {
                        string nocomma = line.Remove(line.Length - 1, 1);
                        jsonchunk = jsonchunk + nocomma;
                    }

                    // Gets values from json
                    nludatamodel m = JsonConvert.DeserializeObject<nludatamodel>(jsonchunk);

                    returntext = (m.text).ToLower();
                    returnrel = m.relevance;

                    // Runs comparison function, 
                    // Returns empty struct if no match found. 
                    // Returns populated struct if a match is found
                    NLUReturnValues2 comparisonStruct = jsonCompare(returntext, returnrel);
                    if (comparisonStruct.keyword == "")
                    {
                    }
                    else
                    {
                        NLUReturnValues3 addedStruct = new NLUReturnValues3();
                        addedStruct.keyword = comparisonStruct.keyword;
                        addedStruct.link = comparisonStruct.link;
                        addedStruct.score = returnrel;
                        recommendedSocietityList.Add(addedStruct);
                    }
                    // Resets variable ready for next JSON comparison
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
        List<NLUReturnValues3> anlist = orderByScore(recommendedSocietityList);


        Debug.Log("RECOMENDED");
        Debug.Log("=================================");

        foreach (NLUReturnValues3 o in anlist)
        {
            Debug.Log(o.keyword);
            Debug.Log(o.link);
            Debug.Log(o.score);
        }

        return (anlist);
        //return (recommendedSocietityList);
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

    public NLUReturnValues2 jsonCompare(string keyword, double score)
    {
        NLUReturnValues2 recommendedSociety = new NLUReturnValues2();

        // Checks if score is greater than 0.5
        if (isScoreValid(score) == true)
        {
            string keywordMatch = keyword;
            bool matchFound = false;

            string filePath = Path.Combine(Application.persistentDataPath, "data");
            string SocietyJSONpath = filePath + "/sockey.json";
            StreamReader r = new StreamReader(SocietyJSONpath);
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

                    string keywordCheck = (m.keyword).ToLower();

                    // If match is found
                    if (keywordCheck == keywordMatch)
                    {
                        matchFound = true;
                        string nameout = m.name;
                        string linkout = m.link;
                        recommendedSociety.keyword = nameout;
                        recommendedSociety.link = linkout;
                    }
                }
                else
                {
                    comparison = comparison + line + "\n";
                    j = j + 1;
                }
            }
            // Returns empty struct if no match is found
            if (matchFound == false)
            {
                recommendedSociety.keyword = "";
                recommendedSociety.link = "";
            }

            r.Dispose();
        }

        return (recommendedSociety);
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


    public struct NLUReturnValues2
    {
        public string keyword;
        public string link;
    }

    public struct NLUReturnValues3
    {
        public string keyword;
        public string link;
        public double score;
    }

    public List<NLUReturnValues3> orderByScore(List<NLUReturnValues3> returnList)
    {

        List<double> sortedList = new List<double>();

        foreach (NLUReturnValues3 o in returnList)
        {
            sortedList.Add(o.score);
        }
        sortedList.Sort();

        List<NLUReturnValues3> orderedList = new List<NLUReturnValues3>();
        foreach (double score in sortedList)
        {
            foreach (NLUReturnValues3 astruct in returnList)
            {
                if (score == astruct.score)
                {
                    orderedList.Add(astruct);
                }
            }
        }
        orderedList.Reverse();
        return (orderedList);

    }
}
