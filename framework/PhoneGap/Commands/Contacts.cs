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
using System.Runtime.Serialization;


namespace WP7GapClassLib.PhoneGap.Commands
{
    public class Contacts : BaseCommand
    {
        SaveContactTask contactTask;
        Microsoft.Phone.UserData.Contacts contacts;

        public Contacts()
        {
            contactTask = new SaveContactTask();
            contactTask.Completed += new EventHandler<SaveContactResult>(saveContactTask_Completed);

            contacts = new Microsoft.Phone.UserData.Contacts();
            contacts.SearchCompleted += new EventHandler<ContactsSearchEventArgs>(contacts_SearchCompleted);
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
            // TODO: needs to be modified to take dynamic search param


            string firstName = contactTask.FirstName.Trim();
            string lastName = contactTask.LastName.Trim();
            string middleName = contactTask.MiddleName.Trim();

            if (firstName.Length > 0 || lastName.Length > 0)
            {
                string displayName = formatDisplayName(firstName, lastName, middleName);
                searchByDisplayName(e, displayName);
            }

        }

        private string formatDisplayName(string firstName, string lastName, string middleName)
        {
            string displayName = firstName + " " + lastName;

            if (middleName.Length > 0)
            {
                displayName = firstName + " " + middleName + " " + lastName;
            }

            return displayName;
        }

        private Contact searchByDisplayName(ContactsSearchEventArgs e, string displayName)
        {
            foreach (var result in e.Results)
            {
                if (result.DisplayName.Contains(displayName))
                {
                    // TODO: need to be able to return the matching contact(s) to the JS instead of using this MessageBox
                    MessageBoxResult res = MessageBox.Show("contact: " + result.DisplayName + " found", "Alert", MessageBoxButton.OK);
                    return result;
                }
            }

            // nothing found return null
            return null;
        }

        // TODO: just use firstName for testing purpose to see if I could create a contact
        // refer here for contact properties we can access: http://msdn.microsoft.com/en-us/library/microsoft.phone.tasks.savecontacttask_members%28v=VS.92%29.aspx
        public void create(string contact)
        {
            NewContact newContact = JSON.JsonHelper.Deserialize<NewContact>(contact);

            contactTask.Company = newContact.Company;
            contactTask.FirstName = newContact.FirstName;
            contactTask.HomeAddressCity = newContact.HomeAddressCity;
            contactTask.HomeAddressCountry = newContact.HomeAddressCountry;
            contactTask.HomeAddressState = newContact.HomeAddressState;
            contactTask.HomeAddressStreet = newContact.HomeAddressStreet;
            contactTask.HomeAddressZipCode = newContact.HomeAddressZipCode;
            contactTask.HomePhone = newContact.HomePhone;
            contactTask.JobTitle = newContact.JobTitle;
            contactTask.LastName = newContact.LastName;
            contactTask.MiddleName = newContact.MiddleName;
            contactTask.MobilePhone = newContact.MobilePhone;
            contactTask.Nickname = newContact.Nickname;
            contactTask.Notes = newContact.Notes;
            contactTask.OtherEmail = newContact.OtherEmail;
            contactTask.PersonalEmail = newContact.PersonalEmail;
            contactTask.Suffix = newContact.Suffix;
            contactTask.Title = newContact.Title;
            contactTask.Website = newContact.Title;
            contactTask.WorkAddressCity = newContact.WorkAddressCity;
            contactTask.WorkAddressCountry = newContact.WorkAddressCountry;
            contactTask.WorkAddressState = newContact.WorkAddressState;
            contactTask.WorkAddressStreet = newContact.WorkAddressStreet;
            contactTask.WorkAddressZipCode = newContact.WorkAddressZipCode;
            contactTask.WorkEmail = newContact.WorkEmail;
            contactTask.WorkPhone = newContact.WorkPhone;

            contactTask.Show();

            DispatchCommandResult(new PluginResult(PluginResult.Status.OK, "blah"));
        }

        // TODO: we need to be able to pass a search param in.
        public void find(string contact)
        {
            NewContact newContact = JSON.JsonHelper.Deserialize<NewContact>(contact);

            contactTask.Company = newContact.Company;
            contactTask.FirstName = newContact.FirstName;
            contactTask.HomeAddressCity = newContact.HomeAddressCity;
            contactTask.HomeAddressCountry = newContact.HomeAddressCountry;
            contactTask.HomeAddressState = newContact.HomeAddressState;
            contactTask.HomeAddressStreet = newContact.HomeAddressStreet;
            contactTask.HomeAddressZipCode = newContact.HomeAddressZipCode;
            contactTask.HomePhone = newContact.HomePhone;
            contactTask.JobTitle = newContact.JobTitle;
            contactTask.LastName = newContact.LastName;
            contactTask.MiddleName = newContact.MiddleName;
            contactTask.MobilePhone = newContact.MobilePhone;
            contactTask.Nickname = newContact.Nickname;
            contactTask.Notes = newContact.Notes;
            contactTask.OtherEmail = newContact.OtherEmail;
            contactTask.PersonalEmail = newContact.PersonalEmail;
            contactTask.Suffix = newContact.Suffix;
            contactTask.Title = newContact.Title;
            contactTask.Website = newContact.Title;
            contactTask.WorkAddressCity = newContact.WorkAddressCity;
            contactTask.WorkAddressCountry = newContact.WorkAddressCountry;
            contactTask.WorkAddressState = newContact.WorkAddressState;
            contactTask.WorkAddressStreet = newContact.WorkAddressStreet;
            contactTask.WorkAddressZipCode = newContact.WorkAddressZipCode;
            contactTask.WorkEmail = newContact.WorkEmail;
            contactTask.WorkPhone = newContact.WorkPhone;

            contacts.SearchAsync(string.Empty, FilterKind.None, null);
        }
    }
}
