namespace CrestApps.RetsSdk.Models
{
    using System;
    using System.ComponentModel;
    using System.Linq;
    using System.Xml.Linq;

    [Description("METADATA-LOOKUP")]
    public class RetsLookupCollection : RetsCollection<RetsLookup>
    {
        public string Resource { get; set; }
        public RetsLookupTypeCollection LookupTypes { get; set; }

        public override void Load(XElement xElement)
        {
            this.Load(this.GetType(), xElement);
        }

        public override RetsLookup Get(object value)
        {
            RetsLookup item = this.Get().FirstOrDefault(x => x.LookupName.Equals(value.ToString(), StringComparison.CurrentCultureIgnoreCase));

            return item;
        }
    }
}
