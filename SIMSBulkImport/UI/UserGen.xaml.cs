/*
 * Developer : Matt Smith (matt@matt40k.co.uk)
 * All code (c) Matthew Smith all rights reserved
 */

using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using UserGen;
using NLog;

namespace Matt40k.SIMSBulkImport
{
    /// <summary>
    ///     Interaction logic for UserGen.xaml
    /// </summary>
    public partial class UserGen
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private Builder _builder;
        public string UsrExp { get; set; }
        private DataTable _defaultUserData;

        public UserGen(DataTable defaultUserData)
        {
            _defaultUserData = defaultUserData;
            InitializeComponent();
            _builder = new Builder();
        }

        private void Label_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            string expression = "";
            Label lbl = (Label)sender;
            expression = _builder.GetExpressionFromLabel((string)lbl.Content);
            DragDrop.DoDragDrop(lbl, expression, DragDropEffects.Copy);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(UsrExp);
        }

        private void expression_TextChanged(object sender, TextChangedEventArgs e)
        {
            _builder.SetExpression = this.expression.Text;
            SetIsValidExpression = _builder.IsValidExpression;
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

        /*
                private void buttonClick(object sender, RoutedEventArgs e)
                {
                    Switcher.Switch(new Menu());
                }
         */
        public string GenerateExampleUsername
        {
            get
            {
                string exp = this.expression.Text;
                _builder.SetExpression = exp;
                MessageBox.Show(exp);

                DataRow _dr             = _defaultUserData.Rows[0];
                string Forename = "Bruce";
                string Surname = "Wayne";
                string AdmissionNo = (string)_dr["AdmissionNo"];
                string AdmissionYear = (string)_dr["AdmissionYear"];
                string YearGroup = (string)_dr["YearGroup"];
                string EntryYear = (string)_dr["EntryYear"];
                string RegGroup = (string)_dr["RegGroup"];
                string SystemId = (string)_dr["SystemId"];
                string Increment        = "0";
                
                string result = _builder.GenerateUsername(Forename, Surname, AdmissionNo, AdmissionYear, YearGroup, EntryYear, RegGroup, SystemId, Increment);
                MessageBox.Show(result);
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
                }
                else
                {
                    this.expression.BorderBrush = Brushes.Red;
                    this.exampleText.Text = "";
                }
            }
        }
    }


    public class NameValidator : ValidationRule
    {
        public override ValidationResult Validate
          (object value, System.Globalization.CultureInfo cultureInfo)
        {
            if (value == null)
                return new ValidationResult(false, "value cannot be empty.");
            else
            {
                if (value.ToString().Length > 3)
                    return new ValidationResult
                    (false, "Name cannot be more than 3 characters long.");
            }
            return ValidationResult.ValidResult;
        }
    }
}