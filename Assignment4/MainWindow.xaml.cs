using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using hairdresserShop;


namespace Assignment4
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Client client = null;
        int customerType = 0;
        public MainWindow()
        {
            File.Delete("clientDetails.bin");
            InitializeComponent();
            AddComboBoxItems();
        }

        List<string> appointmentSlot = new List<string>();
        private void AddComboBoxItems()
        {
            string[] customers = Enum.GetNames(typeof(Customer));
            foreach (string customer in customers)
            {
                cmbCustomerType.Items.Add(customer);
            }
            appointmentSlot.Clear();
            appointmentSlot.Add("9AM to 10AM ");
            appointmentSlot.Add("10AM to 11AM ");
            appointmentSlot.Add("11AM to 12PM ");
            appointmentSlot.Add("12PM to 01PM ");
            appointmentSlot.Add("01PM to 02PM ");
            appointmentSlot.Add("02PM to 03PM ");
            appointmentSlot.Add("03PM to 04PM ");
            appointmentSlot.Add("04PM to 05PM ");
            appointmentSlot.Add("05PM to 06PM ");
            AppointmentSlotUpdate();
            cmbAdditional.Items.Add("Yes");
            cmbAdditional.Items.Add("No");
            cmbAdditional1.Items.Add("Yes");
            cmbAdditional1.Items.Add("No");
        }

        private void AppointmentSlotUpdate()
        {
            cmbSlot.Items.Clear();
            foreach (string appointment in appointmentSlot)
            {
                cmbSlot.Items.Add(appointment);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            
            string name = txtName.Text;
            bool invalidInput = false;
            if (name.Length > 0)
            {
                txtName.Foreground = Brushes.Black;
            }
            else
            {
                invalidInput = true;
                txtName.Foreground = Brushes.Red;
            }
                       
            int age = 0;
            string ageString = txtAge.Text;
            if (int.TryParse(ageString, out age) && age > 0 && age <= 99)
            {
                txtAge.Foreground = Brushes.Black;

            }
            else
            {
                invalidInput = true;
                txtAge.Foreground = Brushes.Red;
            }

            int height = 0;
            string heightString = txtHeight.Text;
           if (int.TryParse(heightString, out height) && height >= 10 && height <= 96)
            {
                txtHeight.Foreground = Brushes.Black;

            }
            else
            {
                invalidInput = true;
                txtHeight.Foreground = Brushes.Red;
            }
            if (cmbCustomerType.SelectedIndex != -1)
            {
                //invalidInput = false;
                cmbAdditional.Foreground = Brushes.Black;
                cmbAdditional1.Foreground = Brushes.Black;




                switch (customerType)
                {
                    case (int)Customer.Gentlemen:
                        client = new Gentlemen();
                        if (cmbAdditional.SelectedIndex != -1 && cmbAdditional1.SelectedIndex != -1)
                        {
                            ((Gentlemen)client).IsTrimmingNeeded = (cmbAdditional.Text.ToUpper() == "YES") ? true : false;
                            ((Gentlemen)client).Moustaches = (cmbAdditional1.Text.ToUpper() == "YES") ? true : false;
                            client.Slot = SelectAppoinmentSlot();
                        }
                        else
                        {
                            MessageBox.Show("Select additional work option");
                            if (cmbAdditional.SelectedIndex != -1 )
                            {
                                cmbAdditional.Foreground = Brushes.Red;
                            }
                            if ( cmbAdditional1.SelectedIndex != -1)
                            {
                                cmbAdditional1.Foreground = Brushes.Red;
                            }
                        }
                        break;
                    case (int)Customer.Ladies:
                        client = new Ladies();
                        if (cmbAdditional.SelectedIndex != -1)
                        {
                            ((Ladies)client).HairStyle = (cmbAdditional.Text.ToUpper() == "YES") ? true : false;
                            client.Slot = SelectAppoinmentSlot();
                        }
                        else
                        {
                            MessageBox.Show("Select additional work option");
                            if (cmbAdditional.SelectedIndex != -1)
                            {
                                cmbAdditional.Foreground = Brushes.Red;

                            }
                        }
                        break;
                    case (int)Customer.Children:
                        client = new Children();
                        if (cmbAdditional.SelectedIndex != -1 && cmbAdditional1.SelectedIndex != -1)
                        {
                            ((Children)client).SensitiveTrimmers = (cmbAdditional.Text.ToUpper() == "YES") ? true : false;
                            ((Children)client).AdjustableSeats = (cmbAdditional1.Text.ToUpper() == "YES") ? true : false;
                            client.Slot = SelectAppoinmentSlot();
                        }
                        else
                        {
                            MessageBox.Show("Select additional work option");
                            if (cmbAdditional.SelectedIndex != -1)
                            {
                                cmbAdditional.Foreground = Brushes.Red;
                            }
                            if (cmbAdditional1.SelectedIndex != -1)
                            {
                                cmbAdditional1.Foreground = Brushes.Red;
                            }
                        }
                        break;
                }
            }
            else
            {
                MessageBox.Show("Select one customer type.");
            }

            string creditCard = string.Empty;
            if(!ValidCreditCard()) {
                invalidInput = true;
            }
            else
            {
                creditCard = txtCreditCard.Text;
            }
            if(!invalidInput)
            {
                client.Name = name;
                client.Height = height;
                client.Age = age;
                client.CreditCardNumber = creditCard;
                WriteToFile(customerType, client);
                MessageBox.Show("Updated client to file");
                AppointmentSlotUpdate();
                txtName.Text = "";
                txtAge.Text = "";
                txtHeight.Text = "";
                txtCreditCard.Text = "";
                cmbSlot.SelectedIndex = -1;
                cmbCustomerType.SelectedIndex = -1;
                cmbAdditional.SelectedIndex = -1;
                cmbAdditional1.SelectedIndex = -1;

            }
            else
            {
                MessageBox.Show("Invalid inputs");
                client = null;
            }
        }

        private void WriteToFile(int customerType, IClient client)
        {
            FileStream fs = null;
            try
            {
                fs = new FileStream("clientDetails.bin", FileMode.OpenOrCreate | FileMode.Append, FileAccess.Write);
                BinaryWriter bw = new BinaryWriter(fs);
                bw.Write(customerType);
                bw.Write(client.Name);
                bw.Write(client.Age);
                bw.Write(client.Height);
                bw.Write(client.CreditCardNumber);
                bw.Write(client.Slot);
                switch (customerType)
                {
                    case (int)Customer.Gentlemen:
                        bw.Write(((Gentlemen)client).IsTrimmingNeeded);
                        bw.Write(((Gentlemen)client).Moustaches);
                        break;
                    case (int)Customer.Ladies:
                        bw.Write(((Ladies)client).HairStyle);
                        break;
                    case (int)Customer.Children:
                        bw.Write(((Children)client).SensitiveTrimmers);
                        bw.Write(((Children)client).AdjustableSeats);
                        break;
                }
                bw.Close();
                fs.Close();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            finally
            {
                if (fs != null)
                {
                    fs.Close();
                }
            }
        }
        AppointmentList myList = new AppointmentList();
        private void ReadFile()
        {

            FileStream fs = null;
            try
            {
                using (BinaryReader br = new BinaryReader(File.Open("clientDetails.bin", FileMode.Open)))
                {
                    while (br.BaseStream.Position != br.BaseStream.Length)
                    {
                        Client client = null;
                        int customerType = br.ReadInt32();
                        switch (customerType)
                        {
                            case (int)Customer.Gentlemen:
                                client = new Gentlemen();
                                break;
                            case (int)Customer.Ladies:
                                client = new Ladies();
                                break;
                            case (int)Customer.Children:
                                client = new Children();
                                break;
                        }
                        client.Name = br.ReadString();
                        client.Age = br.ReadInt32();
                        client.Height = br.ReadDecimal();
                        client.CreditCardNumber = br.ReadString();
                        client.Slot = br.ReadString();
                        switch (customerType)
                        {
                            case (int)Customer.Gentlemen:
                                ((Gentlemen)client).IsTrimmingNeeded = (br.ReadBoolean()) ? true : false;
                                ((Gentlemen)client).Moustaches = (br.ReadBoolean()) ? true : false;
                                break;
                            case (int)Customer.Ladies:
                                ((Ladies)client).HairStyle = (br.ReadBoolean()) ? true : false;
                                break;
                            case (int)Customer.Children:
                                ((Children)client).SensitiveTrimmers = (br.ReadBoolean()) ? true : false;
                                ((Children)client).AdjustableSeats = (br.ReadBoolean()) ? true : false;
                                break;
                        }
                        myList.Add(client);

                    }
                }

            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            finally
            {
                if (fs != null)
                {
                    fs.Close();
                }
            }
        }

        private string SelectAppoinmentSlot()
        {
            int slotNumber = cmbSlot.SelectedIndex + 1;
            string slot = string.Empty;
            //slot selected
            slot = appointmentSlot[slotNumber - 1];
            //booked appointment remove from available slots
            appointmentSlot.Remove(slot);
            return slot;
        }

        private bool ValidCreditCard()
        {
            string creditCard = txtCreditCard.Text;
            bool isInCorrect = false;

            if (creditCard != string.Empty && creditCard != null && creditCard.Length == 16)
            {
                char[] cardArray = creditCard.ToCharArray();

                foreach (char c in cardArray)
                {
                    if (!char.IsDigit(c))
                    {
                        isInCorrect = true;
                        break;
                    }
                }
            }
            else
            {
                isInCorrect = true;
            }
            if (isInCorrect)
            {
                txtHeight.Foreground = Brushes.Red;
            }
            else
            {
                txtHeight.Foreground = Brushes.Black;

            }

            return !isInCorrect;
        }

        
        private void CmbCustomerType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            customerType = cmbCustomerType.SelectedIndex + 1;
            switch (customerType)
            {
                case (int)Customer.Gentlemen:
                    lblAdditional.Visibility = Visibility.Visible;
                    lblAdditional1.Visibility = Visibility.Visible;
                    cmbAdditional.Visibility = Visibility.Visible;
                    cmbAdditional1.Visibility = Visibility.Visible;
                    lblAdditional.Content = "Trimming?";
                    lblAdditional1.Content = "Moustaches?";
                    break;
                case (int)Customer.Ladies:
                    lblAdditional.Visibility = Visibility.Visible;
                    lblAdditional1.Visibility = Visibility.Hidden;
                    cmbAdditional.Visibility = Visibility.Visible;
                    cmbAdditional1.Visibility = Visibility.Hidden;
                    lblAdditional.Content = "Hairstyle?";
                    break;
                case (int)Customer.Children:
                    lblAdditional.Visibility = Visibility.Visible;
                    lblAdditional1.Visibility = Visibility.Visible;
                    cmbAdditional.Visibility = Visibility.Visible;
                    cmbAdditional1.Visibility = Visibility.Visible;
                    lblAdditional.Content = "Sensitive Trimmer?";
                    lblAdditional1.Content = "Adjustale seat?";
                    break;
            }
        }

        private void BtnShow_Click(object sender, RoutedEventArgs e)
        {
            myList = new AppointmentList();
            txtDisplay.Text = "";
            ReadFile();
            myList.Sort();
            foreach (Client c in myList)
            {
                txtDisplay.Text+=(c.ToString());
            }
        }
    }
}
