using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Security.Cryptography;
using Microsoft.Xna.Framework.GamerServices;

namespace Glider
{
    static class HighScoreTable
    {
        #region Declarations
        //Stores the high score information
        public static string[] ranks = new string[10];
        public static string[] names = new string[10];
        public static int[] scores = new int[10];
        
        //States for sending and receiving scores
        private enum States { Send, Receive };
        private static States currentState = States.Receive;
        
        //Values for submitting scores
        private static string insertName;
        private static int insertScore;
        #endregion

        #region Public Methods
        /// <summary>
        /// Initializes the high score information table.
        /// </summary>
        public static void Initialize()
        {
            for (int i = 0; i < 10; i++)
            {
                ranks[i] = " ";
                names[i] = " ";
                scores[i] = 0;
            }
        }

        /// <summary>
        /// Retrieves the online scores.
        /// </summary>
        public static void RetrieveScores()
        {
            currentState = States.Receive;  //set the state to receive
            Connect();  //connect to the database
        }

        /// <summary>
        /// Sends scores online.
        /// </summary>
        /// <param name="name">Name to be sent.</param>
        /// <param name="score">Score to be sent.</param>
        public static void SendScore(string name, int score)
        {
            insertName = name;
            insertScore = score;
            currentState = States.Send;  //set the state to send
            Connect();  //connect to the database
        }

        public static void Connect()
        {
            string URL = "";  //create the URL string

            //Switch URLs depending if we are sending or receiving
            switch (currentState)
            {
                case States.Send:
                    URL = "http://phunfactory.com/highscores/rocketglider/newscore.php";
                    break;
                case States.Receive:
                    URL = "http://phunfactory.com/highscores/rocketglider/requestscores.php";
                    break;
            }

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);  //request the URL
            request.ContentType = "application/x-www-form-urlencoded";  //set the encoding type

            request.Method = "POST";  //set the method to POST

            request.BeginGetRequestStream(new AsyncCallback(GetRequestStreamCallback), request);  //request the stream of information
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Request to receive or send information to the website.
        /// </summary>
        /// <param name="asynchronousResult">Status of the asynchronous operation.</param>
        private static void GetRequestStreamCallback(IAsyncResult asynchronousResult)
        {
            HttpWebRequest request = (HttpWebRequest)asynchronousResult.AsyncState;
            Stream postStream = request.EndGetRequestStream(asynchronousResult);

            string postData = "";

            //Change POST information depending on whether we want to receive or send info
            switch (currentState)
            {
                case States.Send:
                    postData = "&Name=" + insertName + "&Score=" + insertScore;
                    break;
                case States.Receive:
                    postData = "&Format=" + "TOP10";
                    break;
            }

            byte[] byteArray = Encoding.UTF8.GetBytes(postData);  //set the bytes of the string

            postStream.Write(byteArray, 0, postData.Length);  //write the information to the website
            postStream.Close();  //close the stream

            request.BeginGetResponse(new AsyncCallback(GetResponseCallback), request);  //ask for a response from the website
        }

        private static void GetResponseCallback(IAsyncResult asynchronousResult)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)asynchronousResult.AsyncState;
                HttpWebResponse response = (HttpWebResponse)request.EndGetResponse(asynchronousResult);


                Stream streamResponse = response.GetResponseStream();
                StreamReader streamRead = new StreamReader(streamResponse);

                string fullData = streamRead.ReadToEnd();  //read the information being sent into a string

                if (currentState == States.Receive)  //if we need to receive scores
                    parseData(fullData);  //parse the data into the table

                streamResponse.Close();
                streamRead.Close();

                response.Close();
            }
            catch  //if we can't connect to the server, display a message box error
            {
                List<string> MBOPTIONS = new List<string>();
                MBOPTIONS.Add("OK");
                string msg = "High Scores database is currently down. Please try again later.";
                
                if (currentState == States.Send)
                    Guide.BeginShowMessageBox(
                            "Error Sending: \nHigh Scores Data", msg, MBOPTIONS, 0,
                            MessageBoxIcon.Alert, GetMBResult, null);
                else
                    Guide.BeginShowMessageBox(
                            "Error Receiving: \nHigh Scores Data", msg, MBOPTIONS, 0,
                            MessageBoxIcon.Alert, GetMBResult, null);

            }
        }

        /// <summary>
        /// End the message box alert.
        /// </summary>
        /// <param name="r">Status of the asynchronus operation.</param>
        private static void GetMBResult(IAsyncResult r)
        {
            int? b = Guide.EndShowMessageBox(r);
        }

        /// <summary>
        /// Parse the data that was returned when receiving scores.
        /// </summary>
        /// <param name="tableString">String that holds the high score data.</param>
        private static void parseData(string tableString)
        {
            const string SERVER_VALID_DATA_HEADER = "SERVER_";
            if ((tableString.Trim().Length < SERVER_VALID_DATA_HEADER.Length) || (!tableString.Trim().Substring(0, SERVER_VALID_DATA_HEADER.Length).Equals(SERVER_VALID_DATA_HEADER)))
                return;
            string toParse = tableString.Trim().Substring(SERVER_VALID_DATA_HEADER.Length);
            string[] rows = Regex.Split(toParse, "_ROW_");
            for (int i = 0; i < 10; i++)
            {
                if (rows.Length > i && rows[i].Trim() != "")
                {
                    string[] cols = Regex.Split(rows[i], "_COL_");
                    if (cols.Length == 3)
                    {
                        names[i] = cols[0].Trim();
                        scores[i] = int.Parse(cols[1]);
                        ranks[i] = cols[2];
                    }
                }
                else
                {
                    names[i] = " ";
                    scores[i] = 1;
                    ranks[i] = " ";
                }
            }
        }
        #endregion
    }
}
