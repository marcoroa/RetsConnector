namespace CrestApps.RetsSdk.Models
{
    public class QueryParameter
    {
        public string FieldName { get; set; }
        public string Value { get; set; }

        public QueryParameter()
        {
        }

        public QueryParameter(string fieldName, string value)
        {
            this.FieldName = fieldName;
            this.Value = value;
        }

        public override string ToString()
        {
            if (string.IsNullOrWhiteSpace(this.FieldName))
            {
                return string.Empty;
            }

            return string.Format("({0}={1})", this.FieldName, this.Value);
        }
    }
}
