namespace CrestApps.RetsSdk.Contracts
{
    using System.Xml.Linq;

    public interface IRetsCollectionXElementLoader
    {
        void Load(XElement xElement);
    }
}
