using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Tasks;
using Microsoft.Phone.UserData;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;


namespace WP7GapClassLib.PhoneGap.Commands
{
    public class Contacts : BaseCommand
    {
        private SaveContactTask _contactTask;
        private Microsoft.Phone.UserData.Contacts _contacts;
        private IEnumerable<Contact> _searchResults;

        public Contacts()
        {
            _contactTask = new SaveContactTask();
            _contactTask.Completed += new EventHandler<SaveContactResult>(saveContactTask_Completed);

            _contacts = new Microsoft.Phone.UserData.Contacts();
            _contacts.SearchCompleted += new EventHandler<ContactsSearchEventArgs>(contacts_SearchCompleted);
        }

        [DataContract]
        public class NewContact
        {

            public NewContact()
            {

            }

            [OnDeserializing]
            public void OnDeserializing(StreamingContext context)
            {
                // set defaults
                this.Company = "";
                this.FirstName = "";
                this.HomeAddressCity = "";
                this.HomeAddressCountry = "";
                this.HomeAddressState = "";
                this.HomeAddressStreet = "";
                this.HomeAddressZipCode = "";
                this.HomePhone = "";
                this.JobTitle = "";
                this.LastName = "";
                this.MiddleName = "";
                this.MobilePhone = "";
                this.Nickname = "";
                this.Notes = "";
                this.OtherEmail = "";
                this.PersonalEmail = "";
                this.Suffix = "";
                this.Title = "";
                this.Website = "";
                this.WorkAddressCity = "";
                this.WorkAddressCountry = "";
                this.WorkAddressState = "";
                this.WorkAddressStreet = "";
                this.WorkAddressZipCode = "";
                this.WorkEmail = "";
                this.WorkPhone = "";
            }

            /// <summary>
            /// 
            /// </summary>
            [DataMember]
            public string Company;

            /// <summary>
            /// 
            /// </summary>
            [DataMember]
            public string FirstName;

            /// <summary>
            /// 
            /// </summary>
            [DataMember]
            public string HomeAddressCity;

            /// <summary>
            /// 
            /// </summary>
            [DataMember]
            public string HomeAddressCountry;

            /// <summary>
            /// 
            /// </summary>
            [DataMember]
            public string HomeAddressState;

            /// <summary>
            /// 
            /// </summary>
            [DataMember]
            public string HomeAddressStreet;

            /// <summary>
            /// 
            /// </summary>
            [DataMember]
            public string HomeAddressZipCode;

            /// <summary>
            /// 
            /// </summary>
            [DataMember]
            public string HomePhone;

            /// <summary>
            /// 
            /// </summary>
            [DataMember]
            public string JobTitle;

            /// <summary>
            /// 
            /// </summary>
            [DataMember]
            public string LastName;

            /// <summary>
            /// 
            /// </summary>
            [DataMember]
            public string MiddleName;

            /// <summary>
            /// 
            /// </summary>
            [DataMember]
            public string MobilePhone;

            /// <summary>
            /// 
            /// </summary>
            [DataMember]
            public string Nickname;

            /// <summary>
            /// 
            /// </summary>
            [DataMember]
            public string Notes;

            /// <summary>
            /// 
            /// </summary>
            [DataMember]
            public string OtherEmail;

            /// <summary>
            /// 
            /// </summary>
            [DataMember]
            public string PersonalEmail;

            /// <summary>
            /// 
            /// </summary>
            [DataMember]
            public string Suffix;

            /// <summary>
            /// 
            /// </summary>
            [DataMember]
            public string Title;

            /// <summary>
            /// 
            /// </summary>
            [DataMember]
            public string Website;

            /// <summary>
            /// 
            /// </summary>
            [DataMember]
            public string WorkAddressCity;

            /// <summary>
            /// 
            /// </summary>
            [DataMember]
            public string WorkAddressCountry;

            /// <summary>
            /// 
            /// </summary>
            [DataMember]
            public string WorkAddressState;

            /// <summary>
            /// 
            /// </summary>
            [DataMember]
            public string WorkAddressStreet;

            /// <summary>
            /// 
            /// </summary>
            [DataMember]
            public string WorkAddressZipCode;

            /// <summary>
            /// 
            /// </summary>
            [DataMember]
            public string WorkEmail;

            /// <summary>
            /// 
            /// </summary>
            [DataMember]
            public string WorkPhone;
        }

        private void saveContactTask_Completed(object sender, SaveContactResult e)
        {
            switch (e.TaskResult)
            {
                case TaskResult.OK:
                    // successful save
                    MessageBoxResult res = MessageBox.Show("contact saved", "Alert", MessageBoxButton.OK);
                    break;
                case TaskResult.Cancel:
                    // user cancelled
                    break;
                case TaskResult.None:
                    // no info about result is available
                    break;
            }
        }

        private void contacts_SearchCompleted(object sender, ContactsSearchEventArgs e)
        {
            // TODO: needs to be return results to the JS
            _searchResults = e.Results;

        }

        private void setContactTask(string contact)
        {
            NewContact newContact = JSON.JsonHelper.Deserialize<NewContact>(contact);

            if (newContact != null)
            {
                _contactTask.Company = newContact.Company;
                _contactTask.FirstName = newContact.FirstName;
                _contactTask.HomeAddressCity = newContact.HomeAddressCity;
                _contactTask.HomeAddressCountry = newContact.HomeAddressCountry;
                _contactTask.HomeAddressState = newContact.HomeAddressState;
                _contactTask.HomeAddressStreet = newContact.HomeAddressStreet;
                _contactTask.HomeAddressZipCode = newContact.HomeAddressZipCode;
                _contactTask.HomePhone = newContact.HomePhone;
                _contactTask.JobTitle = newContact.JobTitle;
                _contactTask.LastName = newContact.LastName;
                _contactTask.MiddleName = newContact.MiddleName;
                _contactTask.MobilePhone = newContact.MobilePhone;
                _contactTask.Nickname = newContact.Nickname;
                _contactTask.Notes = newContact.Notes;
                _contactTask.OtherEmail = newContact.OtherEmail;
                _contactTask.PersonalEmail = newContact.PersonalEmail;
                _contactTask.Suffix = newContact.Suffix;
                _contactTask.Title = newContact.Title;
                _contactTask.Website = newContact.Title;
                _contactTask.WorkAddressCity = newContact.WorkAddressCity;
                _contactTask.WorkAddressCountry = newContact.WorkAddressCountry;
                _contactTask.WorkAddressState = newContact.WorkAddressState;
                _contactTask.WorkAddressStreet = newContact.WorkAddressStreet;
                _contactTask.WorkAddressZipCode = newContact.WorkAddressZipCode;
                _contactTask.WorkEmail = newContact.WorkEmail;
                _contactTask.WorkPhone = newContact.WorkPhone;
            }
        }

        private string formatDisplayName(string firstName, string lastName, string middleName)
        {
            string displayName = null;

            firstName = firstName.Trim();
            lastName = lastName.Trim();
            middleName = middleName.Trim();

            if (firstName.Length > 0 || lastName.Length > 0)
            {
                displayName = firstName + " " + lastName;
            }

            if (middleName.Length > 0)
            {
                displayName = firstName + " " + middleName + " " + lastName;
            }

            return displayName;
        }

        // TODO: just use firstName for testing purpose to see if I could create a contact
        // refer here for contact properties we can access: http://msdn.microsoft.com/en-us/library/microsoft.phone.tasks.savecontacttask_members%28v=VS.92%29.aspx
        public void save(string contact)
        {
            setContactTask(contact);

            _contactTask.Show();

            DispatchCommandResult(new PluginResult(PluginResult.Status.OK, "blah"));
        }

        // TODO: we need to be able to pass a search param in.
        public void find(string contact)
        {
            setContactTask(contact);

            string firstName = _contactTask.FirstName;
            string lastName = _contactTask.LastName;
            string middleName = _contactTask.MiddleName;

            string personalEmail = _contactTask.PersonalEmail;
            string workEmail = _contactTask.WorkEmail;

            string homePhone = _contactTask.HomePhone;
            string workPhone = _contactTask.WorkPhone;

            string displayName = formatDisplayName(firstName, lastName, middleName);

            // TODO: need a way to determine which type of Filter is being used...

            if (displayName != null)
            {
                _contacts.SearchAsync(displayName, FilterKind.DisplayName, null);
            }

            // TODO: need to find a way to combine the personalEmail & workEmail search results.
            if (personalEmail != null)
            {
                _contacts.SearchAsync(personalEmail, FilterKind.EmailAddress, null);
            }

            if (workEmail != null)
            {
                _contacts.SearchAsync(workEmail, FilterKind.EmailAddress, null);
            }

            // TODO: need to find a way to combine homePhone & workPhone search results.
            if (homePhone != null)
            {
                _contacts.SearchAsync(homePhone, FilterKind.PhoneNumber, null);
            }

            if (workPhone != null)
            {
                _contacts.SearchAsync(workPhone, FilterKind.PhoneNumber, null);
            }


            //_contacts.SearchAsync(string.Empty, FilterKind.None, null);
        }
    }
}
