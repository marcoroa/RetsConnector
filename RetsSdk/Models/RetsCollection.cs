namespace CrestApps.RetsSdk.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Reflection;
    using System.Xml.Linq;
    using CrestApps.RetsSdk.Contracts;
    using CrestApps.RetsSdk.Helpers.Extensions;

    public abstract class RetsCollection<T> : IMetadataCollection, IMetadataCollectionLoad, IRetsCollectionXElementLoader, IMetadataCollection<T>
        where T : class, new()
    {
        private readonly List<T> items = new List<T>();
        private Type type;

        public string Version { get; set; }
        public DateTime Date { get; set; }

        public void Add(T resource)
        {
            if (resource == null)
            {
                return;
            }

            this.items.Add(resource);
        }

        public IEnumerable<T> Get()
        {
            return this.items;
        }

        public Type GetGenericType()
        {
            if (this.type == null)
            {
                this.type = typeof(T);
            }

            return this.type;
        }

        public void Remove(T resource)
        {
            if (resource == null)
            {
                return;
            }

            this.items.Remove(resource);
        }

        public void Load(Type collectionType, XElement xElement)
        {
            if (collectionType is null)
            {
                throw new ArgumentNullException(nameof(collectionType));
            }

            if (xElement is null)
            {
                throw new ArgumentNullException(nameof(xElement));
            }

            // First, we check all attributes on the XElement
            // for any attribute that match the element, we would set the value accordingly.
            foreach (PropertyInfo property in collectionType.GetProperties())
            {
                var attribute = xElement.Attribute(property.Name);

                if (attribute == null)
                {
                    continue;
                }

                SetValueSafely(this, property, attribute.Value);
            }

            // Second, foreach child in the XElement's children, we need to cast it into the generic model then add it to the collection
            foreach (XElement child in xElement.Elements())
            {
                this.Add(child);
            }
        }

        public abstract void Load(XElement xElement);
        public abstract T Get(object value);

        protected T Cast(XElement element)
        {
            var entity = new T();

            XElement parent = element.Parent;

            IEnumerable<PropertyInfo> properties = typeof(T).GetProperties();

            // Set the entity properites using the current attributes
            foreach (PropertyInfo property in properties)
            {
                if (parent != null)
                {
                    // We first check for any attributes that match this property on the parent and set it
                    var parentAttribute = parent.Attribute(property.Name);

                    if (parentAttribute != null)
                    {
                        SetValueSafely(entity, property, parentAttribute.Value);
                    }
                }

                // Second, check for any attributes that match this property on element directly
                // This value will override the previous value if one already is set
                var attribute = element.Attribute(property.Name);

                if (attribute != null)
                {
                    SetValueSafely(entity, property, attribute.Value);
                }
            }

            // First, foreach child on the given XElement object, find a propery with the same name as the child's localname and set its value accordingly
            foreach (XElement child in element.Elements())
            {
                PropertyInfo property = this.GetGenericType().GetProperty(child.Name.LocalName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

                if (property == null)
                {
                    continue;
                }

                SetValueSafely(entity, property, child.Value);
            }

            // Second, foreach property of the generic entity that implements IMetadataCollection<ANYTHING>
            // find the sub Element and call Load() method using different generics
            var subCollections = properties.Where(x => typeof(IRetsCollectionXElementLoader).IsAssignableFrom(x.PropertyType)).ToList();

            foreach (PropertyInfo subCollection in subCollections)
            {
                DescriptionAttribute attribute = subCollection.PropertyType.GetCustomAttributes(typeof(DescriptionAttribute), false)
                                                              .Select(x => x as DescriptionAttribute)
                                                              .FirstOrDefault();
                if (attribute == null)
                {
                    continue;
                }

                XElement metaDataNode = element.Descendants(attribute.Description).FirstOrDefault();

                if (metaDataNode == null)
                {
                    continue;
                }

                IRetsCollectionXElementLoader newCollection = (IRetsCollectionXElementLoader)Activator.CreateInstance(subCollection.PropertyType);
                newCollection.Load(metaDataNode);

                subCollection.SetValue(entity, newCollection, null);
            }

            return entity;
        }

        private static void SetValueSafely(object entity, PropertyInfo property, string value)
        {
            object safeValue = property.PropertyType.GetSafeObject(value);

            property.SetValue(entity, safeValue, null);
        }

        private void Add(XElement element)
        {
            T model = this.Cast(element);

            this.Add(model);
        }
    }
}
