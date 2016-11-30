using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Notepad.Models;
using System.Linq.Expressions;
using System.Xml;
using System.IO;
using Notepad.Models.Query;

namespace Notepad.Models.Repository
{
    /**
     * Repository for working with XML-files.
     * It used Singleton pattern to provide the only one entity of repo.
     */
    public class XMLRepository : IRepository<NotepadEntry>
    {
        /**
         * Last entity id. Used for adding ids for new entities
         */ 
        private int lastId = 0;

        /**
         * Path to storage - xml file
         */ 
        private string sourcePath;

        /**
         * instance for Singleton pattern realisation
         */
        private static XMLRepository instance;

        /**
         * List of entities. Read from file and new added keeps here.
         */ 
        List<NotepadEntry> entities = new List<NotepadEntry>();

        /**
         * Private contructor, once called it read xml file and load stored entities
         */ 
        private XMLRepository()
        {
            this.sourcePath = AppDomain.CurrentDomain.BaseDirectory + "\\App_Data\\storage.xml";
            this.Init();
        }

        /**
         * Parse entities from xml file.
         * XML-entry has following structure
         *     <user id="id">
         *       <name>name</name>
         *       <lastname>lastname</lastname>
         *       <birthYear>birthYear</birthYear>
         *       <phoneNumber>phoneNumber</phoneNumber>
         *     </user>
         */
        public void Init()
        {
            this.entities.Clear();

            // if file exists, read and parse it
            if (File.Exists(this.sourcePath))
            {

                XmlDocument userSourceDoc = new XmlDocument();
                userSourceDoc.Load(this.sourcePath);

                XmlElement root = userSourceDoc.DocumentElement;

                // go throught all stored nodes 
                foreach (XmlNode node in root)
                {
                    NotepadEntry entity = new NotepadEntry();

                    // match xml nodes to object properties
                    foreach (XmlNode childnode in node.ChildNodes)
                    {

                        switch (childnode.Name)
                        {
                            case "name":
                                {
                                    entity.Name = childnode.InnerText;
                                    break;
                                }
                            case "lastname":
                                {
                                    entity.Lastname = childnode.InnerText;
                                    break;
                                }
                            case "birthYear":
                                {
                                    entity.BirthYear = Int32.Parse(childnode.InnerText);
                                    break;
                                }
                            case "phoneNumber":
                                {
                                    entity.PhoneNumber = childnode.InnerText;
                                    break;
                                }
                        }
                    }

                    entity.Id = Int32.Parse(node.Attributes["id"].Value);

                    // save id, it used for indexing new entities in right way
                    if (this.lastId < entity.Id)
                    {
                        this.lastId = entity.Id;
                    }

                    this.entities.Add(entity);
                }
            }
        }

        /**
         * save new intity to common list.
         * assign id
         */ 
        public bool Store(NotepadEntry entry)
        {
            this.lastId++;
            entry.Id = this.lastId;
            this.entities.Add(entry);

            return true;
        }

        /**
         * Update entity with passed data. Updates all fields
         */ 
        public bool Update(NotepadEntry entry)
        {
            bool result = false;

            NotepadEntry original = this.Find(entry.Id);

            if (original != null)
            {
                result = true;

                // update all fields

                original.Lastname = entry.Lastname;
                original.Name = entry.Name;
                original.BirthYear = entry.BirthYear;
                original.PhoneNumber = entry.PhoneNumber;
            }

            return result;
        }

        /**
         * Change removed flag to true in entities, with passsed ids
         */
        public bool DeleteEntries(int[] ids)
        {
            ConditionCollection collection = new ConditionCollection();
            collection.AddCondition(new InCondition { Field="Id", Value= ids.ToList() });

            var list = this.entities.Where(entity => collection.CheckConditions(entity));
            //var list = this.entities.Where(entity => ids.Contains(entity.Id));

            foreach (var entity in list)
            {
                entity.Removed = true;
            }

            return true;
        }

        /**
         * find entity with passed id
         */ 
        public NotepadEntry Find(int id)
        {
            // make condition and search
            ConditionCollection collection = new ConditionCollection();
            collection.AddCondition(new EqualCondition { Field = "Id", Value = id });

            var result = this.entities.Where(entity => collection.CheckConditions(entity)).Take(1).ToList<NotepadEntry>();

            return result.Count > 0 ? result.First() : null;
        }

        /**
         * Main selection method. Can apply search request and sort by any passed fields
         */ 
        public List<NotepadEntry> SelectEntries(int page = 0, int count = 30, ConditionCollection conditions = null, Ordering ordering = null)
        {
            if (conditions == null)
            {
                conditions = new ConditionCollection();
            }

            // add mandatory condition - hide removed entities
            conditions.AddCondition(new NotEqualCondition { Field = "Removed", Value = true });

            var list = this.entities.Where((entity) => conditions.CheckConditions(entity));

            // start apply orrdering
            if (ordering != null)
            {
                var field = typeof(NotepadEntry).GetProperty(ordering.Field);
                if (field != null)
                {
                    Func<NotepadEntry, object> order = entity => field.GetValue(entity, null);

                    list = ordering.Order == Ordering.DESC ? list.OrderByDescending(order) : list.OrderBy(order);
                }
            }

            return list.Skip(page * count).Take(count).ToList<NotepadEntry>();
        }

        /**
         * Save all entities to storage.
         */ 
        public void Dispose()
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml("<?xml version=\"1.0\" encoding=\"utf-8\"?><users></users>");

            foreach (NotepadEntry entry in this.entities)
            {
                // Exclude removed
                if(!entry.Removed)
                {
                    XmlElement newEntry = doc.CreateElement("user");

                    XmlAttribute attr = doc.CreateAttribute("id");
                    attr.Value = entry.Id.ToString();
                    newEntry.Attributes.Append(attr);

                    string[] fields = new string[] { "name", "lastname", "birthYear", "phoneNumber" };
                    foreach (string field in fields)
                    {
                        XmlNode node = doc.CreateElement(field);

                        string propName = field.First().ToString().ToUpper() + field.Substring(1);

                        node.InnerText = entry.GetType().GetProperty(propName).GetValue(entry).ToString();
                        newEntry.AppendChild(node);
                    }

                    doc.DocumentElement.AppendChild(newEntry);
                }
               
            }

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            // Save the document to a file and auto-indent the output.
            XmlWriter writer = XmlWriter.Create(this.sourcePath, settings);
            doc.Save(writer);
            writer.Close();
        }

        /**
         * Static method return the only instance of repo
         */ 
        public static XMLRepository Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new XMLRepository();
                }
                return instance;
            }
        }

    }
}