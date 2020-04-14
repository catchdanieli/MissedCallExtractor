using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MissedCallExtractor
{
    class Program
    {
        
        
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            List<Records> listRecords = ParseExcel();
            List<Records> missedNumbers = GetMissedNumbers(listRecords);

            foreach(Records missedNumber in missedNumbers)
            {
                Console.WriteLine(missedNumber.Cli + " - " + missedNumber.DialStatus + " - " + missedNumber.CallStartTime + " - " + missedNumber.Dtmf );    
            }
        }

        private static List<Records> GetMissedNumbers(List<Records> listRecords)
        {
            List<Records> missedRecords = new List<Records>();
            listRecords = listRecords.OrderBy(x => x.CallStartTime).ToList<Records>();                
    
            foreach (Records records in listRecords) 
            {
                if (records.DialStatus != "Sucess")
                {
                    if (missedRecords.FirstOrDefault(x => x.Cli == records.Cli) == null)
                    {
                        missedRecords.Add(new Records
                        {
                            Cli = records.Cli,
                            CallStartTime = records.CallStartTime,
                            DialStatus = records.DialStatus,
                            Dtmf = records.Dtmf
                        });
                    }
                }
                else if (records.DialStatus == "Sucess") 
                {
                    if (missedRecords.FirstOrDefault(x => x.Cli == records.Cli) != null) 
                    {
                        missedRecords.Remove(missedRecords.First(x => x.Cli == records.Cli));                    
                    }
                }
            }
            return missedRecords;
        }

        private static List<Records> ParseExcel()
        {
            int rowCounter = 0;
            int colCli = 0, colCallStartTime = 0, colDialStatus = 0, colDtmf = 0;
            List<Records> listRecords = new List<Records>();

            using (TextFieldParser parser = new TextFieldParser(@"Report[14-Apr-2020 to 14-Apr-2020].csv"))
            {
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(",");
                while (!parser.EndOfData)
                {
                    rowCounter++;
                    if (rowCounter == 1) {
                        string[] fields = parser.ReadFields();
                        for (int i = 0; i < fields.Length; i++)
                        {
                            if (fields[i] == "CLI") colCli = i;
                            if (fields[i] == "CALL START TIME") colCallStartTime = i;
                            if (fields[i] == "DIAL STATUS") colDialStatus = i;
                            if (fields[i] == "DTMF") colDtmf = i;
                        }
                        
                    }

                    if (rowCounter > 1 && colCli != 0)
                    {
                        //Processing row
                        string[] fields = parser.ReadFields();
                        if(string.Equals(fields[colDtmf],"9")) continue;
                        Records records = new Records
                        {
                            Cli = fields[colCli],
                            CallStartTime = Convert.ToDateTime(fields[colCallStartTime]),
                            DialStatus = fields[colDialStatus],
                            Dtmf = fields[colDtmf]
                        };

                        listRecords.Add(records);                        
                    }
                }
            }
            return listRecords;
        }
        
    }

    class Records {
        private string cli;
        private DateTime callStartTime;
        private string dtmf;
        private string dialStatus;

        public string Cli { get => cli; set => cli = value; }
        public DateTime CallStartTime { get => callStartTime; set => callStartTime = value; }
        public string Dtmf { get => dtmf; set => dtmf = value.Substring(value.Length - 1); }
        public string DialStatus { get => dialStatus; set => dialStatus = value; }
    }
}
