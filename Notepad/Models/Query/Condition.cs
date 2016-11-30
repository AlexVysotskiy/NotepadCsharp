using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Reflection;

namespace Notepad.Models.Query
{
    /**
     * Abstract class which wraps conditions for query.
     * Has basic mandatory fields, such as:
     *  type - type of the condition
     *  field - name of the checked property 
     *  value - estimated value of the field
     * Defined constants are used in factory, for creation requested condition object.
     */
    abstract public class Condition
    {
        /**
         * ==
         */ 
        public const string TypeEqual = "eq";

        /**
         * !=
         */ 
        public const string TypeNotEqual = "notEq";

        /**
         * LIKE or .StartWith
         */ 
        public const string TypeLike = "like";

        /**
        * IN ()
        */
        public const string TypeIn = "in";

        protected string type;
        protected string field;        
        protected dynamic value;

        public string Type
        {
            get
            {
                return this.type;
            }
            set
            {
                this.type = value;
            }
        }

        public string Field
        {
            get
            {
                return this.field;
            }
            set
            {
                this.field = value;
            }
        }

        public dynamic Value
        {
            get
            {
                return this.value;
            }
            set
            {
                this.value = value;
            }
        }

        /**
         * All child must implement thi method and define the way they checks objects.
         */ 
        abstract public bool Check(object obj);

        /**
         * Return value of property in object
         */ 
        protected PropertyInfo GetProperty(object obj)
        {
            return obj.GetType().GetProperty(this.field);
        }
    }
}