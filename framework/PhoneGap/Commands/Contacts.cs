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


namespace WP7GapClassLib.PhoneGap.Commands
{
    public class Contacts : BaseCommand
    {
        SaveContactTask saveContactTask;
        Microsoft.Phone.UserData.Contacts contacts;

        public Contacts()
        {
            saveContactTask = new SaveContactTask();
            saveContactTask.Completed += new EventHandler<SaveContactResult>(saveContactTask_Completed);

            contacts = new Microsoft.Phone.UserData.Contacts();
            contacts.SearchCompleted += new EventHandler<ContactsSearchEventArgs>(contacts_SearchCompleted);
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
            
            foreach (var result in e.Results)
            {
                if (result.DisplayName.Contains("herm"))
                {
                    // TODO: need to be able to return the matching contact(s) to the JS instead of using this MessageBox
                    MessageBoxResult res = MessageBox.Show("contact found", "Alert", MessageBoxButton.OK);                   
                    break;
                }
            }

        }

        // TODO: just use firstName for testing purpose to see if I could create a contact
        // refer here for contact properties we can access: http://msdn.microsoft.com/en-us/library/microsoft.phone.tasks.savecontacttask_members%28v=VS.92%29.aspx
        public void create(string firstName)
        {
            saveContactTask.FirstName = firstName;
            saveContactTask.Show();
        }

        // TODO: we need to be able to pass a search param in.
        public void find(string searchParam)
        {
            contacts.SearchAsync(string.Empty, FilterKind.None, null);

        }
    }
}
