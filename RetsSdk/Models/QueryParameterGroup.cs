namespace CrestApps.RetsSdk.Models
{
    using System.Collections.Generic;
    using System.Linq;
    using CrestApps.RetsSdk.Models.Enums;

    public class QueryParameterGroup
    {
        public QueryParameterGroup()
        {
            this.Parameters = new List<QueryParameter>();
        }

        public QueryParameterLogicalOperator LogicalOperator { get; set; }

        public List<QueryParameter> Parameters { get; set; }

        public QueryParameterGroup Group { get; set; }

        public void AddParameter(params QueryParameter[] parameters)
        {
            if (parameters == null)
            {
                return;
            }

            foreach (var parameter in parameters)
            {
                this.Parameters.Add(parameter);
            }
        }

        public override string ToString()
        {
            if (this.Parameters == null || !this.Parameters.Any())
            {
                return string.Empty;
            }

            string glue = ",";
            if (this.LogicalOperator == QueryParameterLogicalOperator.Or)
            {
                glue = "|";
            }

            var parameters = this.Parameters.Select(x => x.ToString()).Where(x => !string.IsNullOrWhiteSpace(x));

            string subGroup = this.Group?.ToString();
            if (!string.IsNullOrWhiteSpace(subGroup))
            {
                return string.Format("({0}{1}{2})", string.Join(glue, parameters), glue, subGroup);
            }

            return string.Format("({0})", string.Join(glue, parameters));
        }
    }
}
