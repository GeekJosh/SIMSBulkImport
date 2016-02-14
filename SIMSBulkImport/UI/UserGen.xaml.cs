using System.Collections.Generic;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using UserGen;
using NLog;

namespace SIMSBulkImport
{
    /// <summary>
    ///     Interaction logic for UserGen.xaml
    /// </summary>
    public partial class UserGen
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        public string UsrExp { get; set; }

        public UserGen()
        {
            InitializeComponent();
            
        }

        private void Label_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            string expression = "";
            Label lbl = (Label)sender;
            expression = Switcher.UserGenClass.GetExpressionFromLabel((string)lbl.Content);
            DragDrop.DoDragDrop(lbl, expression, DragDropEffects.Copy);
        }

        private void expression_TextChanged(object sender, TextChangedEventArgs e)
        {
            Switcher.UserGenClass.SetExpression = this.expression.Text;
            SetIsValidExpression = Switcher.UserGenClass.IsValidExpression;
            EnableDisable();
        }

        private void EnableDisable()
        {
            this.Forename.IsEnabled = !this.expression.Text.Contains("Forename");
            this.Surname.IsEnabled = !this.expression.Text.Contains("Surname");
            this.RegGroup.IsEnabled = !this.expression.Text.Contains("RegGroup");

            this.YearGroup.IsEnabled = !this.expression.Text.Contains("YearGroup");
            this.AdmissionYear.IsEnabled = !this.expression.Text.Contains("AdmissionYear");
            this.EntryYear.IsEnabled = !this.expression.Text.Contains("EntryYear");

            this.SystemId.IsEnabled = !this.expression.Text.Contains("SystemId");
            this.AdmissionNo.IsEnabled = !this.expression.Text.Contains("AdmissionNo");
            this.Increment.IsEnabled = !this.expression.Text.Contains("Increment");

        }

        public string GenerateExampleUsername
        {
            get
            {
                string exp = this.expression.Text;
                Switcher.UserGenClass.SetExpression = exp;
                DataRow _dr             = Switcher.UserGenClass.GetDefaultUserData.Rows[0];
                string Forename = _dr["Forename"].ToString();
                string Surname = _dr["Surname"].ToString();
                string AdmissionNo = _dr["AdmissionNo"].ToString();
                string AdmissionYear = _dr["AdmissionYear"].ToString();
                string YearGroup = _dr["YearGroup"].ToString();
                string RegGroup = _dr["RegGroup"].ToString();
                string SystemId = _dr["SystemId"].ToString();
                string Increment        = "0";
                string result = Switcher.UserGenClass.GenerateUsername(Forename, Surname, AdmissionNo, AdmissionYear, YearGroup, RegGroup, SystemId, Increment);
                return result;
            }
        }

        public bool SetIsValidExpression
        {
            set
            {
                if (value)
                {
                    this.expression.BorderBrush = Brushes.Green;
                    this.exampleText.Text = GenerateExampleUsername;
                    this.nextButton.IsEnabled = true;
                    this.nextButton.Visibility = Visibility.Visible;
                }
                else
                {
                    this.expression.BorderBrush = Brushes.Red;
                    this.exampleText.Text = "";
                    this.nextButton.IsEnabled = false;
                    this.nextButton.Visibility = Visibility.Hidden;
                }
            }
        }

        private void backClick(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new UserUdf());
        }

        private void nextClick(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new UserSet());
        }
    }
}