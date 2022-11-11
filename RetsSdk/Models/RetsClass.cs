namespace CrestApps.RetsSdk.Models
{
    using System;
    using System.Threading.Tasks;
    using CrestApps.RetsSdk.Services;

    public class RetsClass
    {
        public string Resource { get; set; }
        public string ClassName { get; set; }
        public string StandardName { get; set; }
        public string VisibleName { get; set; }
        public string Description { get; set; }
        public string TableVersion { get; set; }
        public DateTime? TableDate { get; set; }
        public string UpdateVersion { get; set; }
        public DateTime? UpdateDate { get; set; }

        public RetsFieldCollection Fields { get; set; }

        public async Task<RetsFieldCollection> GetFields(IRetsClient session)
        {
            if (this.Fields == null)
            {
                this.Fields = await session.GetTableMetadata(this.Resource, this.ClassName);
            }

            return this.Fields;
        }
    }
}
