using System;
using System.Runtime.Serialization;

namespace DataTransferObjects
{
    /// <summary>
    /// Provides a basic utility class that is used to store 4 typed objects
    /// </summary>
    [DataContract]
    [Serializable]
    public class Quadruple<A, B, C, D>
    {
        #region Properties

		    /// <summary>
            /// Gets or sets the First object.
            /// </summary>
            [DataMember]
            public A First
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the Second object.
            /// </summary>
            [DataMember]
            public B Second
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the Third object.
            /// </summary>
            [DataMember]
            public C Third
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the Fourth object.
            /// </summary>
            [DataMember]
            public D Fourth
            {
                get;
                set;
            }


	    #endregion    

        /// <summary>
        /// Init constructor of Quadruple.
        /// </summary>
        public Quadruple(A first, B second, C third, D fourth)
        {
            this.First = first;
            this.Second = second;
            this.Third = third;
            this.Fourth = fourth;
        }
    }
}

