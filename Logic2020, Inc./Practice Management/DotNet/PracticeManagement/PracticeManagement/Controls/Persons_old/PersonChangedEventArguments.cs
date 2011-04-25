using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DataTransferObjects;

namespace PraticeManagement.Controls.Persons
{
    public class PersonChangedEventArguments : EventArgs
    {
        public Person SelectedPerson
        {
            get;
            private set;
        }

        /// <summary>
        /// Init constructor of PersonChangedEventArguments.
        /// </summary>
        public PersonChangedEventArguments(Person selectedPerson)
        {
            this.SelectedPerson = selectedPerson;
        }
    }
}

