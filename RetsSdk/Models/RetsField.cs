namespace CrestApps.RetsSdk.Models
{
    using System;
    using System.Threading.Tasks;
    using CrestApps.RetsSdk.Models.Enums;
    using CrestApps.RetsSdk.Services;

    public class RetsField
    {
        public string MetadataEntryId { get; set; }
        public string SystemName { get; set; }
        public string StandardName { get; set; }
        public string LongName { get; set; }
        public string DbName { get; set; }
        public string ShortName { get; set; }
        public int MaximumLength { get; set; }
        public RetsDataType? DataType { get; set; }
        public int? Precision { get; set; }
        public bool Searchable { get; set; }
        public string Interpretation { get; set; }
        public RetsAlignment? Alignment { get; set; }
        public bool UseSeparator { get; set; }
        public string EditMaskId { get; set; }
        public string LookupName { get; set; }
        public int? MaxSelect { get; set; }
        public RetsUnit? Units { get; set; }
        public bool Index { get; set; }
        public decimal? Minimum { get; set; }
        public decimal? Maximum { get; set; }
        public bool Default { get; set; }
        public bool Required { get; set; }
        public string SearchHelpId { get; set; }
        public bool Unique { get; set; }
        public bool ModTimeStamp { get; set; }
        public bool InKeyIndex { get; set; }

        public RetsLookupTypeCollection LookupTypes { get; set; }

        public async Task<RetsLookupTypeCollection> GetLookupTypes(IRetsClient session, string resourceId)
        {
            if (this.LookupTypes == null && this.Interpretation.StartsWith("Lookup", StringComparison.CurrentCultureIgnoreCase))
            {
                this.LookupTypes = await session.GetLookupValues(resourceId, this.SystemName);
            }

            return this.LookupTypes;
        }
    }
}
