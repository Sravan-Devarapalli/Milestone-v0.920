using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace DataTransferObjects.Skills
{
    [DataContract]
    [Serializable]
    public class PersonPractice
    {
       

        [DataMember]
        public Person Person
        {
            set;
            get;
        }

        [DataMember]
        public Practice Pactice
        {
            set;
            get;
        }
    }
}

