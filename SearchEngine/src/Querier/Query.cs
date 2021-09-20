﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Indexing;
using Utilities;
using Tokenize;
using Database;

namespace SearchEngine
{

    public class Querier
    {
        public static string elapsedTime
        {
            get;
            private set;
        }

        private static List<List<Posting>> GetListOfPostingsForQuery(string query)
        {
            List<List<Posting>> posting_list = new();

            using (var reader = new StringReader(query))
            {
                //Get the tokenizer and tokenize the query
                var tokenSource = new Tokenizer();
                tokenSource.SetReader(reader);
                List<string> tokenizedWords = tokenSource.ReadAll();

                foreach (string re in tokenizedWords)
                {
                    Logger.Info("This is the current term--->" + re);
                    if (DatabaseService.GetInvertedIndexEntry(re) != null)
                    {
                        var postList = DatabaseService.GetInvertedIndexEntry(re).Postings;
                        posting_list.Add(postList);
                    }
                    else
                    {
                        Console.WriteLine("No Terms as this exist in Database");
                    }
                }
            }
            return posting_list;
        }

        private static List<int> _Search(string query)
        {
            List<List<Posting>> listOfPostings = GetListOfPostingsForQuery(query);
            List<int> matchedDocs = new();
            foreach(List<Posting> postings in listOfPostings)
            {
                
            }

            return matchedDocs;
        }

        private static void SearchQueryOfTwoTerms(List<Posting> list1, List<Posting> list2) {
            List<Posting> list = _TwoMerge(list1, list2);
            List<int> documentIds = new();
            foreach(Posting posting in list)
            {
                documentIds.Add(posting.DocumentID);
            }



        }
        private static List<Posting> _TwoMerge(List<Posting> list1, List<Posting> list2)
        {
            List<Posting> list = new();

            int current1 = 0;   // current index of list1
            int current2 = 0;   // current index of list2
            int current3 = 0;   // current index of list3

            while (current1 < list1.Count && current2 < list2.Count)
            {
                if (list1[current1].DocumentID > list2[current2].DocumentID)
                {
                    list[current3++] = list2[current2++];
                }
                else
                {
                    list[current3++] = list1[current1++];
                }
            }

            // copy the remaining items in list1 and list2 if any
            while (current1 < list1.Count)
            {
                list[current3++] = list1[current1++];
            }

            while (current2 < list2.Count)
            {
                list[current3++] = list2[current2++];
            }

            return list;
        }

        public static List<int> Search(string query)
        {
            List<int> matchedDocs = new();
            using (var reader = new StringReader(query))    
            {
                Stopwatch watch = new Stopwatch();
                watch.Start();

                //Get the tokenizer and tokenize the query
                var tokenSource = new Tokenizer();
                tokenSource.SetReader(reader);
                List<string> tokenizedWords = tokenSource.ReadAll();

                List<List<Posting>> posting_list = new();

                foreach (string re in tokenizedWords)
                {
                    Logger.Info("This is the current term--->" +  re);
                    if (DatabaseService.GetInvertedIndexEntry(re) != null)
                    {
                        var postList = DatabaseService.GetInvertedIndexEntry(re).Postings;
                        posting_list.Add(postList);

                        for (int i = 0; i < postList.Count; i++)
                        {
                            matchedDocs.Add(postList[i].DocumentID);
                        }
                    }
                    else
                    {
                        Console.WriteLine("No Terms as this exist in Database");
                    }
                }
               
                watch.Stop();
                // Get the elapsed time as a TimeSpan value.
                TimeSpan ts = watch.Elapsed;
                elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:000}",
                    ts.Hours, ts.Minutes, ts.Seconds,
                    ts.TotalMilliseconds);
                watch.Reset();
                Console.WriteLine($"Search Runtime is -> { ts.TotalMilliseconds} milliseconds");
            }

            foreach (var variable in matchedDocs)
            {
                Console.WriteLine("DocIDs returned are ==>" + variable.ToString());
            }
            return matchedDocs;
        }

    }
}