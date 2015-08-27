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
            YearGroup,
            EntryYear,
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

        public int GetLength(UserFields field)
        {
            int len = 0;
            string defaultItem = ExpFieldBuilder(field);
            string fieldStartName = defaultItem.Substring(0, defaultItem.Length - 3);

            int noStart = defaultItem.Length - 3;
            int fieldStart = _expression.IndexOf(fieldStartName, 0);

            string lenStr = _expression.Substring((fieldStart + noStart), 1);

            // Should really think about wrapping this in some error handling incase the
            // clever end user decides to enter a non-number 
            Int32.TryParse(lenStr, out len);  

            return len;
        }

        public string GenerateUsername(string Forename, string Surname,
            string AdmissionNo, string AdmissionYear,
            string YearGroup, string EntryYear,
            string RegGroup, string SystemId,
            string Increment
            )
        {
            string exp = _expression;

            // We don't want to explictly set the incremental number unless
            // its greater then 0 - ie we dont want all our usernames with a zero
            string incrementalNo = "";
            int no = 0;
            Int32.TryParse(Increment, out no);
            if (no > 0)
                incrementalNo = no.ToString();

            string _forenameExp;
            string _surnameExp;

            // Set everything as lowercase
            string _forename = Forename.ToLower();
            string _surname = Surname.ToLower();
            string _yearGroup = YearGroup.ToLower();

            int _forenameLen = GetLength(UserFields.Forename);
            int _surnameLen = GetLength(UserFields.Surname);

            if (_forenameLen > 0)
            {
                _forename = _forename.Substring(0, _forenameLen);
                _forenameExp = ExpFieldBuilder(UserFields.Forename).Substring(0, (ExpFieldBuilder(UserFields.Forename).Length - 3)) + _forenameLen + ")]";
            }
            else
                _forenameExp = ExpFieldBuilder(UserFields.Forename);

            if (_surnameLen > 0)
            {
                _surname = _surname.Substring(0, _surnameLen);
                _surnameExp = ExpFieldBuilder(UserFields.Surname).Substring(0, (ExpFieldBuilder(UserFields.Surname).Length - 3)) + _surnameLen + ")]";
            }
            else
                _surnameExp = ExpFieldBuilder(UserFields.Surname);

            // Clean up the YearGroup
            _yearGroup = _yearGroup.Replace(" ", "");
            _yearGroup = _yearGroup.Replace("year", "");


            exp = exp.Replace(_forenameExp, _forename);
            exp = exp.Replace(_surnameExp, _surname);
            exp = exp.Replace(ExpFieldBuilder(UserFields.AdmissionNo), AdmissionNo.ToLower());
            exp = exp.Replace(ExpFieldBuilder(UserFields.AdmissionYear), AdmissionYear.ToLower());
            exp = exp.Replace(ExpFieldBuilder(UserFields.YearGroup), _yearGroup);
            exp = exp.Replace(ExpFieldBuilder(UserFields.EntryYear), EntryYear.ToLower());
            exp = exp.Replace(ExpFieldBuilder(UserFields.RegGroup), RegGroup.ToLower());
            exp = exp.Replace(ExpFieldBuilder(UserFields.SystemId), SystemId.ToLower());
            exp = exp.Replace(ExpFieldBuilder(UserFields.Increment), incrementalNo.ToLower());
            return exp;
        }
    }
}
