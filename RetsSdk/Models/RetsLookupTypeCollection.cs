namespace CrestApps.RetsSdk.Models
{
    using System;
    using System.ComponentModel;
    using System.Linq;
    using System.Xml.Linq;

    [Description("METADATA-LOOKUP_TYPE")]
    public class RetsLookupTypeCollection : RetsCollection<RetsLookupType>
    {
        public string Resource { get; set; }
        public string Lookup { get; set; }

        public override void Load(XElement xElement)
        {
            this.Load(this.GetType(), xElement);
        }

        public override RetsLookupType Get(object value)
        {
            RetsLookupType item = this.Get().FirstOrDefault(x => x.ShortValue.Equals(value.ToString(), StringComparison.CurrentCultureIgnoreCase));

            return item;
        }
    }
}
