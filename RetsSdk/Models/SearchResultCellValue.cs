namespace CrestApps.RetsSdk.Models
{
    using CrestApps.RetsSdk.Helpers.Extensions;

    public class SearchResultCellValue
    {
        public SearchResultCellValue(string value)
        {
            this.Value = value;
        }

        public bool IsPrimaryKeyValue { get; private set; }
        public bool IsRestricted { get; private set; }
        private string Value { get; set; }

        public void SetIsPrimaryKeyValue(bool isPrimaryKeyValue)
        {
            this.IsPrimaryKeyValue = isPrimaryKeyValue;
        }

        public void SetIsRestricted(bool isRestricted)
        {
            this.IsRestricted = isRestricted;
        }

        public void SetIsRestricted(string restrectedValue)
        {
            this.IsRestricted = this.Value?.Equals(restrectedValue) ?? false;
        }

        public string NullOrValue()
        {
            if (this.IsNullOrWhiteSpace())
            {
                return null;
            }

            return this.Value;
        }

        public string EmptyOrValue()
        {
            return this.Value ?? string.Empty;
        }

        public string Get()
        {
            return this.Value;
        }

        public T Get<T>()
          where T : struct
        {
            return this.TryCastValue<T>();
        }

        public string GetTrimmed()
        {
            return this.Value?.Trim();
        }

        public T? GetNullable<T>()
            where T : struct
        {
            return this.TryCastValueNullable<T>();
        }

        public T? TryCastValueNullable<T>()
            where T : struct
        {
            if (this.IsNullOrWhiteSpace())
            {
                return null;
            }

            return this.TryCastValue<T>();
        }

        public T TryCastValue<T>()
             where T : struct
        {
            object safeValue = typeof(T).GetSafeObject(this.Value);

            return (T)safeValue;
        }

        public bool IsNull()
        {
            return this.Value == null;
        }

        public bool IsNullOrEmpty()
        {
            return string.IsNullOrEmpty(this.Value);
        }

        public bool IsNullOrWhiteSpace()
        {
            return string.IsNullOrWhiteSpace(this.Value);
        }
    }
}
