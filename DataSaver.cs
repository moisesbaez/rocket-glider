using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Runtime.Serialization;

namespace Glider
{
    public class DataSaver<LocalScores>
    {
        #region Declarations
        private const string TargetFolderName = "GliderSaveData";
        private DataContractSerializer _mySerializer;
        private IsolatedStorageFile _isoFile;
        #endregion

        #region Properties
        IsolatedStorageFile IsoFile
        {
            get
            {
                if (_isoFile == null)
                    _isoFile = System.IO.IsolatedStorage.IsolatedStorageFile.GetUserStoreForApplication();
                return _isoFile;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Creates a new object to save data.
        /// </summary>
        public DataSaver()
        {
            _mySerializer = new DataContractSerializer(typeof(LocalScores));
        }
        #endregion

        #region Load and Save Methods
        /// <summary>
        /// Save the scores data to the phone.
        /// </summary>
        /// <param name="sourceData">The scores data.</param>
        /// <param name="targetFileName">The name of the data file.</param>
        public void SaveMyData(LocalScores sourceData, String targetFileName)
        {
            string TargetFileName = String.Format("{0}/{1}.dat", TargetFolderName, targetFileName);

            if (!IsoFile.DirectoryExists(TargetFolderName))
                IsoFile.CreateDirectory(TargetFolderName);

            try
            {
                using (var targetFile = IsoFile.CreateFile(TargetFileName))
                {
                    _mySerializer.WriteObject(targetFile, sourceData);
                }
            }
            catch
            {
                IsoFile.DeleteFile(TargetFileName);
            }
        }

        /// <summary>
        /// Load the scores data from the phone.
        /// </summary>
        /// <param name="sourceName">Name of the data file.</param>
        /// <returns>The scores data.</returns>
        public LocalScores LoadMyData(string sourceName)
        {
            LocalScores retVal = default(LocalScores);
            string TargetFileName = String.Format("{0}/{1}.dat", TargetFolderName, sourceName);

            if (IsoFile.FileExists(TargetFileName))
                using (var sourceStream = IsoFile.OpenFile(TargetFileName, FileMode.Open))
                {
                    retVal = (LocalScores)_mySerializer.ReadObject(sourceStream);
                }
            return retVal;
        }
        #endregion
    }
}
