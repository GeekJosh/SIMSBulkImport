using System;

namespace UserGen
{
    public class Builder
    {
        public enum UserFields
        {
            Forename,
            Surname,
            AdmissionNo,
            AdmissionYear,
            Year,
            YearOfEntry,
            RegGroup,
            SystemId,
            Increment
        }

        public bool IsValidExpression
        {
            get
            {
                if (!string.IsNullOrEmpty(_expression))
                    if (ExpressionContainsIncrement || ExpressionContainsAdmissionNo || ExpressionContainsSystemId)
                        return true;
                return false;
            }
        }

        public string ExpFieldBuilder(UserFields field)
        {
            switch (field)
            {
                case UserFields.Forename:
                case UserFields.Surname:
                    return "[" + field.ToString() + " (0)]";
                default:
                    return "[" + field.ToString() + "]";
            }
        }

        public string ExpFieldBuilder(UserFields field, int length)
        {
            return "[" + field.ToString() + " (" + length + ")]";
        }

        public bool ExpressionContainsIncrement
        {
            get
            {
                return (_expression.Contains(ExpFieldBuilder(UserFields.Increment)));
            }
        }

        public bool ExpressionContainsAdmissionNo
        {
            get
            {
                return (_expression.Contains(ExpFieldBuilder(UserFields.AdmissionNo)));
            }
        }

        public bool ExpressionContainsSystemId
        {
            get
            {
                return (_expression.Contains(ExpFieldBuilder(UserFields.SystemId)));
            }
        }

        private string _expression;

        public string SetExpression
        {
            set
            {
                _expression = value;
            }
        }

        public string GetExpressionFromLabel(string value)
        {
            string value1;
            value1 = value.Replace(" ", "");
            if (value1 == "PersonID") 
                value1 = "SystemId";
            UserFields field;
            if (Enum.TryParse(value1, out field))
                return ExpFieldBuilder(field);
            return string.Empty;
        }

        public string GetDefaultPupilUsernameUDF
        {
            get
            {
                return "Username";
            }
        }
    }
}
