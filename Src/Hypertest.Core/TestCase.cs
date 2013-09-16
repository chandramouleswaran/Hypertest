using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.Practices.Unity;

namespace Hypertest.Core
{
    /// <summary>
    /// The basic unit of a test case in the Hypertest framework
    /// </summary>
    public abstract class TestCase : ICloneable
    {
        #region Virtuals
        public virtual void Setup(){}
        public virtual void Body(){}
        public virtual void Cleanup(Exception e = null)
        {
            //Need to access logger
        }
        #endregion

        public void Run()
        {
            try
            {
                InitRun();
                Setup();
                Body();
                FinalizeRun();
                Cleanup();
            }
            catch (Exception e)
            {
                Cleanup(e);
            }
        }

        [Dependency("UnityContainer")]
        protected IUnityContainer Container { get; set; }

        public object Clone()
        {
            TestCase result = null;
            using (MemoryStream memStream = new MemoryStream())
            {
                BinaryFormatter binFormatter = new BinaryFormatter();
                binFormatter.Serialize(memStream, this);
                memStream.Seek(0, SeekOrigin.Begin);
                result = binFormatter.Deserialize(memStream) as TestCase;
                memStream.Close();
            }
            return result;
        }

        private void InitRun()
        {
            //Set the actual result to None
        }
        
        private void FinalizeRun()
        {
            //This is where we want to look at the properties and assign it to variables
        }
    }
}
