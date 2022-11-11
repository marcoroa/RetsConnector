namespace CrestApps.RetsSdk.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using CrestApps.RetsSdk.Services;

    [Description("METADATA-TABLE")]
    public class RetsFieldCollection : RetsCollection<RetsField>
    {
        public string Resource { get; set; }
        public IEnumerable<RetsLookupTypeCollection> LookupTypes { get; set; }

        public override void Load(XElement xElement)
        {
            this.Load(this.GetType(), xElement);
        }

        public override RetsField Get(object value)
        {
            RetsField item = this.Get().FirstOrDefault(x => x.SystemName.Equals(value.ToString(), StringComparison.CurrentCultureIgnoreCase));

            return item;
        }

        public async Task<IEnumerable<RetsLookupTypeCollection>> GetLookupTypes(IRetsClient session)
        {
            if (this.LookupTypes == null)
            {
                this.LookupTypes = await session.GetLookupValues(this.Resource);
            }

            return this.LookupTypes;
        }

        public async Task<RetsLookupTypeCollection> GetLookupType(IRetsClient session, string lookupName)
        {
            var lookupTypes = await this.GetLookupTypes(session);

            RetsLookupTypeCollection lookupType = lookupTypes.FirstOrDefault(x => x.Lookup.Equals(lookupName, StringComparison.CurrentCultureIgnoreCase));

            return lookupType;
        }
    }
}
