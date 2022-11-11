namespace CrestApps.RetsSdk.Models
{
    using System;
    using System.ComponentModel;
    using System.Linq;
    using System.Xml.Linq;

    [Description("METADATA-OBJECT")]
    public class RetsObjectCollection : RetsCollection<RetsObject>
    {
        public override void Load(XElement xElement)
        {
            this.Load(this.GetType(), xElement);
        }

        public override RetsObject Get(object value)
        {
            RetsObject item = this.Get().FirstOrDefault(x => x.ObjectType.Equals(value.ToString(), StringComparison.CurrentCultureIgnoreCase));

            return item;
        }
    }
}
