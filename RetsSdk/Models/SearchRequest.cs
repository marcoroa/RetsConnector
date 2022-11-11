namespace CrestApps.RetsSdk.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class SearchRequest
    {
        public string SearchType { get; set; }
        public string Class { get; set; }
        public string QueryType { get; set; } = "DMQL2";
        public int Count { get; set; } = 0;
        public string Format { get; set; } = "COMPACT-DECODED"; // COMPACT-DECODED
        public string RestrictedIndicator { get; set; } = "****";
        public int Limit { get; set; } = int.MaxValue;
        public int StandardNames { get; set; } = 0;
        public QueryParameterGroup ParameterGroup { get; set; }
        private List<string> Columns = new List<string>();

        public SearchRequest()
        {
            this.ParameterGroup = new QueryParameterGroup();
        }

        public SearchRequest(string resourceName, string className)
            : this()
        {
            this.SearchType = resourceName;
            this.Class = className;
        }

        public void AddColumn(string columnName)
        {
            if (string.IsNullOrWhiteSpace(columnName) || this.Columns.Contains(columnName, StringComparer.CurrentCultureIgnoreCase))
            {
                return;
            }

            this.Columns.Add(columnName);
        }

        public void AddColumns(IEnumerable<string> columnNames)
        {
            if (columnNames == null)
            {
                return;
            }

            foreach (var columnName in columnNames)
            {
                this.AddColumn(columnName);
            }
        }

        public void RemoveColumn(string columnName)
        {
            this.Columns = this.Columns.Where(x => x != columnName).ToList();
        }

        public void RemoveColumns(IEnumerable<string> columnNames)
        {
            this.Columns = this.Columns.Where(x => !columnNames.Contains(x)).ToList();
        }

        public bool HasColumns()
        {
            return this.Columns.Any();
        }

        public bool HasColumn(string columnName)
        {
            bool exists = this.Columns.Any(x => x.Equals(columnName, StringComparison.CurrentCultureIgnoreCase));

            return exists;
        }

        public IEnumerable<string> GetColumns()
        {
            return this.Columns.Distinct();
        }
    }
}
