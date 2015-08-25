/*
 * Developer : Matt Smith (matt@matt40k.co.uk)
 * All code (c) Matthew Smith all rights reserved
 */

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

        public UserGen()
        {
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
            if (_builder.IsValidExpression)
                this.expression.BorderBrush = Brushes.Green;
            else
                this.expression.BorderBrush = Brushes.Red;
            EnableDisable();
        }

        private void EnableDisable()
        {
            this.Forename.IsEnabled = !this.expression.Text.Contains("Forename");
            this.Surname.IsEnabled = !this.expression.Text.Contains("Surname");
            this.RegGroup.IsEnabled = !this.expression.Text.Contains("RegGroup");

            this.Year.IsEnabled = !this.expression.Text.Contains("Year");
            this.AdmissionYear.IsEnabled = !this.expression.Text.Contains("AdmissionYear");
            this.YearOfEntry.IsEnabled = !this.expression.Text.Contains("YearaOfEntry");

            this.SystemID.IsEnabled = !this.expression.Text.Contains("SystemID");
            this.AdmissionNo.IsEnabled = !this.expression.Text.Contains("AdmissionNo");
            this.Increment.IsEnabled = !this.expression.Text.Contains("Increment");
            
        }

/*
        private void buttonClick(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(new Menu());
        }
 */
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