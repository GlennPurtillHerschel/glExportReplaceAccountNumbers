using System;
using System.IO;
using System.Xml;

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GLExportAccountIssueScript
{
    class Program
    {
        static void Main(string[] args)
        {
            //My directory for the Directories 
            String[] errorDirectory = Directory.GetDirectories(@"C:\glExportData\errors");
            String outputData = "";

            //String accountNumbers = System.IO.File.ReadAllText(@"C:\glExportData\accountNumbers\AccountNumbersCSV.csv");
            Dictionary<string, string> accountNumbers = parseHashMap(System.IO.File.ReadAllText(@"C:\glExportData\accountNumbers\AccountNumbersCSV.csv"));

            int index = 0;

            //Loop through each file directory
            foreach (String errorFileDir in errorDirectory)
            {
                String[] errorFile = Directory.GetFiles(errorFileDir);
                //Loop through each file
                foreach (String fileDir in errorFile)
                {
                    String fileData = System.IO.File.ReadAllText(fileDir);
                    //If its an account number issue (some are pipeline related)
                    if(fileData.IndexOf("The Account Number (ACTNUMST) does not exist in the Account Index Master Table") > 0)
                    {
                        String accountNumber = fileData.Substring(fileData.IndexOf("<ACTNUMST>")+10, fileData.IndexOf("</ACTNUMST>") - fileData.IndexOf("<ACTNUMST>")-10);
                        String company = fileData.Substring(fileData.IndexOf("databasename=") + 13, fileData.IndexOf(";host") - fileData.IndexOf("databasename=")-13);
                        string value;
                        
                        if (accountNumbers.TryGetValue(accountNumber,out value))
                        {
                            fileData = fileData.Substring(fileData.LastIndexOf(".csv.processed") + 15, fileData.Length - fileData.LastIndexOf(".csv.processed") - 15);
                            fileData = changeAccounts(fileData, accountNumbers);
                            //fileData = fileData.Replace(accountNumber, value);
                            System.IO.File.WriteAllText(@"C:\glExportData\output\" + company + "\\" + company+"-" + index + ".csv", fileData);
                            index++;
                        }
                    }
                }
            }
        }

        static string changeAccounts(string fileData, Dictionary<string,string> accountNumbers)
        {
            foreach(KeyValuePair<string, string> entry in accountNumbers)
            {

                fileData = fileData.Replace(entry.Key, entry.Value.Replace("\r",""));
            }
            return fileData;
        }

        static Dictionary<string,string> parseHashMap(string accounts)
        {
            string[] accountArray = accounts.Split('\n');
            string[] seperateAccounts;
            Dictionary<string, string> output = new Dictionary<string, string>();
            foreach(string s in accountArray)
            {
                if (s.Length > 0)
                {
                    seperateAccounts = s.Split(',');
                    if(seperateAccounts[0].Length > 0)
                    {
                        output.Add(seperateAccounts[0], seperateAccounts[1]);
                    }
                }
                
            }
            return output;
        }
    }
}
