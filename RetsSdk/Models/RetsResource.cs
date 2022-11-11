namespace CrestApps.RetsSdk.Models
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using CrestApps.RetsSdk.Services;

    public class RetsResource
    {
        public string ResourceId { get; set; }
        public string StandardName { get; set; }
        public string VisibleName { get; set; }
        public string Description { get; set; }
        public string KeyField { get; set; }
        public string ClassCount { get; set; }
        public string ClassVersion { get; set; }
        public DateTime? ClassDate { get; set; }
        public string ObjectVersion { get; set; }
        public DateTime? ObjectDate { get; set; }
        public string SearchHelpVersion { get; set; }
        public DateTime? SearchHelpDate { get; set; }
        public string EditMaskVersion { get; set; }
        public DateTime? EditMaskDate { get; set; }
        public string LookupVersion { get; set; }
        public DateTime? LookupDate { get; set; }
        public string UpdateHelpVersion { get; set; }
        public DateTime? UpdateHelpDate { get; set; }
        public string ValidationExpressionVersion { get; set; }
        public DateTime? ValidationExpressionDate { get; set; }
        public string ValidationLookupVersion { get; set; }
        public DateTime? ValidationLookupDate { get; set; }
        public string ValidationExternalVersion { get; set; }
        public DateTime? ValidationExternalDate { get; set; }

        public RetsClassCollection Classes { get; set; }
        public RetsObjectCollection Objects { get; set; }
        public RetsLookupCollection Lookups { get; set; }

        public async Task<RetsClassCollection> GetClasses(IRetsClient session)
        {
            if (this.Classes == null)
            {
                this.Classes = await session.GetClassesMetadata(this.ResourceId);
            }

            return this.Classes;
        }

        public async Task<RetsClass> GetClass(IRetsClient session, string className)
        {
            var _classes = await this.GetClasses(session);

            RetsClass _class = _classes.Get().FirstOrDefault(x => x.ClassName.Equals(className, StringComparison.CurrentCultureIgnoreCase));

            return _class;
        }
    }
}
