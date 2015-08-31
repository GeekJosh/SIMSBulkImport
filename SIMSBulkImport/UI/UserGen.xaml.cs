using System.Collections.Generic;
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

        public UserGen(DataTable defaultUserData, string[] Years)
        {
            _defaultUserData = defaultUserData;
            InitializeComponent();
            _builder = new Builder();
            _builder.SetSchoolYearGroups = Years;
            _builder.SetYearStart = "2015-09-01";
        }

        private void Label_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            string expression = "";
            Label lbl = (Label)sender;
            expression = _builder.GetExpressionFromLabel((string)lbl.Content);
            DragDrop.DoDragDrop(lbl, expression, DragDropEffects.Copy);
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

        public string GenerateExampleUsername
        {
            get
            {
                string exp = this.expression.Text;
                _builder.SetExpression = exp;
                DataRow _dr             = _defaultUserData.Rows[0];
                string Forename = (string)_dr["Forename"];
                string Surname = (string)_dr["Surname"];
                string AdmissionNo = (string)_dr["AdmissionNo"];
                string AdmissionYear = (string)_dr["AdmissionYear"];
                string YearGroup = (string)_dr["YearGroup"];
                string EntryYear = (string)_dr["EntryYear"];
                string RegGroup = (string)_dr["RegGroup"];
                string SystemId = (string)_dr["SystemId"];
                string Increment        = "0";
                string result = _builder.GenerateUsername(Forename, Surname, AdmissionNo, AdmissionYear, YearGroup, EntryYear, RegGroup, SystemId, Increment);
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
            Switcher.Switch(new UserFilter());
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