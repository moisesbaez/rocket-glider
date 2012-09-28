using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace Glider
{
    [DataContract]
    public class LocalScores
    {
        #region Properties
        [DataMember]
        public List<int> scoreList { get; set; }  //stores the list of scores
        [DataMember]
        public bool soundOn { get; set; }  //stores the state of the sound
        #endregion

        #region Contstructor
        /// <summary>
        /// Create a new local scores object that is initialized.
        /// </summary>
        public LocalScores()
        {
            scoreList = new List<int>();
            soundOn = true;
        }
        #endregion

        #region Scores Methods
        /// <summary>
        /// Add a score to the local scores.
        /// </summary>
        /// <param name="score"></param>
        public void AddScore(int score)
        {
            int i = 0;
            if (scoreList.Count > 0)  //if there are scores in the list
            {
                while (i < scoreList.Count)  //keep looping through all the scores in the list
                {
                    if (score >= scoreList[i])  //is the new score greater or equal to the current score?
                    {
                        scoreList.Insert(i, score);  //insert it if it is
                        if (scoreList.Count == 11)  //if there are more than 10 scores
                            scoreList.RemoveAt(10);  //remove the last score, to keep the list at 10 scores
                        return;  //exit this call, we are done
                    }
                    i++;  //move to the next score
                }
                if (scoreList.Count < 10)  //check if there aren't 10 scores (this check will occur when the score wasn't better than any saved one)
                    scoreList.Insert(i, score);  //insert it at the last indexed position
            }
            else
                scoreList.Add(score);  //if there are no scores in the list, just add it to the list
        }
        #endregion
    }
}
