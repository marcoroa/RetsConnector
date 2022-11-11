namespace CrestApps.RetsSdk.Models
{
    using System;
    using System.Collections.Generic;

    public class SearchResultRow
    {
        private readonly string restrictedValue;

        public SearchResultRow(string[] columns, string[] values, string primaryKeyColumnName, string restrictedValue)
        {
            if (columns is null)
            {
                throw new ArgumentNullException(nameof(columns));
            }

            if (values is null)
            {
                throw new ArgumentNullException(nameof(values));
            }

            if (string.IsNullOrEmpty(primaryKeyColumnName))
            {
                throw new ArgumentNullException(nameof(primaryKeyColumnName), $"'{nameof(primaryKeyColumnName)}' cannot be null or empty.");
            }

            this.restrictedValue = restrictedValue ?? throw new ArgumentNullException(nameof(restrictedValue));

            var columnLength = columns.Length;

            if (columnLength != values.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(columns), $"Both '{nameof(columns)}' and '{nameof(values)}' must have the same size!");
            }

            int keyIndex = Array.IndexOf(columns, primaryKeyColumnName);
            if (keyIndex == -1)
            {
                throw new IndexOutOfRangeException($"The provided {nameof(primaryKeyColumnName)} is not found in the {nameof(columns)} array.");
            }

            this.PrimaryKeyValue = values[keyIndex];

            for (int index = 0; index < columnLength; index++)
            {
                string rawValue = values[index];
                SearchResultCellValue value = new SearchResultCellValue(rawValue);

                value.SetIsPrimaryKeyValue(keyIndex == index);
                value.SetIsRestricted(this.restrictedValue);

                this.Values.TryAdd(columns[index].ToLower(), value);
            }
        }

        public string PrimaryKeyValue { get; private set; }

        private Dictionary<string, SearchResultCellValue> Values { get; set; } = new Dictionary<string, SearchResultCellValue>();

        public bool IsRestricted(string columnName)
        {
            return this.restrictedValue.Equals(this.Get(columnName));
        }

        public SearchResultCellValue Get(string columnName)
        {
            if (columnName is null)
            {
                throw new ArgumentNullException(nameof(columnName));
            }

            string columnNameLower = columnName.ToLower();
            if (!this.Values.ContainsKey(columnNameLower))
            {
                return null;
            }

            return this.Values[columnNameLower];
        }

        public T? GetValueNullable<T>(string columnName)
            where T : struct
        {
            var cell = this.Get(columnName);

            return cell?.GetNullable<T>();
        }

        public string GetValue(string columnName)
        {
            var cell = this.Get(columnName);

            return cell?.Get();
        }

        public T GetValue<T>(string columnName)
            where T : struct
        {
            var cell = this.Get(columnName);

            if (cell == null)
            {
                throw new Exception("Unable to find the provided column");
            }

            return cell.Get<T>();
        }

        public string GetNullOrValue(string columnName)
        {
            var cell = this.Get(columnName);

            return cell?.NullOrValue();
        }
    }
}
