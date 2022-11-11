namespace CrestApps.RetsSdk.Models
{
    using System.Collections.Generic;
    using System.Linq;
    using System;

    public class SearchResult
    {
        public RetsResource Resource { get; private set; }
        public string ClassName { get; private set; }
        private string RestrictedValue;
        private string[] Columns { get; set; }
        private Dictionary<string, SearchResultRow> Rows { get; set; }

        public SearchResult(RetsResource resource, string className, string restrictedValue)
        {
            this.Columns = new string[] { };
            this.Rows = new Dictionary<string, SearchResultRow>();
            this.Resource = resource ?? throw new ArgumentNullException($"{nameof(resource)} cannot be null.");
            this.ClassName = className ?? throw new ArgumentNullException($"{nameof(className)} cannot be null.");
            this.RestrictedValue = restrictedValue ?? throw new ArgumentNullException($"{nameof(restrictedValue)} cannot be null.");
        }

        public SearchResultRow GetRow(string primaryKeyValue)
        {
            if (this.Rows.ContainsKey(primaryKeyValue))
            {
                return this.Rows[primaryKeyValue];
            }

            return null;
        }

        public bool AddRow(SearchResultRow row)
        {
            if (row == null)
            {
                throw new ArgumentNullException($"{nameof(row)} cannot be null.");
            }

            return this.Rows.TryAdd(row.PrimaryKeyValue, row);
        }

        public bool RemoveRow(string primaryKeyValue)
        {
            if (primaryKeyValue == null)
            {
                throw new ArgumentNullException($"{nameof(primaryKeyValue)} cannot be null.");
            }

            if (this.Rows.ContainsKey(primaryKeyValue))
            {
                return this.Rows.Remove(primaryKeyValue);
            }

            return false;
        }
        public bool RemoveRow(SearchResultRow row)
        {
            if (row == null)
            {
                throw new ArgumentNullException($"{nameof(row)} cannot be null.");
            }

            return this.Rows.Remove(row.PrimaryKeyValue);
        }

        public IEnumerable<SearchResultCellValue> Pluck(string columnName)
        {
            var values = this.Rows.Select(x => x.Value.Get(columnName));

            return values;
        }

        public IEnumerable<T> Pluck<T>(string columnName)
            where T : struct
        {
            IEnumerable<T> values = this.Rows.Select(x => x.Value.Get(columnName).Get<T>());

            return values;
        }

        public IEnumerable<T?> PluckNullable<T>(string columnName)
            where T : struct
        {
            IEnumerable<T?> values = this.Rows.Select(x => x.Value.Get(columnName).GetNullable<T>());

            return values;
        }

        public IEnumerable<SearchResultRow> GetRows()
        {
            return this.Rows.Select(x => x.Value);
        }

        public IEnumerable<string> GetColumns()
        {
            return this.Columns;
        }

        public void SetColumns(string[] columns)
        {
            this.Columns = columns ?? throw new ArgumentNullException($"{nameof(columns)} cannot be null.");
        }

        public void SetColumns(IEnumerable<string> columns)
        {
            this.SetColumns(columns?.AsEnumerable());
        }

        public bool IsRestricted(string value)
        {
            return this.RestrictedValue.Equals(value);
        }

        public int Count()
        {
            return this.Rows.Count();
        }
    }
}
